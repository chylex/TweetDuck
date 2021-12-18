import { $TD } from "../api/bridge.js";
import { $, $$ } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { prioritizeNewestEvent } from "../globals/prioritize_newest_event.js";

const openSearchExternally = function(event, input) {
	$TD.openBrowser("https://twitter.com/search/?q=" + encodeURIComponent(input.val() || ""));
	event.preventDefault();
	event.stopPropagation();
	
	input.val("").blur();
	document.querySelector(".js-app").click(); // unfocus everything
};

/**
 * Submitting search queries while holding Ctrl or by middle-clicking the search icon opens the search externally.
 */
export default function() {
	onAppReady(function setupExternalSearchEvents() {
		$$(".js-app-search-input").on("keydown", function(e) {
			(e.ctrlKey && e.keyCode === 13) && openSearchExternally(e, $(this)); // enter
		});
		
		$$(".js-perform-search").on("click auxclick", function(e) {
			(e.ctrlKey || e.button === 1) && openSearchExternally(e, $(".js-app-search-input:visible"));
		}).each(function() {
			prioritizeNewestEvent($(this)[0], "click");
		});
		
		$$("[data-action='show-search']").on("click auxclick", function(e) {
			(e.ctrlKey || e.button === 1) && openSearchExternally(e, $());
		});
	});
};
