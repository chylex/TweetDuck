(function($, $TDU){
  //
  // Variable: Current timeout ID for update checking.
  //
  var updateCheckTimeoutID;
  
  //
  // Constant: Update exe file name.
  //
  const updateFileName = $TDU.brandName+".Update.exe";
  
  //
  // Constant: Url that returns JSON data about latest version.
  //
  const updateCheckUrl = "https://api.github.com/repos/chylex/"+$TDU.brandName+"/releases/latest";
  
  //
  // Function: Creates the update notification element. Removes the old one if already exists.
  //
  var createUpdateNotificationElement = function(version, download){
    var outdated = version === "unsupported";
    var tweetdick = version === "tweetdick";
    
    var ele = $("#tweetdck-update");
    var existed = ele.length > 0;
    
    if (existed > 0){
      ele.remove();
    }
    
    var html = outdated ? [
      "<div id='tweetdck-update'>",
      "<p class='tdu-title'>Unsupported System</p>",
      "<p class='tdu-info'>You will not receive updates.</p>",
      "<div class='tdu-buttons'>",
      "<button class='btn btn-positive tdu-btn-unsupported'><span class='label'>Read More</span></button>",
      "<button class='btn btn-negative tdu-btn-dismiss'><span class='label'>Dismiss</span></button>",
      "</div>",
      "</div>"
    ] : tweetdick ? [
      "<div id='tweetdck-update'>",
      "<p class='tdu-title'>TweetDick Ending</p>",
      "<p class='tdu-info'>Please, move to TweetDuck.</p>",
      "<div class='tdu-buttons'>",
      "<button class='btn btn-positive tdu-btn-unsupported'><span class='label'>Read More</span></button>",
      "<button class='btn btn-negative tdu-btn-dismiss'><span class='label'>Dismiss</span></button>",
      "</div>",
      "</div>"
    ] : [
      "<div id='tweetdck-update'>",
      "<p class='tdu-title'>"+$TDU.brandName+" Update</p>",
      "<p class='tdu-info'>Version "+version+" is now available.</p>",
      "<div class='tdu-buttons'>",
      "<button class='btn btn-positive tdu-btn-download'><span class='label'>Download</span></button>",
      "<button class='btn btn-negative tdu-btn-dismiss'><span class='label'>Dismiss</span></button>",
      "</div>",
      "</div>"
    ];

    $(document.body).append(html.join(""));

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
      borderRadius: "2px",
      zIndex: 9999
    });

    ele.children("p.tdu-title").first().css({
      fontSize: "17px",
      fontWeight: "bold",
      textAlign: "center",
      letterSpacing: "0.2px",
      margin: "5px auto 2px"
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
      $TDU.onUpdateAccepted(version, download);
    });

    buttonDiv.children(".tdu-btn-unsupported").click(function(){
      $TDU.openBrowser("https://github.com/chylex/TweetDuck/wiki/Supported-Systems");
    });

    buttonDiv.children(".tdu-btn-tweetdick").click(function(){
      $TDU.openBrowser("https://github.com/chylex/TweetDick/wiki/Future-of-TweetDick");
    });

    buttonDiv.children(".tdu-btn-dismiss,.tdu-btn-unsupported").click(function(){
      $TDU.onUpdateDismissed(version);
      ele.slideUp(function(){ ele.remove(); });
    });
    
    if (!existed){
      ele.slideDown();
    }
    
    return ele;
  };
  
  //
  // Function: Runs an update check and updates all DOM elements appropriately.
  //
  var runUpdateCheck = function(force, eventID){
    if (!$TDU.isSystemSupported){
      if ($TDU.dismissedVersionTag !== "unsupported"){
        createUpdateNotificationElement("unsupported");
      }
      
      return;
    }
    else if ($TDU.brandName === "TweetDick"){
      if ($TDU.dismissedVersionTag !== "tweetdick"){
        createUpdateNotificationElement("tweetdick");
      }
      
      return;
    }
    
    clearTimeout(updateCheckTimeoutID);
    updateCheckTimeoutID = setTimeout(runUpdateCheck, 1000*60*60); // 1 hour
    
    if (!$TDU.updateCheckEnabled && !force)return;
    
    $.getJSON(updateCheckUrl, function(response){
      var tagName = response.tag_name;
      var hasUpdate = tagName !== $TDU.versionTag && tagName !== $TDU.dismissedVersionTag && response.assets.length > 0;
      
      if (hasUpdate){
        var obj = response.assets.find(asset => asset.name === updateFileName) || response.assets[0];
        createUpdateNotificationElement(tagName, obj.browser_download_url);
      }
      
      if (eventID !== 0){
        $TDU.onUpdateCheckFinished(eventID, hasUpdate, tagName);
      }
    });
  };
  
  //
  // Block: Setup global functions.
  //
  window.TDUF_runUpdateCheck = runUpdateCheck;
  runUpdateCheck();
})($, $TDU);
