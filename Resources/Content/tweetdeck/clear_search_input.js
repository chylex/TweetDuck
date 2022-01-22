import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * @typedef SearchEventData
 * @type {Object}
 *
 * @property {string} query
 * @property {string} [searchScope]
 * @property {string} [columnKey]
 * @property {boolean} [tduckResetInput]
 */

/**
 * Clears search input after confirmation.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "columnManager", "_columnOrder");
	ensurePropertyExists(TD, "controller", "columnManager", "move");
	
	/**
	 * @param e
	 * @param {SearchEventData} data
	 */
	const onSearch = function(e, data) {
		if (data.query && data.searchScope !== "users" && !data.columnKey && !("tduckResetInput" in data)) {
			$(".js-app-search-input").val("");
			$(".js-perform-search").blur();
		}
	};
	
	$(document).on("uiSearchNoTemporaryColumn", onSearch);
};
