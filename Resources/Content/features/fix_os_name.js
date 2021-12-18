import { TD } from "../api/td.js";
import { ensurePropertyExists } from "../api/utils.js";

/**
 * Fixes broken OS detection.
 */
export default function() {
	const doc = document.documentElement;
	doc.classList.remove("os-");
	doc.classList.add("os-windows");
	
	ensurePropertyExists(TD, "util", "getOSName");
	
	TD.util.getOSName = function() {
		return "windows";
	};
};
