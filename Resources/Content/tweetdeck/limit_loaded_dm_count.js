// noinspection JSUnusedGlobalSymbols

import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { replaceFunction } from "./globals/patch_functions.js";

/**
 * Limits amount of loaded DMs to avoid massive lag from re-opening them several times.
 */
export default function() {
	ensurePropertyExists(TD, "services", "TwitterConversation", "prototype");

	replaceFunction(TD.services.TwitterConversation.prototype, "renderThread", /** @this TwitterConversation */ function(func, args) {
		const prevMessages = this.messages;
		
		this.messages = prevMessages.slice(0, 100);
		const result = func.apply(this, args);
		this.messages = prevMessages;
		
		return result;
	});
};
