for (const summary of document.getElementsByTagName("summary")) {
	summary.addEventListener("mousedown", function(e) {
		e.target.classList.add("webkit-workaround");
	});
	
	summary.addEventListener("mouseup", function(e) {
		e.target.classList.remove("webkit-workaround");
		e.target.blur();
	});
}

for (const link of document.getElementsByTagName("A")) {
	link.addEventListener("click", function(e) {
		e.preventDefault();
		window.open(link.getAttribute("href"));
	});
}

if (location.hash.length > 1) {
	const element = document.getElementById(location.hash.substring(1));
	
	if (element?.tagName === "SUMMARY") {
		element.click();
		element.blur();
		element.scrollIntoView(true);
		
		if (window.innerWidth === 0) {
			const ffs = function() {
				element.scrollIntoView(true);
				window.removeEventListener("resize", ffs);
			};
			
			window.addEventListener("resize", ffs);
		}
	}
}

window.addEventListener("hashchange", function() {
	location.reload();
});
