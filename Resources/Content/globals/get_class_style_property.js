/**
 * Retrieves the actual value of a CSS property of an element with the specified class.
 * @param {string} elementClass
 * @param {string} cssProperty
 * @returns {string}
 */
export function getClassStyleProperty(elementClass, cssProperty) {
	const column = document.createElement("div");
	column.classList.add(elementClass);
	column.style.display = "none";
	
	document.body.appendChild(column);
	const value = window.getComputedStyle(column).getPropertyValue(cssProperty);
	document.body.removeChild(column);
	
	return value;
}
