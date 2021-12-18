import { $TD } from "../api/bridge.js";
import { TD } from "../api/td.js";
import { checkPropertyExists } from "../api/utils.js";
import { replaceFunction } from "../globals/patch_functions.js";

/**
 * @property {string[]} eventNames
 * @property {function(this: HTMLElement, e: Event)} eventHandler
 */
function delegateLinkEvents(eventNames, eventHandler) {
	for (const eventName of eventNames) {
		document.body.addEventListener(eventName, function(e) {
			const ele = e.target;
			
			if (ele.hasAttribute("data-full-url")) {
				eventHandler.call(ele, e);
			}
		}, { capture: true });
	}
}

/**
 * Bypasses t.co when clicking/dragging links, in media, and in user profiles.
 */
export default function() {
	delegateLinkEvents([ "click", "auxclick" ], /** @this HTMLElement */ function(e) {
		// event.which seems to be borked in auxclick
		// tweet links open directly in the column
		if ((e.button === 0 || e.button === 1) && this.getAttribute("rel") !== "tweet") {
			$TD.openBrowser(this.getAttribute("data-full-url"));
			e.preventDefault();
		}
	});
	
	delegateLinkEvents([ "dragstart" ], /** @this HTMLElement */ function(e) {
		const url = this.getAttribute("data-full-url");
		const data = e.dataTransfer;
		
		data.clearData();
		data.setData("text/uri-list", url);
		data.setData("text/plain", url);
		data.setData("text/html", `<a href="${url}">${url}</a>`);
	});
	
	if (checkPropertyExists(TD, "services", "TwitterUser", "prototype")) {
		replaceFunction(TD.services.TwitterUser.prototype, "fromJSONObject", function(func, args) {
			const [ e ] = args;
			const obj = func.apply(this, args);
			const expandedUrl = e.entities?.url?.urls?.[0]?.expanded_url;
			
			if (expandedUrl) {
				obj.url = expandedUrl;
			}
			
			return obj;
		});
	}
	
	if (checkPropertyExists(TD, "services", "TwitterMedia", "prototype")) {
		replaceFunction(TD.services.TwitterMedia.prototype, "fromMediaEntity", function(func, args) {
			const [ e ] = args;
			const obj = func.apply(this, args);
			
			if (e.expanded_url) {
				if (obj.url === obj.shortUrl) {
					obj.shortUrl = e.expanded_url;
				}
				
				obj.url = e.expanded_url;
			}
			
			return obj;
		});
	}
	
	if (checkPropertyExists(TD, "services", "TwitterStatus", "prototype")) {
		replaceFunction(TD.services.TwitterStatus.prototype, "_generateHTMLText", /** @this TwitterStatus */ function(func, args) {
			const card = this.card;
			const entities = this.entities;
			
			if (card && entities) {
				const urls = entities.urls;
				
				if (urls && urls.length) {
					const shortUrl = card.url;
					const urlObj = entities.urls.find(obj => obj.url === shortUrl && obj.expanded_url);
					
					if (urlObj) {
						const expandedUrl = urlObj.expanded_url;
						const innerCardUrl = card.binding_values?.card_url;
						
						card.url = expandedUrl;
						
						if (innerCardUrl) {
							innerCardUrl.string_value = expandedUrl;
						}
					}
				}
			}
			
			return func.apply(this, args);
		});
	}
};
