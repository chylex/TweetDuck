import { $ } from "../api/jquery.js";
import { TD } from "../api/td.js";
import { checkPropertyExists } from "../api/utils.js";
import { replaceFunction } from "./globals/patch_functions.js";

/**
 * Allows restoring cleared columns by holding Shift.
 */
export default function() {
	let holdingShift = false;
	
	const updateShiftState = (pressed) => {
		if (holdingShift !== pressed) {
			holdingShift = pressed;
			$("button[data-action='clear']").children("span").text(holdingShift ? "Restore" : "Clear");
		}
	};
	
	document.addEventListener("keydown", function(e) {
		if (e.shiftKey && (document.activeElement === null || !("value" in document.activeElement))) {
			updateShiftState(true);
		}
	});
	
	document.addEventListener("keyup", function(e) {
		if (!e.shiftKey) {
			updateShiftState(false);
		}
	});
	
	if (checkPropertyExists(TD, "vo", "Column", "prototype")) {
		replaceFunction(TD.vo.Column.prototype, "clear", function(func, args) {
			window.setTimeout(function() {
				document.activeElement.blur(); // unfocuses the Clear button, otherwise it steals keyboard input
			}, 0);
			
			if (holdingShift) {
				this.model.setClearedTimestamp(0);
				this.reloadTweets();
			}
			else {
				func.apply(this, args);
			}
		});
	}
};
