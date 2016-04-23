(function($,$TD){
  //
  // Constant: List of valid font size classes.
  //
  var fontSizeClasses = [ "txt-base-smallest", "txt-base-small", "txt-base-medium", "txt-base-large", "txt-base-largest" ];
  
  //
  // Variable: Says whether TweetD*ck events was initialized.
  //
  var isInitialized = false;
  
  //
  // Variable: Previous font size class in the <html> tag.
  //
  var prevFontSizeClass;
  
  //
  // Variable: Current timeout ID for update checking.
  //
  var updateCheckTimeoutID;
  
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
    
    // Force popup notification settings
    window.TD.controller.notifications.hasNotifications = function(){
      return true;
    };
    
    window.TD.controller.notifications.isPermissionGranted = function(){
      return true;
    };
    
    // Run update check
    runUpdateCheck();
    
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
    
    if (html.find(".icon-reply").length > 0){
      return; // ignore sent messages
    }
    
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
  // Function: Creates the update notification element. Removes the old one if already exists.
  //
  var createUpdateNotificationElement = function(version, download){
    var ele = $("#tweetdck-update");
    var existed = ele.length > 0;
    
    if (existed > 0){
      ele.remove();
    }
    
    var html = [
      "<div id='tweetdck-update'>",
      "<p class='tdu-title'>"+$TD.brandName+" Update</p>",
      "<p class='tdu-info'>Version "+version+" is now available.</p>",
      "<div class='tdu-buttons'>",
      "<button class='btn btn-positive tdu-btn-download'><span class='label'>Download</button>",
      "<button class='btn btn-negative tdu-btn-dismiss'><span class='label'>Dismiss</button>",
      "</div>",
      "</div>"
    ];

    $("h1.app-title").after(html.join(""));

    ele = $("#tweetdck-update");

    var buttonDiv = ele.children("div.tdu-buttons").first();

    ele.css({
      color: "#fff",
      backgroundColor: "rgb(32,94,138)",
      position: "absolute",
      left: "4px",
      bottom: "4px",
      width: "192px",
      height: "86px",
      display: existed ? "block" : "none",
      borderRadius: "2px"
    });

    ele.children("p.tdu-title").first().css({
      fontSize: "17px",
      fontWeight: "bold",
      textAlign: "center",
      letterSpacing: "0.2px",
      margin: "4px auto 2px"
    });

    ele.children("p.tdu-info").first().css({
      fontSize: "12px",
      textAlign: "center",
      margin: "2px auto 6px"
    });

    buttonDiv.css({
      textAlign: "center"
    });

    buttonDiv.children().css({
      margin: "0 4px",
      minHeight: "25px",
      boxShadow: "1px 1px 1px rgba(17,17,17,0.5)"
    });

    buttonDiv.find("span").css({
      verticalAlign: "baseline"
    });

    ele.find("span.tdu-data-tag").first().css({
      cursor: "pointer",
      textDecoration: "underline"
    });

    buttonDiv.children(".tdu-btn-download").click(function(){
      ele.remove();
      $TD.onUpdateAccepted(version,download);
    });

    buttonDiv.children(".tdu-btn-dismiss").click(function(){
      $TD.onUpdateDismissed(version);
      ele.slideUp(function(){ ele.remove(); });
    });
    
    if (!existed){
      ele.slideDown();
    }
    
    return ele;
  };
  
  //
  // Function: Runs an update check and updates all DOM elements appropriately
  //
  var runUpdateCheck = function(){
    clearTimeout(updateCheckTimeoutID);
    updateCheckTimeoutID = setTimeout(runUpdateCheck,1000*60*60); // 1 hour
    
    if (!$TD.updateCheckEnabled)return;
    
    $.getJSON("https://api.github.com/repos/chylex/"+$TD.brandName+"/releases/latest",function(response){
      var tagName = response.tag_name;
      
      if (tagName != $TD.versionTag && tagName != $TD.dismissedVersionTag && response.assets.length > 0){
        createUpdateNotificationElement(tagName,response.assets[0].browser_download_url);
      }
    });
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
  
  //
  // Block: Setup global functions.
  //
  window.TDGF_runUpdateCheck = runUpdateCheck;
})($,$TD);
