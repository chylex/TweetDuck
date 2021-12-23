/**
 * Works around broken smooth scrolling in Chromium/CEF.
 */
export default function() {
	let targetY = 0;
	let delay = -1;
	let scrolling = false;
	
	window.TDGF_scrollSmoothly = function(delta) {
		targetY += delta;
		
		if (targetY < 0) {
			targetY = 0;
		}
		else if (targetY > document.body.offsetHeight - window.innerHeight) {
			targetY = document.body.offsetHeight - window.innerHeight;
		}
		
		const prevY = window.scrollY;
		window.scrollTo({ top: targetY, left: window.scrollX, behavior: "smooth" });
		scrolling = true;
		
		const diff = Math.abs(targetY - prevY);
		const time = 420 * (Math.log(diff + 510) - 6);
		
		clearTimeout(delay);
		delay = setTimeout(() => scrolling = false, time);
	};
	
	window.addEventListener("scroll", function() {
		if (!scrolling) {
			targetY = window.scrollY;
		}
	});
};
