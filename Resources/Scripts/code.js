(function($, $TD, $TDX, TD){
  //
  // Variable: Current highlighted column jQuery & data objects.
  //
  var highlightedColumnEle, highlightedColumnObj;
  
  //
  // Variable: Currently highlighted tweet jQuery & data objects.
  //
  var highlightedTweetEle, highlightedTweetObj;
  
  //
  // Variable: Array of functions called after the website app is loaded.
  //
  var onAppReady = [];
  
  //
  // Variable: DOM object containing the main app element.
  //
  var app = $(document.body).children(".js-app");
  
  //
  // Constant: Column types mapped to their titles.
  //
  const columnTypes = {
    "col_home": "Home",
    "col_timeline" : "Home",
    "col_mentions": "Mentions",
    "col_me": "Mentions",
    "col_inbox": "Messages",
    "col_messages": "Messages",
    "col_interactions": "Notifications",
    "col_followers": "Followers",
    "col_activity": "Activity",
    "col_favorites": "Likes",
    "col_usertweets": "User",
    "col_search": "Search",
    "col_list": "List",
    "col_customtimeline": "Timeline",
    "col_dataminr": "Dataminr",
    "col_livevideo": "Live video",
    "col_scheduled": "Scheduled"
  };
  
  //
  // Function: Prepends code at the beginning of a function. If the prepended function returns true, execution of the original function is cancelled.
  //
  var prependToFunction = function(func, extension){
    return function(){
      return extension.apply(this, arguments) === true ? undefined : func.apply(this, arguments);
    };
  };
  
  //
  // Function: Appends code at the end of a function.
  //
  var appendToFunction = function(func, extension){
    return function(){
      var res = func.apply(this, arguments);
      extension.apply(this, arguments);
      return res;
    };
  };
  
  //
  // Function: Retrieves a property of an element with a specified class.
  //
  var getClassStyleProperty = function(cls, property){
    let column = document.createElement("div");
    column.classList.add(cls);
    column.style.display = "none";
    
    document.body.appendChild(column);
    let value = window.getComputedStyle(column).getPropertyValue(property);
    document.body.removeChild(column);
    
    return value;
  };
  
  //
  // Function: Event callback for a new tweet.
  //
  var onNewTweet = function(column, tweet){
    if (column.model.getHasNotification()){
      let html = $(tweet.render({
        withFooter: false,
        withTweetActions: false,
        withMediaPreview: true,
        isMediaPreviewOff: true,
        isMediaPreviewSmall: false,
        isMediaPreviewLarge: false
      }));
      
      html.css("border", "0");
      html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
      html.find(".js-media").last().remove(); // and quoted tweets still show media previews, nice nice
      html.find(".js-quote-detail").removeClass("is-actionable"); // prevent quoted tweets from changing the cursor
      
      html.find("a[href='#']").each(function(){ // remove <a> tags around links that don't lead anywhere (such as account names the tweet replied to)
        this.outerHTML = this.innerHTML;
      });
      
      if (tweet.getChirpType().includes("list_member")){
        html.find(".activity-header").first().css("margin-top", "2px");
        html.find(".avatar").first().css("margin-bottom", "0");
      }
      
      let source = tweet.getRelatedTweet();
      let duration = source ? source.text.length+(source.quotedTweet ? source.quotedTweet.text.length : 0) : tweet.text.length;
      let tweetUrl = source ? source.getChirpURL() : "";
      let quoteUrl = source && source.quotedTweet ? source.quotedTweet.getChirpURL() : "";
      
      $TD.onTweetPopup(columnTypes[column.getColumnType()] || "", html.html(), duration, tweetUrl, quoteUrl);
    }
    
    if (column.model.getHasSound()){
      $TD.onTweetSound();
    }
  };
  
  //
  // Function: Retrieves the tags to be put into <head> for notification HTML code.
  //
  var getNotificationHeadContents = function(){
    let tags = [];
    
    $(document.head).children("link[rel='stylesheet']:not([title]),link[title='"+TD.settings.getTheme()+"'],meta[charset],meta[http-equiv]").each(function(){
      tags.push($(this)[0].outerHTML);
    });
    
    tags.push("<style type='text/css'>");
    tags.push("body { background: "+getClassStyleProperty("column", "background-color")+" }"); // set background color
    tags.push("a[data-full-url] { word-break: break-all }"); // break long urls
    tags.push(".txt-base-smallest .badge-verified:before { height: 13px !important }"); // fix cut off badge icon
    tags.push(".activity-header { align-items: center !important; margin-bottom: 4px }"); // tweak alignment of avatar and text in notifications
    tags.push(".activity-header .tweet-timestamp { line-height: unset }"); // fix timestamp position in notifications
    tags.push("</style>");
    
    return tags.join("");
  };
  
  //
  // Block: Hook into settings object to detect when the settings change.
  //
  TD.settings.setFontSize = appendToFunction(TD.settings.setFontSize, function(name){
    $TD.loadFontSizeClass(name);
  });
  
  TD.settings.setTheme = appendToFunction(TD.settings.setTheme, function(){
    setTimeout(function(){
      $TD.loadNotificationHeadContents(getNotificationHeadContents());
    }, 0);
  });
  
  //
  // Block: Enable popup notifications.
  //
  TD.controller.notifications.hasNotifications = function(){
    return true;
  };

  TD.controller.notifications.isPermissionGranted = function(){
    return true;
  };
  
  $.subscribe("/notifications/new", function(obj){
    for(let index = obj.items.length-1; index >= 0; index--){
      onNewTweet(obj.column, obj.items[index]);
    }
  });
  
  //
  // Block: Add TweetDuck buttons to the settings menu.
  //
  onAppReady.push(function(){
    $("[data-action='settings-menu']").click(function(){
      setTimeout(function(){
        var menu = $(".js-dropdown-content").children("ul").first();
        if (menu.length === 0)return;
        
        menu.children(".drp-h-divider").last().before('<li class="is-selectable" data-std><a href="#" data-action="tweetduck">TweetDuck</a></li>');
        
        var button = menu.children("[data-std]");

        button.on("click", "a", function(){
          $TD.openContextMenu();
        });

        button.hover(function(){
          $(this).addClass("is-selected");
        }, function(){
          $(this).removeClass("is-selected");
        });
      }, 0);
    });
  });
  
  //
  // Block: Expand shortened links on hover or display tooltip.
  //
  (function(){
    var cutStart = function(str, search){
      return str.startsWith(search) ? str.substr(search.length) : str;
    };
    
    var prevMouseX = -1, prevMouseY = -1;
    var tooltipTimer, tooltipDisplayed;
    
    $(document.body).delegate("a[data-full-url]", "mouseenter mouseleave mousemove", function(e){
      var me = $(this);

      if (e.type === "mouseenter"){
        var text = me.text();
        
        if (text.charCodeAt(text.length-1) !== 8230){ // horizontal ellipsis
          return;
        }
        
        if ($TDX.expandLinksOnHover){
          tooltipTimer = window.setTimeout(function(){
            var expanded = me.attr("data-full-url");
            expanded = cutStart(expanded, "https://");
            expanded = cutStart(expanded, "http://");
            expanded = cutStart(expanded, "www.");

            me.attr("td-prev-text", text);
            me.text(expanded);
          }, 200);
        }
        else{
          tooltipTimer = window.setTimeout(function(){
            $TD.displayTooltip(me.attr("data-full-url"), false);
            tooltipDisplayed = true;
          }, 400);
        }
      }
      else if (e.type === "mouseleave"){
        if ($TDX.expandLinksOnHover){
          var prevText = me.attr("td-prev-text");

          if (prevText){
            me.text(prevText);
          }
        }
        
        window.clearTimeout(tooltipTimer);
        
        if (tooltipDisplayed){
          tooltipDisplayed = false;
          $TD.displayTooltip(null, false);
        }
      }
      else if (e.type === "mousemove"){
        if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)){
          $TD.displayTooltip(me.attr("data-full-url"), false);
          prevMouseX = e.clientX;
          prevMouseY = e.clientY;
        }
      }
    });
  })();
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  $(document.body).delegate("a", "contextmenu", function(){
    $TD.setLastRightClickedLink($(this).attr("data-full-url") || "");
  });
  
  //
  // Block: Hook into the notification sound effect.
  //
  (function(){
    var soundEle = document.getElementById("update-sound");
    
    soundEle.play = prependToFunction(soundEle.play, function(){
      return $TDX.muteNotifications || $TDX.hasCustomNotificationSound;
    });
  })();
  
  //
  // Block: Update highlighted column and tweet for context menu and other functionality.
  //
  (function(){
    var lastTweet = "";
    
    var updateHighlightedColumn = function(ele){
      highlightedColumnEle = ele;
      highlightedColumnObj = ele ? TD.controller.columnManager.get(ele.attr("data-column")) : null;
      return !!highlightedColumnObj;
    };
    
    var updateHighlightedTweet = function(ele, obj, link, embeddedLink){
      highlightedTweetEle = ele;
      highlightedTweetObj = obj;
      
      if (lastTweet !== link){
        $TD.setLastHighlightedTweet(link, embeddedLink);
        lastTweet = link;
      }
    };
    
    app.delegate("section.js-column", "mouseenter mouseleave", function(e){
      if (e.type === "mouseenter"){
        if (!highlightedColumnObj){
          updateHighlightedColumn($(this));
        }
      }
      else if (e.type === "mouseleave"){
        updateHighlightedColumn(null);
      }
    });
    
    app.delegate("article.js-stream-item", "mouseenter mouseleave", function(e){
      if (e.type === "mouseenter"){
        var me = $(this);
        
        if (!me[0].hasAttribute("data-account-key") || (!highlightedColumnObj && !updateHighlightedColumn(me.closest("section.js-column")))){
          return;
        }
        
        var tweet = highlightedColumnObj.findChirp(me.attr("data-tweet-id")) || highlightedColumnObj.findChirp(me.attr("data-key"));
        
        if (tweet){
          if (tweet.chirpType === TD.services.ChirpBase.TWEET){
            var link = tweet.getChirpURL();
            var embedded = tweet.quotedTweet ? tweet.quotedTweet.getChirpURL() : "";

            updateHighlightedTweet(me, tweet, link || "", embedded || "");
          }
          else{
            updateHighlightedTweet(me, tweet, "", "");
          }
        }
      }
      else if (e.type === "mouseleave"){
        updateHighlightedTweet(null, null, "", "");
      }
    });
  })();
  
  //
  // Block: Screenshot tweet to clipboard.
  //
  (function(){
    var selectedTweet;
    
    var setImportantProperty = function(obj, property, value){
      if (obj.length === 1){
        obj[0].style.setProperty(property, value, "important");
      }
    };
    
    app.delegate("article.js-stream-item", "contextmenu", function(){
      selectedTweet = $(this);
    });
    
    window.TDGF_triggerScreenshot = function(){
      if (selectedTweet){
        var tweetWidth = Math.floor(selectedTweet.width());
        var parent = selectedTweet.parent();
        
        var isDetail = parent.hasClass("js-tweet-detail");
        var isReply = !isDetail && (parent.hasClass("js-replies-to") || parent.hasClass("js-replies-before"));
        
        selectedTweet = selectedTweet.clone();
        selectedTweet.children().first().addClass($(document.documentElement).attr("class")).css("padding-bottom", "0");
        
        setImportantProperty(selectedTweet.find(".js-tweet-text"), "margin-bottom", "8px");
        setImportantProperty(selectedTweet.find(".js-quote-detail"), "margin-bottom", "10px");
        setImportantProperty(selectedTweet.find(".js-poll-link").next(), "margin-bottom", "8px");
        
        if (isDetail){
          if (selectedTweet.find("[class*='media-grid-']").length > 0){
            setImportantProperty(selectedTweet.find(".js-tweet-media"), "margin-bottom", "10px");
          }
          else{
            setImportantProperty(selectedTweet.find(".js-tweet-media"), "margin-bottom", "6px");
          }
          
          setImportantProperty(selectedTweet.find(".js-media-preview-container"), "margin-bottom", "4px");
          selectedTweet.find(".js-translate-call-to-action").first().remove();
          selectedTweet.find(".js-tweet").first().nextAll().remove();
          selectedTweet.find("footer").last().prevUntil(":not(.txt-mute.txt-small)").addBack().remove(); // footer, date, location
        }
        else{
          setImportantProperty(selectedTweet.find(".js-media-preview-container"), "margin-bottom", "10px");
          selectedTweet.find("footer").last().remove();
        }
        
        if (isReply){
          selectedTweet.find(".is-conversation").removeClass("is-conversation");
        }
        
        selectedTweet.find(".js-poll-link").remove();
        selectedTweet.find(".td-screenshot-remove").remove();
        
        var testTweet = selectedTweet.clone().css({
          position: "absolute",
          left: "-999px",
          width: tweetWidth+"px"
        }).appendTo(document.body);
        
        var realHeight = Math.floor(testTweet.height());
        testTweet.remove();
        
        $TD.screenshotTweet(selectedTweet.html(), tweetWidth, realHeight);
      }
    };
  })();
  
  //
  // Block: Paste images when tweeting.
  //
  onAppReady.push(function(){
    var uploader = $._data(document, "events")["uiComposeAddImageClick"][0].handler.context;
    
    app.delegate(".js-compose-text,.js-reply-tweetbox", "paste", function(e){
      for(let item of e.originalEvent.clipboardData.items){
        if (item.type.startsWith("image/")){
          $(this).closest(".rpl").find(".js-reply-popout").click(); // popout direct messages
          uploader.addFilesToUpload([ item.getAsFile() ]);
          break;
        }
      }
    });
  });
  
  //
  // Block: Support for extra mouse buttons.
  //
  (function(){
    var tryClickSelector = function(selector, parent){
      return $(selector, parent).click().length;
    };
    
    var tryCloseModal = function(){
      var modal = $("#open-modal");
      return modal.is(":visible") && tryClickSelector("a[rel=dismiss]", modal);
    };
    
    var tryCloseHighlightedColumn = function(){
      if (highlightedColumnEle){
        var column = highlightedColumnEle.closest(".js-column");
        return (column.is(".is-shifted-2") && tryClickSelector(".js-tweet-social-proof-back", column)) || (column.is(".is-shifted-1") && tryClickSelector(".js-column-back", column));
      }
    };
    
    window.TDGF_onMouseClickExtra = function(button){
      if (button === 1){ // back button
        tryCloseModal() ||
        tryClickSelector(".js-inline-compose-close") ||
        tryCloseHighlightedColumn() ||
        tryClickSelector(".js-app-content.is-open .js-drawer-close:visible") ||
        tryClickSelector(".is-shifted-2 .js-tweet-social-proof-back, .is-shifted-2 .js-dm-participants-back") ||
        $(".js-column-back").click();
      }
      else if (button === 2){ // forward button
        if (highlightedTweetEle){
          highlightedTweetEle.children().first().click();
        }
      }
    };
  })();
  
  //
  // Block: Fix scheduled tweets not showing up sometimes.
  //
  $(document).on("dataTweetSent", function(e, data){
    if (data.response.state && data.response.state === "scheduled"){
      var column = Object.values(TD.controller.columnManager.getAll()).find(column => column.model.state.type === "scheduled");

      if (column){
        setTimeout(function(){
          column.reloadTweets();
        }, 1000);
      }
    }
  });
  
  //
  // Block: Hold Shift to reset cleared column.
  //
  (function(){
    var holdingShift = false;
    
    var updateShiftState = (pressed) => {
      if (pressed != holdingShift){
        holdingShift = pressed;
        $("button[data-action='clear']").children("span").text(holdingShift ? "Reset" : "Clear");
      }
    };
    
    var resetActiveFocus = () => {
      document.activeElement.blur();
    };
    
    $(document).keydown(function(e){
      if (e.shiftKey && (document.activeElement === null || !("value" in document.activeElement))){
        updateShiftState(true);
      }
    }).keyup(function(e){
      if (!e.shiftKey){
        updateShiftState(false);
      }
    });
    
    TD.vo.Column.prototype.clear = prependToFunction(TD.vo.Column.prototype.clear, function(){
      window.setTimeout(resetActiveFocus, 0); // unfocuses the Clear button, otherwise it steals keyboard input
      
      if (holdingShift){
        this.model.setClearedTimestamp(0);
        this.reloadTweets();
        return true;
      }
    });
  })();
  
  //
  // Block: Swap shift key functionality for selecting accounts.
  //
  onAppReady.push(function(){
    var toggleEventShiftKey = function(e){
      if ($TDX.switchAccountSelectors){
        e.shiftKey = !e.shiftKey;
      }
    };
    
    $(".js-drawer[data-drawer='compose']").delegate(".js-account-list > .js-account-item", "click", toggleEventShiftKey);

    TD.components.AccountSelector.prototype.refreshPostingAccounts = appendToFunction(TD.components.AccountSelector.prototype.refreshPostingAccounts, function(){
      if (!this.$node.attr("td-account-selector-hook")){
        this.$node.attr("td-account-selector-hook", "1");
        this.$node.delegate(".js-account-item", "click", toggleEventShiftKey);
      }
    });
  });
  
  //
  // Block: Work around clipboard HTML formatting.
  //
  $(document).on("copy", function(e){
    window.setTimeout($TD.fixClipboard, 0);
  });
  
  //
  // Block: Inject custom CSS and layout into the page.
  //
  (function(){
    var styleOfficial = document.createElement("style");
    document.head.appendChild(styleOfficial);
    
    styleOfficial.sheet.insertRule("a[data-full-url] { word-break: break-all; }", 0); // break long urls
    styleOfficial.sheet.insertRule(".column-nav-link .attribution { position: absolute; }", 0); // fix cut off account names
    styleOfficial.sheet.insertRule(".txt-base-smallest .sprite-verified-mini { width: 13px !important; height: 13px !important; background-position: -223px -99px !important; }", 0); // fix cut off badge icon when zoomed in
    styleOfficial.sheet.insertRule(".keyboard-shortcut-list { vertical-align: top; }", 0); // fix keyboard navigation alignment
    styleOfficial.sheet.insertRule(".sprite-logo { background-position: -5px -46px !important; }", 0); // fix TweetDeck logo on certain zoom levels
    styleOfficial.sheet.insertRule(".app-navigator .tooltip { display: none !important; }", 0); // hide broken tooltips in the menu
    styleOfficial.sheet.insertRule(".account-inline .username { vertical-align: 10%; }", 0); // move usernames a bit higher
    styleOfficial.sheet.insertRule(".activity-header { align-items: center !important; margin-bottom: 4px; }", 0); // tweak alignment of avatar and text in notifications
    styleOfficial.sheet.insertRule(".activity-header .tweet-timestamp { line-height: unset }"); // fix timestamp position in notifications
    
    styleOfficial.sheet.insertRule(".app-columns-container::-webkit-scrollbar-track { border-left: 0; }", 0); // remove weird border in the column container scrollbar
    styleOfficial.sheet.insertRule(".app-columns-container { bottom: 0 !important; }", 0); // move column container scrollbar to bottom to fit updated style
    
    styleOfficial.sheet.insertRule(".is-video a:not([href*='youtu']), .is-gif .js-media-gif-container { cursor: alias; }", 0); // change cursor on unsupported videos
    styleOfficial.sheet.insertRule(".is-video a:not([href*='youtu']) .icon-bg-dot, .is-gif .icon-bg-dot { color: #bd3d37; }", 0); // change play icon color on unsupported videos
    
    TD.services.TwitterActionRetweetedRetweet.prototype.iconClass = "icon-retweet icon-retweet-color txt-base-medium"; // fix retweet icon mismatch
    
    window.TDGF_reinjectCustomCSS = function(styles){
      $("#tweetduck-custom-css").remove();
      
      if (styles && styles.length){
        $(document.head).append("<style type='text/css' id='tweetduck-custom-css'>"+styles+"</style>");
      }
    };
  })();
  
  //
  // Block: Setup unsupported video element hook.
  //
  (function(){
    var cancelModal = false;
    
    TD.components.MediaGallery.prototype._loadTweet = appendToFunction(TD.components.MediaGallery.prototype._loadTweet, function(){
      var media = this.chirp.getMedia().find(media => media.mediaId === this.clickedMediaEntityId);

      if (media && media.isVideo && media.service !== "youtube"){
        $TD.openBrowser(this.clickedLink);
        cancelModal = true;
      }
    });

    TD.components.BaseModal.prototype.setAndShowContainer = prependToFunction(TD.components.BaseModal.prototype.setAndShowContainer, function(){
      if (cancelModal){
        cancelModal = false;
        return true;
      }
    });
    
    TD.ui.Column.prototype.playGifIfNotManuallyPaused = function(){};
    TD.mustaches["status/media_thumb.mustache"] = TD.mustaches["status/media_thumb.mustache"].replace("is-gif", "is-gif is-paused");
    
    app.delegate(".js-gif-play", "click", function(e){
      var parent = $(e.target).closest(".js-tweet").first();
      var link = (parent.hasClass("tweet-detail") ? parent.find("a[rel='url']") : parent.find("time").first().children("a")).first();
      
      $TD.openBrowser(link.attr("href"));
      e.stopPropagation();
    });
  })();
  
  //
  // Block: Fix youtu.be previews not showing up for https links.
  //
  TD.services.TwitterMedia.YOUTUBE_TINY_RE = new RegExp(TD.services.TwitterMedia.YOUTUBE_TINY_RE.source.replace("http:", "https?:"));
  TD.services.TwitterMedia.YOUTUBE_RE = new RegExp(TD.services.TwitterMedia.YOUTUBE_LONG_RE.source+"|"+TD.services.TwitterMedia.YOUTUBE_TINY_RE.source);
  TD.services.TwitterMedia.SERVICES["youtube"] = TD.services.TwitterMedia.YOUTUBE_RE;
  
  //
  // Block: Fix DM reply input box not getting focused after opening a conversation.
  //
  TD.components.ConversationDetailView.prototype.showChirp = appendToFunction(TD.components.ConversationDetailView.prototype.showChirp, function(){
    setTimeout(function(){
      $(".js-reply-tweetbox").first().focus();
    }, 100);
  });
  
  //
  // Block: Disable TweetDeck metrics.
  //
  TD.metrics.inflate = function(){};
  TD.metrics.inflateMetricTriple = function(){};
  TD.metrics.log = function(){};
  TD.metrics.makeKey = function(){};
  TD.metrics.send = function(){};
  
  onAppReady.push(function(){
    let data = $._data(window);
    delete data.events["metric"];
    delete data.events["metricsFlush"];
  });
  
  //
  // Block: Register the TD.ready event, finish initialization, and load plugins.
  //
  $(document).one("TD.ready", function(){
    onAppReady.forEach(func => func());
    
    $TD.loadFontSizeClass(TD.settings.getFontSize());
    $TD.loadNotificationHeadContents(getNotificationHeadContents());
    
    if (window.TD_PLUGINS){
      window.TD_PLUGINS.onReady();
    }
  });
  
  //
  // Block: Skip the initial pre-login page.
  //
  $(document).on("uiLoginFormImpression", function(){
    location.href = $("a.btn", ".js-login-form").first().attr("href");
  });
})($, $TD, $TDX, TD);
