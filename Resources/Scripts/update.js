(function($TDU){
  //
  // Function: Creates the update notification element. Removes the old one if already exists.
  //
  var displayNotification = function(version, changelog){
    
    // styles
    let css = document.getElementById("tweetduck-update-css");
    
    if (!css){
      css = document.createElement("style");
      css.id = "tweetduck-update-css";
      css.innerText = `
#tweetduck-update {
  position: fixed;
  bottom: 0;
  width: 200px;
  height: 178px;
  z-index: 9999;
  color: #fff;
  background-color: rgb(32, 94, 138);
  text-align: center;
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.5);
  transition: transform 400ms cubic-bezier(.02, .01, .47, 1);
}

#tweetduck-update.hidden-below {
  transform: translateY(178px);
}

#tweetduck-update .tdu-title {
  font-size: 15px;
  font-weight: bold;
  margin: 8px 0 2px;
  cursor: default;
}

#tweetduck-update .tdu-info {
  display: inline-block;
  font-size: 14px;
  margin: 3px 0;
}

#tweetduck-update .tdu-showlog {
  text-decoration: underline;
  cursor: pointer;
}

#tweetduck-update .tdu-buttons button {
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

#tweetduck-update .tdu-buttons button:hover {
  text-shadow: 1px 1px 1px rgba(0, 0, 0, 0.75);
  box-shadow: 1px 1px 1px rgba(17, 17, 17, 0.75), 0 -2px 0 rgba(17, 17, 17, 0.33) inset !important;
}

#tweetduck-update .tdu-buttons button.tdu-btn-ignore, .tdu-buttons button.tdu-btn-later {
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

#tweetduck-changelog h2 + br {
  display: none;
}

#tweetduck-changelog h3 {
  margin: 0 0 5px 7px;
  font-size: 18px;
}

#tweetduck-changelog p {
  margin: 8px 8px 0 6px;
}

#tweetduck-changelog p.li {
  margin: 0 8px 2px 30px;
  display: list-item;
}

#tweetduck-changelog p.l2 {
  margin-left: 50px !important;
}

#tweetduck-changelog a {
  color: #247fbb;
}

#tweetduck-changelog code {
  padding: 0 4px;
  font-family: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, Courier, monospace;
  color: #24292e;
  background-color: rgba(27, 31, 35, 0.05);
}`;
      
      document.head.appendChild(css);
    }
    
    // changelog
    let log = document.getElementById("tweetduck-changelog");
    
    if (!log){
      log = document.createElement("div");
      log.id = "tweetduck-changelog";
      log.style.display = "none";
      log.innerHTML = `
<div id='tweetduck-changelog-box'>
  <h2>TweetDuck Update ${version}</h2>
  ${markdown(atob(changelog))}
</div>`;
      
      document.body.appendChild(log);
    }
    
    // notification
    let ele = document.getElementById("tweetduck-update");
    let existed = !!ele;
    
    if (existed){
      ele.remove();
    }
    
    ele = document.createElement("div");
    ele.id = "tweetduck-update";
    ele.innerHTML = `
<p class='tdu-title'>T&#8202;weetDuck Update ${version}</p>
<p class='tdu-info tdu-showlog'>View update information</p>
<div class='tdu-buttons'>
  <button class='tdu-btn-download'>Update now</button>
  <button class='tdu-btn-later'>Remind me later</button>
  <button class='tdu-btn-ignore'>Ignore this update</button>
</div>`;
    
    if (!existed){
      ele.classList.add("hidden-below");
    }
    
    document.body.appendChild(ele);
    
    // ui functions
    const exitNow = function(){
      ele.remove();
      log.remove();
      css.remove();
    };
    
    const exitSlide = function(){
      log.style.display = "none";
      
      ele.classList.add("hidden-below");
      setTimeout(exitNow, 400);
    };
    
    const onClick = function(element, callback){
      element.addEventListener("click", callback);
    };
    
    // ui listeners
    onClick(ele.querySelector(".tdu-showlog"), function(){
      log.style.display = window.getComputedStyle(log).display === "none" ? "block" : "none";
    });
    
    onClick(log, function(){
      log.style.display = "none";
    });
    
    onClick(log.children[0], function(e){
      e.stopPropagation();
    });
    
    onClick(ele.querySelector(".tdu-btn-download"), function(){
      exitNow();
      $TDU.onUpdateAccepted();
    });
    
    onClick(ele.querySelector(".tdu-btn-later"), function(){
      $TDU.onUpdateDelayed();
      exitSlide();
    });
    
    onClick(ele.querySelector(".tdu-btn-ignore"), function(){
      $TDU.onUpdateDismissed();
      exitSlide();
    });
    
    // finalize notification
    if (!existed){
      ele.getBoundingClientRect(); // reflow
      ele.classList.remove("hidden-below");
    }
    
    return ele;
  };
  
  //
  // Function: Ghetto-converts markdown to HTML.
  //
  const markdown = function(md){
    return md.replace(/&/g, "&amp;")
             .replace(/</g, "&lt;")
             .replace(/>/g, "&gt;")
             .replace(/^##? (.*?)$/gm, "<h2>$1</h2>")
             .replace(/^### (.*?)$/gm, "<h3>$1</h3>")
             .replace(/^- (.*?)$/gm, "<p class='li'>$1</p>")
             .replace(/^  - (.*?)$/gm, "<p class='li l2'>$1</p>")
             .replace(/\*\*(.*?)\*\*/g, "<strong>$1</strong>")
             .replace(/\*(.*?)\*/g, "<em>$1</em>")
             .replace(/`(.*?)`/g, "<code>$1</code>")
             .replace(/\[(.*?)\]\((.*?)\)/g, "<a href='$2' target='_blank'>$1</a>")
             .replace(/^([a-z0-9].*?)$/gmi, "<p>$1</p>")
             .replace(/\n\r?\n\r?/g, "<br>");
  };
  
  //
  // Block: Check updates on startup.
  //
  if ("$" in window && typeof $._data === "function" && "TD" in $._data(document, "events")){
    $(document).one("TD.ready", function(){
      $TDU.triggerUpdateCheck();
    });
  }
  else{
    $TDU.triggerUpdateCheck();
  }
  
  //
  // Block: Setup global functions.
  //
  window.TDUF_displayNotification = displayNotification;
})($TDU);
