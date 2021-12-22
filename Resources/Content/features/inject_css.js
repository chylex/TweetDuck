function createStyle(id, styles) {
	const ele = document.createElement("style");
	ele.id = id;
	ele.innerText = styles;
	document.head.appendChild(ele);
}

/**
 * Adds support for injecting CSS.
 */
export default function() {
	/**
	 * @param {string|null} styles
	 */
	window.TDGF_reinjectCustomCSS = function(styles) {
		document.getElementById("tweetduck-custom-css")?.remove();
		
		if (styles?.length) {
			createStyle("tweetduck-custom-css", styles);
		}
	};
};
