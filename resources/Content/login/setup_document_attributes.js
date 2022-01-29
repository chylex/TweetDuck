/**
 * Sets up attributes on the <html> element for styling login/logout pages.
 */
export default function() {
	if (location.pathname === "/login") {
		document.documentElement.setAttribute("login", "");
	}
	else if (location.pathname === "/logout") {
		document.documentElement.setAttribute("logout", "");
	}
};
