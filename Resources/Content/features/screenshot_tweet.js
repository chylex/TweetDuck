import { $TD } from "../api/bridge.js";
import { $ } from "../api/jquery.js";
import { getClassStyleProperty } from "../globals/get_class_style_property.js";
import { getHoveredTweet } from "../globals/get_hovered_tweet.js";

export default function() {
	/**
	 * Screenshots the hovered tweet to clipboard.
	 */
	window.TDGF_triggerScreenshot = function() {
		const hovered = getHoveredTweet();
		if (!hovered) {
			return;
		}
		
		const columnWidth = $(hovered.column.ele).width();
		const tweet = hovered.wrap || hovered.obj;
		
		const html = $(tweet.render({
			withFooter: false,
			withTweetActions: false,
			isInConvo: false,
			isFavorite: false,
			isRetweeted: false, // keeps retweet mark above tweet
			isPossiblySensitive: false,
			mediaPreviewSize: hovered.column.obj.getMediaPreviewSize()
		}));
		
		html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
		html.find(".td-screenshot-remove").remove();
		
		html.find("p.link-complex-target,p.txt-mute").filter(function() {
			return $(this).text() === "Show this thread";
		}).remove();
		
		html.addClass($(document.documentElement).attr("class"));
		html.addClass($(document.body).attr("class"));
		
		html.css("background-color", getClassStyleProperty("stream-item", "background-color"));
		html.css("border", "none");
		
		for (const selector of [ ".js-quote-detail", ".js-media-preview-container", ".js-media" ]) {
			const ele = html.find(selector);
			
			if (ele.length) {
				ele[0].style.setProperty("margin-bottom", "2px", "important");
				break;
			}
		}
		
		const gif = html.find(".js-media-gif-container");
		
		if (gif.length) {
			gif.css("background-image", "url(\"" + tweet.getMedia()[0].small() + "\")");
		}
		
		const type = tweet.getChirpType();
		
		if ((type.startsWith("favorite") || type.startsWith("retweet")) && tweet.isAboutYou()) {
			html.addClass("td-notification-padded");
		}
		
		$TD.screenshotTweet(html[0].outerHTML, columnWidth);
	};
};
