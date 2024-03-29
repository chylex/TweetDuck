if (!("$TD" in window)) {
	throw "Missing $TD in global scope";
}

if (!("$TDX" in window)) {
	throw "Missing $TDX in global scope";
}

/**
 * @typedef TD_Bridge
 * @type {Object}
 *
 * @property {function(type: "error"|"warning"|"info"|"", contents: string)} alert
 * @property {function(message: string)} crashDebug
 * @property {function(tooltip: string|null)} displayTooltip
 * @property {function} fixClipboard
 * @property {function} loadNextNotification
 * @property {function(fontSize: string, headLayout: string)} loadNotificationLayout
 * @property {function(namespace: string)} onModulesLoaded
 * @property {function(columnId: string, chirpId: string, columnName: string, tweetHtml: string, tweetCharacters: int, tweetUrl: string, quoteUrl: string)} onTweetPopup
 * @property {function} onTweetSound
 * @property {function(url: string)} openBrowser
 * @property {function} openContextMenu
 * @property {function(showGuide: boolean)} onIntroductionClosed
 * @property {function(videoUrl: string, tweetUrl: string, username: string, callback: function)} playVideo
 * @property {function(columnId: string, chirpId: string, tweetUrl: string, quoteUrl: string, chirpAuthors: string, chirpImages: string)} setRightClickedChirp
 * @property {function(type: string, url: string)} setRightClickedLink
 * @property {function} showTweetDetail
 * @property {function(html: string, width: number)} screenshotTweet
 * @property {function} stopVideo
 */

/**
 * @typedef TD_Properties
 * @type {Object}
 *
 * @property {boolean} [expandLinksOnHover]
 * @property {number} [firstDayOfWeek]
 * @property {boolean} [focusDmInput]
 * @property {boolean} [hideTweetsByNftUsers]
 * @property {boolean} [keepLikeFollowDialogsOpen]
 * @property {boolean} [muteNotifications]
 * @property {boolean} [notificationMediaPreviews]
 * @property {boolean} [openSearchInFirstColumn]
 * @property {boolean} [skipOnLinkClick]
 * @property {string} [translationTarget]
 */

/** @type {TD_Bridge} */
export const $TD = window.$TD;

/** @type {TD_Properties} */
export const $TDX = window.$TDX;

/**
 * @type {function(TD_Properties)[]}
 */
const propertyUpdateCallbacks = [];

/**
 * Registers a callback that responds to `$TDX` property value changes.
 * @param {function(TD_Properties)} callback
 * @param {boolean} [callAfterRegistering] whether to call the callback immediately after registering
 */
export function registerPropertyUpdateCallback(callback, callAfterRegistering) {
	propertyUpdateCallbacks.push(callback);
	if (callAfterRegistering) {
		callback($TDX);
	}
}

/**
 * Triggers all registered callbacks.
 */
export function triggerPropertiesUpdated() {
	propertyUpdateCallbacks.forEach(func => func($TDX));
}
