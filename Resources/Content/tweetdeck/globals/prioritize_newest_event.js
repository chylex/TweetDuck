import { getEvents } from "../../api/jquery.js";

/**
 * Pushes the newest jQuery event to the beginning of the event handler list, so that it runs before anything else.
 * @param {EventTarget} element
 * @param {string} eventName
 */
export function prioritizeNewestEvent(element, eventName) {
	const events = getEvents(element);
	
	const handlers = events[eventName];
	const newHandler = handlers[handlers.length - 1];
	
	for (let index = handlers.length - 1; index > 0; index--) {
		handlers[index] = handlers[index - 1];
	}
	
	handlers[0] = newHandler;
}
