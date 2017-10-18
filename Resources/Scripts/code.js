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
  // Variable: DOM HTML element.
  //
  var doc = document.documentElement;
  
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
      let res = func.apply(this, arguments);
      extension.apply(this, arguments);
      return res;
    };
  };
  
  //
  // Function: Returns true if an object has a specified property, otherwise returns false without throwing an error.
  //
  var ensurePropertyExists = function(obj, ...chain){
    for(let index = 0; index < chain.length; index++){
      if (!obj.hasOwnProperty(chain[index])){
        $TD.crashDebug("Missing property "+chain[index]+" in chain [obj]."+chain.join("."));
        return false;
      }
      
      obj = obj[chain[index]];
    }
    
    return true;
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
  var onNewTweet = (function(){
    let recentMessages = new Set();
    let recentTweets = new Set();
    let recentTweetTimer = null;
    
    let resetRecentTweets = () => {
      recentTweetTimer = null;
      recentTweets.clear();
    };
    
    let startRecentTweetTimer = () => {
      recentTweetTimer && window.clearTimeout(recentTweetTimer);
      recentTweetTimer = window.setTimeout(resetRecentTweets, 20000);
    };
    
    let checkTweetCache = (set, id) => {
      return true if set.has(id);
      
      if (set.size > 50){
        set.clear();
      }
      
      set.add(id);
      return false;
    };
    
    let fixMedia = (html, media) => {
      return html.find(".js-media a[data-media-entity-id='"+media.mediaId+"']").css("background-image", 'url("'+media.thumb()+'")').removeClass("is-zoomable");
    };
    
    return function(column, tweet){
      if (tweet instanceof TD.services.TwitterConversation || tweet instanceof TD.services.TwitterConversationMessageEvent){
        return if checkTweetCache(recentMessages, tweet.id);
      }
      else{
        return if checkTweetCache(recentTweets, tweet.id);
      }
      
      startRecentTweetTimer();
      
      if (column.model.getHasNotification()){
        let sensitive = (tweet.getRelatedTweet() && tweet.getRelatedTweet().possiblySensitive || (tweet.quotedTweet && tweet.quotedTweet.possiblySensitive));
        let previews = $TDX.notificationMediaPreviews && (!sensitive || TD.settings.getDisplaySensitiveMedia());
        
        let html = $(tweet.render({
          withFooter: false,
          withTweetActions: false,
          withMediaPreview: true,
          isMediaPreviewOff: !previews,
          isMediaPreviewSmall: previews,
          isMediaPreviewLarge: false,
          isMediaPreviewCompact: false,
          isMediaPreviewInQuoted: previews,
          thumbSizeClass: "media-size-medium"
        }));
        
        html.css("border", "0");
        html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
        html.find(".js-quote-detail").removeClass("is-actionable margin-b--8"); // prevent quoted tweets from changing the cursor and reduce bottom margin
        
        if (previews){
          html.find(".reverse-image-search").remove();
          
          for(let media of tweet.getMedia()){
            fixMedia(html, media);
          }
          
          if (tweet.quotedTweet){
            for(let media of tweet.quotedTweet.getMedia()){
              fixMedia(html, media).addClass("media-size-medium");
            }
          }
        }
        else if (tweet instanceof TD.services.TwitterActionOnTweet){
          html.find(".js-media").remove();
        }
        
        html.find("a[href='#']").each(function(){ // remove <a> tags around links that don't lead anywhere (such as account names the tweet replied to)
          this.outerHTML = this.innerHTML;
        });
        
        let type = tweet.getChirpType();
        
        if (type === "follow"){
          html.find(".js-user-actions-menu").parent().remove();
          html.find(".account-bio").removeClass("padding-t--5").css("padding-top", "2px");
        }
        else if (type.includes("list_member")){
          html.find(".activity-header").css("margin-top", "2px");
          html.find(".avatar").first().css("margin-bottom", "0");
        }
        
        if (sensitive){
          html.find(".media-badge").each(function(){
            $(this)[0].lastChild.textContent += " (possibly sensitive)";
          });
        }
        
        let source = tweet.getRelatedTweet();
        let duration = source ? source.text.length+(source.quotedTweet ? source.quotedTweet.text.length : 0) : tweet.text.length;
        
        let chirpId = source ? source.id : "";
        let tweetUrl = source ? source.getChirpURL() : "";
        let quoteUrl = source && source.quotedTweet ? source.quotedTweet.getChirpURL() : "";

        $TD.onTweetPopup(column.model.privateState.apiid, chirpId, columnTypes[column.getColumnType()] || "", html.html(), duration, tweetUrl, quoteUrl);
      }

      if (column.model.getHasSound()){
        $TD.onTweetSound();
      }
    };
  })();
  
  //
  // Function: Shows tweet detail, used in notification context menu.
  //
  (function(){
    var showTweetDetailInternal = function(column, chirp){
      TD.ui.updates.showDetailView(column, chirp, column.findChirp(chirp) || chirp);
      TD.controller.columnManager.showColumn(column.model.privateState.key);

      $(document).trigger("uiGridClearSelection");
    };
    
    window.TDGF_showTweetDetail = function(columnId, chirpId, fallbackUrl){
      if (!TD.ready){
        onAppReady.push(function(){
          window.TDGF_showTweetDetail(columnId, chirpId, fallbackUrl);
        });
        
        return;
      }
      
      let column = TD.controller.columnManager.getByApiid(columnId);
      
      if (!column){
        if (confirm("error|The column which contained the tweet no longer exists. Would you like to open the tweet in your browser instead?")){
          $TD.openBrowser(fallbackUrl);
        }
        
        return;
      }
      
      let chirp = column.findMostInterestingChirp(chirpId);
      
      if (chirp){
        showTweetDetailInternal(column, chirp);
      }
      else{
        TD.controller.clients.getPreferredClient().show(chirpId, function(chirp){
          showTweetDetailInternal(column, chirp);
        }, function(){
          if (confirm("error|Could not retrieve the requested tweet. Would you like to open the tweet in your browser instead?")){
            $TD.openBrowser(fallbackUrl);
          }
        });
      }
    };
  })();
  
  //
  // Block: Hook into settings object to detect when the settings change, and update html attributes and notification layout.
  //
  (function(){
    let refreshSettings = function(){      
      let fontSizeName = TD.settings.getFontSize();
      let themeName = TD.settings.getTheme();
      
      let tags = [
        `<html class='os-windows ${(doc.getAttribute("class").match(/txt-\S+/) || [ "txt-size--14" ])[0]}' data-td-font='${fontSizeName}' data-td-theme='${themeName}'><head>`
      ];
      
      $(document.head).children("link[rel='stylesheet']:not([title]),link[title='"+themeName+"'],meta[charset],meta[http-equiv]").each(function(){
        tags.push($(this)[0].outerHTML);
      });
      
      tags.push("<style type='text/css'>");
      tags.push("body { background: "+getClassStyleProperty("column", "background-color")+" }"); // set background color
      tags.push("a[data-full-url] { word-break: break-all }"); // break long urls
      tags.push(".media-item, .media-preview { border-radius: 1px !important }"); // square-ify media
      tags.push(".quoted-tweet { border-radius: 0 !important }"); // square-ify quoted tweets
      tags.push(".activity-header { align-items: center !important; margin-bottom: 4px }"); // tweak alignment of avatar and text in notifications
      tags.push(".activity-header .tweet-timestamp { line-height: unset }"); // fix timestamp position in notifications
      
      if (fontSizeName === "smallest"){
        tags.push(".badge-verified:before { width: 13px !important; height: 13px !important; background-position: -223px -98px !important }"); // fix cut off badge icon
      }
      
      tags.push("</style>");
      
      doc.setAttribute("data-td-font", fontSizeName);
      doc.setAttribute("data-td-theme", themeName);
      $TD.loadNotificationLayout(fontSizeName, tags.join(""));
    };
    
    TD.settings.setFontSize = appendToFunction(TD.settings.setFontSize, function(name){
      setTimeout(refreshSettings, 0);
    });

    TD.settings.setTheme = appendToFunction(TD.settings.setTheme, function(name){
      setTimeout(refreshSettings, 0);
    });

    onAppReady.push(refreshSettings);
  })();
  
  //
  // Block: Fix OS name.
  //
  if (ensurePropertyExists(TD, "util", "getOSName")){
    TD.util.getOSName = function(){
      return "windows";
    };
    
    doc.classList.remove("os-");
    doc.classList.add("os-windows");
  }
  
  //
  // Block: Enable popup notifications.
  //
  if (ensurePropertyExists(TD, "controller", "notifications")){
    TD.controller.notifications.hasNotifications = function(){
      return true;
    };

    TD.controller.notifications.isPermissionGranted = function(){
      return true;
    };
  }
  
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
        let menu = $(".js-dropdown-content").children("ul").first();
        return if menu.length === 0;
        
        let button = $('<li class="is-selectable" data-tweetduck><a href="#" data-action>TweetDuck</a></li>');
        button.insertBefore(menu.children(".drp-h-divider").last());

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
    var prevMouseX = -1, prevMouseY = -1;
    var tooltipTimer, tooltipDisplayed;
    
    $(document.body).delegate("a[data-full-url]", {
      mouseenter: function(){
        let me = $(this);
        let text = me.text();
        return if text.charCodeAt(text.length-1) !== 8230; // horizontal ellipsis
        
        if ($TDX.expandLinksOnHover){
          tooltipTimer = window.setTimeout(function(){
            me.attr("td-prev-text", text);
            me.text(me.attr("data-full-url").replace(/^https?:\/\/(www\.)?/, ""));
          }, 200);
        }
        else{
          tooltipTimer = window.setTimeout(function(){
            $TD.displayTooltip(me.attr("data-full-url"), false);
            tooltipDisplayed = true;
          }, 400);
        }
      },
      
      mouseleave: function(){
        let me = $(this);
        
        if (me[0].hasAttribute("td-prev-text")){
          me.text(me.attr("td-prev-text"));
        }
        
        window.clearTimeout(tooltipTimer);
        
        if (tooltipDisplayed){
          tooltipDisplayed = false;
          $TD.displayTooltip(null, false);
        }
      },
      
      mousemove: function(e){
        if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)){
          $TD.displayTooltip($(this).attr("data-full-url"), false);
          prevMouseX = e.clientX;
          prevMouseY = e.clientY;
        }
      }
    });
  })();
  
  //
  // Block: Bypass t.co when clicking links and media.
  //
  $(document.body).delegate("a[data-full-url]", "click auxclick", function(e){
    if (e.button === 0 || e.button === 1){ // event.which seems to be borked in auxclick
      $TD.openBrowser($(this).attr("data-full-url"));
      e.preventDefault();
    }
  });
  
  if (ensurePropertyExists(TD, "services", "TwitterUser", "prototype", "fromJSONObject")){
    let prevFunc = TD.services.TwitterUser.prototype.fromJSONObject;
    
    TD.services.TwitterUser.prototype.fromJSONObject = function(){
      let obj = prevFunc.apply(this, arguments);
      let e = arguments[0].entities;
      
      if (e && e.url && e.url.urls && e.url.urls.length && e.url.urls[0].expanded_url){
        obj.url = e.url.urls[0].expanded_url;
      }
      
      return obj;
    };
  }
  
  if (ensurePropertyExists(TD, "services", "TwitterMedia", "prototype", "fromMediaEntity")){
    let prevFunc = TD.services.TwitterMedia.prototype.fromMediaEntity;
    
    TD.services.TwitterMedia.prototype.fromMediaEntity = function(){
      let obj = prevFunc.apply(this, arguments);
      let e = arguments[0];
      
      if (e.expanded_url){
        if (obj.url === obj.shortUrl){
          obj.shortUrl = e.expanded_url;
        }
        
        obj.url = e.expanded_url;
      }
      
      return obj;
    };
  }
  
  //
  // Block: Include additional information in context menus.
  //
  $(document.body).delegate("a", "contextmenu", function(){
    let me = $(this)[0];
    
    if (me.classList.contains("js-media-image-link") && highlightedTweetObj){
      let media = (highlightedTweetObj.quotedTweet || highlightedTweetObj).getMedia().find(media => media.mediaId === me.getAttribute("data-media-entity-id"));
      
      if ((media.isVideo && media.service === "twitter") || media.isAnimatedGif){
        $TD.setLastRightClickInfo("video", media.chooseVideoVariant().url);
      }
      else{
        $TD.setLastRightClickInfo("image", media.large());
      }
    }
    else if (me.classList.contains("js-gif-play")){
      $TD.setLastRightClickInfo("video", $(this).closest(".js-media-gif-container").find("video").attr("src"));
    }
    else{
      $TD.setLastRightClickInfo("link", me.getAttribute("data-full-url"));
    }
  });
  
  //
  // Block: Hook into the notification sound effect.
  //
  HTMLAudioElement.prototype.play = prependToFunction(HTMLAudioElement.prototype.play, function(){
    return $TDX.muteNotifications || $TDX.hasCustomNotificationSound;
  });
  
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
    
    var updateHighlightedTweet = function(ele, obj, tweetUrl, quoteUrl, authors, imageList){
      highlightedTweetEle = ele;
      highlightedTweetObj = obj;
      
      if (lastTweet !== tweetUrl){
        $TD.setLastHighlightedTweet(tweetUrl, quoteUrl, authors, imageList);
        lastTweet = tweetUrl;
      }
    };
    
    var processMedia = function(chirp){
      return chirp.getMedia().filter(item => !item.isAnimatedGif).map(item => item.entity.media_url_https+":small").join(";");
    };
    
    app.delegate("section.js-column", {
      mouseenter: function(){
        if (!highlightedColumnObj){
          updateHighlightedColumn($(this));
        }
      },
      
      mouseleave: function(){
        updateHighlightedColumn(null);
      }
    });
    
    app.delegate("article.js-stream-item", {
      mouseenter: function(){
        let me = $(this);
        return if !me[0].hasAttribute("data-account-key") || (!highlightedColumnObj && !updateHighlightedColumn(me.closest("section.js-column")));
        
        let tweet = highlightedColumnObj.findChirp(me.attr("data-tweet-id")) || highlightedColumnObj.findChirp(me.attr("data-key"));
        return if !tweet;
        
        if (tweet.chirpType === TD.services.ChirpBase.TWEET){
          let tweetUrl = tweet.getChirpURL();
          let quoteUrl = tweet.quotedTweet ? tweet.quotedTweet.getChirpURL() : "";
          let authors = tweet.quotedTweet ? [ tweet.getMainUser().screenName, tweet.quotedTweet.getMainUser().screenName ].join(";") : tweet.getMainUser().screenName;
          let imageList = tweet.quotedTweet && tweet.quotedTweet.hasImage() ? processMedia(tweet.quotedTweet) : tweet.hasImage() ? processMedia(tweet) : "";
          
          updateHighlightedTweet(me, tweet, tweetUrl || "", quoteUrl || "", authors, imageList);
        }
        else{
          updateHighlightedTweet(me, tweet, "", "", "", "");
        }
      },
      
      mouseleave: function(){
        updateHighlightedTweet(null, null, "", "", "", "");
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
        let tweetWidth = Math.floor(selectedTweet.width());
        let parent = selectedTweet.parent();
        
        let isDetail = parent.hasClass("js-tweet-detail");
        let isReply = !isDetail && (parent.hasClass("js-replies-to") || parent.hasClass("js-replies-before"));
        
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
          selectedTweet.find("footer").last().prevUntil(":not(.txt-mute)").addBack().remove(); // footer, date, location
        }
        else{
          setImportantProperty(selectedTweet.find(".js-media-preview-container"), "margin-bottom", "10px");
          selectedTweet.find("footer").last().remove();
        }
        
        if (isReply){
          selectedTweet.find(".is-conversation").removeClass("is-conversation");
          selectedTweet.find(".thread").remove();
        }
        
        selectedTweet.find(".js-poll-link").remove();
        selectedTweet.find(".td-screenshot-remove").remove();
        
        let testTweet = selectedTweet.clone().css({
          position: "absolute",
          left: "-999px",
          width: tweetWidth+"px"
        }).appendTo(document.body);
        
        let realHeight = Math.floor(testTweet.height());
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
    
    app.delegate(".js-compose-text,.js-reply-tweetbox,.td-detect-image-paste", "paste", function(e){
      for(let item of e.originalEvent.clipboardData.items){
        if (item.type.startsWith("image/")){
          if (!$(this).closest(".rpl").find(".js-reply-popout").click().length){ // popout direct messages
            return if $(".js-add-image-button").is(".is-disabled"); // tweetdeck does not check upload count properly
          }
          
          uploader.addFilesToUpload([ item.getAsFile() ]);
          
          $(".js-compose-text", ".js-docked-compose").focus();
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
    
    var tryCloseModal1 = function(){
      let modal = $("#open-modal");
      return modal.is(":visible") && tryClickSelector("a.mdl-dismiss", modal);
    };
    
    var tryCloseModal2 = function(){
      let modal = $(".js-modals-container");
      return modal.length && tryClickSelector("a.mdl-dismiss", modal);
    };
    
    var tryCloseHighlightedColumn = function(){
      if (highlightedColumnEle){
        let column = highlightedColumnEle.closest(".js-column");
        return (column.is(".is-shifted-2") && tryClickSelector(".js-tweet-social-proof-back", column)) || (column.is(".is-shifted-1") && tryClickSelector(".js-column-back", column));
      }
    };
    
    window.TDGF_onMouseClickExtra = function(button){
      if (button === 1){ // back button
        tryClickSelector(".is-shifted-2 .js-tweet-social-proof-back", ".js-modal-panel") ||
        tryClickSelector(".is-shifted-1 .js-column-back", ".js-modal-panel") ||
        tryCloseModal1() ||
        tryCloseModal2() ||
        tryClickSelector(".js-inline-compose-close") ||
        tryCloseHighlightedColumn() ||
        tryClickSelector(".js-app-content.is-open .js-drawer-close:visible") ||
        tryClickSelector(".is-shifted-2 .js-tweet-social-proof-back, .is-shifted-2 .js-dm-participants-back") ||
        $(".is-shifted-1 .js-column-back").click();
      }
      else if (button === 2){ // forward button
        if (highlightedTweetEle){
          highlightedTweetEle.children().first().click();
        }
      }
    };
  })();
  
  //
  // Block: Allow drag & drop behavior for dropping links on columns to open their detail view.
  //
  (function(){
    let tweetRegex = /^https?:\/\/twitter\.com\/[A-Za-z0-9_]+\/status\/(\d+)\/?$/;
    let isDraggingValid = false;
    
    window.TDGF_onGlobalDragStart = function(type, data){
      isDraggingValid = (type === "link" || type === "text") && tweetRegex.test(data);
    };
    
    app.delegate("section.js-column", {
      dragover: function(e){
        e.originalEvent.dataTransfer.dropEffect = isDraggingValid ? "move" : "none";
        e.preventDefault();
        e.stopPropagation();
      },
      
      drop: function(e){
        let match = tweetRegex.exec(e.originalEvent.dataTransfer.getData("URL"));
        
        if (match.length === 2){
          let column = TD.controller.columnManager.get($(this).attr("data-column"));
          
          if (column){
            TD.controller.clients.getPreferredClient().show(match[1], function(chirp){
              TD.ui.updates.showDetailView(column, chirp, column.findChirp(chirp) || chirp);
              $(document).trigger("uiGridClearSelection");
            }, function(){
              alert("error|Could not retrieve the requested tweet.");
            });
          }
        }
        
        e.preventDefault();
        e.stopPropagation();
      }
    });
  })();
  
  //
  // Block: Fix scheduled tweets not showing up sometimes.
  //
  $(document).on("dataTweetSent", function(e, data){
    if (data.response.state && data.response.state === "scheduled"){
      let column = Object.values(TD.controller.columnManager.getAll()).find(column => column.model.state.type === "scheduled");

      if (column){
        setTimeout(function(){
          column.reloadTweets();
        }, 1000);
      }
    }
  });
  
  //
  // Block: Hold Shift to restore cleared column.
  //
  (function(){
    var holdingShift = false;
    
    var updateShiftState = (pressed) => {
      if (pressed != holdingShift){
        holdingShift = pressed;
        $("button[data-action='clear']").children("span").text(holdingShift ? "Restore" : "Clear");
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
    
    return if !ensurePropertyExists(TD, "vo", "Column", "prototype", "clear");
    
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
  // Block: Swap shift key functionality for selecting accounts, and refocus the textbox afterwards.
  //
  onAppReady.push(function(){
    var onAccountClick = function(e){
      if ($TDX.switchAccountSelectors){
        e.shiftKey = !e.shiftKey;
      }
      
      $(".js-compose-text", ".js-docked-compose").focus();
    };
    
    $(".js-drawer[data-drawer='compose']").delegate(".js-account-list > .js-account-item", "click", onAccountClick);
    
    return if !ensurePropertyExists(TD, "components", "AccountSelector", "prototype", "refreshPostingAccounts");
    
    TD.components.AccountSelector.prototype.refreshPostingAccounts = appendToFunction(TD.components.AccountSelector.prototype.refreshPostingAccounts, function(){
      if (!this.$node.attr("td-account-selector-hook")){
        this.$node.attr("td-account-selector-hook", "1");
        this.$node.delegate(".js-account-item", "click", onAccountClick);
      }
    });
  });
  
  //
  // Block: Make middle click on tweet reply icon open the compose drawer, retweet icon trigger a quote, and favorite icon open a 'Like from accounts...' modal.
  //
  app.delegate(".tweet-action,.tweet-detail-action", "auxclick", function(e){
    return if e.which !== 2;
    
    let column = TD.controller.columnManager.get($(this).closest("section.js-column").attr("data-column"));
    return if !column;
    
    let ele = $(this).closest("article");
    let tweet = column.findChirp(ele.attr("data-tweet-id")) || column.findChirp(ele.attr("data-key"));
    return if !tweet;
    
    switch($(this).attr("rel")){
      case "reply":
        let main = tweet.getMainTweet();
        
        $(document).trigger("uiDockedComposeTweet", {
          type: "reply",
          from: [ tweet.account.getKey() ],
          inReplyTo: {
            id: tweet.id,
            htmlText: main.htmlText,
            user: {
              screenName: main.user.screenName,
              name: main.user.name,
              profileImageURL: main.user.profileImageURL
            }
          },
          mentions: tweet.getReplyUsers(),
          element: ele
        });
        
        break;
        
      case "favorite":
        $(document).trigger("uiShowFavoriteFromOptions", { tweet });
        break;
        
      case "retweet":
        TD.controller.stats.quoteTweet();
        
        $(document).trigger("uiComposeTweet", {
          type: "tweet",
          from: [ tweet.account.getKey() ],
          quotedTweet: tweet.getMainTweet(),
          element: ele // triggers reply-account plugin
        });
        
        break;
      
      default:
        return;
    }
    
    e.preventDefault();
    e.stopPropagation();
    e.stopImmediatePropagation();
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
    var createStyle = function(id, styles){
      let ele = document.createElement("style");
      ele.id = id;
      ele.innerText = styles;
      document.head.appendChild(ele);
    };
    
    window.TDGF_injectBrowserCSS = function(styles){
      if (!document.getElementById("tweetduck-browser-css")){
        createStyle("tweetduck-browser-css", styles);
      }
    };
    
    window.TDGF_reinjectCustomCSS = function(styles){
      $("#tweetduck-custom-css").remove();
      
      if (styles && styles.length){
        createStyle("tweetduck-custom-css", styles);
      }
    };
  })();
  
  //
  // Block: Let's make retweets lowercase again.
  //
  TD.mustaches["status/tweet_single.mustache"] = TD.mustaches["status/tweet_single.mustache"].replace("{{_i}} Retweeted{{/i}}", "{{_i}} retweeted{{/i}}");
  
  if (ensurePropertyExists(TD, "services", "TwitterActionRetweet", "prototype", "generateText")){
    TD.services.TwitterActionRetweet.prototype.generateText = appendToFunction(TD.services.TwitterActionRetweet.prototype.generateText, function(){
      this.text = this.text.replace(" Retweeted", " retweeted");
      this.htmlText = this.htmlText.replace(" Retweeted", " retweeted");
    });
  }
  
  if (ensurePropertyExists(TD, "services", "TwitterActionRetweetedInteraction", "prototype", "generateText")){
    TD.services.TwitterActionRetweetedInteraction.prototype.generateText = appendToFunction(TD.services.TwitterActionRetweetedInteraction.prototype.generateText, function(){
      this.htmlText = this.htmlText.replace(" Retweeted", " retweeted").replace(" Retweet", " retweet");
    });
  }
  
  //
  // Block: Setup video player hooks.
  //
  (function(){
    window.TDGF_playVideo = function(url, username){
      $('<div id="td-video-player-overlay" class="ovl" style="display:block"></div>').on("click contextmenu", function(){
        $TD.playVideo(null, null);
      }).appendTo(app);
      
      $TD.playVideo(url, username || null);
    };
    
    var getVideoTweetLink = function(obj){
      let parent = obj.closest(".js-tweet").first();
      let link = (parent.hasClass("tweet-detail") ? parent.find("a[rel='url']") : parent.find("time").first().children("a")).first();
      return link.attr("href");
    }
    
    var getUsername = function(tweet){
      return tweet && (tweet.quotedTweet || tweet).getMainUser().screenName;
    }
    
    app.delegate(".js-gif-play", {
      click: function(e){
        let src = !e.ctrlKey && $(this).closest(".js-media-gif-container").find("video").attr("src");
        
        if (src){
          window.TDGF_playVideo(src, getUsername(highlightedTweetObj));
        }
        else{
          $TD.openBrowser(getVideoTweetLink($(this)));
        }
        
        e.stopPropagation();
      },
      
      mousedown: function(e){
        if (e.button === 1){
          e.preventDefault();
        }
      },
      
      mouseup: function(e){
        if (e.button === 1){
          $TD.openBrowser(getVideoTweetLink($(this)));
          e.preventDefault();
        }
      }
    });
    
    TD.mustaches["status/media_thumb.mustache"] = TD.mustaches["status/media_thumb.mustache"].replace("is-gif", "is-gif is-paused");
    TD.mustaches["media/native_video.mustache"] = '<div class="js-media-gif-container media-item nbfc is-video" style="background-image:url({{imageSrc}})"><video class="js-media-gif media-item-gif full-width block {{#isPossiblySensitive}}is-invisible{{/isPossiblySensitive}}" loop src="{{videoUrl}}"></video><a class="js-gif-play pin-all is-actionable">{{> media/video_overlay}}</a></div>';
    
    if (!ensurePropertyExists(TD, "components", "MediaGallery", "prototype", "_loadTweet") ||
        !ensurePropertyExists(TD, "components", "BaseModal", "prototype", "setAndShowContainer") ||
        !ensurePropertyExists(TD, "ui", "Column", "prototype", "playGifIfNotManuallyPaused"))return;
    
    var cancelModal = false;
    
    TD.components.MediaGallery.prototype._loadTweet = appendToFunction(TD.components.MediaGallery.prototype._loadTweet, function(){
      let media = this.chirp.getMedia().find(media => media.mediaId === this.clickedMediaEntityId);
      
      if (media && media.isVideo && media.service === "twitter"){
        window.TDGF_playVideo(media.chooseVideoVariant().url, getUsername(this.chirp));
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
  })();
  
  //
  // Block: Fix youtu.be previews not showing up for https links.
  //
  if (ensurePropertyExists(TD, "services", "TwitterMedia")){
    let media = TD.services.TwitterMedia;
    
    if (!ensurePropertyExists(media, "YOUTUBE_TINY_RE") ||
        !ensurePropertyExists(media, "YOUTUBE_LONG_RE") ||
        !ensurePropertyExists(media, "YOUTUBE_RE") ||
        !ensurePropertyExists(media, "SERVICES", "youtube"))return;
    
    media.YOUTUBE_TINY_RE = new RegExp(media.YOUTUBE_TINY_RE.source.replace("http:", "https?:"));
    media.YOUTUBE_RE = new RegExp(media.YOUTUBE_LONG_RE.source+"|"+media.YOUTUBE_TINY_RE.source);
    media.SERVICES["youtube"] = media.YOUTUBE_RE;
  }
  
  //
  // Block: Add a pin icon to make tweet compose drawer stay open.
  //
  onAppReady.push(function(){
    let ele = $(`<svg id="td-compose-drawer-pin" viewBox="0 0 24 24" class="icon js-show-tip" data-original-title="Stay open" data-tooltip-position="left">
       <path d="M9.884,16.959l3.272,0.001l-0.82,4.568l-1.635,0l-0.817,-4.569Z"/>
       <rect x="8.694" y="7.208" width="5.652" height="7.445"/>
       <path d="M16.877,17.448c0,-1.908 -1.549,-3.456 -3.456,-3.456l-3.802,0c-1.907,0 -3.456,1.548 -3.456,3.456l10.714,0Z"/>
       <path d="M6.572,5.676l2.182,2.183l5.532,0l2.182,-2.183l0,-1.455l-9.896,0l0,1.455Z"/>
       </svg>`
    ).appendTo(".js-docked-compose .js-compose-header");
    
    ele.click(function(){
      if (TD.settings.getComposeStayOpen()){
        ele.css("transform", "rotate(0deg)");
        TD.settings.setComposeStayOpen(false);
      }
      else{
        ele.css("transform", "rotate(90deg)");
        TD.settings.setComposeStayOpen(true);
      }
    });
    
    if (TD.settings.getComposeStayOpen()){
      ele.css("transform", "rotate(90deg)");
    }
  });
  
  //
  // Block: Make temporary search column appear as the first one and clear the input box.
  //
  $(document).on("uiSearchNoTemporaryColumn", function(e, data){
    if (data.query && data.searchScope !== "users" && !data.columnKey){
      if ($TDX.openSearchInFirstColumn){
        let order = TD.controller.columnManager._columnOrder;
        
        if (order.length > 1){
          let columnKey = order[order.length-1];
          
          order.splice(order.length-1, 1);
          order.splice(1, 0, columnKey);
          TD.controller.columnManager.move(columnKey, "left");
        }
      }
      
      $(".js-app-search-input").val("");
      $(".js-perform-search").blur();
    }
  });
  
  //
  // Block: Fix DM reply input box not getting focused after opening a conversation.
  //
  if (ensurePropertyExists(TD, "components", "ConversationDetailView", "prototype", "showChirp")){
    TD.components.ConversationDetailView.prototype.showChirp = appendToFunction(TD.components.ConversationDetailView.prototype.showChirp, function(){
      setTimeout(function(){
        $(".js-reply-tweetbox").first().focus();
      }, 100);
    });
  }
  
  //
  // Block: Fix DM notifications not showing if the conversation is open.
  //
  if (ensurePropertyExists(TD, "services", "TwitterConversation", "prototype", "getUnreadChirps")){
    var prevFunc = TD.services.TwitterConversation.prototype.getUnreadChirps;
    
    TD.services.TwitterConversation.prototype.getUnreadChirps = function(e){
      return (e && e.sortIndex && !e.id && !this.notificationsDisabled)
        ? this.messages.filter(t => t.chirpType === TD.services.ChirpBase.MESSAGE && !t.isOwnChirp() && !t.read && !t.belongsBelow(e)) // changed from belongsAbove
        : prevFunc.apply(this, arguments);
    };
  }
  
  //
  // Block: Detect and notify about connection issues.
  //
  (function(){
    let onConnectionError = function(){
      return if $("#tweetduck-conn-issues").length;
      
      let ele = $(`
<div id="tweetduck-conn-issues" class="Layer NotificationListLayer">
  <ul class="NotificationList">
    <li class="Notification Notification--red" style="height:63px;">
      <div class="Notification-inner">
        <div class="Notification-icon"><span class="Icon Icon--medium Icon--circleError"></span></div>
        <div class="Notification-content"><div class="Notification-body">Experiencing connection issues</div></div>
        <button type="button" class="Notification-closeButton" aria-label="Close"><span class="Icon Icon--smallest Icon--close" aria-hidden="true"></span></button>
      </div>
    </li>
  </ul>
</div>
`).appendTo(document.body);

      ele.find("button").click(function(){
        ele.fadeOut(200);
      });
    };
    
    let onConnectionFine = function(){
      let ele = $("#tweetduck-conn-issues");
      
      ele.fadeOut(200, function(){
        ele.remove();
      });
    };
    
    window.addEventListener("offline", onConnectionError);
    window.addEventListener("online", onConnectionFine);
  })();
  
  //
  // Block: Custom reload function with memory cleanup.
  //
  window.TDGF_reload = function(){
    let session = TD.storage.feedController.getAll()
                    .filter(feed => !!feed.getTopSortIndex())
                    .reduce((obj, feed) => (obj[feed.privateState.key] = feed.getTopSortIndex(), obj), {});
    
    $TD.setSessionData("gc", JSON.stringify(session)).then(() => {
      window.gc && window.gc();
      window.location.reload();
    });
  };
  
  (function(){
    var lastActivity = Date.now();
    
    $(document).click(function(e){
      lastActivity = Date.now();
    });
    
    window.TDGF_tryRunCleanup = function(){
      // no recent activity
      return false if Date.now()-lastActivity < 15e3;
      
      // no modals are visible
      return false if $(".js-modal").is(":visible") || !$(".js-modals-container").is(":empty");

      // all columns are in a default state
      return false if $("section.js-column").is(".is-shifted-1,.is-shifted-2");

      // all textareas are empty
      return false if Array.prototype.some.call(document.getElementsByTagName("textarea"), ele => ele.value.length > 0);

      // all columns are scrolled to top
      return false if Array.prototype.some.call(document.getElementsByClassName("js-column-scroller"), ele => ele.scrollTop > 0);

      // cleanup
      window.TDGF_reload();
      return true;
    };
  })();
  
  if (window.TD_SESSION && window.TD_SESSION.gc){
    var state;
    
    try{
      state = JSON.parse(window.TD_SESSION.gc);
    }catch(err){
      $TD.crashDebug("Invalid session gc data: "+window.TD_SESSION.gc);
      state = {};
    }
    
    var showMissedNotifications = function(){
      let tweets = [];
      let columns = {};
      
      let tmp = new TD.services.ChirpBase;
      
      for(let column of Object.values(TD.controller.columnManager.getAll())){
        for(let feed of column.getFeeds()){
          if (feed.privateState.key in state){
            tmp.sortIndex = state[feed.privateState.key];
            
            for(let tweet of [].concat.apply([], column.updateArray.map(function(chirp){
              return chirp.getUnreadChirps(tmp);
            }))){
              tweets.push(tweet);
              columns[tweet.id] = column;
            }
          }
        }
      }
      
      tweets.sort(TD.util.chirpReverseColumnSort);
      
      for(let tweet of tweets){
        onNewTweet(columns[tweet.id], tweet);
      }
    };
    
    $(document).one("dataColumnsLoaded", function(){
      let columns = Object.values(TD.controller.columnManager.getAll());
      let remaining = columns.length;

      for(let column of columns){
        column.ui.getChirpContainer().one("dataColumnFeedUpdated", () => {
          if (--remaining === 0){
            setTimeout(showMissedNotifications, 1);
          }
        });
      }
    });
  }
  
  //
  // Block: Disable TweetDeck metrics.
  //
  if (ensurePropertyExists(TD, "metrics")){
    TD.metrics.inflate = function(){};
    TD.metrics.inflateMetricTriple = function(){};
    TD.metrics.log = function(){};
    TD.metrics.makeKey = function(){};
    TD.metrics.send = function(){};
  }
  
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
    onAppReady = null;
    
    delete window.TD_SESSION;
    
    if (window.TD_PLUGINS){
      window.TD_PLUGINS.onReady();
    }
  });
  
  //
  // Block: Skip the initial pre-login page.
  //
  if (ensurePropertyExists(TD, "controller", "init", "showLogin")){
    TD.controller.init.showLogin = function(){
      location.href = "https://twitter.com/login?hide_message=true&redirect_after_login=https%3A%2F%2Ftweetdeck.twitter.com%2F%3Fvia_twitter_login%3Dtrue";
    };
  }
})($, $TD, $TDX, TD);
