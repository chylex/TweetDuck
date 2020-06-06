//
// Block: Paste images when tweeting.
//
onAppReady.push(function supportImagePaste(){
  const uploader = $._data(document, "events")["uiComposeAddImageClick"][0].handler.context;
  
  app.delegate(".js-compose-text,.js-reply-tweetbox,.td-detect-image-paste", "paste", function(e){
    for(let item of e.originalEvent.clipboardData.items){
      if (item.type.startsWith("image/")){
        if (!$(this).closest(".rpl").find(".js-reply-popout").click().length){ // popout direct messages
          return if $(".js-add-image-button").is(".is-disabled"); // tweetdeck does not check upload count properly
        }
        
        uploader.addFilesToUpload([ item.getAsFile() ]);
        break;
      }
    }
  });
});

//
// Block: Allow changing first day of week in date picker.
//
if (checkPropertyExists($, "tools", "dateinput", "conf", "firstDay")){
  $.tools.dateinput.conf.firstDay = $TDX.firstDayOfWeek;
  
  onAppReady.push(function setupDatePickerFirstDayCallback(){
    window.TDGF_registerPropertyUpdateCallback(function($TDX){
      $.tools.dateinput.conf.firstDay = $TDX.firstDayOfWeek;
    });
  });
}

//
// Block: Override language used for translations.
//
if (checkPropertyExists(TD, "languages", "getSystemLanguageCode")){
  const prevFunc = TD.languages.getSystemLanguageCode;
  
  TD.languages.getSystemLanguageCode = function(returnShortCode){
    return returnShortCode ? ($TDX.translationTarget || "en") : prevFunc.apply(this, arguments);
  };
}

//
// Block: Add missing languages for Bing Translator (Bengali, Icelandic, Tagalog, Tamil, Telugu, Urdu).
//
execSafe(function addMissingTranslationLanguages(){
  ensurePropertyExists(TD, "languages", "getSupportedTranslationSourceLanguages");
  
  const newCodes = [ "bn", "is", "tl", "ta", "te", "ur" ];
  const codeSet = new Set(TD.languages.getSupportedTranslationSourceLanguages());
  
  for(const lang of newCodes){
    codeSet.add(lang);
  }
  
  const codeList = [...codeSet];
  
  TD.languages.getSupportedTranslationSourceLanguages = function(){
    return codeList;
  };
});

//
// Block: Bypass t.co when clicking/dragging links, media, and in user profiles.
//
execSafe(function setupShortenerBypass(){
  $(document.body).delegate("a[data-full-url]", "click auxclick", function(e){
    // event.which seems to be borked in auxclick
    // tweet links open directly in the column
    if ((e.button === 0 || e.button === 1) && $(this).attr("rel") !== "tweet"){
      $TD.openBrowser($(this).attr("data-full-url"));
      e.preventDefault();
    }
  });
  
  $(document.body).delegate("a[data-full-url]", "dragstart", function(e){
    const url = $(this).attr("data-full-url");
    const data = e.originalEvent.dataTransfer;
    
    data.clearData();
    data.setData("text/uri-list", url);
    data.setData("text/plain", url);
    data.setData("text/html", `<a href="${url}">${url}</a>`);
  });
  
  if (checkPropertyExists(TD, "services", "TwitterStatus", "prototype", "_generateHTMLText")){
    TD.services.TwitterStatus.prototype._generateHTMLText = prependToFunction(TD.services.TwitterStatus.prototype._generateHTMLText, function(){
      const card = this.card;
      const entities = this.entities;
      return if !(card && entities);
      
      const urls = entities.urls;
      return if !(urls && urls.length);
      
      const shortUrl = card.url;
      const urlObj = entities.urls.find(obj => obj.url === shortUrl && obj.expanded_url);
      
      if (urlObj){
        const expandedUrl = urlObj.expanded_url;
        card.url = expandedUrl;
        
        const values = card.binding_values;
        
        if (values && values.card_url){
          values.card_url.string_value = expandedUrl;
        }
      }
    });
  }
  
  if (checkPropertyExists(TD, "services", "TwitterMedia", "prototype", "fromMediaEntity")){
    const prevFunc = TD.services.TwitterMedia.prototype.fromMediaEntity;
    
    TD.services.TwitterMedia.prototype.fromMediaEntity = function(){
      const obj = prevFunc.apply(this, arguments);
      const e = arguments[0];
      
      if (e.expanded_url){
        if (obj.url === obj.shortUrl){
          obj.shortUrl = e.expanded_url;
        }
        
        obj.url = e.expanded_url;
      }
      
      return obj;
    };
  }
  
  if (checkPropertyExists(TD, "services", "TwitterUser", "prototype", "fromJSONObject")){
    const prevFunc = TD.services.TwitterUser.prototype.fromJSONObject;
    
    TD.services.TwitterUser.prototype.fromJSONObject = function(){
      const obj = prevFunc.apply(this, arguments);
      const e = arguments[0].entities;
      
      if (e && e.url && e.url.urls && e.url.urls.length && e.url.urls[0].expanded_url){
        obj.url = e.url.urls[0].expanded_url;
      }
      
      return obj;
    };
  }
});

