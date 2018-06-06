(function($TD, $TDX){
  //
  // Variable: Collection of all <a> tags.
  //
  var links = document.getElementsByTagName("A");
  
  //
  // Function: Adds an event listener to all elements in the array or collection.
  //
  var addEventListener = function(collection, type, listener){
    for(let index = 0; index < collection.length; index++){
      collection[index].addEventListener(type, listener);
    }
  };
  
  //
  // Block: Hook into links to bypass default open function, and handle skipping notification when opening links.
  //
  (function(){
    const onLinkClick = function(e){
      if (e.button === 0 || e.button === 1){
        let ele = e.currentTarget;

        $TD.openBrowser(ele.href);
        e.preventDefault();

        if ($TDX.skipOnLinkClick){
          let parentClasses = ele.parentNode.classList;

          if (parentClasses.contains("js-tweet-text") || parentClasses.contains("js-quoted-tweet-text") || parentClasses.contains("js-timestamp")){
            $TD.loadNextNotification();
          }
        }
      }
    };
    
    addEventListener(links, "click", onLinkClick);
    addEventListener(links, "auxclick", onLinkClick);
  })();
  
  //
  // Block: Expand shortened links on hover or display tooltip.
  //
  (function(){
    var prevMouseX = -1, prevMouseY = -1;
    var tooltipTimer, tooltipDisplayed;
    
    addEventListener(links, "mouseenter", function(e){
      var me = e.currentTarget;
      
      var url = me.getAttribute("data-full-url");
      return if !url;
      
      var text = me.textContent;
      return if text.charCodeAt(text.length-1) !== 8230 && text.charCodeAt(0) !== 8230; // horizontal ellipsis

      if ($TDX.expandLinksOnHover){
        tooltipTimer = window.setTimeout(function(){
          me.setAttribute("td-prev-text", text);
          me.innerHTML = url.replace(/^https?:\/\/(www\.)?/, "");
        }, 200);
      }
      else{
        tooltipTimer = window.setTimeout(function(){
          $TD.displayTooltip(url);
          tooltipDisplayed = true;
        }, 400);
      }
    });
    
    addEventListener(links, "mouseleave", function(e){
      return if !e.currentTarget.hasAttribute("data-full-url");
      
      if ($TDX.expandLinksOnHover){
        var prevText = e.currentTarget.getAttribute("td-prev-text");

        if (prevText){
          e.currentTarget.innerHTML = prevText;
        }
      }
      
      window.clearTimeout(tooltipTimer);
      
      if (tooltipDisplayed){
        tooltipDisplayed = false;
        $TD.displayTooltip(null);
      }
    });
    
    addEventListener(links, "mousemove", function(e){
      if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)){
        var url = e.currentTarget.getAttribute("data-full-url");
        return if !url;
        
        $TD.displayTooltip(url);
        prevMouseX = e.clientX;
        prevMouseY = e.clientY;
      }
    });
  })();
  
  //
  // Block: Work around clipboard HTML formatting.
  //
  document.addEventListener("copy", function(e){
    window.setTimeout($TD.fixClipboard, 0);
  });
  
  //
  // Block: Setup a handler for 'Show this thread'.
  //
  (function(){
    var btn = document.getElementById("tduck-show-thread");
    return if !btn;
    
    btn.addEventListener("click", function(){
      $TD.showTweetDetail();
    });
  })();
  
  //
  // Block: Setup a skip button.
  //
  if (!document.body.hasAttribute("td-example-notification")){
    document.body.children[0].insertAdjacentHTML("beforeend", `
<svg id="td-skip" width="10" height="17" viewBox="0 0 350 600">
  <path fill="#888" d="M0,151.656l102.208-102.22l247.777,247.775L102.208,544.986L0,442.758l145.546-145.547">
</svg>`);
    
    document.getElementById("td-skip").addEventListener("click", function(){
      $TD.loadNextNotification();
    });
  }
  
  //
  // Block: Setup a hover class on body.
  //
  document.body.addEventListener("mouseenter", function(){
    document.body.classList.add("td-hover");
  });

  document.body.addEventListener("mouseleave", function(){
    document.body.classList.remove("td-hover");
  });
  
  //
  // Block: Force a reset of scroll position on every load.
  //
  history.scrollRestoration = "manual";
})($TD, $TDX);
