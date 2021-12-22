import { $, single } from "../api/jquery.js";

/**
 * @typedef TD_Event_uiColumnRendered_data
 * @type {Object}
 *
 * @property {TD_Column} column
 * @property {JQuery<HTMLElement>} $column
 */

/**
 * Adds column icons as attributes on the column element to column types stylable.
 */
export default function() {
	$(document).on("uiColumnRendered", function(e, data) {
		const ele = single(data.$column);
		const icon = ele?.querySelector(".column-type-icon");
		const name = icon && Array.prototype.find.call(icon.classList, cls => cls.startsWith("icon-"));
		
		if (name) {
			ele.setAttribute("data-td-icon", name);
			data.column._tduck_icon = name;
		}
	});
}
