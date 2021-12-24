/**
 * @param {number} seconds
 * @returns {string}
 */
function formatRelativeTime(seconds) {
	if (seconds < 0) {
		return "";
	}
	else if (seconds < 60) {
		return seconds + "s";
	}
	
	const minutes = Math.floor(seconds / 60);
	if (minutes < 60) {
		return minutes + "m";
	}
	
	const hours = Math.floor(minutes / 60);
	if (hours < 24) {
		return hours + "h";
	}
	
	const days = Math.floor(hours / 24);
	return days + "d";
}

/**
 * @param {HTMLElement} textElement
 * @param {number} sentTime
 */
function refreshTime(textElement, sentTime) {
	const seconds = Math.floor((Date.now() - sentTime) / 1000);
	const newText = formatRelativeTime(seconds);
	
	if (textElement.innerText !== newText) {
		textElement.innerText = newText;
	}
	
	const refreshInMillis = seconds < 60 ? 100 : 60000;
	setTimeout(() => refreshTime(textElement, sentTime), refreshInMillis);
}

/**
 * Recalculates how much time has passed since a tweet was sent, so that the relative time shown in the notification stays up to date.
 */
export default function() {
	const timeElement = document.querySelector("time[data-time]");
	const textElement = timeElement?.querySelector("a");
	
	if (textElement) {
		const sentTime = parseInt(timeElement.getAttribute("data-time"), 10);
		refreshTime(textElement, sentTime);
	}
};
