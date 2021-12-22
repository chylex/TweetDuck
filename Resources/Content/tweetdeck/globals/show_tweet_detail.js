import { $TD } from "../../api/bridge.js";
import { $ } from "../../api/jquery.js";
import { isAppReady, onAppReady } from "../../api/ready.js";
import { TD } from "../../api/td.js";
import { checkPropertyExists } from "../../api/utils.js";

function isSupported() {
	return checkPropertyExists(TD, "ui", "updates", "showDetailView") &&
	       checkPropertyExists(TD, "controller", "columnManager", "showColumn") &&
	       checkPropertyExists(TD, "controller", "columnManager", "getByApiid") &&
	       checkPropertyExists(TD, "controller", "clients", "getPreferredClient");
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
	
	const column = TD.controller.columnManager.getByApiid(columnId);
	if (!column) {
		if (confirm("error|The column which contained the tweet no longer exists. Would you like to open the tweet in your browser instead?")) {
			$TD.openBrowser(fallbackUrl);
		}
		
		return;
	}
	
	const chirp = column.findMostInterestingChirp(chirpId);
	if (chirp) {
		showTweetDetailInternal(column, chirp);
	}
	else {
		TD.controller.clients.getPreferredClient().show(chirpId, function(chirp) {
			showTweetDetailInternal(column, chirp);
		}, function() {
			if (confirm("error|Could not retrieve the requested tweet. Would you like to open the tweet in your browser instead?")) {
				$TD.openBrowser(fallbackUrl);
			}
		});
	}
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
