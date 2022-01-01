import { replaceFunction } from "../api/patch.js";

function redirectToTweetDeck() {
	location.href = "https://tweetdeck.twitter.com";
}

function hookHistoryStateFunction(func, args) {
	debugger;
	if (args[2] === "/") {
		redirectToTweetDeck();
	}
	else {
		func.apply(this, args);
	}
}

/**
 * Redirects plain twitter.com to TweetDeck, so that users cannot accidentally land on twitter.com login.
 */
export default function() {
	replaceFunction(window.history, "pushState", hookHistoryStateFunction);
	replaceFunction(window.history, "replaceState", hookHistoryStateFunction);
};
