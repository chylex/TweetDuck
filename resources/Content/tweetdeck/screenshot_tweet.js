import { $TD } from "../api/bridge.js";
import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { checkPropertyExists } from "../api/utils.js";
import { getClassStyleProperty } from "./globals/get_class_style_property.js";
import { COLUMN_NOT_FOUND, retrieveTweet, TWEET_NOT_FOUND } from "./globals/retrieve_tweet.js";

function isSupported() {
	return checkPropertyExists(TD, "controller", "columnManager", "getByApiid") &&
	       checkPropertyExists(TD, "controller", "clients", "getPreferredClient") &&
	       checkPropertyExists(TD, "services", "TwitterClient", "prototype", "show");
}

/**
 * @param {TD_Column} column
 * @param {ChirpBase} chirp
 */
function screenshotTweetInternal(column, chirp) {
	const html = $(chirp.render({
		withFooter: false,
		withTweetActions: false,
		isInConvo: false,
		isFavorite: false,
		isRetweeted: false, // keeps retweet mark above tweet
		isPossiblySensitive: false,
		mediaPreviewSize: column.getMediaPreviewSize()
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
		gif.css("background-image", "url(\"" + chirp.getMedia()[0].small() + "\")");
	}
	
	const type = chirp.getChirpType();
	
	if ((type.startsWith("favorite") || type.startsWith("retweet")) && chirp.isAboutYou()) {
		html.addClass("td-notification-padded");
	}
	
	$TD.screenshotTweet(html[0].outerHTML, column.visibility.columnWidth);
}

/**
 * @param {string} columnId
 * @param {string} chirpId
 */
function screenshotTweetImpl(columnId, chirpId) {
	retrieveTweet(columnId, chirpId, screenshotTweetInternal, error => {
		if (error === COLUMN_NOT_FOUND) {
			alert("error|The column which contained the tweet no longer exists.");
		}
		else if (error === TWEET_NOT_FOUND) {
			alert("error|Could not retrieve the requested tweet.");
		}
	});
}

export default function() {
	/**
	 * Screenshots the specified tweet to clipboard.
	 * @param {string} columnId
	 * @param {string} chirpId
	 */
	window.TDGF_triggerScreenshot = isSupported() ? screenshotTweetImpl : function() {
		alert("error|This feature is not available due to an internal error.");
	};
};
