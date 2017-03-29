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
  // Function: Event callback for a new tweet.
  //
  var onNewTweet = function(column, tweet){
    if (column.model.getHasNotification()){
      var html = $(tweet.render({
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
      html.find(".js-quote-detail").removeClass("is-actionable");
      
      var url = html.find("time").first().children("a").first().attr("href") || "";
      
      $TD.onTweetPopup(html.html(), url, tweet.text.length); // TODO column
    }
    
    if (column.model.getHasSound()){
      $TD.onTweetSound();
    }
  };
  
  //
  // Function: Retrieves the tags to be put into <head> for notification HTML code.
  //
  var getNotificationHeadContents = function(){
    var tags = [];
    
    $(document.head).children("link[rel='stylesheet'],meta[charset],meta[http-equiv]").each(function(){
      tags.push($(this)[0].outerHTML);
    });
    
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
        
        if (!me[0].hasAttribute("data-tweet-id") || !highlightedColumnObj && !updateHighlightedColumn(me.closest("section.js-column"))){
          return;
        }
        
        var tweet = highlightedColumnObj.findChirp(me.attr("data-tweet-id")) || highlightedColumnObj.findChirp(me.attr("data-key"));

        if (tweet && tweet.chirpType === TD.services.ChirpBase.TWEET){
          var link = tweet.getChirpURL();
          var embedded = tweet.quotedTweet ? tweet.quotedTweet.getChirpURL() : "";
          
          updateHighlightedTweet(me, tweet, link || "", embedded || "");
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
        var tweetWidth = selectedTweet.width();
        var parent = selectedTweet.parent();
        
        var isDetail = parent.hasClass("js-tweet-detail");
        var isReply = !isDetail && (parent.hasClass("js-replies-to") || parent.hasClass("js-replies-before"));
        
        selectedTweet = selectedTweet.clone();
        selectedTweet.children().first().addClass($(document.documentElement).attr("class")).css("padding-bottom", "0");
        
        setImportantProperty(selectedTweet.find(".js-tweet-text"), "margin-bottom", "8px");
        setImportantProperty(selectedTweet.find(".js-quote-detail"), "margin-bottom", "10px");
        setImportantProperty(selectedTweet.find(".js-media-preview-container"), "margin-bottom", "10px");
        
        if (isDetail){
          setImportantProperty(selectedTweet.find(".js-tweet-media"), "margin-bottom", "0");
          selectedTweet.find(".js-translate-call-to-action").first().remove();
          selectedTweet.find(".js-tweet").first().nextAll().remove();
          selectedTweet.find("footer").last().prevUntil(":not(.txt-mute.txt-small)").addBack().remove(); // footer, date, location
        }
        else{
          selectedTweet.find("footer").last().remove();
        }
        
        if (isReply){
          selectedTweet.find(".is-conversation").removeClass("is-conversation");
          selectedTweet.find(".timeline-poll-container").first().remove(); // fix for timeline polls plugin
        }
        
        selectedTweet.find(".js-poll-link").remove();
        
        var testTweet = selectedTweet.clone().css({
          position: "absolute",
          left: "-999px",
          width: tweetWidth+"px"
        }).appendTo(document.body);
        
        var realHeight = testTweet.height();
        testTweet.remove();
        
        $TD.screenshotTweet(selectedTweet.html(), tweetWidth, realHeight);
      }
    };
  })();
  
  //
  // Block: Paste images when tweeting.
  //
  (function(){
    var lastPasteElement;
    var prevScrollTop;
    
    var getScroller = function(){
      return $(".js-drawer").find(".js-compose-scroller").first().children().first();
    };
    
    var clickUpload = function(){
      $(document).one("uiFilesAdded", function(){
        getScroller().scrollTop(prevScrollTop);
        $(".js-drawer").find(".js-compose-text").first()[0].focus();
      });
      
      var button = $(".js-add-image-button").first();
      
      var scroller = getScroller();
      prevScrollTop = scroller.scrollTop();
      
      scroller.scrollTop(0);
      scroller.scrollTop(button.offset().top); // scrolls the button into view
      
      var buttonPos = button.children().first().offset(); // finds the camera icon offset
      $TD.clickUploadImage(Math.floor(buttonPos.left), Math.floor(buttonPos.top));
    };
    
    app.delegate(".js-compose-text,.js-reply-tweetbox", "paste", function(){
      lastPasteElement = $(this);
      $TD.tryPasteImage();
    });

    window.TDGF_tryPasteImage = function(){
      if (lastPasteElement){
        var parent = lastPasteElement.parent();

        if (parent.siblings(".js-add-image-button").length === 0){
          var pop = parent.closest(".js-inline-reply,.rpl").find(".js-inline-compose-pop,.js-reply-popout");

          if (pop.length === 0){
            lastPasteElement = null;
            return;
          }
          
          pop.click();
          
          var drawer = $(".js-drawer");
          var counter = 0;
          
          var interval = setInterval(function(){
            if (drawer.offset().left >= 195){
              clickUpload();
              clearInterval(interval);
            }
            else if (++counter >= 10){
              clearInterval(interval);
            }
          }, 51);
        }
        else{
          clickUpload();
        }
        
        lastPasteElement = null;
      }
    };
  })();
  
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
    $(".js-drawer[data-drawer='compose']").delegate(".js-account-list > .js-account-item", "click", function(e){
      e.shiftKey = !e.shiftKey;
    });

    TD.components.AccountSelector.prototype.refreshPostingAccounts = appendToFunction(TD.components.AccountSelector.prototype.refreshPostingAccounts, function(){
      if (!this.$node.attr("td-account-selector-hook")){
        this.$node.attr("td-account-selector-hook", "1");

        this.$node.delegate(".js-account-item", "click", function(e){
          e.shiftKey = !e.shiftKey;
        });
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
    styleOfficial.sheet.insertRule(".txt-base-smallest .badge-verified:before { height: 13px !important; }", 0); // fix cut off badge icon
    styleOfficial.sheet.insertRule(".keyboard-shortcut-list { vertical-align: top; }", 0); // fix keyboard navigation alignment
    
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
  // Block: Finish initialization and load plugins.
  //
  onAppReady.push(function(){
    $TD.loadFontSizeClass(TD.settings.getFontSize());
    $TD.loadNotificationHeadContents(getNotificationHeadContents());
    
    if (window.TD_PLUGINS){
      window.TD_PLUGINS.onReady();
    }
  });
  
  //
  // Block: Observe the main app element and call the ready event whenever the contents are loaded.
  //
  new MutationObserver(function(){
    if (window.TD_APP_READY && app.hasClass("is-hidden")){
      window.TD_APP_READY = false;
    }
    else if (!window.TD_APP_READY && !app.hasClass("is-hidden")){
      onAppReady.forEach(func => func());
      window.TD_APP_READY = true;
    }
  }).observe(app[0], {
    attributes: true,
    attributeFilter: [ "class" ]
  });
})($, $TD, $TDX, TD);
