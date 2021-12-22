import { $ } from "../api/jquery.js";
import { onAppReady } from "../api/ready.js";
import { replaceFunction } from "./globals/patch_functions.js";
import { prioritizeNewestEvent } from "./globals/prioritize_newest_event.js";

/**
 * Refocuses composer input after Alt+Tab.
 */
export default function() {
	onAppReady(function fixDockedComposerRefocus() {
		$(document).on("tduckOldComposerActive", function() {
			const ele = document.querySelector(".js-docked-compose .js-compose-text");
			
			let cancelBlur = false;
			
			$(ele).on("blur", function() {
				cancelBlur = true;
				setTimeout(function() {
					cancelBlur = false;
				}, 0);
			});
			
			prioritizeNewestEvent(ele, "blur");
			
			replaceFunction(ele, "blur", function(func, args) {
				if (!cancelBlur) {
					func.apply(this, args);
				}
			});
		});
	});
};
