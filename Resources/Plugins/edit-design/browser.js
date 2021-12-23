enabled(){
  // elements & data
  this.css = null;
  this.icons = null;
  this.config = null;
  
  this.defaultConfig = {
    _theme: "light",
    themeOverride: false,
    columnWidth: "310px",
    composerWidth: "default",
    fontSize: "12px",
    hideTweetActions: true,
    moveTweetActionsToRight: true,
    themeColorTweaks: true,
    revertIcons: true,
    showCharacterCount: true,
    forceArialFont: true,
    increaseQuoteTextSize: false,
    smallComposeTextSize: false,
    optimizeAnimations: true,
    avatarRadius: 2
  };
  
  const prepareDefaultConfig = () => {
    this.defaultConfig._theme = TD.settings.getTheme();
    
    switch(TD.settings.getColumnWidth()){
      case "wide": this.defaultConfig.columnWidth = "350px"; break;
      case "narrow": this.defaultConfig.columnWidth = "270px"; break;
    }
    
    switch(TD.settings.getFontSize()){
      case "small": this.defaultConfig.fontSize = "13px"; break;
      case "medium": this.defaultConfig.fontSize = "14px"; break;
      case "large": this.defaultConfig.fontSize = "15px"; break;
      case "largest": this.defaultConfig.fontSize = "16px"; break;
    }
  };
  
  this.firstTimeLoad = null;
  
  const me = this;
  
  // configuration
  const configFile = "config.json";
  
  this.tmpConfig = null;
  this.currentStage = 0;
  
  this.onStageReady = () => {
    if (this.currentStage === 0){
      this.currentStage = 1;
    }
    else if (this.tmpConfig !== null){
      let needsResave = !("_theme" in this.tmpConfig);
      
      prepareDefaultConfig();
      this.config = $.extend(this.defaultConfig, this.tmpConfig);
      this.tmpConfig = null;
      this.reinjectAll();
      
      if (this.firstTimeLoad || needsResave){
        $TDP.writeFile(this.$token, configFile, JSON.stringify(this.config));
      }
    }
  };
  
  const loadConfigObject = obj => {
    this.tmpConfig = obj || {};
    this.firstTimeLoad = obj === null;
    
    this.onStageReady();
  };
  
  if (TD.ready){
    this.onStageReady();
  }
  else{
    $(document).one("dataSettingsValues", () => this.onStageReady());
  }
  
  $TDP.checkFileExists(this.$token, configFile).then(exists => {
    if (!exists){
      loadConfigObject(null);
    }
    else{
      $TDP.readFile(this.$token, configFile, true).then(contents => {
        try{
          loadConfigObject(JSON.parse(contents));
        }catch(err){
          loadConfigObject(null);
        }
      }).catch(err => {
        loadConfigObject(null);
        $TD.alert("error", "Problem loading configuration for the design edit plugin: "+err.message);
      });
    }
  });
  
  this.saveConfig = () => {
    $TDP.writeFile(this.$token, configFile, JSON.stringify(this.config)).catch(err => {
      $TD.alert("error", "Problem saving configuration for the design edit plugin: "+err.message);
    });
  };
  
  // settings click event
  this.onSettingsMenuClickedEvent = () => {
    if (this.config === null) {
      return;
    }
    
    setTimeout(() => {
      let menu = $(".js-dropdown-content").children("ul").first();
      if (menu.length === 0) {
        return;
      }
      
      let itemTD = menu.children("[data-tweetduck]").first();
      if (itemTD.length === 0) {
        return;
      }
      
      if (!itemTD.prev().hasClass("drp-h-divider")){
        itemTD.before('<li class="drp-h-divider"></li>');
      }
      
      let itemEditDesign = $('<li class="is-selectable"><a href="#" data-action>Edit layout &amp; design</a></li>');
      itemEditDesign.insertAfter(itemTD);
      
      itemEditDesign.on("click", "a", this.configure.bind(this));
      
      itemEditDesign.hover(function(){
        $(this).addClass("is-selected");
      }, function(){
        $(this).removeClass("is-selected");
      });
    }, 2);
  };
  
  // modal dialog setup
  const updateKey = function(key, value){
    me.config[key] = value;
    
    setTimeout(function(){
      me.saveConfig();
      me.reinjectAll();
    }, 1); // delays the slight lag caused by saving and reinjection
  };
  
  this.customDesignModal = TD.components.BaseModal.extend(function(){
    let modal = $("#td-design-plugin-modal");
    this.setAndShowContainer(modal, false);
    
    // RELOAD
    this.reloadPage = false;
    modal.find("[data-td-reload]").click(() => this.reloadPage = true);
    
    // UI EVENTS
    let getTextForCustom = function(key){
      return "Custom ("+me.config[key]+")";
    };
    
    modal.find("[data-td-key]").each(function(){
      let item = $(this);
      let tag = item.prop("tagName");
      let key = item.attr("data-td-key");
      
      // INPUTS
      if (tag === "INPUT"){
        let type = item.attr("type");
        
        if (type === "checkbox"){
          item.prop("checked", me.config[key]);
          
          item.change(function(){
            updateKey(key, item.prop("checked"));
          });
        }
      }
      // SELECTS
      else if (tag === "SELECT"){
        let optionCustom = item.find("option[value^='custom']");
        let optionCustomNew = item.find("option[value^='change-custom']");
        
        let resetMyValue = () => {
          if (!item.val(me.config[key]).val() && optionCustom.length === 1){
            item.val(optionCustom.attr("value"));
            optionCustom.text(getTextForCustom(key));
            optionCustomNew.show();
          }
          else{
            optionCustom.text("Custom");
            optionCustomNew.hide();
          }
        };
        
        resetMyValue();
        
        item.change(function(){
          let val = item.val();
          
          if (val.endsWith("custom-px")){
            val = (prompt("Enter custom value (px):") || "").trim();
            
            if (val){
              if (val.endsWith("px")){
                val = val.slice(0, -2).trim();
              }
              
              if (/^[0-9]+$/.test(val)){
                updateKey(key, val+"px");
              }
              else{
                alert("Invalid value, only px values are supported.");
              }
            }
          }
          else{
            updateKey(key, item.val());
          }
          
          resetMyValue();
        });
      }
      // CUSTOM ELEMENTS
      else{
        let value = item.attr("data-td-value");
        
        if (value == me.config[key]){
          item.addClass("selected");
        }
        
        item.click(function(){
          modal.find("[data-td-key='"+key+"']").removeClass("selected");
          item.addClass("selected");
          updateKey(key, value);
        });
      }
    });
    
    // THEMES
    let selectedTheme = TD.settings.getTheme();
    
    if (selectedTheme === "dark" && me.config.themeOverride === "black"){
      selectedTheme = me.config.themeOverride;
    }
    
    modal.find("[data-td-theme='"+selectedTheme+"']").prop("checked", true);
    
    modal.find("[data-td-theme]").change(function(){
      let theme = $(this).attr("data-td-theme");
      me.config._theme = theme;
      
      if (theme === "black"){
        me.config.themeOverride = theme;
        theme = "dark";
      }
      else{
        me.config.themeOverride = false;
      }
      
      setTimeout(function(){
        if (theme != TD.settings.getTheme()){
          TD.settings.setTheme(theme);
        }
        
        me.saveConfig();
        me.reinjectAll();
      }, 1);
    });
  }).methods({
    _render: function(){
      return $(me.htmlModal);
    },
    destroy: function(){
      if (this.reloadPage){
        window.TDPF_requestReload();
        return;
      }
      
      delete me.htmlModal;
      
      $("#td-design-plugin-modal").hide();
      this.supr();
    }
  });
  
  // animation optimization
  this.optimizations = null;
  this.optimizationTimer = null;
  
  let clearOptimizationTimer = () => {
    if (this.optimizationTimer){
      window.clearTimeout(this.optimizationTimer);
      this.optimizationTimer = null;
    }
  };
  
  let runOptimizationTimer = timeout => {
    if (!this.optimizationTimer){
      this.optimizationTimer = window.setTimeout(optimizationTimerFunc, timeout);
    }
  };
  
  let optimizationTimerFunc = () => {
    this.optimizationTimer = null;
    
    if (this.config.optimizeAnimations){
      $TD.getIdleSeconds().then(s => {
        if (s >= 16){
          disableOptimizations();
          runOptimizationTimer(2500);
        }
        else{
          injectOptimizations();
        }
      });
    }
  };
  
  let injectOptimizations = force => {
    if (!this.optimizations && (force || document.hasFocus())){
      this.optimizations = window.TDPF_createCustomStyle(this);
      this.optimizations.insert(".app-content { will-change: transform }");
      this.optimizations.insert(".column-holder { will-change: transform }");
    }
    
    clearOptimizationTimer();
    runOptimizationTimer(10000);
  };
  
  let disableOptimizations = () => {
    if (this.optimizations){
      this.optimizations.remove();
      this.optimizations = null;
    }
  };
  
  this.onWindowFocusEvent = () => {
    if (this.config && this.config.optimizeAnimations){
      injectOptimizations(true);
    }
  };
  
  this.onWindowBlurEvent = () => {
    if (this.config && this.config.optimizeAnimations){
      disableOptimizations();
      clearOptimizationTimer();
    }
  };
  
  // css and layout injection
  this.resetDesign = () => {
    if (this.css){
      this.css.remove();
    }
    
    this.css = window.TDPF_createCustomStyle(this);
    
    if (this.theme){
      this.theme.remove();
    }
    
    if (this.config.themeOverride){
      this.theme = window.TDPF_createCustomStyle(this);
    }
    
    if (this.icons){
      document.head.removeChild(this.icons);
      this.icons = null;
    }
  };
  
  this.reinjectAll = () => {
    this.resetDesign();
    
    clearOptimizationTimer();
    
    if (this.config.optimizeAnimations){
      injectOptimizations();
    }
    else{
      disableOptimizations();
    }
    
    this.css.insert("#general_settings .cf { display: none !important }");
    this.css.insert("#settings-modal .js-setting-list li:nth-child(3) { border-bottom: 1px solid #ccd6dd }");
    
    this.css.insert(`html[data-td-font] { font-size: ${this.config.fontSize} !important }`);
    this.css.insert(`.avatar { border-radius: ${this.config.avatarRadius}% !important }`);
    
    if (this.config.composerWidth !== "default"){
      const width = this.config.composerWidth;
      this.css.insert(`.js-app-content.is-open { margin-right: ${width} !important; transform: translateX(${width}) !important }`);
      this.css.insert(`#tduck .js-app-content.tduck-is-opening { margin-right: 0 !important }`);
      this.css.insert(`.js-drawer { width: ${width} !important; left: -${width} !important }`);
    }
    
    let currentTheme = TD.settings.getTheme();
    
    if (currentTheme === "dark" && this.config.themeOverride){
      currentTheme = this.config.themeOverride;
    }
    
    let notificationScrollbarColor = null;
    
    if (this.config.themeColorTweaks){
      switch(currentTheme){
        case "black":
          this.css.insert(".app-content, .app-columns-container { background-color: #444448 !important }");
          this.css.insert(".column-header-temp { background-color: transparent !important }");
          this.css.insert(".column-drag-handle { opacity: 0.5 !important }");
          this.css.insert(".column-drag-handle:hover { opacity: 1 !important }");
          this.css.insert(".column-message.is-actionable span:hover > .icon-small-valigned { filter: saturate(20) }");
          this.css.insert(".scroll-styled-v:not(.scroll-alt)::-webkit-scrollbar-thumb:not(:hover), .scroll-styled-h:not(.scroll-alt)::-webkit-scrollbar-thumb:not(:hover) { background-color: #666 !important }");
          notificationScrollbarColor = "666";
          break;
          
        case "dark":
          this.css.insert(".scroll-styled-v:not(.scroll-alt)::-webkit-scrollbar-track, .scroll-styled-h:not(.scroll-alt)::-webkit-scrollbar-track { border-left-color: #14171A !important }");
          break;
          
        case "light":
          this.css.insert(".scroll-styled-v:not(.scroll-alt)::-webkit-scrollbar-thumb:not(:hover), .scroll-styled-h:not(.scroll-alt)::-webkit-scrollbar-thumb:not(:hover) { background-color: #d2d6da !important }");
          this.css.insert(".app-columns-container.scroll-styled-h::-webkit-scrollbar-thumb:not(:hover) { background-color: #a5aeb5 !important }");
          notificationScrollbarColor = "a5aeb5";
          break;
      }
    }
    
    if (this.config.showCharacterCount){
      this.css.insert("#tduck .js-character-count.is-hidden { display: inline !important }");
    }
    
    if (this.config.hideTweetActions){
      this.css.insert(".tweet-action { opacity: 0; }");
      this.css.insert(".tweet-actions.is-visible .tweet-action { opacity: 0.5 }");
      this.css.insert(".is-favorite .tweet-action, .is-retweet .tweet-action { opacity: 0.5; visibility: visible !important }");
      this.css.insert(".tweet:hover .tweet-action, .tweet-action.is-selected, .is-favorite .tweet-action[rel='favorite'], .is-retweet .tweet-action[rel='retweet'] { opacity: 1 !important; visibility: visible !important }");
    }
    
    if (this.config.moveTweetActionsToRight){
      this.css.insert("#tduck .tweet-actions { float: right !important; width: auto !important }");
      this.css.insert("#tduck .tweet-actions > li:nth-child(4) { margin-right: 2px !important }");
    }
    
    if (this.config.forceArialFont){
      this.css.insert("#tduck { font-family: Arial, sans-serif; font-weight: 400 }");
      this.css.insert("#tduck input, #tduck label, #tduck select, #tduck textarea { font-family: Arial }")
    }
    
    if (this.config.increaseQuoteTextSize){
      this.css.insert(".quoted-tweet { font-size: 1em !important }");
    }
    
    if (this.config.smallComposeTextSize){
      this.css.insert("#tduck .compose-text { font-size: 12px !important; height: 120px !important }");
    }
    
    if (this.config.revertIcons){
      let iconData = [
        [ "twitter-bird", "00" ],
        [ "mention", "01" ],
        [ "following", "02" ],
        [ "message", "03" ],
        [ "home", "04" ],
        [ "hashtag", "05" ],
        [ "reply", "06" ],
        [ "favorite", "55" ],
        [ "retweet", "08" ],
        [ "drafts", "09" ],
        [ "search", "0a" ],
        [ "trash", "0c" ],
        [ "close", "0d" ],
        [ "arrow-r:before,.Icon--caretRight", "0e" ],
        [ "arrow-l:before,.Icon--caretLeft", "0f" ],
        [ "protected", "13" ],
        [ "list", "14" ],
        [ "camera", "15" ],
        [ "more", "16" ],
        [ "settings", "18" ],
        [ "notifications", "19" ],
        [ "user-dd", "1a" ],
        [ "activity", "1c" ],
        [ "trending", "1d" ],
        [ "minus", "1e" ],
        [ "plus", "1f" ],
        [ "geo", "20" ],
        [ "check", "21" ],
        [ "schedule", "22" ],
        [ "dot", "23" ],
        [ "user", "24" ],
        [ "content", "25" ],
        [ "arrow-d:before,.Icon--caretDown", "26" ],
        [ "arrow-u", "27" ],
        [ "share", "28" ],
        [ "info", "29" ],
        [ "verified", "2a" ],
        [ "translator", "2b" ],
        [ "blocked", "2c" ],
        [ "constrain", "2d" ],
        [ "play-video", "2e" ],
        [ "empty", "2f" ],
        [ "clear-input", "30" ],
        [ "compose", "31" ],
        [ "mark-read", "32" ],
        [ "arrow-r-double", "33" ],
        [ "arrow-l-double", "34" ],
        [ "follow", "35" ],
        [ "image", "36" ],
        [ "popout", "37" ],
        [ "move", "39" ],
        [ "compose-grid", "3a" ],
        [ "compose-minigrid", "3b" ],
        [ "compose-list", "3c" ],
        [ "edit", "40" ],
        [ "clear-timeline", "41" ],
        [ "sliders", "42" ],
        [ "custom-timeline", "43" ],
        [ "compose-dm", "44" ],
        [ "bg-dot", "45" ],
        [ "user-team-mgr", "46" ],
        [ "user-switch", "47" ],
        [ "conversation", "48" ],
        [ "dataminr", "49" ],
        [ "link", "4a", ],
        [ "flash", "50" ],
        [ "pointer-u", "51" ],
        [ "analytics", "54" ],
        [ "heart", "55" ],
        [ "calendar", "56" ],
        [ "attachment", "57" ],
        [ "play", "58" ],
        [ "bookmark", "59" ],
        [ "play-badge", "60" ],
        [ "gif-badge", "61" ],
        [ "poll", "62" ],
        
        [ "heart-filled", "55" ],
        [ "retweet-filled", "08" ],
        [ "list-filled", "14" ],
        [ "user-filled", "35" ],
      ];
      
      this.icons = document.createElement("style");
      this.icons.innerHTML = `
@font-face {
  font-family: '_of';
  src: url("https://ton.twimg.com/tweetdeck-web/web/assets/fonts/tweetdeck-regular-webfont.5f4ea87976.woff") format("woff");
  font-weight: normal;
  font-style: normal;
}

${iconData.map(entry => `#tduck .icon-${entry[0]}:before{content:\"\\f0${entry[1]}\";font-family:_of!important}`).join("")}

.drawer .btn .icon, .app-header .btn .icon { line-height: 1em !important }
.app-search-fake .icon { margin-top: -3px !important }
#tduck .search-input-control .icon { font-size: 20px !important; top: -4px !important }
#tduck .js-docked-compose .js-drawer-close { margin: 20px 0 0 !important }
#tduck .compose-media-bar-remove .icon-close, #tduck .compose-media-grid-remove .icon-close { padding: 3px 2px 1px !important }

.js-column-header .column-type-icon { margin-top: 0 !important }
.inline-reply .pull-left .Button--link { margin-top: 3px !important }
.js-inline-compose-pop .icon-popout { font-size: 23px !important }

.tweet-action-item .icon-favorite-toggle { font-size: 16px !important; }
.tweet-action-item .heartsprite { top: -260% !important; left: -260% !important; transform: scale(0.4, 0.39) translateY(0.5px) !important; }
.tweet-footer { margin-top: 6px !important }`;
      
      document.head.appendChild(this.icons);
    }
    
    if (currentTheme === "black"){
      $TDP.readFileRoot(this.$token, "theme.black.css").then(contents => {
        if (this.theme){
          this.theme.element.innerHTML = contents;
          TD.settings.setTheme("dark"); // forces refresh of notification head tag
        }
      });
    }
    
    if (this.config.columnWidth[0] === '/'){
      let cols = this.config.columnWidth.slice(1);
      
      this.css.insert(".column { width: calc((100vw - 205px) / "+cols+" - 6px) !important; min-width: 160px }");
      this.css.insert(".is-condensed .column { width: calc((100vw - 65px) / "+cols+" - 6px) !important }");
    }
    else{
      this.css.insert(".column { width: "+this.config.columnWidth+" !important }");
    }
    
    switch(this.config.columnWidth){
      case "/6":
        TD.settings.setColumnWidth("narrow");
        break;
        
      case "310px":
      case "/5":
        TD.settings.setColumnWidth("medium");
        break;
        
      default:
        TD.settings.setColumnWidth(parseInt(this.config.columnWidth, 10) < 310 ? "narrow" : "wide"); // NaN will give "wide"
        break;
    }
    
    switch(this.config.fontSize){
      case "13px": TD.settings.setFontSize("small"); break;
      case "14px": TD.settings.setFontSize("medium"); break;
      case "15px": TD.settings.setFontSize("large"); break;
      default: TD.settings.setFontSize(parseInt(this.config.fontSize, 10) >= 16 ? "largest" : "smallest"); break;
    }
    
    $TDP.injectIntoNotificationsBefore(this.$token, "css", "</head>", `
<style type='text/css'>
html[data-td-font] { font-size: ${this.config.fontSize} !important }
.avatar { border-radius: ${this.config.avatarRadius}% !important }

${this.config.forceArialFont ? `
#tduck { font-family: Arial, sans-serif; font-weight: 400 }
` : ``}

${this.config.increaseQuoteTextSize ? `
.quoted-tweet { font-size: 1em !important }
` : ``}

${this.config.revertIcons ? `
@font-face { font-family: '_of'; src: url(\"https://ton.twimg.com/tweetdeck-web/web/assets/fonts/tweetdeck-regular-webfont.5f4ea87976.woff\") format(\"woff\"); font-weight: normal; font-style: normal }
#tduck .icon-reply:before{content:"\\f006";font-family:_of!important}
#tduck .icon-heart-filled:before{content:"\\f055";font-family:_of!important}
#tduck .icon-retweet-filled:before{content:"\\f008";font-family:_of!important}
#tduck .icon-list-filled:before{content:"\\f014";font-family:_of!important}
#tduck .icon-user-filled:before{content:"\\f035";font-family:_of!important}
#tduck .icon-user-dd:before{content:"\\f01a";font-family:_of!important}
` : ``}

${currentTheme === "black" ? `
html.dark a, html.dark a:hover, html.dark a:focus, html.dark a:active { color: #8bd }
#tduck-show-thread, .other-replies-link { color: #8bd !important }
.quoted-tweet { border-color: #292f33 !important }
` : ``}

${notificationScrollbarColor ? `
.scroll-styled-v::-webkit-scrollbar-thumb:not(:hover), .scroll-styled-h::-webkit-scrollbar-thumb:not(:hover) { background-color: #${notificationScrollbarColor} !important }
` : ``}
</style>`);
  };
  
  this.uiShowActionsMenuEvent = () => {
    if (this.config.moveTweetActionsToRight){
      $(".js-dropdown.pos-r").toggleClass("pos-r pos-l");
    }
  };
  
  this.uiDrawerActiveEvent = (e, data) => {
    if (data.activeDrawer === null || this.config.composerWidth === "default") {
      return;
    }
    
    const ele = $(".js-app-content").addClass("tduck-is-opening");
    setTimeout(() => ele.removeClass("tduck-is-opening"), 250);
  };
}

ready(){
  // optimization events
  $(window).on("focus", this.onWindowFocusEvent);
  $(window).on("blur", this.onWindowBlurEvent);
  
  // layout events
  $(document).on("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  $(document).on("uiDrawerActive", this.uiDrawerActiveEvent);
  
  // modal
  $("[data-action='settings-menu']").on("click", this.onSettingsMenuClickedEvent);
  $(".js-app").append('<div id="td-design-plugin-modal" class="js-modal settings-modal ovl scroll-v scroll-styled-v"></div>');
  
  // global settings override
  const me = this;
  
  this.prevFuncSettingsGetInfo = TD.components.GlobalSettings.prototype.getInfo;
  this.prevFuncSettingsSwitchTab = TD.components.GlobalSettings.prototype.switchTab;
  
  TD.components.GlobalSettings.prototype.getInfo = function(){
    let data = me.prevFuncSettingsGetInfo.apply(this, arguments);
    
    data.tabs.push({
      title: "Layout & Design",
      action: "tdp-edit-design"
    });
    
    return data;
  };
  
  TD.components.GlobalSettings.prototype.switchTab = function(tab){
    if (tab === "tdp-edit-design"){
      me.configure();
    }
    else{
      return me.prevFuncSettingsSwitchTab.apply(this, arguments);
    }
  };
}

configure(){
  $TDP.readFileRoot(this.$token, "modal.html").then(contents => {
    this.htmlModal = contents;
    new this.customDesignModal();
  }).catch(err => {
    $TD.alert("error", "Error loading the configuration dialog: "+err.message);
  });
}

disabled(){
  if (this.css){
    this.css.remove();
  }
  
  if (this.theme){
    this.theme.remove();
  }
  
  if (this.icons){
    document.head.removeChild(this.icons);
  }
  
  if (this.optimizations){
    this.optimizations.remove();
  }
  
  if (this.optimizationTimer){
    window.clearTimeout(this.optimizationTimer);
  }
  
  $(window).off("focus", this.onWindowFocusEvent);
  $(window).off("blur", this.onWindowBlurEvent);
  
  $(document).off("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  $(document).off("uiDrawerActive", this.uiDrawerActiveEvent);
  
  TD.components.GlobalSettings.prototype.getInfo = this.prevFuncSettingsGetInfo;
  TD.components.GlobalSettings.prototype.switchTab = this.prevFuncSettingsSwitchTab;
  
  $("[data-action='settings-menu']").off("click", this.onSettingsMenuClickedEvent);
  $("#td-design-plugin-modal").remove();
}
