import { $, $$ } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";

const refocusInput = function() {
	document.querySelector(".js-docked-compose .js-compose-text").focus();
};

const accountItemClickEvent = function() {
	setTimeout(refocusInput, 0);
};

/**
 * Refocuses composer input after switching account.
 */
export default function() {
	onAppReady(function setupAccountSwitchRefocus() {
		$(document).on("tduckOldComposerActive", function() {
			$$(".js-account-list", ".js-docked-compose").delegate(".js-account-item", "click", accountItemClickEvent);
		});
	});
};
