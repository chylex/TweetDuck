/**
 * @typedef TD_Screenshot_Bridge
 * @type {Object}
 *
 * @property {function} triggerScreenshot
 * @property {function(number)} setHeight
 */

/**
 * @param $TDS
 */
(function($TDS) {
	const ele = document.getElementsByTagName("article")[0];
	ele.style.width = "{width}px";
	
	ele.style.position = "absolute";
	const contentHeight = ele.offsetHeight;
	ele.style.position = "static";
	
	const avatar = ele.querySelector(".tweet-avatar");
	const avatarBottom = avatar ? avatar.getBoundingClientRect().bottom : 0;
	
	$TDS.setHeight(Math.floor(Math.max(contentHeight, avatarBottom + 9))).then(() => {
		let framesLeft = 1/*FRAMES*/; // basic render is done in 1 frame, large media take longer
		
		const onNextFrame = function() {
			if (--framesLeft < 0) {
				$TDS.triggerScreenshot();
			}
			else {
				requestAnimationFrame(onNextFrame);
			}
		};
		
		onNextFrame();
	});
})(/** @type TD_Screenshot_Bridge */ $TD_NotificationScreenshot);