//
// Block: Fix youtu.be previews not showing up for https links.
//
execSafe(function fixYouTubePreviews(){
  ensurePropertyExists(TD, "services", "TwitterMedia");
  
  const media = TD.services.TwitterMedia;
  
  ensurePropertyExists(media, "YOUTUBE_TINY_RE");
  ensurePropertyExists(media, "YOUTUBE_LONG_RE");
  ensurePropertyExists(media, "YOUTUBE_RE");
  ensurePropertyExists(media, "SERVICES", "youtube");
  
  media.YOUTUBE_TINY_RE = new RegExp(media.YOUTUBE_TINY_RE.source.replace("http:", "https?:"));
  media.YOUTUBE_RE = new RegExp(media.YOUTUBE_LONG_RE.source + "|" + media.YOUTUBE_TINY_RE.source);
  media.SERVICES["youtube"] = media.YOUTUBE_RE;
});

//
// Block: Refocus the textbox after switching accounts.
//
onAppReady.push(function setupAccountSwitchRefocus(){
  const refocusInput = function(){
    document.querySelector(".js-docked-compose .js-compose-text").focus();
  };
  
  const accountItemClickEvent = function(e){
    setTimeout(refocusInput, 0);
  };
  
  $(document).on("tduckOldComposerActive", function(e){
    $$(".js-account-list", ".js-docked-compose").delegate(".js-account-item", "click", accountItemClickEvent);
  });
});

//
// Block: Fix docked composer not re-focusing after Alt+Tab & image upload.
//
onAppReady.push(function fixDockedComposerRefocus(){
  $(document).on("tduckOldComposerActive", function(e){
    const ele = $$(".js-compose-text", ".js-docked-compose");
    const node = ele[0];
    
    let cancelBlur = false;
    
    ele.on("blur", function(e){
      cancelBlur = true;
      setTimeout(function(){ cancelBlur = false; }, 0);
    });
    
    window.TDGF_prioritizeNewestEvent(node, "blur");
    
    node.blur = prependToFunction(node.blur, function(){
      return cancelBlur;
    });
  });
  
  ensureEventExists(document, "uiComposeImageAdded");
  
  $(document).on("uiComposeImageAdded", function(){
    document.querySelector(".js-docked-compose .js-compose-text").focus();
  });
});

//
// Block: Fix DM reply input box not getting focused after opening a conversation.
//
if (checkPropertyExists(TD, "components", "ConversationDetailView", "prototype", "showChirp")){
  TD.components.ConversationDetailView.prototype.showChirp = appendToFunction(TD.components.ConversationDetailView.prototype.showChirp, function(){
    return if !$TDX.focusDmInput;
    
    setTimeout(function(){
      document.querySelector(".js-reply-tweetbox").focus();
    }, 100);
  });
}

