(function($TD){
  //
  // Function: Bubbles up the parents until it hits an element with the specified tag (includes the first element), and returns true if the search was successful.
  //
  var bubbleParents = function(element, tag, callback){
    do{
      if (element.tagName === "A"){
        callback(element);
        return true;
      }
    }while((element = element.parentElement) != null);
    
    return false;
  };
  
  //
  // Function: Adds an event listener which calls listener(event, element) when an event was triggered by an element of the specified type or one of its children.
  //
  EventTarget.prototype.addBubbledEventListener = function(element, type, listener){
    this.addEventListener(type,function(e){
      bubbleParents(e.target,element.toUpperCase(),function(ele){
        listener(e,ele);
      });
    });
  };
  
  //
  // Block: Hook into links to bypass default open function.
  //
  document.body.addEventListener("click",function(e){
    if (bubbleParents(e.target,"A",function(ele){
      $TD.openBrowser(ele.getAttribute("href"));
    })){
      e.preventDefault();
    }
  });
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  document.body.addBubbledEventListener("a","contextmenu",function(e, ele){
    $TD.setLastRightClickedLink(ele.getAttribute("data-full-url") || "");
  });
})($TD);