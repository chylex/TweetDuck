/**
 * Adds a class to the <body> element when hovering the notification.
 */
export default function() {
	document.body.addEventListener("mouseenter", function(){
		document.body.classList.add("td-hover");
	});
	
	document.body.addEventListener("mouseleave", function(){
		document.body.classList.remove("td-hover");
	});
};
