import { $TD, $TDX } from "../api/bridge.js";
import { $ } from "../api/jquery.js";
import { replaceFunction } from "../api/patch.js";
import { TD } from "../api/td.js";
import { checkPropertyExists, ensurePropertyExists } from "../api/utils.js";
import { getColumnName } from "./globals/get_column_name.js";

/**
 * Event callback for a new tweet.
 * @returns {function(column: TD_Column, tweet: ChirpBase)}
 */
const onNewTweet = (function() {
	const recentMessages = new Set();
	const recentTweets = new Set();
	let recentTweetTimer = null;
	
	const resetRecentTweets = () => {
		recentTweetTimer = null;
		recentTweets.clear();
	};
	
	const startRecentTweetTimer = () => {
		if (recentTweetTimer) {
			window.clearTimeout(recentTweetTimer);
		}
		
		recentTweetTimer = window.setTimeout(resetRecentTweets, 20000);
	};
	
	const checkTweetCache = (set, id) => {
		if (set.has(id)) {
			return true;
		}
		
		if (set.size > 50) {
			set.clear();
		}
		
		set.add(id);
		return false;
	};
	
	const isSensitive = (tweet) => {
		const main = tweet.getMainTweet && tweet.getMainTweet();
		if (main?.possiblySensitive) {
			return true; // TODO these don't show media badges when hiding sensitive media
		}
		
		const related = tweet.getRelatedTweet && tweet.getRelatedTweet();
		if (related?.possiblySensitive) {
			return true;
		}
		
		// noinspection RedundantIfStatementJS
		if (tweet.quotedTweet?.possiblySensitive) {
			return true;
		}
		
		return false;
	};
	
	const fixMedia = function(html, media) {
		return html.find("a[data-media-entity-id='" + media.mediaId + "'], .media-item").first().removeClass("is-zoomable").css("background-image", "url(\"" + media.small() + "\")");
	};
	
	/**
	 * @param {TD_Column} column
	 * @param {ChirpBase} tweet
	 */
	return function(column, tweet) {
		if (tweet instanceof TD.services.TwitterConversation || tweet instanceof TD.services.TwitterConversationMessageEvent) {
			if (checkTweetCache(recentMessages, tweet.id)) {
				return;
			}
		}
		else {
			if (checkTweetCache(recentTweets, tweet.id)) {
				return;
			}
		}
		
		startRecentTweetTimer();
		
		if (column.model.getHasNotification()) {
			const sensitive = isSensitive(tweet);
			const previews = $TDX.notificationMediaPreviews && (!sensitive || TD.settings.getDisplaySensitiveMedia());
			// TODO new cards don't have either previews or links
			
			const html = $(tweet.render({
				withFooter: false,
				withTweetActions: false,
				withMediaPreview: true,
				isMediaPreviewOff: !previews,
				isMediaPreviewSmall: previews,
				isMediaPreviewLarge: false,
				isMediaPreviewCompact: false,
				isMediaPreviewInQuoted: previews,
				thumbSizeClass: "media-size-medium",
				mediaPreviewSize: "medium"
			}));
			
			html.find("time[data-time] a").text(""); // remove time since tweet was sent, since it will be recalculated anyway
			html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
			html.find(".js-quote-detail").removeClass("is-actionable margin-b--8"); // prevent quoted tweets from changing the cursor and reduce bottom margin
			
			if (previews) {
				html.find(".reverse-image-search").remove();
				
				const container = html.find(".js-media");
				
				for (const media of tweet.getMedia()) {
					fixMedia(container, media);
				}
				
				if (tweet.quotedTweet) {
					for (const media of tweet.quotedTweet.getMedia()) {
						fixMedia(container, media).addClass("media-size-medium");
					}
				}
			}
			else if (tweet instanceof TD.services.TwitterActionOnTweet) {
				html.find(".js-media").remove();
			}
			
			html.find("a[data-full-url]").each(function() { // bypass t.co on all links and fix tooltips
				this.href = this.getAttribute("data-full-url");
				this.removeAttribute("title");
			});
			
			html.find("a[href='#']").each(function() { // remove <a> tags around links that don't lead anywhere (such as account names the tweet replied to)
				this.outerHTML = this.innerHTML;
			});
			
			html.find("p.link-complex-target").filter(function() {
				return $(this).text() === "Show this thread";
			}).first().each(function() {
				this.id = "tduck-show-thread";
				
				const moveBefore = html.find(".tweet-body > .js-media, .tweet-body > .js-media-preview-container, .quoted-tweet");
				
				if (moveBefore) {
					$(this).css("margin-top", "5px").removeClass("margin-b--5").parent("span").detach().insertBefore(moveBefore);
				}
			});
			
			if (tweet.quotedTweet) {
				html.find("p.txt-mute").filter(function() {
					return $(this).text() === "Show this thread";
				}).first().remove();
			}
			
			const type = tweet.getChirpType();
			
			if (type === "follow") {
				html.find(".js-user-actions-menu").parent().remove();
				html.find(".account-bio").removeClass("padding-t--5").css("padding-top", "2px");
			}
			else if ((type.startsWith("favorite") || type.startsWith("retweet")) && tweet.isAboutYou()) {
				html.children().first().addClass("td-notification-padded");
			}
			else if (type.includes("list_member")) {
				html.children().first().addClass("td-notification-padded td-notification-padded-alt");
				html.find(".activity-header").css("margin-top", "2px");
				html.find(".avatar").first().css("margin-bottom", "0");
			}
			
			if (sensitive) {
				html.find(".media-badge").each(function() {
					$(this)[0].lastChild.textContent += " (possibly sensitive)";
				});
			}
			
			const source = tweet.getRelatedTweet();
			const duration = source ? (source.text.length + (source.quotedTweet?.text.length ?? 0)) : tweet.text.length;
			
			const chirpId = source ? source.id : "";
			const tweetUrl = source ? source.getChirpURL() : "";
			const quoteUrl = source && source.quotedTweet ? source.quotedTweet.getChirpURL() : "";
			
			$TD.onTweetPopup(column.model.privateState.apiid, chirpId, getColumnName(column), html.html(), duration, tweetUrl, quoteUrl);
		}
		
		if (column.model.getHasSound()) {
			$TD.onTweetSound();
		}
	};
})();

