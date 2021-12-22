import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { runAfterFunction } from "./globals/patch_functions.js";
import { prioritizeNewestEvent } from "./globals/prioritize_newest_event.js";

/**
 * Fixes broken horizontal scrolling of column container when holding Shift.
 */
export default function() {
	ensurePropertyExists(TD, "ui", "columns");
	
	runAfterFunction(TD.ui.columns, "setupColumnScrollListeners", function(func, args) {
		const [ column ] = args;
		const ele = document.querySelector(".js-column[data-column='" + column.model.getKey() + "']");
		
		if (!ele) {
			return;
		}
		
		$(ele).off("onmousewheel").on("mousewheel", ".scroll-v", function(e) {
			e.stopImmediatePropagation();
		});
		
		prioritizeNewestEvent(ele, "mousewheel");
	});
};
