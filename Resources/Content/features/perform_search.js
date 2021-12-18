import { getEvents } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { crashDebug } from "../api/utils.js";

/**
 * @typedef Searcher
 * @type {Object}
 *
 * @property {function({ query: string, tduckResetInput: boolean })} performSearch
 */

/**
 * @returns {Searcher|null}
 */
function getSearcher() {
	try {
		const context = getEvents(document)["uiSearchInputSubmit"][0].handler.context;
		if ("performSearch" in context) {
			return context;
		}
	} catch (e) {
		crashDebug(e.toString());
	}
	
	return null;
}

/**
 * @this {Searcher} searcher
 * @param {string} query
 */
function performSearch(query) {
	this.performSearch({ query, tduckResetInput: true });
}

export default function() {
	onAppReady(function addPerformSearchFunction() {
		const searcher = getSearcher();
		
		/**
		 * Adds a search column with the specified query.
		 */
		window.TDGF_performSearch = searcher ? performSearch.bind(searcher) : function() {
			alert("error|This feature is not available due to an internal error.");
		};
	});
};
