(function($TD){
  //
  // Variable: Collection of all <a> tags.
  //
  var links = document.getElementsByTagName("A");
  
  //
  // Function: Adds an event listener to all elements in the array or collection.
  //
  var addEventListener = function(collection, type, listener){
    for(let index = 0; index < collection.length; index++){
      collection[index].addEventListener(type,listener);
    }
  };
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  addEventListener(links,"contextmenu",function(e){
    $TD.setLastRightClickedLink(e.currentTarget.getAttribute("data-full-url") || "");
  });
  
  //
  // Block: Expand shortened links on hover or display tooltip.
  //
  (function(){
    var cutStart = function(str, search){
      return str.startsWith(search) ? str.substr(search.length) : str;
    };
    
    var prevMouseX = -1, prevMouseY = -1;
    var tooltipTimer, tooltipDisplayed;
    
    addEventListener(links,"mouseenter",function(e){
      var me = e.currentTarget;
      
      var url = me.getAttribute("data-full-url");
      if (!url)return;
      
      var text = me.textContent;
        
      if (text.charCodeAt(text.length-1) !== 8230){ // horizontal ellipsis
        return;
      }

      if ($TD.expandLinksOnHover){
        tooltipTimer = window.setTimeout(function(){
          var expanded = url;
          expanded = cutStart(expanded,"https://");
          expanded = cutStart(expanded,"http://");
          expanded = cutStart(expanded,"www.");

          me.setAttribute("td-prev-text",text);
          me.innerHTML = expanded;
        },200);
      }
      else{
        tooltipTimer = window.setTimeout(function(){
          $TD.displayTooltip(url,true);
          tooltipDisplayed = true;
        },400);
      }
    });
    
    addEventListener(links,"mouseleave",function(e){
      if (!e.currentTarget.hasAttribute("data-full-url"))return;
      
      if ($TD.expandLinksOnHover){
        var prevText = e.currentTarget.getAttribute("td-prev-text");

        if (prevText){
          e.currentTarget.innerHTML = prevText;
        }
      }
      
      window.clearTimeout(tooltipTimer);
      
      if (tooltipDisplayed){
        tooltipDisplayed = false;
        $TD.displayTooltip(null,true);
      }
    });
    
    addEventListener(links,"mousemove",function(e){
      if (tooltipDisplayed && (prevMouseX !== e.clientX || prevMouseY !== e.clientY)){
        var url = e.currentTarget.getAttribute("data-full-url");
        if (!url)return;
        
        $TD.displayTooltip(url,true);
        prevMouseX = e.clientX;
        prevMouseY = e.clientY;
      }
    });
  })();
  
  //
  // Block: Setup embedded tweet address for context menu
  //
  (function(){
    var embedded = document.getElementsByClassName("quoted-tweet");
    if (embedded.length === 0)return;
    
    var tweetId = embedded[0].getAttribute("data-tweet-id");
    if (!tweetId)return;
    
    var account = embedded[0].getElementsByClassName("account-link");
    if (account.length === 0)return;
    
    $TD.setNotificationTweetEmbedded(account[0].getAttribute("href")+"/status/"+tweetId);
  })();
})($TD);