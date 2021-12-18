(function() {
	const features = [
		"add_tweetduck_to_settings_menu",
		"bypass_t.co_links",
		"clear_search_input",
		"configure_first_day_of_week",
		"configure_language_for_translations",
		"disable_clipboard_formatting",
		"disable_td_metrics",
		"drag_links_onto_columns",
		"expand_links_or_show_tooltip",
		"fix_dm_input_box_focus",
		"fix_horizontal_scrolling_of_column_container",
		"fix_marking_dm_as_read_when_replying",
		"fix_media_preview_urls",
		"fix_missing_bing_translator_languages",
		"fix_os_name",
		"fix_scheduled_tweets_not_appearing",
		"fix_youtube_previews",
		"focus_composer_after_alt_tab",
		"focus_composer_after_image_upload",
		"focus_composer_after_switching_account",
		"handle_extra_mouse_buttons",
		"hook_theme_settings",
		"inject_css",
		"keep_like_follow_dialogs_open",
		"limit_loaded_dm_count",
		"make_retweets_lowercase",
		"middle_click_tweet_icon_actions",
		"move_accounts_above_hashtags_in_search",
		"offline_notification",
		"open_search_externally",
		"open_search_in_first_column",
		"paste_images_from_clipboard",
		"perform_search",
		"pin_composer_icon",
		"ready_plugins",
		"register_composer_active_event",
		"register_global_functions",
		"restore_cleared_column",
		"screenshot_tweet",
		"setup_column_type_attributes",
		"setup_desktop_notifications",
		"setup_link_context_menu",
		"setup_sound_notifications",
		"setup_tweet_context_menu",
		"setup_tweetduck_account_bamboozle",
		"setup_video_player",
		"skip_pre_login_page",
	];
	
	document.documentElement.id = "tduck";
	window.jQuery = window.$;
	
	const script = document.createElement("script");
	script.id = "tweetduck-bootstrap";
	script.type = "text/javascript";
	script.async = false;
	script.src = "td://resources/bootstrap.js";
	script.setAttribute("data-features", features.join("|"));
	document.head.appendChild(script);
})();
