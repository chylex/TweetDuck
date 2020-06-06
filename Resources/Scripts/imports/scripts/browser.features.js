(function(){
  //
  // Function: Event callback for a new tweet.
  //
  const onNewTweet = (function(){
    const recentMessages = new Set();
    const recentTweets = new Set();
    let recentTweetTimer = null;
    
    const resetRecentTweets = () => {
      recentTweetTimer = null;
      recentTweets.clear();
    };
    
    const startRecentTweetTimer = () => {
      recentTweetTimer && window.clearTimeout(recentTweetTimer);
      recentTweetTimer = window.setTimeout(resetRecentTweets, 20000);
    };
    
    const checkTweetCache = (set, id) => {
      return true if set.has(id);
      
      if (set.size > 50){
        set.clear();
      }
      
      set.add(id);
      return false;
    };
    
    const isSensitive = (tweet) => {
      const main = tweet.getMainTweet && tweet.getMainTweet();
      return true if main && main.possiblySensitive; // TODO these don't show media badges when hiding sensitive media
      
      const related = tweet.getRelatedTweet && tweet.getRelatedTweet();
      return true if related && related.possiblySensitive;
      
      const quoted = tweet.quotedTweet;
      return true if quoted && quoted.possiblySensitive;
      
      return false;
    };
    
    const fixMedia = (html, media) => {
      return html.find("a[data-media-entity-id='" + media.mediaId + "'], .media-item").first().removeClass("is-zoomable").css("background-image", 'url("' + media.small() + '")');
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
        const sensitive = isSensitive(tweet);
        const previews = $TDX.notificationMediaPreviews && (!sensitive || TD.settings.getDisplaySensitiveMedia());
        // TODO new cards don't have either previews or links
        
        const html = $(tweet.render({
          withFooter: false,
          withTweetActions: false,
          withMediaPreview: true,
          isMediaPreviewOff: !previews,
          isMediaPreviewSmall: previews,
          isMediaPreviewLarge: false,
          isMediaPreviewCompact: false,
          isMediaPreviewInQuoted: previews,
          thumbSizeClass: "media-size-medium",
          mediaPreviewSize: "medium"
        }));
        
        html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
        html.find(".js-quote-detail").removeClass("is-actionable margin-b--8"); // prevent quoted tweets from changing the cursor and reduce bottom margin
        
        if (previews){
          html.find(".reverse-image-search").remove();
          
          const container = html.find(".js-media");
          
          for(let media of tweet.getMedia()){
            fixMedia(container, media);
          }
          
          if (tweet.quotedTweet){
            for(let media of tweet.quotedTweet.getMedia()){
              fixMedia(container, media).addClass("media-size-medium");
            }
          }
        }
        else if (tweet instanceof TD.services.TwitterActionOnTweet){
          html.find(".js-media").remove();
        }
        
        html.find("a[data-full-url]").each(function(){ // bypass t.co on all links
          this.href = this.getAttribute("data-full-url");
        });
        
        html.find("a[href='#']").each(function(){ // remove <a> tags around links that don't lead anywhere (such as account names the tweet replied to)
          this.outerHTML = this.innerHTML;
        });
        
        html.find("p.link-complex-target").filter(function(){
          return $(this).text() === "Show this thread";
        }).first().each(function(){
          this.id = "tduck-show-thread";
          
          const moveBefore = html.find(".tweet-body > .js-media, .tweet-body > .js-media-preview-container, .quoted-tweet");
          
          if (moveBefore){
            $(this).css("margin-top", "5px").removeClass("margin-b--5").parent("span").detach().insertBefore(moveBefore);
          }
        });
        
        if (tweet.quotedTweet){
          html.find("p.txt-mute").filter(function(){
            return $(this).text() === "Show this thread";
          }).first().remove();
        }
        
        const type = tweet.getChirpType();
        
        if (type === "follow"){
          html.find(".js-user-actions-menu").parent().remove();
          html.find(".account-bio").removeClass("padding-t--5").css("padding-top", "2px");
        }
        else if ((type.startsWith("favorite") || type.startsWith("retweet")) && tweet.isAboutYou()){
          html.children().first().addClass("td-notification-padded");
        }
        else if (type.includes("list_member")){
          html.children().first().addClass("td-notification-padded td-notification-padded-alt");
          html.find(".activity-header").css("margin-top", "2px");
          html.find(".avatar").first().css("margin-bottom", "0");
        }
        
        if (sensitive){
          html.find(".media-badge").each(function(){
            $(this)[0].lastChild.textContent += " (possibly sensitive)";
          });
        }
        
        const source = tweet.getRelatedTweet();
        const duration = source ? source.text.length + (source.quotedTweet ? source.quotedTweet.text.length : 0) : tweet.text.length;
        
        const chirpId = source ? source.id : "";
        const tweetUrl = source ? source.getChirpURL() : "";
        const quoteUrl = source && source.quotedTweet ? source.quotedTweet.getChirpURL() : "";
        
        $TD.onTweetPopup(column.model.privateState.apiid, chirpId, window.TDGF_getColumnName(column), html.html(), duration, tweetUrl, quoteUrl);
      }
      
      if (column.model.getHasSound()){
        $TD.onTweetSound();
      }
    };
  })();
  
  //
  // Block: Enable popup notifications.
  //
  execSafe(function hookDesktopNotifications(){
    ensurePropertyExists(TD, "controller", "notifications");
    
    TD.controller.notifications.hasNotifications = function(){
      return true;
    };
    
    TD.controller.notifications.isPermissionGranted = function(){
      return true;
    };
    
    $.subscribe("/notifications/new", function(obj){
      for(let index = obj.items.length - 1; index >= 0; index--){
        onNewTweet(obj.column, obj.items[index]);
      }
    });
  });
  
  //
  // Block: Fix DM notifications not showing if the conversation is open.
  //
  if (checkPropertyExists(TD, "vo", "Column", "prototype", "mergeMissingChirps")){
    TD.vo.Column.prototype.mergeMissingChirps = prependToFunction(TD.vo.Column.prototype.mergeMissingChirps, function(e){
      const model = this.model;
      
      if (model && model.state && model.state.type === "privateMe" && !this.notificationsDisabled && e.poller.feed.managed){
        const unread = [];
        
        for(let chirp of e.chirps){
          if (Array.isArray(chirp.messages)){
            Array.prototype.push.apply(unread, chirp.messages.filter(message => message.read === false));
          }
        }
        
        if (unread.length > 0){
          if (checkPropertyExists(TD, "util", "chirpReverseColumnSort")){
            unread.sort(TD.util.chirpReverseColumnSort);
          }
          
          for(let message of unread){
            onNewTweet(this, message);
          }
          
          // TODO sound notifications are borked as well
          // TODO figure out what to do with missed notifications at startup
        }
      }
    });
  }
})();

