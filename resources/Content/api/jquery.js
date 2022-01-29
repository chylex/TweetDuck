if (!("$" in window)) {
	throw "Missing jQuery in global scope";
}

/** @type {JQuery} */
export const $ = window.$;

/**
 * Returns a jQuery object, or throws if no elements are found.
 * @param {JQuery.Selector} selector
 * @param {Element|string|null} [context]
 * @returns {JQuery}
 */
export const $$ = function(selector, context) {
	// noinspection JSValidateTypes
	const result = $(selector, context);
	
	if (!result.length) {
		throw "No elements were found for selector: " + selector;
	}
	
	return result;
};

/**
 * @typedef InternalJQueryData
 * @type {Object}
 *
 * @property {InternalJQueryEvents} events
 */

/**
 * @typedef InternalJQueryEvents
 * @type {Object<string, function[]>}
 */

/**
 * @param {EventTarget} target
 * @returns {InternalJQueryEvents}
 */
export function getEvents(target) {
	// noinspection JSUnresolvedFunction
	/** @type {InternalJQueryData} */
	const jqData = $._data(target);
	return jqData.events;
}

/**
 * Throws if an element does not have a registered jQuery event.
 * @param {EventTarget|HTMLElement} element
 * @param {string} eventName
 */
export function ensureEventExists(element, eventName) {
	if (!(eventName in getEvents(element))) {
		const tagName = element?.tagName.toLowerCase() ?? element?.nodeName;
		const classList = "classList" in element ? Array.from(element.classList).map(cls => `.${cls}`).join("") : "";
		throw `Missing jQuery event '${eventName}' in: ${tagName}${classList}`;
	}
}

/**
 * @param jq {JQuery}
 * @returns {HTMLElement|null}
 */
export function single(jq) {
	return jq.length === 1 ? jq[0] : null;
}
