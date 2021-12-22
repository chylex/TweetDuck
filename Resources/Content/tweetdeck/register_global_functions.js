import { registerPropertyUpdateCallback, triggerPropertiesUpdated } from "../api/bridge.js";
import { applyROT13 } from "./globals/apply_rot13.js";
import { getColumnName } from "./globals/get_column_name.js";
import { injectMustache } from "./globals/inject_mustache.js";
import { prioritizeNewestEvent } from "./globals/prioritize_newest_event.js";
import { reloadBrowser } from "./globals/reload_browser.js";
import { reloadColumns } from "./globals/reload_columns.js";
import { showTweetDetail } from "./globals/show_tweet_detail.js";

export default function() {
	window.jQuery = window.$;
	window.TDGF_applyROT13 = applyROT13;
	window.TDGF_getColumnName = getColumnName;
	window.TDGF_injectMustache = injectMustache;
	window.TDGF_onPropertiesUpdated = triggerPropertiesUpdated;
	window.TDGF_prioritizeNewestEvent = prioritizeNewestEvent;
	window.TDGF_registerPropertyUpdateCallback = registerPropertyUpdateCallback;
	window.TDGF_reload = reloadBrowser;
	window.TDGF_reloadColumns = reloadColumns;
	window.TDGF_showTweetDetail = showTweetDetail;
};
