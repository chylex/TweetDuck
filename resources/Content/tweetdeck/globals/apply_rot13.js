// noinspection FunctionNamingConventionJS

/**
 * Applies the ROT13 cipher to the selected input text.
 */
export function applyROT13() {
	const activeElement = document.activeElement;
	const inputValue = activeElement?.value;
	if (!inputValue) {
		return;
	}
	
	const selection = inputValue.substring(activeElement.selectionStart, activeElement.selectionEnd);
	if (!selection) {
		return;
	}
	
	// noinspection JSDeprecatedSymbols
	document.execCommand("insertText", false, selection.replace(/[a-zA-Z]/g, function(chr) {
		const code = chr.charCodeAt(0);
		const start = code <= 90 ? 65 : 97;
		return String.fromCharCode(start + (code - start + 13) % 26);
	}));
}
