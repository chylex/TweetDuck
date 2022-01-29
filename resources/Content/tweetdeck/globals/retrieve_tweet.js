import { TD } from "../../api/td.js";

export const COLUMN_NOT_FOUND = "column";
export const TWEET_NOT_FOUND = "tweet";

/**
 * @param {string} columnId
 * @param {string} chirpId
 * @param {function(column: TD_Column, chirp: ChirpBase)} onSuccess
 * @param {function("column"|"tweet")} onError
 */
export function retrieveTweet(columnId, chirpId, onSuccess, onError) {
	const column = TD.controller.columnManager.getByApiid(columnId);
	if (!column) {
		onError(COLUMN_NOT_FOUND);
		return;
	}
	
	const chirp = column.findMostInterestingChirp(chirpId);
	if (chirp) {
		onSuccess(column, chirp);
	}
	else {
		TD.controller.clients.getPreferredClient().show(chirpId, function(chirp) {
			onSuccess(column, chirp);
		}, function() {
			onError(TWEET_NOT_FOUND);
		});
	}
}
