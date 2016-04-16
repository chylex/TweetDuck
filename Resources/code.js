(function($,$TD){
  //
  // Constant: List of valid font size classes.
  //
  var fontSizeClasses = [ "txt-base-smallest", "txt-base-small", "txt-base-medium", "txt-base-large", "txt-base-largest" ];
  
  //
  // Variable: Says whether TweetDick events was initialized.
  //
  var isInitialized = false;
  
  //
  // Variable: Previous font size class in the <html> tag.
  //
  var prevFontSizeClass;
  
  //
  // Function: Initializes TweetDick events. Called after the website app is loaded.
  //
  var initializeTweetDick = function(){
    // Settings button hook
    $("[data-action='settings-menu']").click(function(){
      setTimeout(function(){
        var menu = $(".js-dropdown-content").children("ul").first();
        if (menu.length === 0)return;

        menu.children(".drp-h-divider").last().after('<li class="is-selectable" data-std><a href="#" data-action>'+$TD.brandName+'</a></li><li class="drp-h-divider"></li>');

        var tweetDickBtn = menu.children("[data-std]").first();

        tweetDickBtn.on("click","a",function(){
          $TD.openSettingsMenu();
        });

        tweetDickBtn.hover(function(){
          $(this).addClass("is-selected");
        },function(){
          $(this).removeClass("is-selected");
        });
      },0);
    });
    
    // Tweet notifications
    (function(){
      var columns = $(".js-app-columns").first();

      var refreshColumnObservers = function(){
        columns.children().each(function(){
          registerTweetObserverForColumn($(this));
        });
      };

      new MutationObserver(refreshColumnObservers).observe(columns[0],{
        childList: true
      });

      refreshColumnObservers();
    })();
    
    // Popup notifications
    window.TD.controller.notifications.hasNotifications = function(){
      return true;
    };
    
    window.TD.controller.notifications.isPermissionGranted = function(){
      return true;
    };
    
    // Finish init
    isInitialized = true;
  };
  
  //
  // Function: Registers an observer to a TweetDeck column, which reports new tweets.
  //
  var registerTweetObserverForColumn = function(column){
    if (column[0].hasAttribute("data-std-observed"))return;
    
    var mid = column;
    mid = mid.children().first();
    mid = mid.children().first();
    mid = mid.children(".js-column-content").first();
    mid = mid.children(".js-column-scroller").first();
    
    var container = mid.children(".js-chirp-container").first();
    if (container.length === 0)return;
    
    var scroller = container.parent();
    
    new MutationObserver(function(mutations){
      if (!container[0].hasAttribute("data-std-loaded")){
        container[0].setAttribute("data-std-loaded","");
        return;
      }
      
      var data = TD.controller.columnManager.get(column.attr("data-column"));
      if (!data.model.getHasNotification())return;
      
      if (scroller.scrollTop() != 0)return;
      
      Array.prototype.forEach.call(mutations,function(mutation){
        Array.prototype.forEach.call(mutation.addedNodes,function(node){
          if (node.tagName !== "ARTICLE")return;
          
          onNewTweet(column,node);
        });
      });
    }).observe(container[0],{
      childList: true
    });
    
    column[0].setAttribute("data-std-observed","");
  };
  
  //
  // Function: Event callback for a new tweet.
  //
  var onNewTweet = function(column, tweet){
    var html = $(tweet.outerHTML);
    var body = html.find(".tweet-body").first();
    
    body.children("div.js-quote-detail").each(function(){
      $(this).html("(quoted tweet)");
    });
    
    body.children().not("p,span,div.js-quote-detail").remove();
    
    var characters = html.find(".js-tweet-text:first").text().length;
    if (characters == 0)return;
    
    $TD.onTweetPopup(html.html(),characters); // TODO column & remove pic links from text()
  };
  
  //
  // Function: Retrieves the font size using <html> class attribute.
  //
  var getFontSizeClass = function(){
    for(var index = 0; index < fontSizeClasses.length; index++){
      if (document.documentElement.classList.contains(fontSizeClasses[index])){
        return fontSizeClasses[index];
      }
    }
    
    return fontSizeClasses[0];
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
  // Block: Observe the app <div> element and initialize TweetDick whenever possible.
  //
  var app = $("body").children(".js-app");
  
  new MutationObserver(function(mutations){
    if (isInitialized && app.hasClass("is-hidden")){
      isInitialized = false;
    }
    else if (!isInitialized && !app.hasClass("is-hidden")){
      initializeTweetDick();
    }
  }).observe(app[0],{
    attributes: true,
    attributeFilter: [ "class" ]
  });
  
  //
  // Block: Observe changes in <html> class to update font size.
  //
  new MutationObserver(function(mutations){
    var fsClass = getFontSizeClass();
    
    if (fsClass != prevFontSizeClass){
      prevFontSizeClass = fsClass;
      $TD.loadFontSizeClass(fsClass);
    }
  }).observe(document.documentElement,{
    attributes: true,
    attributeFilter: [ "class" ]
  });
  
  //
  // Block: Observe stylesheet swapping.
  //
  new MutationObserver(function(mutations){
    $TD.loadNotificationHeadContents(getNotificationHeadContents());
  }).observe(document.head.querySelector("[http-equiv='Default-Style']"),{
    attributes: true,
    attributeFilter: [ "content" ]
  });
  
  $TD.loadNotificationHeadContents(getNotificationHeadContents());
  
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

      if (!me.is(".link-complex") && !(rel === "mediaPreview" && me.closest("#open-modal").length === 0) && rel !== "list"){
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
})($,$TD);
