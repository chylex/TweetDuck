import { $, ensureEventExists } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";

/**
 * Refocuses composer input after uploading an image.
 */
export default function() {
	onAppReady(function focusComposerAfterImageUpload() {
		ensureEventExists(document, "uiComposeImageAdded");
		
		$(document).on("uiComposeImageAdded", function() {
			document.querySelector(".js-docked-compose .js-compose-text").focus();
		});
	});
};
