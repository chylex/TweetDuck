import { $TDX } from "../api/bridge.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { runAfterFunction } from "../globals/patch_functions.js";

function focusDmInput() {
	document.querySelector(".js-reply-tweetbox").focus();
}

/**
 * Fixes DM reply input box not getting focused after opening a conversation.
 */
export default function() {
	ensurePropertyExists(TD, "components", "ConversationDetailView", "prototype");
	
	runAfterFunction(TD.components.ConversationDetailView.prototype, "showChirp", function() {
		if ($TDX.focusDmInput) {
			setTimeout(focusDmInput, 100);
		}
	});
};
