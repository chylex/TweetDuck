(function(){
  const isLogin = location.pathname === "/login";
  const isLogout = location.pathname === "/logout";
  const isMobile = location.host === "mobile.twitter.com";
  
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
    
    if (isLogin){
      document.documentElement.setAttribute("login", "");
    }
    else if (isLogout){
      document.documentElement.setAttribute("logout", "");
    }
    
    if (isMobile){
      document.documentElement.setAttribute("mobile", "");
    }
    else{
      document.documentElement.setAttribute("desktop", "");
    }
  };
  
  setTimeout(injectCSS, 1);
  
  //
  // Function: Trigger once element exists.
  //
  const triggerWhenExists = function(query, callback){
    let id = window.setInterval(function(){
      let ele = document.querySelector(query);
      
      if (ele && callback(ele)){
        window.clearInterval(id);
      }
    }, 5);
  };
  
  //
  // Block: Add profile import button & enable custom styling, make page links external on old login page.
  //
  if (isLogin){
    document.addEventListener("DOMContentLoaded", function(){
      if (isMobile){
        triggerWhenExists("main h1", function(heading){
          heading.parentNode.setAttribute("tweetduck-login-wrapper", "");
          return true;
        });
        
        triggerWhenExists("a[href='/i/flow/signup']", function(texts){
          texts = texts.parentNode;
          
          let link = texts.childNodes[0];
          let separator = texts.childNodes[1];
          
          if (link && separator){
            texts.classList.add("tweetduck-login-links");
            
            link = link.cloneNode(false);
            link.id = "tweetduck-helper";
            link.href = "#";
            link.innerText = "Import TweetDuck profile";
            
            texts.appendChild(separator.cloneNode(true));
            texts.appendChild(link);
            
            link.addEventListener("click", function(){
              $TD.openProfileImport();
            });
            
            return true;
          }
          else{
            return false;
          }
        });
      }
      else{
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
          texts.insertAdjacentHTML("beforeend", `<p id="tweetduck-helper">Used the TweetDuck app before? <a href="#">Import your profile »</a></p>`);
          
          texts.querySelector("#tweetduck-helper > a").addEventListener("click", function(){
            $TD.openProfileImport();
          });
        }
      }
    });
  }
  
  //
  // Block: Hide cookie crap.
  //
  if (isMobile){
    document.addEventListener("DOMContentLoaded", function(){
      triggerWhenExists("a[href^='https://help.twitter.com/rules-and-policies/twitter-cookies']", function(cookie){
        while(!!cookie){
          if (cookie.offsetHeight > 30){
            cookie.remove();
            return true;
          }
          else{
            cookie = cookie.parentNode;
          }
        }
        
        return false;
      });
    });
  }
})();
