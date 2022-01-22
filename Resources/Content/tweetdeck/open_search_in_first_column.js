import { $TDX } from "../api/bridge.js";
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
 * Creates temporary search columns at the leftmost position according to app configuration.
 */
export default function() {
	ensurePropertyExists(TD, "controller", "columnManager", "_columnOrder");
	ensurePropertyExists(TD, "controller", "columnManager", "move");
	
	/**
	 * @param e
	 * @param {SearchEventData} data
	 */
	const onSearch = function(e, data) {
		if (data.query && data.searchScope !== "users" && !data.columnKey && $TDX.openSearchInFirstColumn) {
			const order = TD.controller.columnManager._columnOrder;
			
			if (order.length > 1) {
				const columnKey = order[order.length - 1];
				
				order.splice(order.length - 1, 1);
				order.splice(1, 0, columnKey);
				TD.controller.columnManager.move(columnKey, "left");
			}
		}
	};
	
	$(document).on("uiSearchNoTemporaryColumn", onSearch);
};
