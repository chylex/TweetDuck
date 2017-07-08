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
  var displayNotification = function(version, download, changelog){
    var outdated = version === "unsupported";
    
    // styles
    var css = $("#tweetduck-update-css");
    
    if (!css.length){
      css = $(`
<style id='tweetduck-update-css'>
#tweetduck-update {
  position: absolute;
  bottom: 0;
  width: 200px;
  height: 178px;
  z-index: 9999;
  color: #fff;
  background-color: rgb(32, 94, 138);
  text-align: center;
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.5);
}

.tdu-title {
  font-size: 15px;
  font-weight: bold;
  margin: 8px 0 2px;
  cursor: default;
}

.tdu-info {
  display: inline-block;
  font-size: 14px;
  margin: 3px 0;
}

.tdu-showlog {
  text-decoration: underline;
  cursor: pointer;
}

.tdu-buttons button {
  display: block;
  margin: 7px auto 0;
  padding: 4px 10px;
  width: 80%;
  height: 30px;
  border: 0;
  border-radius: 1px;
  outline: none;
  font-size: 14px;
  color: #fff;
  background-color: #419de0;
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.5);
  box-shadow: 1px 1px 1px rgba(17, 17, 17, 0.5) !important;
  transition: box-shadow 0.2s ease;
}

.tdu-buttons button:hover {
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.75);
  box-shadow: 1px 1px 1px rgba(17, 17, 17, 0.75), 0 -2px 0 rgba(17, 17, 17, 0.33) inset !important;
}

.tdu-buttons button.tdu-btn-ignore, .tdu-buttons button.tdu-btn-later {
  background-color: #607a8e;
  color: #dfdfdf;
}

#tweetduck-changelog {
  position: absolute;
  left: 0;
  top: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 9998;
}

#tweetduck-changelog-box {
  position: absolute;
  width: 60%;
  height: 75%;
  max-width: calc(90% - 200px);
  max-height: 90%;
  left: calc(50% + 100px);
  top: 50%;
  padding: 12px;
  overflow-y: auto;
  transform: translateX(-50%) translateY(-50%);
  font-size: 14px;
  color: #000;
  background-color: #fff;
  box-sizing: border-box;
}

#tweetduck-changelog h2 {
  margin: 0 0 7px;
  font-size: 23px;
}

#tweetduck-changelog h3 {
  margin: 0 0 5px 7px;
  font-size: 18px;
}

#tweetduck-changelog p {
  margin: 0 0 2px 30px;
  display: list-item;
}

#tweetduck-changelog p.l2 {
  margin-left: 50px;
}

#tweetduck-changelog a {
  color: #247fbb;
}

#tweetduck-changelog code {
  padding: 0 4px;
  font-family: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, Courier, monospace;
  color: #24292e;
  background-color: rgba(27, 31, 35, 0.05);
}
</style>
`).appendTo(document.head);
    }
    
    // changelog
    var log = $("#tweetduck-changelog");
    
    if (!log.length){
      var log = $(`
<div id='tweetduck-changelog'>
  <div id='tweetduck-changelog-box'>
    <h2>TweetDuck Update ${version}</h2>
    ${changelog}
  </div>
</div>
`).appendTo(document.body).css("display", "none");
    }
    
    // notification
    var ele = $("#tweetduck-update");
    var existed = ele.length > 0;
    
    if (existed){
      ele.remove();
    }
    
    ele = $(outdated ? `
<div id='tweetduck-update'>
  <p class='tdu-title'>Unsupported System</p>
  <p class='tdu-info'>You will not receive updates</p>
  <div class='tdu-buttons'>
    <button class='tdu-btn-unsupported'>Read more</button>
    <button class='tdu-btn-ignore'>Dismiss</button>
  </div>
</div>
` : `
<div id='tweetduck-update'>
  <p class='tdu-title'>T&#8202;weetDuck Update ${version}</p>
  <p class='tdu-info tdu-showlog'>View update information</p>
  <div class='tdu-buttons'>
    <button class='tdu-btn-download'>Update now</button>
    <button class='tdu-btn-later'>Remind me later</button>
    <button class='tdu-btn-ignore'>Ignore this update</button>
  </div>
</div>
`).appendTo(document.body).css("display", existed ? "block" : "none");
    
    // ui logic
    var hide = function(){
      ele.remove();
      log.remove();
      css.remove();
    };
    
    var slide = function(){
      log.hide();
      ele.slideUp(hide);
    };
    
    ele.children(".tdu-showlog").click(function(){
      log.toggle();
    });
    
    log.click(function(){
      log.hide();
    }).children().first().click(function(e){
      e.stopPropagation();
    });
    
    var buttonDiv = ele.children(".tdu-buttons").first();

    buttonDiv.children(".tdu-btn-download").click(function(){
      hide();
      
      if (download){
        $TDU.onUpdateAccepted();
      }
      else{
        $TDU.openBrowser(updateDownloadFallback);
      }
    });
    
    buttonDiv.children(".tdu-btn-later").click(function(){
      clearTimeout(updateCheckTimeoutID);
      slide();
    });

    buttonDiv.children(".tdu-btn-unsupported").click(function(){
      $TDU.openBrowser("https://github.com/chylex/TweetDuck/wiki/Supported-Systems");
    });

    buttonDiv.children(".tdu-btn-ignore,.tdu-btn-unsupported").click(function(){
      $TDU.onUpdateDismissed();
      slide();
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
  // Function: Ghetto-converts markdown to HTML.
  //
  var markdown = function(md){
    return md.replace(/&/g, "&amp;")
             .replace(/</g, "&lt;")
             .replace(/>/g, "&gt;")
             .replace(/^### (.*?)$/gm, "<h3>$1</h3>")
             .replace(/^- (.*?)$/gm, "<p>$1</p>")
             .replace(/^  - (.*?)$/gm, "<p class='l2'>$1</p>")
             .replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>")
             .replace(/\*(.*?)\*/g, "<em>$1</em>")
             .replace(/`(.*?)`/g, "<code>$1</code>")
             .replace(/\[(.*?)\]\((.*?)\)/g, "<a href='$2'>$1</a>")
             .replace(/\n\r?\n/g, "<br>");
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
        displayNotification(tagName, obj.browser_download_url, markdown(release.body));
        
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