//
// Block: Mute sound notifications.
//
HTMLAudioElement.prototype.play = prependToFunction(HTMLAudioElement.prototype.play, function(){
  return $TDX.muteNotifications;
});

//
// Block: Add additional link information to context menu.
//
execSafe(function setupLinkContextMenu(){
  $(document.body).delegate("a", "contextmenu", function(){
    const me = $(this)[0];
    
    if (me.classList.contains("js-media-image-link")){
      const hovered = getHoveredTweet();
      return if !hovered;
      
      const tweet = hovered.obj.hasMedia() ? hovered.obj : hovered.obj.quotedTweet;
      const media = tweet.getMedia().find(media => media.mediaId === me.getAttribute("data-media-entity-id"));
      
      if ((media.isVideo && media.service === "twitter") || media.isAnimatedGif){
        $TD.setRightClickedLink("video", media.chooseVideoVariant().url);
      }
      else{
        $TD.setRightClickedLink("image", media.large());
      }
    }
    else if (me.classList.contains("js-gif-play")){
      $TD.setRightClickedLink("video", $(this).closest(".js-media-gif-container").find("video").attr("src"));
    }
    else if (me.hasAttribute("data-full-url")){
      $TD.setRightClickedLink("link", me.getAttribute("data-full-url"));
    }
  });
});

