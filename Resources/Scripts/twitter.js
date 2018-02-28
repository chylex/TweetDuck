(function(){
  //
  // Function: Inject custom CSS into the page.
  //
  var injectCSS = function(){
    if (!document.head){
      setTimeout(injectCSS, 5);
      return;
    }
    
    var style = document.createElement("style");
    document.head.appendChild(style);
    
    let addRule = (rule) => {
      style.sheet.insertRule(rule, 0);
    };
    
    addRule("body { overflow: hidden !important; }"); // remove scrollbar
    addRule(".page-canvas { box-shadow: 0 0 150px rgba(255, 255, 255, 0.3) !important; }"); // change page box shadow
    addRule(".topbar { display: none !important; }"); // hide top bar
    
    addRule(".page-canvas, .buttons, .btn, input { border-radius: 0 !important; }"); // sharpen borders
    addRule("input { padding: 5px 8px 4px !important; }"); // tweak input padding
    addRule("button[type='submit'] { border: 1px solid rgba(0, 0, 0, 0.3) !important; border-radius: 0 !important; }"); // style buttons
    
    addRule("#doc { width: 100%; height: 100%; margin: 0; position: absolute; display: table; }"); // center everything
    addRule("#page-outer { display: table-cell; vertical-align: middle; }"); // center everything
    addRule("#page-container { padding: 0 20px !important; width: 100% !important; box-sizing: border-box !important; }"); // center everything
    addRule(".page-canvas { margin: 0 auto !important; }"); // center everything
    
    if (location.pathname === "/logout"){
      addRule(".page-canvas { width: auto !important; max-width: 888px; }"); // fix min width
      addRule(".signout-wrapper { width: auto !important; margin: 0 auto !important; }"); // fix min width and margins
      addRule(".signout { margin: 60px 0 54px !important; }"); // fix dialog margins
      addRule(".buttons { padding-bottom: 0 !important; }"); // fix dialog margins
      addRule(".aside { display: none; }"); // hide elements around logout dialog
      addRule(".buttons button, .buttons a { display: inline-block; margin: 0 4px !important; border: 1px solid rgba(0, 0, 0, 0.3) !important; border-radius: 0 !important; }"); // style buttons
    }
  };
  
  setTimeout(injectCSS, 1);
  
  //
  // Block: Make login page links external.
  //
  if (location.pathname === "/login"){
    document.addEventListener("DOMContentLoaded", function(){
      let openLinkExternally = function(e){
        let href = e.currentTarget.getAttribute("href");
        $TD.openBrowser(href[0] === '/' ? location.origin+href : href);

        e.preventDefault();
        e.stopPropagation();
      };

      let links = document.getElementsByTagName("A");

      for(let index = 0; index < links.length; index++){
        links[index].addEventListener("click", openLinkExternally);
      }
    });
  }
})();
