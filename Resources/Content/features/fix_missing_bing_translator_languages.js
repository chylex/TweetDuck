import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";
import { replaceFunction } from "../globals/patch_functions.js";

/**
 * Adds missing languages for Bing Translator (Bengali, Icelandic, Tagalog, Tamil, Telugu, Urdu).
 */
export default function() {
	ensurePropertyExists(TD, "languages", "getSupportedTranslationSourceLanguages");
	
	const newCodes = [ "bn", "is", "tl", "ta", "te", "ur" ];
	const codeSet = new Set(TD.languages.getSupportedTranslationSourceLanguages());
	
	for (const lang of newCodes) {
		codeSet.add(lang);
	}
	
	const codeList = [ ...codeSet ];
	
	replaceFunction(TD.languages, "getSupportedTranslationSourceLanguages", function() {
		return codeList;
	});
};
