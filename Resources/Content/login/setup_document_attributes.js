export default function() {
	if (location.pathname === "/login") {
		document.documentElement.setAttribute("login", "");
	}
	else if (location.pathname === "/logout") {
		document.documentElement.setAttribute("logout", "");
	}
};
