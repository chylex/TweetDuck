(function($TD){
  //
  // Block: Hook into links to bypass default open function.
  //
  document.body.addEventListener("click",function(e){
    if (e.target.tagName == "A"){
      $TD.openBrowser(e.target.getAttribute("href"));
      e.preventDefault();
    }
  });
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  document.body.addEventListener("contextmenu",function(e){
    var element = e.target;
    
    do{
      if (element.tagName == "A"){
        $TD.setLastRightClickedLink(element.getAttribute("data-full-url") || "");
        break;
      }
    }while((element = element.parentElement) != null);
  });
})($TD);