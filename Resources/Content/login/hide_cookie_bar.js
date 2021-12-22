/**
 * Calls a callback once an element exists.
 * @param {string} selector
 * @param {function(HTMLElement)} callback
 */
const triggerWhenExists = function(selector, callback) {
	const id = window.setInterval(function() {
		const ele = document.querySelector(selector);
		
		if (ele && callback(ele)) {
			window.clearInterval(id);
		}
	}, 5);
};

/**
 * Hides cookie bar.
 */
export default function() {
	triggerWhenExists("a[href^='https://help.twitter.com/rules-and-policies/twitter-cookies']", function(cookie) {
		while (!!cookie) {
			if (cookie.offsetHeight > 30) {
				cookie.remove();
				return true;
			}
			else {
				cookie = cookie.parentNode;
			}
		}
		
		return false;
	});
};
