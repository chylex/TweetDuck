import { getHoveredColumn } from "./get_hovered_column.js";

/**
 * Returns an object containing data about the tweet below the cursor.
 * @returns {{ ele: Element, obj: ChirpBase, wrap: ChirpBase, column: { ele: Element, obj: TD_Column }}|null}
 */
export function getHoveredTweet() {
	const hovered = document.querySelectorAll(":hover");
	
	for (let index = hovered.length - 1; index >= 0; index--) {
		const ele = hovered[index];
		
		if (ele.tagName === "ARTICLE" && ele.classList.contains("js-stream-item") && ele.hasAttribute("data-account-key")) {
			const column = getHoveredColumn();
			
			if (column) {
				const wrap = column.obj.findChirp(ele.getAttribute("data-key"));
				const obj = column.obj.findChirp(ele.getAttribute("data-tweet-id")) || wrap;
				
				if (obj) {
					return { ele, obj, wrap, column };
				}
			}
		}
	}
	
	return null;
}
