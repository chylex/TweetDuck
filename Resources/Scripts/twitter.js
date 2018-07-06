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
    
    style.innerText = `#import "styles/twitter.base.css"`;
    
    if (location.pathname === "/logout"){
      style.innerText += `#import "styles/twitter.logout.css"`;
    }
    
    document.head.appendChild(style);
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