/**
 * Fixes DM notifications not showing if the conversation is open.
 * @this {TD_Column}
 * @param {{ chirps: ChirpBase[], poller: { feed: { managed: boolean } } }} e
 */
function handleNotificationEvent(e) {
	if (this.model?.state?.type === "privateMe" && !this.notificationsDisabled && e.poller.feed.managed) {
		const unread = [];
		
		for (const chirp of e.chirps) {
			if (Array.isArray(chirp.messages)) {
				Array.prototype.push.apply(unread, chirp.messages.filter(message => message.read === false));
			}
		}
		
		if (unread.length > 0) {
			if (checkPropertyExists(TD, "util", "chirpReverseColumnSort")) {
				unread.sort(TD.util.chirpReverseColumnSort);
			}
			
			for (const message of unread) {
				onNewTweet(this, message);
			}
			
			// TODO sound notifications are borked as well
			// TODO figure out what to do with missed notifications at startup
		}
	}
}

/**
 * Adds support for desktop notifications.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "notifications");
	TD.controller.notifications.hasNotifications = function() {
		return true;
	};
	
	TD.controller.notifications.isPermissionGranted = function() {
		return true;
	};
	
	$["subscribe"]("/notifications/new", function(obj) {
		for (let index = obj.items.length - 1; index >= 0; index--) {
			onNewTweet(obj.column, obj.items[index]);
		}
	});
	
	if (checkPropertyExists(TD, "vo", "Column", "prototype")) {
		replaceFunction(TD.vo.Column.prototype, "mergeMissingChirps", function(func, args) {
			handleNotificationEvent.call(this, args[0]);
			return func.apply(this, args);
		});
	}
};
