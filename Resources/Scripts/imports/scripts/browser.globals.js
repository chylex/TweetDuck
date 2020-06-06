//
// Functions: Responds to updating $TDX properties.
//
(function(){
  let callbacks = [];
  
  window.TDGF_registerPropertyUpdateCallback = function(callback){
    callbacks.push(callback);
  };
  
  window.TDGF_onPropertiesUpdated = function(){
    callbacks.forEach(func => func($TDX));
  };
})();

//
// Function: Injects custom HTML into mustache templates.
//
window.TDGF_injectMustache = function(name, operation, search, custom){
  let replacement;
  
  switch(operation){
    case "replace": replacement = custom; break;
    case "append": replacement = search + custom; break;
    case "prepend": replacement = custom + search; break;
    default: throw "Invalid mustache injection operation. Only 'replace', 'append', 'prepend' are supported.";
  }
  
  const prev = TD.mustaches && TD.mustaches[name];
  
  if (!prev){
    crashDebug("Mustache injection is referencing an invalid mustache: " + name);
    return false;
  }
  
  TD.mustaches[name] = prev.replace(search, replacement);
  
  if (prev === TD.mustaches[name]){
    crashDebug("Mustache injection had no effect: " + name);
    return false;
  }
  
  return true;
};

//
// Function: Pushes the newest jQuery event to the beginning of the event handler list.
//
window.TDGF_prioritizeNewestEvent = function(element, event){
  const events = $._data(element, "events");
  
  const handlers = events[event];
  const newHandler = handlers[handlers.length - 1];
  
  for(let index = handlers.length - 1; index > 0; index--){
    handlers[index] = handlers[index - 1];
  }
  
  handlers[0] = newHandler;
};

//
// Function: Returns a display name for a column object.
//
window.TDGF_getColumnName = (function(){
  const titles = {
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
  
  return function(column){
    return titles[column._tduck_icon] || "";
  };
})();

//
// Function: Adds a search column with the specified query.
//
onAppReady.push(() => execSafe(function setupSearchFunction(){
  const context = $._data(document, "events")["uiSearchInputSubmit"][0].handler.context;
  
  window.TDGF_performSearch = function(query){
    context.performSearch({ query, tduckResetInput: true });
  };
}, function(){
  window.TDGF_performSearch = function(){
    alert("error|This feature is not available due to an internal error.");
  };
}));

//
// Function: Plays sound notification.
//
window.TDGF_playSoundNotification = function(){
  document.getElementById("update-sound").play();
};

//
// Function: Plays video using the internal player.
//
window.TDGF_playVideo = function(videoUrl, tweetUrl, username){
  return if !videoUrl;
  
  $TD.playVideo(videoUrl, tweetUrl || videoUrl, username || null, function(){
    $('<div id="td-video-player-overlay" class="ovl" style="display:block"></div>').on("click contextmenu", function(){
      $TD.stopVideo();
    }).appendTo(app);
  });
};

//
// Function: Shows tweet detail, used in notification context menu.
//
execSafe(function setupShowTweetDetail(){
  ensurePropertyExists(TD, "ui", "updates", "showDetailView");
  ensurePropertyExists(TD, "controller", "columnManager", "showColumn");
  ensurePropertyExists(TD, "controller", "columnManager", "getByApiid");
  ensurePropertyExists(TD, "controller", "clients", "getPreferredClient");
  
  const showTweetDetailInternal = function(column, chirp){
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
    
    const column = TD.controller.columnManager.getByApiid(columnId);
    
    if (!column){
      if (confirm("error|The column which contained the tweet no longer exists. Would you like to open the tweet in your browser instead?")){
        $TD.openBrowser(fallbackUrl);
      }
      
      return;
    }
    
    const chirp = column.findMostInterestingChirp(chirpId);
    
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
}, function(){
  window.TDGF_showTweetDetail = function(){
    alert("error|This feature is not available due to an internal error.");
  };
});

//
// Function: Screenshots tweet to clipboard.
//
execSafe(function setupTweetScreenshot(){
  window.TDGF_triggerScreenshot = function(){
    const hovered = getHoveredTweet();
    return if !hovered;
    
    const columnWidth = $(hovered.column.ele).width();
    const tweet = hovered.wrap || hovered.obj;
    
    const html = $(tweet.render({
      withFooter: false,
      withTweetActions: false,
      isInConvo: false,
      isFavorite: false,
      isRetweeted: false, // keeps retweet mark above tweet
      isPossiblySensitive: false,
      mediaPreviewSize: hovered.column.obj.getMediaPreviewSize()
    }));
    
    html.find("footer").last().remove(); // apparently withTweetActions breaks for certain tweets, nice
    html.find(".td-screenshot-remove").remove();
    
    html.find("p.link-complex-target,p.txt-mute").filter(function(){
      return $(this).text() === "Show this thread";
    }).remove();
    
    html.addClass($(document.documentElement).attr("class"));
    html.addClass($(document.body).attr("class"));
    
    html.css("background-color", getClassStyleProperty("stream-item", "background-color"));
    html.css("border", "none");
    
    for(let selector of [ ".js-quote-detail", ".js-media-preview-container", ".js-media" ]){
      const ele = html.find(selector);
      
      if (ele.length){
        ele[0].style.setProperty("margin-bottom", "2px", "important");
        break;
      }
    }
    
    const gif = html.find(".js-media-gif-container");
    
    if (gif.length){
      gif.css("background-image", 'url("'+tweet.getMedia()[0].small()+'")');
    }
    
    const type = tweet.getChirpType();
    
    if ((type.startsWith("favorite") || type.startsWith("retweet")) && tweet.isAboutYou()){
      html.addClass("td-notification-padded");
    }
    
    $TD.screenshotTweet(html[0].outerHTML, columnWidth);
  };
}, function(){
  window.TDGF_triggerScreenshot = function(){
    alert("error|This feature is not available due to an internal error.");
  };
});

//
// Function: Apply ROT13 to input selection.
//
window.TDGF_applyROT13 = function(){
  const ele = document.activeElement;
  return if !ele || !ele.value;
  
  const selection = ele.value.substring(ele.selectionStart, ele.selectionEnd);
  return if !selection;
  
  document.execCommand("insertText", false, selection.replace(/[a-zA-Z]/g, function(chr){
    const code = chr.charCodeAt(0);
    const start = code <= 90 ? 65 : 97;
    return String.fromCharCode(start + (code - start + 13) % 26);
  }));
};

//
// Function: Reloads all columns.
//
if (checkPropertyExists(TD, "controller", "columnManager", "getAll")){
  window.TDGF_reloadColumns = function(){
    Object.values(TD.controller.columnManager.getAll()).forEach(column => column.reloadTweets());
  };
}
else{
  window.TDGF_reloadColumns = function(){};
}

//
// Function: Reloads the website with memory cleanup.
//
window.TDGF_reload = function(){
  window.gc && window.gc();
  window.location.reload();
  
  window.TDGF_reload = function(){}; // redefine to prevent reloading multiple times
};
