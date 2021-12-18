import { $TD } from "../api/bridge.js";
import { getHoveredTweet } from "../globals/get_hovered_tweet.js";

/**
 * @this HTMLAnchorElement
 */
function handleLinkContextMenu() {
	if (this.classList.contains("js-media-image-link")) {
		const hovered = getHoveredTweet();
		if (!hovered) {
			return;
		}
		
		const tweet = hovered.obj.hasMedia() ? hovered.obj : hovered.obj.quotedTweet;
		const media = tweet.getMedia().find(media => media.mediaId === this.getAttribute("data-media-entity-id"));
		
		if ((media.isVideo && media.service === "twitter") || media.isAnimatedGif) {
			$TD.setRightClickedLink("video", media.chooseVideoVariant().url);
		}
		else {
			$TD.setRightClickedLink("image", media.large());
		}
	}
	else if (this.classList.contains("js-gif-play")) {
		$TD.setRightClickedLink("video", this.closest(".js-media-gif-container")?.querySelector("video")?.src);
	}
	else if (this.hasAttribute("data-full-url")) {
		$TD.setRightClickedLink("link", this.getAttribute("data-full-url"));
	}
}

/**
 * Adds additional information about links to the context menu.
 */
export default function() {
	document.body.addEventListener("contextmenu", function(e) {
		const ele = e.target;
		if (ele.tagName === "A") {
			handleLinkContextMenu.call(ele);
		}
	}, { capture: true });
};