//
// Block: Add tweet-related options to context menu.
//
execSafe(function setupTweetContextMenu(){
  ensurePropertyExists(TD, "controller", "columnManager", "get");
  ensurePropertyExists(TD, "services", "ChirpBase", "TWEET");
  ensurePropertyExists(TD, "services", "TwitterActionFollow");
  
  const processMedia = function(chirp){
    return chirp.getMedia().filter(item => !item.isAnimatedGif).map(item => item.entity.media_url_https + ":small").join(";");
  };
  
  app.delegate("section.js-column", "contextmenu", function(){
    const hovered = getHoveredTweet();
    return if !hovered;
    
    const tweet = hovered.obj;
    const quote = tweet.quotedTweet;
    
    if (tweet.chirpType === TD.services.ChirpBase.TWEET){
      const tweetUrl = tweet.getChirpURL();
      const quoteUrl = quote && quote.getChirpURL();
      
      const chirpAuthors = quote ? [ tweet.getMainUser().screenName, quote.getMainUser().screenName ].join(";") : tweet.getMainUser().screenName;
      const chirpImages = tweet.hasImage() ? processMedia(tweet) : quote && quote.hasImage() ? processMedia(quote) : "";
      
      $TD.setRightClickedChirp(tweetUrl || "", quoteUrl || "", chirpAuthors, chirpImages);
    }
    else if (tweet instanceof TD.services.TwitterActionFollow){
      $TD.setRightClickedLink("link", tweet.following.getProfileURL());
    }
  });
});

//
// Block: Expand shortened links on hover or display tooltip.
//
execSafe(function setupLinkExpansionOrTooltip(){
  let prevMouseX = -1, prevMouseY = -1;
  let tooltipTimer, tooltipDisplayed;
  
  $(document.body).delegate("a[data-full-url]", {
    mouseenter: function(){
      const me = $(this);
      const text = me.text();
      return if text.charCodeAt(text.length - 1) !== 8230 && text.charCodeAt(0) !== 8230; // horizontal ellipsis
      
      if ($TDX.expandLinksOnHover){
        tooltipTimer = window.setTimeout(function(){
          me.attr("td-prev-text", text);
          me.text(me.attr("data-full-url").replace(/^https?:\/\/(www\.)?/, ""));
        }, 200);
      }
      else{
        tooltipTimer = window.setTimeout(function(){
          $TD.displayTooltip(me.attr("data-full-url"));
          tooltipDisplayed = true;
        }, 400);
      }
    },
    
    mouseleave: function(){
      const me = $(this)[0];
      
      if (me.hasAttribute("td-prev-text")){
        me.innerText = me.getAttribute("td-prev-text");
      }
      
      window.clearTimeout(tooltipTimer);
      
      if (tooltipDisplayed){
        tooltipDisplayed = false;
        $TD.displayTooltip(null);
      }
    },
    
    mousemove: function(e){
      if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)){
        $TD.displayTooltip($(this).attr("data-full-url"));
        prevMouseX = e.clientX;
        prevMouseY = e.clientY;
      }
    }
  });
});

