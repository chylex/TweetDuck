(function($,$TD,TD){
  //
  // Variable: Says whether TweetD*ck events was initialized.
  //
  var isInitialized = false;
  
  //
  // Function: Initializes TweetD*ck events. Called after the website app is loaded.
  //
  var initializeTweetDck = function(){
    // Settings button hook
    $("[data-action='settings-menu']").click(function(){
      setTimeout(function(){
        var menu = $(".js-dropdown-content").children("ul").first();
        if (menu.length === 0)return;

        menu.children(".drp-h-divider").last().after('<li class="is-selectable" data-std><a href="#" data-action>'+$TD.brandName+' settings</a></li><li class="drp-h-divider"></li>');

        var tweetDckBtn = menu.children("[data-std]").first();

        tweetDckBtn.on("click","a",function(){
          $TD.openSettingsMenu();
        });

        tweetDckBtn.hover(function(){
          $(this).addClass("is-selected");
        },function(){
          $(this).removeClass("is-selected");
        });
      },0);
    });
    
    // Notification handling
    $.subscribe("/notifications/new",function(obj){
      for(var item of obj.items){
        onNewTweet(obj.column,item);
      }
    });
    
    // Finish init
    $TD.loadFontSizeClass(TD.settings.getFontSize());
    $TD.loadNotificationHeadContents(getNotificationHeadContents());
    
    isInitialized = true;
  };
  
  //
  // Function: Prepends code at the beginning of a function. If the prepended function returns true, execution of the original function is cancelled.
  //
  var prependToFunction = function(func, extension){
    return function(){
      return extension.apply(this,arguments) === true ? undefined : func.apply(this,arguments);
    };
  };
  
  //
  // Function: Appends code at the end of a function.
  //
  var appendToFunction = function(func, extension){
    return function(){
      var res = func.apply(this,arguments);
      extension.apply(this,arguments);
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
        withMediaPreview: false
      }));

      html.css("border","0");

      var body = html.find(".tweet-body").first();

      body.children("div.js-quote-detail").each(function(){
        $(this).html("(quoted tweet)");
        $(this).removeClass("padding-al");
        $(this).css("padding","6px");
      });

      body.children("footer").remove();

      $TD.onTweetPopup(html.html(),tweet.fullLength); // TODO column
    }
    else if (column.model.getHasSound()){
      $TD.onTweetSound(); // TODO disable original
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
  // Block: Observe the app <div> element and initialize TweetD*ck whenever possible.
  //
  var app = $("body").children(".js-app");
  
  new MutationObserver(function(mutations){
    if (isInitialized && app.hasClass("is-hidden")){
      isInitialized = false;
    }
    else if (!isInitialized && !app.hasClass("is-hidden")){
      initializeTweetDck();
    }
  }).observe(app[0],{
    attributes: true,
    attributeFilter: [ "class" ]
  });
  
  //
  // Block: Hook into settings object to detect when the settings change.
  //
  TD.settings.setFontSize = appendToFunction(TD.settings.setFontSize,function(name){
    $TD.loadFontSizeClass(name);
  });
  
  TD.settings.setTheme = appendToFunction(TD.settings.setTheme,function(){
    setTimeout(function(){
      $TD.loadNotificationHeadContents(getNotificationHeadContents());
    },0);
  });
  
  //
  // Block: Force popup notification settings
  //
  TD.controller.notifications.hasNotifications = function(){
    return true;
  };

  TD.controller.notifications.isPermissionGranted = function(){
    return true;
  };
  
  //
  // Block: Hook into links to bypass default open function
  //
  (function(){
    var urlWait = false;
    
    var onUrlOpened = function(){
      urlWait = true;
      setTimeout(function(){ urlWait = false; },0);
    };
    
    $(document.body).delegate("a[target='_blank']","click",function(e){
      if (urlWait)return;

      var me = $(this);
      var rel = me.attr("rel");

      if (!me.is(".link-complex") && !(rel === "mediaPreview" && me.closest("#open-modal").length === 0) && rel !== "list" && rel !== "user"){
        $TD.openBrowser(me.attr("href"));
      }

      e.preventDefault();
      onUrlOpened();
    });
    
    window.open = function(url){
      if (urlWait)return;
      
      $TD.openBrowser(url);
      onUrlOpened();
    };
  })();
  
  TD.util.maybeOpenClickExternally = prependToFunction(TD.util.maybeOpenClickExternally,function(e){
    if (e.ctrlKey){
      $TD.openBrowser(e.currentTarget.getAttribute("href"));
      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();
      return true;
    }
  });
  
  //
  // Block: Expand shortened links on hover.
  //
  (function(){
    var cutStart = function(str, search){
      return _.startsWith(str,search) ? str.substr(search.length) : str;
    };
    
    $(document.body).delegate("a[data-full-url]","mouseenter mouseleave",function(e){
      var me = $(this);

      if (e.type === "mouseenter"){
        var text = me.text();
        
        if (text.charCodeAt(text.length-1) !== 8230){ // horizontal ellipsis
          return;
        }
        
        var expanded = me.attr("data-full-url");
        expanded = cutStart(expanded,"https://");
        expanded = cutStart(expanded,"http://");
        expanded = cutStart(expanded,"www.");
        
        me.css("word-break","break-all");
        me.attr("td-prev-text",text);
        me.text(expanded);
      }
      else if (e.type === "mouseleave"){
        var prevText = me.attr("td-prev-text");
        
        if (prevText){
          me.text(prevText);
        }
      }
    });
  })();
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  $(document.body).delegate("a","contextmenu",function(){
    $TD.setLastRightClickedLink($(this).attr("data-full-url") || "");
  });
  
  //
  // Block: Hook into the notification sound effect.
  //
  document.getElementById("update-sound").play = function(){
    $TD.onTweetSound();
  };
  
  //
  // Block: Hook into mp4 video element clicking 
  //
  $(document.body).delegate("video.js-media-gif","click",function(e){
    var src = $(this).attr("src");
    
    if (src.endsWith(".mp4")){
      $TD.openBrowser(src);
      e.preventDefault();
    }
  });
})($,$TD,TD);
