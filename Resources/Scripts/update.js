(function(){
  if (!("$TDU" in window)){
    console.error("Missing $TDU");
    return;
  }
  
  //
  // Function: Creates the update notification element. Removes the old one if already exists.
  //
  const displayNotification = function(version, changelog){
    
    // styles
    let css = document.getElementById("tweetduck-update-css");
    
    if (!css){
      css = document.createElement("style");
      css.id = "tweetduck-update-css";
      css.innerText = `#import "styles/update.css"`;
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
    const existed = !!ele;
    
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
  const triggerCheck = function(){
    $TDU.triggerUpdateCheck();
  };
  
  try{
    throw "Missing jQuery or TD.ready event" if !($._data(document, "events").TD.some(obj => obj.namespace === "ready"));
    
    $(document).one("TD.ready", triggerCheck);
  }catch(err){
    console.warn(err);
    setTimeout(triggerCheck, 500);
  }
  
  //
  // Block: Setup global functions.
  //
  window.TDUF_displayNotification = displayNotification;
})();
