import { TD } from "../api/td.js";

/**
 * Returns an object containing data about the column below the cursor.
 * @returns {{ ele: Element, obj: TD_Column }|null}
 */
export function getHoveredColumn() {
	const hovered = document.querySelectorAll(":hover");
	
	for (let index = hovered.length - 1; index >= 0; index--) {
		const ele = hovered[index];
		
		if (ele.tagName === "SECTION" && ele.classList.contains("js-column")) {
			const obj = TD.controller.columnManager.get(ele.getAttribute("data-column"));
			
			if (obj) {
				return { ele, obj };
			}
		}
	}
	
	return null;
}
