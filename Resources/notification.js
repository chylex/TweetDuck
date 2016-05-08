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
  document.body.addEventListener("contextmenu",function(e){
    bubbleParents(e.target,"A",function(ele){
      $TD.setLastRightClickedLink(ele.getAttribute("data-full-url") || "");
    });
  });
})($TD);