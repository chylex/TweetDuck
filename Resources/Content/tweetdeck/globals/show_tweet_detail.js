import { $TD } from "../../api/bridge.js";
import { $ } from "../../api/jquery.js";
import { isAppReady, onAppReady } from "../../api/ready.js";
import { TD } from "../../api/td.js";
import { checkPropertyExists } from "../../api/utils.js";
import { COLUMN_NOT_FOUND, retrieveTweet, TWEET_NOT_FOUND } from "./retrieve_tweet.js";

function isSupported() {
	return checkPropertyExists(TD, "ui", "updates", "showDetailView") &&
	       checkPropertyExists(TD, "controller", "columnManager", "showColumn") &&
	       checkPropertyExists(TD, "controller", "columnManager", "getByApiid") &&
	       checkPropertyExists(TD, "controller", "clients", "getPreferredClient") &&
	       checkPropertyExists(TD, "services", "TwitterClient", "prototype", "show");
}

/**
 * @param {TD_Column} column
 * @param {ChirpBase} chirp
 */
function showTweetDetailInternal(column, chirp) {
	TD.ui.updates.showDetailView(column, chirp, column.findChirp(chirp.id) || chirp);
	TD.controller.columnManager.showColumn(column.model.privateState.key);
	
	$(document).trigger("uiGridClearSelection");
}

/**
 * @param {string} columnId
 * @param {string} chirpId
 * @param {string} fallbackUrl
 */
function showTweetDetailImpl(columnId, chirpId, fallbackUrl) {
	if (!isAppReady()) {
		onAppReady(() => showTweetDetailImpl(columnId, chirpId, fallbackUrl));
		return;
	}
	
	retrieveTweet(columnId, chirpId, showTweetDetailInternal, error => {
		// noinspection NestedConditionalExpressionJS
		const message = error === COLUMN_NOT_FOUND ? "The column which contained the tweet no longer exists." :
		                error === TWEET_NOT_FOUND ? "Could not retrieve the requested tweet." :
		                null;
		
		if (message && confirm("error|" + message + " Would you like to open the tweet in your browser instead?")) {
			$TD.openBrowser(fallbackUrl);
		}
	});
}

/**
 * Opens the tweet detail view in the specified column.
 * @param {string} columnId
 * @param {string} chirpId
 * @param {string} fallbackUrl
 */
export const showTweetDetail = isSupported() ? showTweetDetailImpl : function() {
	alert("error|This feature is not available due to an internal error.");
};
