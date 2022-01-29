import { $ } from "../api/jquery.js";
import { prioritizeNewestEvent } from "./globals/prioritize_newest_event.js";
import { showTweetDetail } from "./globals/show_tweet_detail.js";

/**
 * Registers global functions which require jQuery.
 */
export default function() {
	window.jQuery = $;
	window.TDGF_prioritizeNewestEvent = prioritizeNewestEvent;
	window.TDGF_showTweetDetail = showTweetDetail;
};
