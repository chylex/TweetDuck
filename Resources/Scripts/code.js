(function($, TD){
  if ($ === null){
    console.error("Missing jQuery");
  }
  
  if (!("$TD" in window)){
    console.error("Missing $TD");
  }
  
  if (!("$TDX" in window)){
    console.error("Missing $TDX");
  }
  
  //
  // Variable: Array of functions called after the website app is loaded.
  //
  let onAppReady = [];
  
  //
  // Variable: DOM object containing the main app element.
  //
  const app = typeof $ === "function" && $(document.body).children(".js-app");
  
  //
  // Function: Prepends code at the beginning of a function. If the prepended function returns true, execution of the original function is cancelled.
  //
  const prependToFunction = function(func, extension){
    return function(){
      return extension.apply(this, arguments) === true ? undefined : func.apply(this, arguments);
    };
  };
  
  //
  // Function: Appends code at the end of a function.
  //
  const appendToFunction = function(func, extension){
    return function(){
      const res = func.apply(this, arguments);
      extension.apply(this, arguments);
      return res;
    };
  };
  
  //
  // Function: Triggers an internal debug crash when something is missing.
  //
  const crashDebug = function(message){
    console.error(message);
    debugger;
    
    if ("$TD" in window){
      $TD.crashDebug(message);
    }
  }
  
  //
  // Function: Throws if an object does not have a specified property.
  //
  const ensurePropertyExists = function(obj, ...chain){
    for(let index = 0; index < chain.length; index++){
      if (!obj.hasOwnProperty(chain[index])){
        throw "Missing property " + chain[index] + " in chain [obj]." + chain.join(".");
      }
      
      obj = obj[chain[index]];
    }
  };
  
  //
  // Function: Returns true if an object has a specified property, otherwise returns false with a debug-only error message.
  //
  const checkPropertyExists = function(obj, ...chain){
    try{
      ensurePropertyExists(obj, ...chain);
      return true;
    }catch(err){
      crashDebug(err);
      return false;
    }
  };
  
  //
  // Function: Throws if an element does not have a registered jQuery event.
  //
  const ensureEventExists = function(element, eventName){
    if (!(eventName in $._data(element, "events"))){
      throw "Missing jQuery event " + eventName + " in " + element.cloneNode().outerHTML;
    }
  };
  
  //
  // Function: Returns a jQuery object but also shows a debug-only error message if no elements are found.
  //
  const $$ = function(selector, context){
    const result = $(selector, context);
    
    if (!result.length){
      crashDebug("No elements were found for selector " + selector);
    }
    
    return result;
  };
  
  //
  // Function: Executes a function inside a try-catch to stop it from crashing everything.
  //
  const execSafe = function(func, fail){
    try{
      func();
    }catch(err){
      crashDebug("Caught error in function " + func.name)
      fail && fail();
    }
  };
  
  //
  // Function: Returns an object containing data about the column below the cursor.
  //
  const getHoveredColumn = function(){
    const hovered = document.querySelectorAll(":hover");
    
    for(let index = hovered.length - 1; index >= 0; index--){
      const ele = hovered[index];
      
      if (ele.tagName === "SECTION" && ele.classList.contains("js-column")){
        const obj = TD.controller.columnManager.get(ele.getAttribute("data-column"));
        
        if (obj){
          return { ele, obj };
        }
      }
    }
    
    return null;
  };
  
  //
  // Function: Returns an object containing data about the tweet below the cursor.
  //
  const getHoveredTweet = function(){
    const hovered = document.querySelectorAll(":hover");
    
    for(let index = hovered.length - 1; index >= 0; index--){
      const ele = hovered[index];
      
      if (ele.tagName === "ARTICLE" && ele.classList.contains("js-stream-item") && ele.hasAttribute("data-account-key")){
        const column = getHoveredColumn();
        
        if (column){
          const wrap = column.obj.findChirp(ele.getAttribute("data-key"));
          const obj = column.obj.findChirp(ele.getAttribute("data-tweet-id")) || wrap;
          
          if (obj){
            return { ele, obj, wrap, column };
          }
        }
      }
    }
    
    return null;
  };
  
  //
  // Function: Retrieves a property of an element with a specified class.
  //
  const getClassStyleProperty = function(cls, property){
    const column = document.createElement("div");
    column.classList.add(cls);
    column.style.display = "none";
    
    document.body.appendChild(column);
    const value = window.getComputedStyle(column).getPropertyValue(property);
    document.body.removeChild(column);
    
    return value;
  };
  
  //
  // Block: Fix columns missing any identifiable attributes to allow individual styles.
  //
  execSafe(function setupColumnAttrIdentifiers(){
    $(document).on("uiColumnRendered", function(e, data){
      const icon = data.$column.find(".column-type-icon").first();
      return if icon.length !== 1;
      
      const name = Array.prototype.find.call(icon[0].classList, cls => cls.startsWith("icon-"));
      return if !name;
      
      data.$column.attr("data-td-icon", name);
      data.column._tduck_icon = name;
    });
  });
  
  //
  // Block: Add TweetDuck buttons to the settings menu.
  //
  onAppReady.push(function setupSettingsDropdown(){
    document.querySelector("[data-action='settings-menu']").addEventListener("click", function(){
      setTimeout(function(){
        const menu = document.querySelector(".js-dropdown-content ul");
        return if !menu;
        
        const dividers = menu.querySelectorAll(":scope > li.drp-h-divider");
        const target = dividers[dividers.length - 1];
        
        target.insertAdjacentHTML("beforebegin", '<li class="is-selectable" data-tweetduck><a href="#" data-action>TweetDuck</a></li>');
        
        const button = menu.querySelector("[data-tweetduck]");
        
        button.querySelector("a").addEventListener("click", function(){
          $TD.openContextMenu();
        });
        
        button.addEventListener("mouseenter", function(){
          button.classList.add("is-selected");
        });
        
        button.addEventListener("mouseleave", function(){
          button.classList.remove("is-selected");
        })
      }, 0);
    });
  });
  
  //
  // Block: Hook into settings object to detect when the settings change, and update html attributes and notification layout.
  //
  execSafe(function hookTweetDeckSettings(){
    ensurePropertyExists(TD, "settings", "getFontSize");
    ensurePropertyExists(TD, "settings", "setFontSize");
    ensurePropertyExists(TD, "settings", "getTheme");
    ensurePropertyExists(TD, "settings", "setTheme");
    
    const doc = document.documentElement;
    
    const refreshSettings = function(){
      const fontSizeName = TD.settings.getFontSize();
      const themeName = TD.settings.getTheme();
      
      const tags = [
        "<html " + Array.prototype.map.call(doc.attributes, ele => `${ele.name}="${ele.value}"`).join(" ") + "><head>"
      ];
      
      for(let ele of document.head.querySelectorAll("link[rel='stylesheet']:not([data-td-exclude-notification]),meta[charset]")){
        tags.push(ele.outerHTML);
      }
      
      tags.push("<style type='text/css'>body { background: " + getClassStyleProperty("column-panel", "background-color") + " !important }</style>");
      
      doc.setAttribute("data-td-font", fontSizeName);
      doc.setAttribute("data-td-theme", themeName);
      $TD.loadNotificationLayout(fontSizeName, tags.join(""));
    };
    
    TD.settings.setFontSize = appendToFunction(TD.settings.setFontSize, function(name){
      setTimeout(refreshSettings, 0);
    });
    
    TD.settings.setTheme = appendToFunction(TD.settings.setTheme, function(name){
      setTimeout(refreshSettings, 0);
    });
    
    onAppReady.push(refreshSettings);
  });
  
  //
  // Block: Setup CSS injections.
  //
  execSafe(function setupStyleInjection(){
    const createStyle = function(id, styles){
      const ele = document.createElement("style");
      ele.id = id;
      ele.innerText = styles;
      document.head.appendChild(ele);
    };
    
    window.TDGF_injectBrowserCSS = function(styles){
      if (!document.getElementById("tweetduck-browser-css")){
        createStyle("tweetduck-browser-css", styles);
      }
    };
    
    window.TDGF_reinjectCustomCSS = function(styles){
      const prev = document.getElementById("tweetduck-custom-css");
      
      if (prev){
        prev.remove();
      }
      
      if (styles && styles.length){
        createStyle("tweetduck-custom-css", styles);
      }
    };
  });
  
  //
  // Block: Setup custom sound notification.
  //
  window.TDGF_setSoundNotificationData = function(custom, volume){
    const audio = document.getElementById("update-sound");
    audio.volume = volume / 100;
    
    const sourceId = "tduck-custom-sound-source";
    let source = document.getElementById(sourceId);
    
    if (custom && !source){
      source = document.createElement("source");
      source.id = sourceId;
      source.src = "https://ton.twimg.com/tduck/updatesnd";
      audio.prepend(source);
    }
    else if (!custom && source){
      audio.removeChild(source);
    }
    
    audio.load();
  };
  
  //
  // Block: Hook into composer event.
  //
  execSafe(function hookComposerEvents(){
    $(document).on("uiDrawerActive uiRwebComposerOptOut", function(e, data){
      return if e.type === "uiDrawerActive" && data.activeDrawer !== "compose";
      
      setTimeout(function(){
        $(document).trigger("tduckOldComposerActive");
      }, 0);
    });
  });
  
  //
  // Block: Setup a top tier account bamboozle scheme.
  //
  execSafe(function setupAccountLoadHook(){
    const realDisplayName = "TweetDuck";
    const realAvatar = "https://ton.twimg.com/tduck/avatar";
    const accountId = "957608948189880320";
    
    if (checkPropertyExists(TD, "services", "TwitterUser", "prototype", "fromJSONObject")){
      const prevFunc = TD.services.TwitterUser.prototype.fromJSONObject;
      
      TD.services.TwitterUser.prototype.fromJSONObject = function(){
        const obj = prevFunc.apply(this, arguments);
        
        if (obj.id === accountId){
          obj.name = realDisplayName;
          obj.emojifiedName = realDisplayName;
          obj.profileImageURL = realAvatar;
          obj.url = "https://tweetduck.chylex.com";
          
          if (obj.entities && obj.entities.url){
            obj.entities.url.urls = [{
              url: obj.url,
              expanded_url: obj.url,
              display_url: "tweetduck.chylex.com",
              indices: [ 0, 23 ]
            }];
          }
        }
        
        return obj;
      };
    }
    
    if (checkPropertyExists(TD, "services", "TwitterClient", "prototype", "typeaheadSearch")){
      const prevFunc = TD.services.TwitterClient.prototype.typeaheadSearch;
      
      TD.services.TwitterClient.prototype.typeaheadSearch = function(data, onSuccess, onError){
        if (data.query && data.query.toLowerCase().endsWith("tweetduck")){
          data.query = "TryMyAwesomeApp";
        }
        
        return prevFunc.call(this, data, function(result){
          for(let user of result.users){
            if (user.id_str === accountId){
              user.name = realDisplayName;
              user.profile_image_url = realAvatar;
              user.profile_image_url_https = realAvatar;
              break;
            }
          }
          
          onSuccess.apply(this, arguments);
        }, onError);
      };
    }
  });
  
  //
  // Block: Work around clipboard HTML formatting.
  //
  document.addEventListener("copy", function(){
    window.setTimeout($TD.fixClipboard, 0);
  });
  
  //
  // Block: Fix OS name and add ID to the document for priority CSS selectors.
  //
  (function(){
    const doc = document.documentElement;
    
    if (checkPropertyExists(TD, "util", "getOSName")){
      TD.util.getOSName = function(){
        return "windows";
      };
      
      doc.classList.remove("os-");
      doc.classList.add("os-windows");
    }
    
    doc.id = "tduck";
  })();
  
  //
  // Block: Disable TweetDeck metrics.
  //
  if (checkPropertyExists(TD, "metrics")){
    const noop = function(){};
    TD.metrics.inflate = noop;
    TD.metrics.inflateMetricTriple = noop;
    TD.metrics.log = noop;
    TD.metrics.makeKey = noop;
    TD.metrics.send = noop;
  }
  
  onAppReady.push(function disableMetrics(){
    const data = $._data(window);
    delete data.events["metric"];
    delete data.events["metricsFlush"];
  });
  
  //
  // Block: Import scripts.
  //
  #import "scripts/browser.globals.js"
  #import "scripts/browser.features.js"
  #import "scripts/browser.tweaks.js"
  
  //
  // Block: Register the TD.ready event, finish initialization, and load plugins.
  //
  $(document).one("TD.ready", function(){
    onAppReady.forEach(func => execSafe(func));
    onAppReady = null;
    
    if (window.TD_PLUGINS){
      window.TD_PLUGINS.onReady();
    }
  });

  //
  // Block: Ensure window.jQuery is available.
  //
  window.jQuery = $;
  
  //
  // Block: Skip the initial pre-login page.
  //
  if (checkPropertyExists(TD, "controller", "init", "showLogin")){
    TD.controller.init.showLogin = function(){
      location.href = "https://twitter.com/login?hide_message=true&redirect_after_login=https%3A%2F%2Ftweetdeck.twitter.com%2F%3Fvia_twitter_login%3Dtrue";
    };
  }
})(window.$ || null, window.TD || {});
