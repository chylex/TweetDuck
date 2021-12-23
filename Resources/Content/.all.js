// noinspection ES6UnusedImports,JSUnusedLocalSymbols

/*
 * Imports all root modules and global functions to mark them as used.
 */

import introduction from "./introduction/introduction.js";
import hide_cookie_bar from "./login/hide_cookie_bar.js";
import setup_document_attributes from "./login/setup_document_attributes.js";
import add_skip_button from "./notification/add_skip_button.js";
import disable_clipboard_formatting_notification from "./notification/disable_clipboard_formatting.js";
import expand_links_or_show_tooltip_notification from "./notification/expand_links_or_show_tooltip.js";
import handle_links from "./notification/handle_links.js";
import handle_show_this_thread_link from "./notification/handle_show_this_thread_link.js";
import reset_scroll_position_on_load from "./notification/reset_scroll_position_on_load.js";
import scroll_smoothly from "./notification/scroll_smoothly.js";
import setup_body_hover_class from "./notification/setup_body_hover_class.js";
import add_tweetduck_to_settings_menu from "./tweetdeck/add_tweetduck_to_settings_menu.js";
import bypass_tco_links from "./tweetdeck/bypass_t.co_links.js";
import clear_search_input from "./tweetdeck/clear_search_input.js";
import configure_first_day_of_week from "./tweetdeck/configure_first_day_of_week.js";
import configure_language_for_translations from "./tweetdeck/configure_language_for_translations.js";
import disable_clipboard_formatting from "./tweetdeck/disable_clipboard_formatting.js";
import disable_td_metrics from "./tweetdeck/disable_td_metrics.js";
import drag_links_onto_columns from "./tweetdeck/drag_links_onto_columns.js";
import expand_links_or_show_tooltip from "./tweetdeck/expand_links_or_show_tooltip.js";
import fix_dm_input_box_focus from "./tweetdeck/fix_dm_input_box_focus.js";
import fix_horizontal_scrolling_of_column_container from "./tweetdeck/fix_horizontal_scrolling_of_column_container.js";
import fix_marking_dm_as_read_when_replying from "./tweetdeck/fix_marking_dm_as_read_when_replying.js";
import fix_media_preview_urls from "./tweetdeck/fix_media_preview_urls.js";
import fix_missing_bing_translator_languages from "./tweetdeck/fix_missing_bing_translator_languages.js";
import fix_os_name from "./tweetdeck/fix_os_name.js";
import fix_scheduled_tweets_not_appearing from "./tweetdeck/fix_scheduled_tweets_not_appearing.js";
import fix_youtube_previews from "./tweetdeck/fix_youtube_previews.js";
import focus_composer_after_alt_tab from "./tweetdeck/focus_composer_after_alt_tab.js";
import focus_composer_after_image_upload from "./tweetdeck/focus_composer_after_image_upload.js";
import focus_composer_after_switching_account from "./tweetdeck/focus_composer_after_switching_account.js";
import handle_extra_mouse_buttons from "./tweetdeck/handle_extra_mouse_buttons.js";
import hook_theme_settings from "./tweetdeck/hook_theme_settings.js";
import inject_css from "./tweetdeck/inject_css.js";
import keep_like_follow_dialogs_open from "./tweetdeck/keep_like_follow_dialogs_open.js";
import limit_loaded_dm_count from "./tweetdeck/limit_loaded_dm_count.js";
import make_retweets_lowercase from "./tweetdeck/make_retweets_lowercase.js";
import middle_click_tweet_icon_actions from "./tweetdeck/middle_click_tweet_icon_actions.js";
import move_accounts_above_hashtags_in_search from "./tweetdeck/move_accounts_above_hashtags_in_search.js";
import offline_notification from "./tweetdeck/offline_notification.js";
import open_search_externally from "./tweetdeck/open_search_externally.js";
import open_search_in_first_column from "./tweetdeck/open_search_in_first_column.js";
import paste_images_from_clipboard from "./tweetdeck/paste_images_from_clipboard.js";
import perform_search from "./tweetdeck/perform_search.js";
import pin_composer_icon from "./tweetdeck/pin_composer_icon.js";
import ready_plugins from "./tweetdeck/ready_plugins.js";
import register_composer_active_event from "./tweetdeck/register_composer_active_event.js";
import register_global_functions from "./tweetdeck/register_global_functions.js";
import register_global_functions_jquery from "./tweetdeck/register_global_functions_jquery.js";
import restore_cleared_column from "./tweetdeck/restore_cleared_column.js";
import screenshot_tweet from "./tweetdeck/screenshot_tweet.js";
import setup_column_type_attributes from "./tweetdeck/setup_column_type_attributes.js";
import setup_desktop_notifications from "./tweetdeck/setup_desktop_notifications.js";
import setup_link_context_menu from "./tweetdeck/setup_link_context_menu.js";
import setup_sound_notifications from "./tweetdeck/setup_sound_notifications.js";
import setup_tweet_context_menu from "./tweetdeck/setup_tweet_context_menu.js";
import setup_tweetduck_account_bamboozle from "./tweetdeck/setup_tweetduck_account_bamboozle.js";
import setup_video_player from "./tweetdeck/setup_video_player.js";
import skip_pre_login_page from "./tweetdeck/skip_pre_login_page.js";
import update from "./update/update.js";

const globalFunctions = [
	window.PluginBase,
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
	window.TDGF_scrollSmoothly,
	window.TDGF_setSoundNotificationData,
	window.TDGF_showTweetDetail,
	window.TDGF_triggerScreenshot,
	window.TDPF_configurePlugin,
	window.TDPF_createCustomStyle,
	window.TDPF_createStorage,
	window.TDPF_loadConfigurationFile,
	window.TDPF_playVideo,
	window.TDPF_registerPropertyUpdateCallback,
	window.TDPF_requestReload,
	window.TDPF_setPluginState,
	window.TDUF_displayNotification,
	window.TD_PLUGINS_INSTALL,
	window.jQuery,
];