//
// Block: Support for extra mouse buttons.
//
execSafe(function supportExtraMouseButtons(){
  const tryClickSelector = function(selector, parent){
    return $(selector, parent).click().length;
  };
  
  const tryCloseModal1 = function(){
    const modal = $("#open-modal");
    return modal.is(":visible") && tryClickSelector("a.mdl-dismiss", modal);
  };
  
  const tryCloseModal2 = function(){
    const modal = $(".js-modals-container");
    return modal.length && tryClickSelector("a.mdl-dismiss", modal);
  };
  
  const tryCloseHighlightedColumn = function(){
    const column = getHoveredColumn();
    return false if !column;
    
    const ele = $(column.ele);
    return ((ele.is(".is-shifted-2") && tryClickSelector(".js-tweet-social-proof-back", ele)) || (ele.is(".is-shifted-1") && tryClickSelector(".js-column-back", ele)));
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
      const hovered = getHoveredTweet();
      
      if (hovered){
        $(hovered.ele).children().first().click();
      }
    }
  };
});

//
// Block: Allow drag & drop behavior for dropping links on columns to open their detail view.
//
execSafe(function supportDragDropOverColumns(){
  const regexTweet = /^https?:\/\/twitter\.com\/[A-Za-z0-9_]+\/status\/(\d+)\/?\??/;
  const regexAccount = /^https?:\/\/twitter\.com\/(?!signup$|tos$|privacy$|search$|search-)([^/?]+)\/?$/;
  
  let dragType = false;
  
  const events = {
    dragover: function(e){
      e.originalEvent.dataTransfer.dropEffect = dragType ? "all" : "none";
      e.preventDefault();
      e.stopPropagation();
    },
    
    drop: function(e){
      const url = e.originalEvent.dataTransfer.getData("URL");
      
      if (dragType === "tweet"){
        const match = regexTweet.exec(url);
        
        if (match.length === 2){
          const column = TD.controller.columnManager.get($(this).attr("data-column"));
          
          if (column){
            TD.controller.clients.getPreferredClient().show(match[1], function(chirp){
              TD.ui.updates.showDetailView(column, chirp, column.findChirp(chirp) || chirp);
              $(document).trigger("uiGridClearSelection");
            }, function(){
              alert("error|Could not retrieve the requested tweet.");
            });
          }
        }
      }
      else if (dragType === "account"){
        const match = regexAccount.exec(url);
        
        if (match.length === 2){
          $(document).trigger("uiShowProfile", { id: match[1] });
        }
      }
      
      e.preventDefault();
      e.stopPropagation();
    }
  };
  
  const selectors = {
    tweet: "section.js-column",
    account: app
  };
  
  window.TDGF_onGlobalDragStart = function(type, data){
    if (dragType){
      app.undelegate(selectors[dragType], events);
      dragType = null;
    }
    
    if (type === "link"){
      dragType = regexTweet.test(data) ? "tweet" : regexAccount.test(data) ? "account": null;
      app.delegate(selectors[dragType], events);
    }
  };
});

