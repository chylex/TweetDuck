import { registerPropertyUpdateCallback, triggerPropertiesUpdated } from "../api/bridge.js";
import { applyROT13 } from "./globals/apply_rot13.js";
import { getColumnName } from "./globals/get_column_name.js";
import { injectMustache } from "./globals/inject_mustache.js";
import { reloadBrowser } from "./globals/reload_browser.js";
import { reloadColumns } from "./globals/reload_columns.js";

/**
 * Registers global functions which do not require jQuery.
 */
export default function() {
	window.TDGF_applyROT13 = applyROT13;
	window.TDGF_getColumnName = getColumnName;
	window.TDGF_injectMustache = injectMustache;
	window.TDGF_onPropertiesUpdated = triggerPropertiesUpdated;
	window.TDGF_registerPropertyUpdateCallback = registerPropertyUpdateCallback;
	window.TDGF_reload = reloadBrowser;
	window.TDGF_reloadColumns = reloadColumns;
};
