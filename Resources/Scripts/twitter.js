(function(){
  //
  // Function: Inject custom CSS into the page.
  //
  const injectCSS = function(){
    if (!document.head){
      setTimeout(injectCSS, 5);
      return;
    }
    
    let link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "https://abs.twimg.com/tduck/css";
    
    document.head.appendChild(link);
    
    if (location.pathname === "/logout"){
      document.documentElement.setAttribute("logout", "");
    }
  };
  
  setTimeout(injectCSS, 1);
  
  //
  // Block: Make login page links external.
  //
  if (location.pathname === "/login"){
    document.addEventListener("DOMContentLoaded", function(){
      const openLinkExternally = function(e){
        let href = e.currentTarget.getAttribute("href");
        $TD.openBrowser(href[0] === '/' ? location.origin+href : href);
        
        e.preventDefault();
        e.stopPropagation();
      };
      
      for(let link of document.getElementsByTagName("A")){
        link.addEventListener("click", openLinkExternally);
      }
      
      let texts = document.querySelector(".page-canvas > div:last-child");
      
      if (texts){
        texts.insertAdjacentHTML("beforeend", `<p class="tweetduck-helper">Used the TweetDuck app before? <a href="#">Import your profile »</a></p>`);
        
        texts.querySelector(".tweetduck-helper > a").addEventListener("click", function(){
          $TD.openProfileImport();
        });
      }
    });
  }
  //
  // Block: Fix broken Cancel button on logout page.
  //
  else if (location.pathname === "/logout"){
    document.addEventListener("DOMContentLoaded", function(){
      let cancel = document.querySelector(".buttons .cancel");
      
      if (cancel && cancel.tagName === "A"){
        cancel.href = "https://tweetdeck.twitter.com/";
      }
    });
  }
})();
