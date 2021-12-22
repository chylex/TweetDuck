import { $TDX } from "../api/bridge.js";
import { replaceFunction } from "./globals/patch_functions.js";

/**
 * Adds support for sound notification settings.
 */
export default function() {
	/**
	 * @param {boolean} isCustom
	 * @param {number} volume
	 */
	window.TDGF_setSoundNotificationData = function(isCustom, volume) {
		/** @type {HTMLAudioElement} */
		const audio = document.getElementById("update-sound");
		audio.volume = volume / 100;
		
		const customSourceId = "tduck-custom-sound-source";
		const existingCustomSource = document.getElementById(customSourceId);
		
		if (isCustom && !existingCustomSource) {
			const newCustomSource = document.createElement("source");
			newCustomSource.id = customSourceId;
			newCustomSource.src = "https://ton.twimg.com/tduck/updatesnd";
			audio.prepend(newCustomSource);
		}
		else if (!isCustom && existingCustomSource) {
			audio.removeChild(existingCustomSource);
		}
		
		audio.load();
	};
	
	window.TDGF_playSoundNotification = function() {
		document.getElementById("update-sound").play();
	};
	
	replaceFunction(HTMLAudioElement.prototype, "play", function(func, args) {
		if (!$TDX.muteNotifications) {
			func.apply(this, args);
		}
	});
};
