const columnIconsToNames = {
	"icon-home": "Home",
	"icon-mention": "Mentions",
	"icon-message": "Messages",
	"icon-notifications": "Notifications",
	"icon-follow": "Followers",
	"icon-activity": "Activity",
	"icon-favorite": "Likes",
	"icon-user": "User",
	"icon-search": "Search",
	"icon-list": "List",
	"icon-custom-timeline": "Timeline",
	"icon-dataminr": "Dataminr",
	"icon-play-video": "Live video",
	"icon-schedule": "Scheduled"
};

/**
 * Returns the display name of a column, or an empty string if the column type is unknown.
 * @param {TD_Column} column
 * @returns {string}
 */
export function getColumnName(column) {
	return columnIconsToNames[column._tduck_icon] || "";
}
