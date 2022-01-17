import { $TDX } from "../api/bridge.js";
import { replaceFunction } from "../api/patch.js";
import { isAppReady } from "../api/ready.js";

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
	
	let forcePlayNotification = false;
	
	window.TDGF_playSoundNotification = function(force) {
		forcePlayNotification = force;
		document.getElementById("update-sound").play();
		forcePlayNotification = false;
	};
	
	replaceFunction(HTMLAudioElement.prototype, "play", function(func, args) {
		if ((!$TDX.muteNotifications || forcePlayNotification) && isAppReady()) {
			func.apply(this, args);
		}
	});
};