//
// Block: Make middle click on tweet reply icon open the compose drawer, retweet icon trigger a quote, and favorite icon open a 'Like from accounts...' modal.
//
execSafe(function supportMiddleClickTweetActions(){
  app.delegate(".tweet-action,.tweet-detail-action", "auxclick", function(e){
    return if e.which !== 2;
    
    const column = TD.controller.columnManager.get($(this).closest("section.js-column").attr("data-column"));
    return if !column;
    
    const ele = $(this).closest("article");
    const tweet = column.findChirp(ele.attr("data-tweet-id")) || column.findChirp(ele.attr("data-key"));
    return if !tweet;
    
    switch($(this).attr("rel")){
      case "reply":
        const main = tweet.getMainTweet();
        
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
});

//
// Block: Add a pin icon to make tweet compose drawer stay open.
//
execSafe(function setupStayOpenPin(){
  $(document).on("tduckOldComposerActive", function(e){
    const ele = $(`#import "markup/pin.html"`).appendTo(".js-docked-compose .js-compose-header");
    
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
});

//
// Block: Make submitting search queries while holding Ctrl or middle-clicking the search icon open the search externally.
//
onAppReady.push(function setupSearchTriggerHook(){
  const openSearchExternally = function(event, input){
    $TD.openBrowser("https://twitter.com/search/?q=" + encodeURIComponent(input.val() || ""));
    event.preventDefault();
    event.stopPropagation();
    
    input.val("").blur();
    app.click(); // unfocus everything
  };
  
  $$(".js-app-search-input").keydown(function(e){
    (e.ctrlKey && e.keyCode === 13) && openSearchExternally(e, $(this)); // enter
  });
  
  $$(".js-perform-search").on("click auxclick", function(e){
    (e.ctrlKey || e.button === 1) && openSearchExternally(e, $(".js-app-search-input:visible"));
  }).each(function(){
    window.TDGF_prioritizeNewestEvent($(this)[0], "click");
  });
  
  $$("[data-action='show-search']").on("click auxclick", function(e){
    (e.ctrlKey || e.button === 1) && openSearchExternally(e, $());
  });
});

//
// Block: Setup video player hooks.
//
execSafe(function setupVideoPlayer(){
  const getGifLink = function(ele){
    return ele.attr("src") || ele.children("source[video-src]").first().attr("video-src");
  };
  
  const getVideoTweetLink = function(obj){
    let parent = obj.closest(".js-tweet").first();
    let link = (parent.hasClass("tweet-detail") ? parent.find("a[rel='url']") : parent.find("time").first().children("a")).first();
    return link.attr("href");
  };
  
  const getUsername = function(tweet){
    return tweet && (tweet.quotedTweet || tweet).getMainUser().screenName;
  };
  
  app.delegate(".js-gif-play", {
    click: function(e){
      let src = !e.ctrlKey && getGifLink($(this).closest(".js-media-gif-container").find("video"));
      let tweet = getVideoTweetLink($(this));
      
      if (src){
        let hovered = getHoveredTweet();
        window.TDGF_playVideo(src, tweet, getUsername(hovered && hovered.obj));
      }
      else{
        $TD.openBrowser(tweet);
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
  
  window.TDGF_injectMustache("status/media_thumb.mustache", "append", "is-gif", " is-paused");
  
  TD.mustaches["media/native_video.mustache"] = '<div class="js-media-gif-container media-item nbfc is-video" style="background-image:url({{imageSrc}})"><video class="js-media-gif media-item-gif full-width block {{#isPossiblySensitive}}is-invisible{{/isPossiblySensitive}}" loop src="{{videoUrl}}"></video><a class="js-gif-play pin-all is-actionable">{{> media/video_overlay}}</a></div>';
  
  ensurePropertyExists(TD, "components", "MediaGallery", "prototype", "_loadTweet");
  ensurePropertyExists(TD, "components", "BaseModal", "prototype", "setAndShowContainer");
  ensurePropertyExists(TD, "ui", "Column", "prototype", "playGifIfNotManuallyPaused");
  
  let cancelModal = false;
  
  TD.components.MediaGallery.prototype._loadTweet = appendToFunction(TD.components.MediaGallery.prototype._loadTweet, function(){
    const media = this.chirp.getMedia().find(media => media.mediaId === this.clickedMediaEntityId);
    
    if (media && media.isVideo && media.service === "twitter"){
      window.TDGF_playVideo(media.chooseVideoVariant().url, this.chirp.getChirpURL(), getUsername(this.chirp));
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
});

//
// Block: Detect and notify about connection issues.
//
(function(){
  const onConnectionError = function(){
    return if $("#tweetduck-conn-issues").length;
    
    const ele = $(`#import "markup/offline.html"`).appendTo(document.body);
    
    ele.find("button").click(function(){
      ele.fadeOut(200);
    });
  };
  
  const onConnectionFine = function(){
    const ele = $("#tweetduck-conn-issues");
    
    ele.fadeOut(200, function(){
      ele.remove();
    });
  };
  
  window.addEventListener("offline", onConnectionError);
  window.addEventListener("online", onConnectionFine);
})();
