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
  // Block: Hook into links to bypass default open function.
  //
  addEventListener(links,"click",function(e){
    $TD.openBrowser(e.currentTarget.getAttribute("href"));
    e.preventDefault();
  });
  
  //
  // Block: Allow bypassing of t.co in context menus.
  //
  addEventListener(links,"contextmenu",function(e){
    $TD.setLastRightClickedLink(e.currentTarget.getAttribute("data-full-url") || "");
  });
})($TD);