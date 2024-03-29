import { $TDX } from "../api/bridge.js";
import { replaceFunction } from "../api/patch.js";
import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * Sets language for automatic translations.
 */
export default function() {
	ensurePropertyExists(TD, "languages");
	
	replaceFunction(TD.languages, "getSystemLanguageCode", function(func, args) {
		const [ returnShortCode ] = args;
		return returnShortCode ? ($TDX.translationTarget || "en") : func.apply(this, args);
	});
};
