import { $, ensureEventExists } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

function reloadScheduledColumn() {
	const column = Object.values(TD.controller.columnManager.getAll()).find(column => column.model.state.type === "scheduled");
	
	if (column) {
		setTimeout(function() {
			column.reloadTweets();
		}, 1000);
	}
}

/**
 * Works around scheduled tweets not showing up sometimes after being sent.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "columnManager", "getAll");
	ensureEventExists(document, "dataTweetSent");
	
	$(document).on("dataTweetSent", function(e, data) {
		if ("request" in data && "scheduledDate" in data.request) {
			reloadScheduledColumn();
		}
	});
};
