(function() {
	document.documentElement.id = "tduck";
	
	const script = document.createElement("script");
	script.async = false;
	script.type = "text/javascript";
	script.id = "tweetduck-bootstrap-{namespace}";
	script.src = "td://resources/load.js";
	script.setAttribute("data-namespace", "{namespace}");
	script.setAttribute("data-modules", "{modules}");
	document.head.appendChild(script);
	
	const style = document.createElement("link");
	style.id = "tweetduck-styles-{namespace}";
	style.rel = "stylesheet";
	style.href = "td://resources/styles/{stylesheet}";
	document.head.appendChild(style);
})();