//
// Block: Hold Shift to restore cleared column.
//
execSafe(function supportShiftToClearColumn(){
  ensurePropertyExists(TD, "vo", "Column", "prototype", "clear");
  
  let holdingShift = false;
  
  const updateShiftState = (pressed) => {
    if (pressed != holdingShift){
      holdingShift = pressed;
      $("button[data-action='clear']").children("span").text(holdingShift ? "Restore" : "Clear");
    }
  };
  
  const resetActiveFocus = () => {
    document.activeElement.blur();
  };
  
  document.addEventListener("keydown", function(e){
    if (e.shiftKey && (document.activeElement === null || !("value" in document.activeElement))){
      updateShiftState(true);
    }
  });
  
  document.addEventListener("keyup", function(e){
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
});

//
// Block: Make temporary search column appear as the first one and clear the input box.
//
execSafe(function setupSearchColumnHook(){
  ensurePropertyExists(TD, "controller", "columnManager", "_columnOrder");
  ensurePropertyExists(TD, "controller", "columnManager", "move");
  
  $(document).on("uiSearchNoTemporaryColumn", function(e, data){
    if (data.query && data.searchScope !== "users" && !data.columnKey){
      if ($TDX.openSearchInFirstColumn){
        const order = TD.controller.columnManager._columnOrder;
        
        if (order.length > 1){
          const columnKey = order[order.length - 1];
          
          order.splice(order.length - 1, 1);
          order.splice(1, 0, columnKey);
          TD.controller.columnManager.move(columnKey, "left");
        }
      }
      
      if (!("tduckResetInput" in data)){
        $(".js-app-search-input").val("");
        $(".js-perform-search").blur();
      }
    }
  });
});

//
// Block: Reorder search results to move accounts above hashtags.
//
onAppReady.push(function reorderSearchResults(){
  const container = $(".js-search-in-popover");
  const hashtags = $$(".js-typeahead-topic-list", container);
  
  $$(".js-typeahead-user-list", container).insertBefore(hashtags);
  hashtags.addClass("list-divider");
});

//
// Block: Revert Like/Follow dialogs being closed after clicking an action.
//
execSafe(function setupLikeFollowDialogRevert(){
  const prevSetTimeout = window.setTimeout;
  
  const overrideState = function(){
    return if !$TDX.keepLikeFollowDialogsOpen;
    
    window.setTimeout = function(func, timeout){
      return timeout !== 500 && prevSetTimeout.apply(this, arguments);
    };
  };
  
  const restoreState = function(context, key){
    window.setTimeout = prevSetTimeout;
    
    if ($TDX.keepLikeFollowDialogsOpen && key in context.state){
      context.state[key] = false;
    }
  };
  
  $(document).on("uiShowFavoriteFromOptions", function(){
    $(".js-btn-fav", ".js-modal-inner").each(function(){
      let event = $._data(this, "events").click[0];
      let handler = event.handler;
      
      event.handler = function(){
        overrideState();
        handler.apply(this, arguments);
        restoreState($._data(document, "events").dataFavoriteState[0].handler.context, "stopSubsequentLikes");
      };
    });
  });
  
  $(document).on("uiShowFollowFromOptions", function(){
    $(".js-component", ".js-modal-inner").each(function(){
      let event = $._data(this, "events").click[0];
      let handler = event.handler;
      let context = handler.context;
      
      event.handler = function(){
        overrideState();
        handler.apply(this, arguments);
        restoreState(context, "stopSubsequentFollows");
      };
    });
  });
});

//
// Block: Fix broken horizontal scrolling of column container when holding Shift.
//
if (checkPropertyExists(TD, "ui", "columns", "setupColumnScrollListeners")){
  TD.ui.columns.setupColumnScrollListeners = appendToFunction(TD.ui.columns.setupColumnScrollListeners, function(column){
    const ele = document.querySelector(".js-column[data-column='" + column.model.getKey() + "']");
    return if ele == null;
    
    $(ele).off("onmousewheel").on("mousewheel", ".scroll-v", function(e){
      e.stopImmediatePropagation();
    });
    
    window.TDGF_prioritizeNewestEvent(ele, "mousewheel");
  });
}

//
// Block: Fix DM image previews and GIF thumbnails not loading due to new URLs.
//
if (checkPropertyExists(TD, "services", "TwitterMedia", "prototype", "getTwitterPreviewUrl")){
  const prevFunc = TD.services.TwitterMedia.prototype.getTwitterPreviewUrl;
  
  TD.services.TwitterMedia.prototype.getTwitterPreviewUrl = function(){
    const url = prevFunc.apply(this, arguments);
    
    if (url.startsWith("https://ton.twitter.com/1.1/ton/data/dm/") || url.startsWith("https://pbs.twimg.com/tweet_video_thumb/")){
      const format = url.match(/\?.*format=(\w+)/);
      
      if (format && format.length === 2){
        const fix = `.${format[1]}?`;
        
        if (!url.includes(fix)){
          return url.replace("?", fix);
        }
      }
    }
    
    return url;
  };
}

//
// Block: Fix DMs not being marked as read when replying to them.
//
execSafe(function markRepliedDMsAsRead(){
  ensurePropertyExists(TD, "controller", "clients", "getClient");
  ensurePropertyExists(TD, "services", "Conversations", "prototype", "getConversation");
  
  $(document).on("dataDmSent", function(e, data){
    const client = TD.controller.clients.getClient(data.request.accountKey);
    return if !client;
    
    const conversation = client.conversations.getConversation(data.request.conversationId);
    return if !conversation;
    
    conversation.markAsRead();
  });
});

//
// Block: Limit amount of loaded DMs to avoid massive lag from re-opening them several times.
//
if (checkPropertyExists(TD, "services", "TwitterConversation", "prototype", "renderThread")){
  const prevFunc = TD.services.TwitterConversation.prototype.renderThread;
  
  TD.services.TwitterConversation.prototype.renderThread = function(){
    const prevMessages = this.messages;
    
    this.messages = prevMessages.slice(0, 100);
    const result = prevFunc.apply(this, arguments);
    this.messages = prevMessages;
    
    return result;
  };
}

//
// Block: Fix scheduled tweets not showing up sometimes.
//
execSafe(function fixScheduledTweets(){
  ensurePropertyExists(TD, "controller", "columnManager", "getAll");
  ensureEventExists(document, "dataTweetSent");
  
  $(document).on("dataTweetSent", function(e, data){
    if (data.response.state && data.response.state === "scheduled"){
      const column = Object.values(TD.controller.columnManager.getAll()).find(column => column.model.state.type === "scheduled");
      return if !column;
      
      setTimeout(function(){
        column.reloadTweets();
      }, 1000);
    }
  });
});

//
// Block: Let's make retweets lowercase again.
//
window.TDGF_injectMustache("status/tweet_single.mustache", "replace", "{{_i}} Retweeted{{/i}}", "{{_i}} retweeted{{/i}}");

if (checkPropertyExists(TD, "services", "TwitterActionRetweet", "prototype", "generateText")){
  TD.services.TwitterActionRetweet.prototype.generateText = appendToFunction(TD.services.TwitterActionRetweet.prototype.generateText, function(){
    this.text = this.text.replace(" Retweeted", " retweeted");
    this.htmlText = this.htmlText.replace(" Retweeted", " retweeted");
  });
}

if (checkPropertyExists(TD, "services", "TwitterActionRetweetedInteraction", "prototype", "generateText")){
  TD.services.TwitterActionRetweetedInteraction.prototype.generateText = appendToFunction(TD.services.TwitterActionRetweetedInteraction.prototype.generateText, function(){
    this.htmlText = this.htmlText.replace(" Retweeted", " retweeted").replace(" Retweet", " retweet");
  });
}
