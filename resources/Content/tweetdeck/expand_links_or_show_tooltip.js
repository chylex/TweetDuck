import { $TD, $TDX } from "../api/bridge.js";

/**
 * @property {string} eventName
 * @property {function(this: HTMLElement, e: Event)} eventHandler
 */
function delegateLinkEvent(eventName, eventHandler) {
	document.body.addEventListener(eventName, function(e) {
		const ele = e.target;
		if (ele.hasAttribute("data-full-url")) {
			eventHandler.call(ele, e);
		}
	}, { capture: true });
}

/**
 * Either expands links or shows tooltips on hover, depending on app configuration.
 */
export default function() {
	let prevMouseX = -1, prevMouseY = -1;
	let tooltipTimer, tooltipDisplayed;
	
	delegateLinkEvent("mouseenter", /** @this HTMLElement */ function() {
		const text = this.innerText;
		
		if (text.charCodeAt(text.length - 1) !== 8230 && text.charCodeAt(0) !== 8230) {
			return; // horizontal ellipsis
		}
		
		if ($TDX.expandLinksOnHover) {
			tooltipTimer = window.setTimeout(() => {
				this.setAttribute("td-prev-text", text);
				this.innerText = this.getAttribute("data-full-url").replace(/^https?:\/\/(www\.)?/, "");
			}, 200);
		}
		else {
			this.removeAttribute("title");
			
			tooltipTimer = window.setTimeout(() => {
				$TD.displayTooltip(this.getAttribute("data-full-url"));
				tooltipDisplayed = true;
			}, 400);
		}
	});
	
	delegateLinkEvent("mouseleave", /** @this HTMLElement */ function() {
		if (this.hasAttribute("td-prev-text")) {
			this.innerText = this.getAttribute("td-prev-text");
		}
		
		window.clearTimeout(tooltipTimer);
		
		if (tooltipDisplayed) {
			tooltipDisplayed = false;
			$TD.displayTooltip(null);
		}
	});
	
	delegateLinkEvent("mousemove", /** @this HTMLElement */ function(e) {
		if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)) {
			$TD.displayTooltip(this.getAttribute("data-full-url"));
			prevMouseX = e.clientX;
			prevMouseY = e.clientY;
		}
	});
};
