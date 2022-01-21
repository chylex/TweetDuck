import { TD } from "../../api/td.js";
import { checkPropertyExists, noop } from "../../api/utils.js";

function isSupported() {
	return checkPropertyExists(TD, "controller", "clients", "getPreferredClient") &&
	       checkPropertyExists(TD, "services", "TwitterClient", "prototype", "API_BASE_URL") &&
	       checkPropertyExists(TD, "services", "TwitterClient", "prototype", "makeTwitterCall") &&
	       checkPropertyExists(TD, "services", "TwitterClient", "prototype", "processUsers") &&
	       checkPropertyExists(TD, "services", "TwitterUser", "prototype");
}

/**
 * @type {function(id: string)[]}
 */
const nftUserListeners = [];

/**
 * @type {Map<string, boolean>}
 */
const knownStatus = new Map();

/**
 * @type {Map<string, function(result: boolean)[]>}
 */
const deferredCallbacks = new Map();

/**
 * @type {Set<string>}
 */
const usersInQueue = new Set();

/**
 * @type {Set<string>}
 */
const usersPending = new Set();

let checkTimer = -1;

function requestQueuedUserInfo() {
	if (usersInQueue.size === 0) {
		return;
	}
	
	const ids = [];
	
	for (const id of usersInQueue) {
		if (ids.length === 100) {
			break;
		}
		
		ids.push(id);
		usersInQueue.delete(id);
	}
	
	// noinspection JSUnusedGlobalSymbols
	const data = {
		user_id: ids.join(",")
	};
	
	/**
	 * @param {TwitterUserJSON[]} users
	 */
	const processUserData = function(users) {
		for (const user of users) {
			setUserNftStatus(user.id_str, user.ext_has_nft_avatar === true);
		}
	};
	
	const client = TD.controller.clients.getPreferredClient();
	
	client.makeTwitterCall(client.API_BASE_URL + "users/lookup.json?include_ext_has_nft_avatar=1", data, "POST", processUserData, noop, function() {
		// In case of API error, assume the users are not associated with NFTs so that callbacks can fire.
		for (const id of ids) {
			setUserNftStatus(id, false);
		}
	});
	
	if (usersInQueue.size === 0) {
		checkTimer = -1;
	}
	else {
		checkTimer = window.setTimeout(requestQueuedUserInfo, 400);
	}
}

/**
 * Calls the provided callback function with the result of whether a user id is associated with NFTs.
 * If the user id is null, it will be presumed as not associated with NFTs.
 * @param {string|null} id
 * @param {function(nft: boolean)} [callback]
 */
function checkUserNftStatusCallback(id, callback) {
	if (id === null) {
		callback && callback(false);
		return;
	}
	
	const status = knownStatus.get(id);
	
	if (status !== undefined) {
		callback && callback(status);
		return;
	}
	
	if (callback) {
		let callbackList = deferredCallbacks.get(id);
		if (callbackList === undefined) {
			deferredCallbacks.set(id, callbackList = []);
		}
		
		callbackList.push(callback);
	}
	
	if (usersPending.has(id)) {
		return;
	}
	
	usersInQueue.add(id);
	usersPending.add(id);
	
	window.clearTimeout(checkTimer);
	checkTimer = window.setTimeout(requestQueuedUserInfo, 400);
}

/**
 * Checks whether a user id is associated with NFTs, but only using already known results.
 * If the user id is null or has not been checked yet, it will be presumed as not associated with NFTs.
 * @param {string|null} id
 * @return {boolean}
 */
export function checkUserNftStatusImmediately(id) {
	return id !== null && knownStatus.get(id) === true;
}

/**
 * Adds a listener that gets called when a user is added to the list of users associated with NFTs.
 * If some users were already known to be associated with NFTs before registering the listener, the listener will be called for every user.
 * @param {function(id: string)} listener
 */
export function addNftUserListener(listener) {
	nftUserListeners.push(listener);
	
	for (const entry of knownStatus.entries()) {
		if (entry[1]) {
			listener(entry[0]);
		}
	}
}

/**
 * Sets whether a user id is associated with NFTs.
 * @param {string} id
 * @param {boolean} status
 */
export function setUserNftStatus(id, status) {
	usersInQueue.delete(id);
	usersPending.delete(id);
	
	if (knownStatus.get(id) !== status) {
		knownStatus.set(id, status);
		
		if (status) {
			for (const listener of nftUserListeners) {
				try {
					listener(id);
				} catch (e) {
					console.error("Error in NFT user listener: " + e);
				}
			}
		}
	}
	
	if (deferredCallbacks.has(id)) {
		for (const callback of deferredCallbacks.get(id)) {
			callback(status);
		}
		
		deferredCallbacks.delete(id);
	}
}

/**
 * Calls the provided callback function with the result of whether a user id is associated with NFTs.
 * @param {string} id
 * @param {function(nft: boolean)} [callback]
 */
export const checkUserNftStatus = isSupported() ? checkUserNftStatusCallback : function(id, callback) {
	callback && callback(false);
};

/**
 * Utility function that returns the user id from a tweet.
 * @param {ChirpBase} tweet
 * @returns {string|null}
 */
export function getTweetUserId(tweet) {
	const user = tweet.user;
	return typeof user === "object" && typeof user.id === "string" ? user.id : null;
}

/**
 * Clears known status of users who are not associated with NFTs, in case they became associated with NFTs in the meantime.
 */
window.setInterval(function() {
	for (const entry of knownStatus.entries()) {
		if (!entry[1]) {
			knownStatus.delete(entry[0]);
		}
	}
}, 1000 * 60 * 60);
