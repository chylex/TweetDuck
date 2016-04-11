(function($TD){
  //
  // Block: Hook into links to bypass default open function
  //
  document.body.addEventListener("click",function(e){
    if (e.target.tagName == "A"){
      $TD.openBrowser(e.target.getAttribute("href"));
      e.preventDefault();
    }
  });
})($TD);