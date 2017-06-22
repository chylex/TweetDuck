(function($, $TDU){
  //
  // Variable: Current timeout ID for update checking.
  //
  var updateCheckTimeoutID;
  
  //
  // Constant: Update exe file name.
  //
  const updateFileName = "TweetDuck.Update.exe";
  
  //
  // Constant: Url that returns JSON data about latest version.
  //
  const updateCheckUrlLatest = "https://api.github.com/repos/chylex/TweetDuck/releases/latest";
  
  //
  // Constant: Url that returns JSON data about all versions, including prereleases.
  //
  const updateCheckUrlAll = "https://api.github.com/repos/chylex/TweetDuck/releases";
  
  //
  // Constant: Fallback url in case the update installer file is missing.
  //
  const updateDownloadFallback = "https://tweetduck.chylex.com";
  
  //
  // Function: Creates the update notification element. Removes the old one if already exists.
  //
  var displayNotification = function(version, download){
    var outdated = version === "unsupported";
    
    var css = $("#tweetduck-update-css");
    
    if (!css.length){
      css = $(`
<style id='tweetduck-update-css'>
#tweetduck-update {
  position: absolute;
  bottom: 0;
  width: 200px;
  height: 170px;
  z-index: 9999;
  color: #fff;
  background-color: rgb(32, 94, 138);
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.5);
}

p.tdu-title {
  font-size: 17px;
  font-weight: bold;
  text-align: center;
  letter-spacing: 0.2px;
  margin: 8px auto 2px;
}

p.tdu-info {
  font-size: 12px;
  text-align: center;
  margin: 3px auto 0;
}

div.tdu-buttons button {
  display: block;
  margin: 7px auto 0;
  width: 80%;
  height: 30px;
  border-radius: 1px;
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.5);
  box-shadow: 1px 1px 1px rgba(17, 17, 17, 0.5);
}
</style>
`).appendTo(document.head);
    }
    
    var ele = $("#tweetduck-update");
    var existed = ele.length > 0;
    
    if (existed){
      ele.remove();
    }
    
    ele = $(outdated ? `
<div id='tweetduck-update'>
  <p class='tdu-title'>Unsupported System</p>
  <p class='tdu-info'>You will not receive updates.</p>
  <div class='tdu-buttons'>
    <button class='btn btn-positive tdu-btn-unsupported'>Read more</button>
    <button class='btn btn-negative tdu-btn-ignore'>Dismiss</button>
  </div>
</div>
` : `
<div id='tweetduck-update'>
  <p class='tdu-title'>TweetDuck Update</p>
  <p class='tdu-info'>Version ${version} is now available.</p>
  <div class='tdu-buttons'>
    <button class='btn btn-positive tdu-btn-download'>Update now</button>
    <button class='btn btn-negative tdu-btn-ignore'>Ignore this update</button>
  </div>
</div>
`).appendTo(document.body).css("display", existed ? "block" : "none");
    
    var hide = function(){
      ele.remove();
      css.remove();
    };
    
    var buttonDiv = ele.children("div.tdu-buttons").first();

    buttonDiv.children(".tdu-btn-download").click(function(){
      hide();
      
      if (download){
        $TDU.onUpdateAccepted();
      }
      else{
        $TDU.openBrowser(updateDownloadFallback);
      }
    });

    buttonDiv.children(".tdu-btn-unsupported").click(function(){
      $TDU.openBrowser("https://github.com/chylex/TweetDuck/wiki/Supported-Systems");
    });

    buttonDiv.children(".tdu-btn-ignore,.tdu-btn-unsupported").click(function(){
      $TDU.onUpdateDismissed();
      ele.slideUp(hide);
    });
    
    if (!existed){
      ele.slideDown();
    }
    
    return ele;
  };
  
  //
  // Function: Returns milliseconds until the start of the next hour, with an extra offset in seconds that can skip an hour if the clock would roll over too soon.
  //
  var getTimeUntilNextHour = function(extra){
    var now = new Date();
    var offset = new Date(+now+extra*1000);
    return new Date(offset.getFullYear(), offset.getMonth(), offset.getDate(), offset.getHours()+1, 0, 0)-now;
  };
  
  //
  // Function: Runs an update check and updates all DOM elements appropriately.
  //
  var runUpdateCheck = function(eventID, versionTag, dismissedVersionTag, allowPre){
    clearTimeout(updateCheckTimeoutID);
    updateCheckTimeoutID = setTimeout($TDU.triggerUpdateCheck, getTimeUntilNextHour(60*30)); // 30 minute offset
    
    $.getJSON(allowPre ? updateCheckUrlAll : updateCheckUrlLatest, function(response){
      var release = allowPre ? response[0] : response;
      
      var tagName = release.tag_name;
      var hasUpdate = tagName !== versionTag && tagName !== dismissedVersionTag && release.assets.length > 0;
      
      if (hasUpdate){
        var obj = release.assets.find(asset => asset.name === updateFileName) || { browser_download_url: "" };
        displayNotification(tagName, obj.browser_download_url);
        
        if (eventID){ // ignore undefined and 0
          $TDU.onUpdateCheckFinished(eventID, tagName, obj.browser_download_url);
        }
      }
      else if (eventID){ // ignore undefined and 0
        $TDU.onUpdateCheckFinished(eventID, null, null);
      }
    });
  };
  
  //
  // Block: Setup global functions.
  //
  window.TDUF_displayNotification = displayNotification;
  window.TDUF_runUpdateCheck = runUpdateCheck;
})($, $TDU);
