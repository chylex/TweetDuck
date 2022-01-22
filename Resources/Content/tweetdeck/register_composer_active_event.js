import { $ } from "../api/jquery.js";

/**
 * Creates a `tduckOldComposerActive` event on the `document` object, which triggers when the composer is activated.
 */
export default function() {
	/**
	 * @param e
	 * @param {{ activeDrawer: string }} data
	 */
	const onDrawerEvent = function(e, data) {
		if (e.type === "uiDrawerActive" && data.activeDrawer !== "compose") {
			return;
		}
		
		setTimeout(function() {
			$(document).trigger("tduckOldComposerActive");
		}, 0);
	};
	
	$(document).on("uiDrawerActive uiRwebComposerOptOut", onDrawerEvent);
};
