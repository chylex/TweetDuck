import { $TD } from "../api/bridge.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { getHoveredTweet } from "./globals/get_hovered_tweet.js";

function processMedia(chirp) {
	return chirp.getMedia().filter(item => !item.isAnimatedGif).map(item => item.entity.media_url_https + ":small").join(";");
}

function handleTweetContextMenu() {
	const hovered = getHoveredTweet();
	if (!hovered) {
		return;
	}
	
	const tweet = hovered.obj;
	const quote = tweet.quotedTweet;
	
	if (tweet.chirpType === TD.services.ChirpBase.TWEET) {
		const tweetUrl = tweet.getChirpURL();
		const quoteUrl = quote && quote.getChirpURL();
		
		const chirpAuthors = quote ? [ tweet.getMainUser().screenName, quote.getMainUser().screenName ].join(";") : tweet.getMainUser().screenName;
		const chirpImages = tweet.hasImage() ? processMedia(tweet) : quote?.hasImage() ? processMedia(quote) : "";
		
		$TD.setRightClickedChirp(tweetUrl || "", quoteUrl || "", chirpAuthors, chirpImages);
	}
	else if (tweet instanceof TD.services.TwitterActionFollow) {
		$TD.setRightClickedLink("link", tweet.following.getProfileURL());
	}
}

/**
 * Adds additional information about tweets to the context menu.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "columnManager", "get");
	ensurePropertyExists(TD, "services", "ChirpBase", "TWEET");
	ensurePropertyExists(TD, "services", "TwitterActionFollow");
	
	document.querySelector(".js-app").addEventListener("contextmenu", function(e) {
		if (e.target.closest("section.js-column") !== null) {
			handleTweetContextMenu();
		}
	}, { capture: true });
};
