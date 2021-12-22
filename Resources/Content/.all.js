// noinspection ES6UnusedImports,JSUnusedLocalSymbols

/*
 * Imports all root modules and global functions to mark them as used.
 */

import add_tweetduck_to_settings_menu from "./features/add_tweetduck_to_settings_menu.js";
import bypass_tco_links from "./features/bypass_t.co_links.js";
import clear_search_input from "./features/clear_search_input.js";
import configure_first_day_of_week from "./features/configure_first_day_of_week.js";
import configure_language_for_translations from "./features/configure_language_for_translations.js";
import disable_clipboard_formatting from "./features/disable_clipboard_formatting.js";
import disable_td_metrics from "./features/disable_td_metrics.js";
import drag_links_onto_columns from "./features/drag_links_onto_columns.js";
import expand_links_or_show_tooltip from "./features/expand_links_or_show_tooltip.js";
import fix_dm_input_box_focus from "./features/fix_dm_input_box_focus.js";
import fix_horizontal_scrolling_of_column_container from "./features/fix_horizontal_scrolling_of_column_container.js";
import fix_marking_dm_as_read_when_replying from "./features/fix_marking_dm_as_read_when_replying.js";
import fix_media_preview_urls from "./features/fix_media_preview_urls.js";
import fix_missing_bing_translator_languages from "./features/fix_missing_bing_translator_languages.js";
import fix_os_name from "./features/fix_os_name.js";
import fix_scheduled_tweets_not_appearing from "./features/fix_scheduled_tweets_not_appearing.js";
import fix_youtube_previews from "./features/fix_youtube_previews.js";
import focus_composer_after_alt_tab from "./features/focus_composer_after_alt_tab.js";
import focus_composer_after_image_upload from "./features/focus_composer_after_image_upload.js";
import focus_composer_after_switching_account from "./features/focus_composer_after_switching_account.js";
import handle_extra_mouse_buttons from "./features/handle_extra_mouse_buttons.js";
import hook_theme_settings from "./features/hook_theme_settings.js";
import inject_css from "./features/inject_css.js";
import keep_like_follow_dialogs_open from "./features/keep_like_follow_dialogs_open.js";
import limit_loaded_dm_count from "./features/limit_loaded_dm_count.js";
import make_retweets_lowercase from "./features/make_retweets_lowercase.js";
import middle_click_tweet_icon_actions from "./features/middle_click_tweet_icon_actions.js";
import move_accounts_above_hashtags_in_search from "./features/move_accounts_above_hashtags_in_search.js";
import offline_notification from "./features/offline_notification.js";
import open_search_externally from "./features/open_search_externally.js";
import open_search_in_first_column from "./features/open_search_in_first_column.js";
import paste_images_from_clipboard from "./features/paste_images_from_clipboard.js";
import perform_search from "./features/perform_search.js";
import pin_composer_icon from "./features/pin_composer_icon.js";
import ready_plugins from "./features/ready_plugins.js";
import register_composer_active_event from "./features/register_composer_active_event.js";
import register_global_functions from "./features/register_global_functions.js";
import restore_cleared_column from "./features/restore_cleared_column.js";
import screenshot_tweet from "./features/screenshot_tweet.js";
import setup_column_type_attributes from "./features/setup_column_type_attributes.js";
import setup_desktop_notifications from "./features/setup_desktop_notifications.js";
import setup_link_context_menu from "./features/setup_link_context_menu.js";
import setup_sound_notifications from "./features/setup_sound_notifications.js";
import setup_tweet_context_menu from "./features/setup_tweet_context_menu.js";
import setup_tweetduck_account_bamboozle from "./features/setup_tweetduck_account_bamboozle.js";
import setup_video_player from "./features/setup_video_player.js";
import skip_pre_login_page from "./features/skip_pre_login_page.js";
import hide_cookie_bar from "./login/hide_cookie_bar.js";

const globalFunctions = [
	window.TDGF_applyROT13,
	window.TDGF_getColumnName,
	window.TDGF_injectMustache,
	window.TDGF_onGlobalDragStart,
	window.TDGF_onMouseClickExtra,
	window.TDGF_onPropertiesUpdated,
	window.TDGF_performSearch,
	window.TDGF_playSoundNotification,
	window.TDGF_playVideo,
	window.TDGF_registerPropertyUpdateCallback,
	window.TDGF_reinjectCustomCSS,
	window.TDGF_reload,
	window.TDGF_reloadColumns,
	window.TDGF_setSoundNotificationData,
	window.TDGF_showTweetDetail,
	window.TDGF_triggerScreenshot,
	window.jQuery,
];
