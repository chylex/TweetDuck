import { $TD } from "../api/bridge.js";

/**
 * Handles clicking on the 'Show this thread' link.
 */
export default function() {
	document.getElementById("tduck-show-thread")?.addEventListener("click", function(){
		$TD.showTweetDetail();
	});
};
