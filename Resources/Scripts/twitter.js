(function(){
  const isLogin = location.pathname === "/login";
  const isLogout = location.pathname === "/logout";
  
  //
  // Function: Inject custom CSS into the page.
  //
  const injectCSS = function(){
    if (!document.head){
      setTimeout(injectCSS, 5);
      return;
    }
    
    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = "https://abs.twimg.com/tduck/css";
    
    document.head.appendChild(link);
    
    if (isLogin){
      document.documentElement.setAttribute("login", "");
    }
    else if (isLogout){
      document.documentElement.setAttribute("logout", "");
    }
  };
  
  setTimeout(injectCSS, 1);
  
  //
  // Function: Trigger once element exists.
  //
  const triggerWhenExists = function(query, callback){
    let id = window.setInterval(function(){
      const ele = document.querySelector(query);
      
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
    });
  }
  
  //
  // Block: Hide cookie crap.
  //
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
})();
