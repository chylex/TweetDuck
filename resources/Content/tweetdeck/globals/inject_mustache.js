import { TD } from "../../api/td.js";
import { crashDebug } from "../../api/utils.js";

/**
 * Injects custom HTML into mustache templates.
 * @param {string} name
 * @param {"replace"|"append"|"prepend"} operation
 * @param {string} search
 * @param {string} custom
 * @returns {boolean}
 */
export function injectMustache(name, operation, search, custom) {
	let replacement;
	
	switch (operation) {
		case "replace":
			replacement = custom;
			break;
		
		case "append":
			replacement = search + custom;
			break;
		
		case "prepend":
			replacement = custom + search;
			break;
		
		default:
			throw "Invalid mustache injection operation. Only 'replace', 'append', 'prepend' are supported.";
	}
	
	const prev = TD.mustaches?.[name];
	
	if (!prev) {
		crashDebug("Mustache injection is referencing an invalid mustache: " + name);
		return false;
	}
	
	TD.mustaches[name] = prev.replace(search, replacement);
	
	if (prev === TD.mustaches[name]) {
		crashDebug("Mustache injection had no effect: " + name);
		return false;
	}
	
	return true;
}
