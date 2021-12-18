import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";

/**
 * @param {HTMLElement} ele
 * @param {ChirpBase} tweet
 */
function replyInComposeDrawer(ele, tweet) {
	const main = tweet.getMainTweet();
	
	// noinspection JSUnusedGlobalSymbols
	$(document).trigger("uiDockedComposeTweet", {
		type: "reply",
		from: [ tweet.account.getKey() ],
		inReplyTo: {
			id: tweet.id,
			htmlText: main.htmlText,
			user: {
				screenName: main.user.screenName,
				name: main.user.name,
				profileImageURL: main.user.profileImageURL
			}
		},
		mentions: tweet.getReplyUsers(),
		element: ele
	});
}

/**
 * @param {ChirpBase} tweet
 */
function openFavoriteDialog(tweet) {
	$(document).trigger("uiShowFavoriteFromOptions", { tweet });
}

/**
 * @param {HTMLElement} ele
 * @param {ChirpBase} tweet
 */
function quoteTweet(ele, tweet) {
	TD.controller.stats.quoteTweet();
	
	$(document).trigger("uiComposeTweet", {
		type: "tweet",
		from: [ tweet.account.getKey() ],
		quotedTweet: tweet.getMainTweet(),
		element: ele // triggers reply-account plugin
	});
}

/**
 * Adds support for middle-clicking icons under tweets for alternative behaviors:
 *  - Reply icon opens the compose drawer
 *  - Favorite icon open a 'Like from accounts...' dialog
 *  - Retweet icon triggers a quote
 */
export default function() {
	$(".js-app").delegate(".tweet-action,.tweet-detail-action", "auxclick", function(e) {
		if (e.which !== 2) {
			return;
		}
		
		const column = TD.controller.columnManager.get($(this).closest("section.js-column").attr("data-column"));
		if (!column) {
			return;
		}
		
		const ele = $(this).closest("article");
		const tweet = column.findChirp(ele.attr("data-tweet-id")) || column.findChirp(ele.attr("data-key"));
		if (!tweet) {
			return;
		}
		
		switch ($(this).attr("rel")) {
			case "reply":
				replyInComposeDrawer(ele, tweet);
				break;
			
			case "favorite":
				openFavoriteDialog(tweet);
				break;
			
			case "retweet":
				quoteTweet(ele, tweet);
				break;
			
			default:
				return;
		}
		
		e.preventDefault();
		e.stopPropagation();
		e.stopImmediatePropagation();
	});
};
