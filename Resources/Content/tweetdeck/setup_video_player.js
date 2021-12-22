import { $TD } from "../api/bridge.js";
import { TD } from "../api/td.js";
import { checkPropertyExists, noop } from "../api/utils.js";
import { getHoveredTweet } from "./globals/get_hovered_tweet.js";
import { injectMustache } from "./globals/inject_mustache.js";
import { replaceFunction, runAfterFunction } from "./globals/patch_functions.js";

/**
 * @param {HTMLElement} ele
 * @returns {string|null}
 */
function getGifLink(ele) {
	if (!ele) {
		return null;
	}
	
	return (ele.getAttribute("src") || ele.querySelector("source[video-src]")?.getAttribute("video-src"));
}

/**
 * @param {HTMLElement} obj
 * @returns {string|null}
 */
function getVideoTweetLink(obj) {
	const parent = obj.querySelector(".js-tweet");
	if (!parent) {
		return null;
	}
	
	const link = parent.classList.contains("tweet-detail") ? parent.querySelector("a[rel='url']") : parent.querySelector("time > a");
	return link?.getAttribute("href");
}

/**
 * @param {ChirpBase|null} tweet
 * @returns {string|null}
 */
function getUsername(tweet) {
	return tweet && (tweet.quotedTweet || tweet).getMainUser().screenName;
}

/**
 * @param {string} selector
 * @param {function(this: HTMLElement, e: MouseEvent)} handler
 * @returns {function(e: MouseEvent)}
 */
function delegateMouseEvent(selector, handler) {
	return function(e) {
		const ele = e.target.closest(selector);
		if (ele) {
			handler.call(ele, e);
		}
	};
}

function createOverlay() {
	const stopVideo = function() {
		$TD.stopVideo();
	};
	
	const overlay = document.createElement("div");
	overlay.id = "td-video-player-overlay";
	overlay.classList.add("ovl");
	overlay.style.display = "block";
	overlay.addEventListener("click", stopVideo);
	overlay.addEventListener("contextmenu", stopVideo);
	document.querySelector(".js-app").appendChild(overlay);
}

export default function() {
	const app = document.querySelector(".js-app");
	
	/**
	 * Plays a video using the internal player.
	 * @param {string} videoUrl
	 * @param {string|null} tweetUrl
	 * @param {string|null} username
	 */
	window.TDGF_playVideo = function(videoUrl, tweetUrl, username) {
		if (!videoUrl) {
			return;
		}
		
		$TD.playVideo(videoUrl, tweetUrl || videoUrl, username || null, createOverlay);
	};
	
	app.addEventListener("click", delegateMouseEvent(".js-gif-play", function(e) {
		const src = !e.ctrlKey && getGifLink(this.closest(".js-media-gif-container")?.querySelector("video"));
		const tweet = getVideoTweetLink(this);
		
		if (src) {
			const hovered = getHoveredTweet();
			window.TDGF_playVideo(src, tweet, getUsername(hovered && hovered.obj));
		}
		else {
			$TD.openBrowser(tweet);
		}
		
		e.stopPropagation();
	}));
	
	app.addEventListener("mousedown", delegateMouseEvent(".js-gif-player", function(e) {
		if (e.button === 1) {
			e.preventDefault();
		}
	}));
	
	app.addEventListener("mouseup", delegateMouseEvent(".js-gif-player", function(e) {
		if (e.button === 1) {
			$TD.openBrowser(getVideoTweetLink(this));
			e.preventDefault();
		}
	}));
	
	injectMustache("status/media_thumb.mustache", "append", "is-gif", " is-paused");
	
	// noinspection HtmlUnknownTarget, CssUnknownTarget
	TD.mustaches["media/native_video.mustache"] = "<div class=\"js-media-gif-container media-item nbfc is-video\" style=\"background-image:url({{imageSrc}});\"><video class=\"js-media-gif media-item-gif full-width block {{#isPossiblySensitive}}is-invisible{{/isPossiblySensitive}}\" loop src=\"{{videoUrl}}\"></video><a class=\"js-gif-play pin-all is-actionable\">{{> media/video_overlay}}</a></div>";
	
	let cancelModal = false;
	
	if (checkPropertyExists(TD, "components", "MediaGallery", "prototype")) {
		runAfterFunction(TD.components.MediaGallery.prototype, "_loadTweet", /** @this MediaGallery */ function() {
			const media = this.chirp.getMedia().find(media => media.mediaId === this.clickedMediaEntityId);
			
			if (media && media.isVideo && media.service === "twitter") {
				window.TDGF_playVideo(media.chooseVideoVariant().url, this.chirp.getChirpURL(), getUsername(this.chirp));
				cancelModal = true;
			}
		});
	}
	
	if (checkPropertyExists(TD, "components", "BaseModal", "prototype")) {
		replaceFunction(TD.components.BaseModal.prototype, "setAndShowContainer", function(func, args) {
			if (cancelModal) {
				cancelModal = false;
			}
			else {
				func.apply(this, args);
			}
		});
	}
	
	if (checkPropertyExists(TD, "ui", "Column", "prototype", "playGifIfNotManuallyPaused")) {
		TD.ui.Column.prototype.playGifIfNotManuallyPaused = noop;
	}
};
