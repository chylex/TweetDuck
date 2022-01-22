import { registerPropertyUpdateCallback } from "../api/bridge.js";
import { $ } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * @typedef DateInput
 * @type {Object}
 *
 * @property {DateInputConfiguration} conf
 */

/**
 * @typedef DateInputConfiguration
 * @type {Object}
 *
 * @property {number} firstDay
 */

/**
 * Sets first day of week in date picker according to app configuration.
 */
export default function() {
	ensurePropertyExists($, "tools", "dateinput", "conf", "firstDay");
	
	/** @type {DateInput} */
	const dateinput = $["tools"]["dateinput"];
	
	onAppReady(function setupDatePickerFirstDayCallback() {
		registerPropertyUpdateCallback(function($TDX) {
			dateinput.conf.firstDay = $TDX.firstDayOfWeek;
		}, true);
	});
};
