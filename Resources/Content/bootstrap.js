(function() {
	document.documentElement.id = "tduck";
	
	const namespace = "{{namespace}}";
	const modules = "{{modules}}";
	const stylesheets = "{{stylesheets}}".split("|").filter(name => name.length);
	
	const script = document.createElement("script");
	script.async = false;
	script.type = "text/javascript";
	script.id = `tweetduck-bootstrap-${namespace}`;
	script.src = "td://resources/load.js";
	script.setAttribute("data-namespace", namespace);
	script.setAttribute("data-modules", modules);
	document.head.appendChild(script);
	
	for (const stylesheet of stylesheets) {
		const style = document.createElement("link");
		style.id = `tweetduck-styles-${namespace}-${stylesheet}`;
		style.rel = "stylesheet";
		style.href = `td://resources/${namespace}/${stylesheet}.css`;
		style.setAttribute("data-td-exclude-notification", "");
		document.head.appendChild(style);
	}
})();
