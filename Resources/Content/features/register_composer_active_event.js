import { $ } from "../api/jquery.js";

/**
 * Creates a `tduckOldComposerActive` event on the `document` object, which triggers when the composer is activated.
 */
export default function() {
	$(document).on("uiDrawerActive uiRwebComposerOptOut", function(e, /** @type {{ activeDrawer: string }} */ data) {
		if (e.type === "uiDrawerActive" && data.activeDrawer !== "compose") {
			return;
		}
		
		setTimeout(function() {
			$(document).trigger("tduckOldComposerActive");
		}, 0);
	});
};
