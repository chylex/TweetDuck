(function($,$TD){
  //
  // Variable: Current timeout ID for update checking.
  //
  var updateCheckTimeoutID;
  
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
  // Block: Setup global functions.
  //
  window.TDGF_runUpdateCheck = runUpdateCheck;
  runUpdateCheck();
})($,$TD);
