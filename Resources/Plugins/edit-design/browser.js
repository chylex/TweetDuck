enabled(){
  // elements & data
  this.css = null;
  this.icons = null;
  this.htmlModal = null;
  this.config = null;
  
  this.defaultConfig = {
    _theme: "light",
    columnWidth: "310px",
    fontSize: "12px",
    hideTweetActions: true,
    moveTweetActionsToRight: true,
    themeColorTweaks: true,
    revertIcons: true,
    showCharacterCount: true,
    increaseQuoteTextSize: false,
    smallComposeTextSize: false,
    optimizeAnimations: true,
    avatarRadius: 2
  };
  
  this.firstTimeLoad = null;
  
  // modal dialog loading
  $TDP.readFileRoot(this.$token, "modal.html").then(contents => {
    this.htmlModal = contents;
  }).catch(err => {
    $TD.alert("error", "Problem loading data for the design edit plugin: "+err.message);
  });
  
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
      
      this.config = $.extend(this.defaultConfig, this.tmpConfig);
      this.tmpConfig = null;
      this.reinjectAll();
      
      if (this.firstTimeLoad || needsResave){
        $TDP.writeFile(this.$token, configFile, JSON.stringify(this.config));
      }
    }
  };
  
  var loadConfigObject = obj => {
    this.tmpConfig = obj || {};
    this.firstTimeLoad = obj === null;
    
    this.onStageReady();
  };
  
  if (this.$$wasLoadedBefore){
    this.onStageReady();
  }
  else{
    $(document).one("dataSettingsValues", () => {
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
      
      this.$$wasLoadedBefore = true;
      this.onStageReady();
    });
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
    return if this.htmlModal === null || this.config === null;
    
    setTimeout(() => {
      let menu = $(".js-dropdown-content").children("ul").first();
      return if menu.length === 0;
      
      let itemTD = menu.children("[data-tweetduck]").first();
      return if itemTD.length === 0;
      
      if (!itemTD.prev().hasClass("drp-h-divider")){
        itemTD.before('<li class="drp-h-divider"></li>');
      }
      
      let itemEditDesign = $('<li class="is-selectable"><a href="#" data-action>Edit layout &amp; design</a></li>');
      itemEditDesign.insertAfter(itemTD);
      
      itemEditDesign.on("click", "a", this.openEditDesignDialog);
      
      itemEditDesign.hover(function(){
        $(this).addClass("is-selected");
      }, function(){
        $(this).removeClass("is-selected");
      });
    }, 1);
  };
  
  // modal dialog setup
  var me = this;
  
  var updateKey = function(key, value){
    me.config[key] = value;
    
    setTimeout(function(){
      me.saveConfig();
      me.reinjectAll();
    }, 1); // delays the slight lag caused by saving and reinjection
  };
  
  var customDesignModal = TD.components.BaseModal.extend(function(){
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
        
        let resetMyValue = () => {
          if (!item.val(me.config[key]).val() && optionCustom.length === 1){
            item.val(optionCustom.attr("value"));
            optionCustom.text(getTextForCustom(key));
          }
        };
        
        resetMyValue();
        
        item.change(function(){ // TODO change doesn't fire when Custom is already selected
          let val = item.val();
          
          if (val === "custom-px"){
            val = (prompt("Enter custom value (px):") || "").trim();
            
            if (val){
              if (val.endsWith("px")){
                val = val.slice(0, -2).trim();
              }
              
              if (/^[0-9]+$/.test(val)){
                updateKey(key, val+"px");
                optionCustom.text(getTextForCustom(key));
              }
              else{
                alert("Invalid value, only px values are supported.");
                resetMyValue();
              }
            }
            else{
              resetMyValue();
            }
          }
          else{
            updateKey(key, item.val());
            optionCustom.text("Custom");
          }
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
    modal.find("[data-td-theme='"+TD.settings.getTheme()+"']").prop("checked", true);
    
    modal.find("[data-td-theme]").change(function(){
      me.config._theme = $(this).attr("data-td-theme");
      
      setTimeout(function(){
        $(document).trigger("uiToggleTheme");
        me.saveConfig();
        me.reinjectAll();
      }, 1);
    });
  }).methods({
    _render: () => $(this.htmlModal),
    destroy: function(){
      if (this.reloadPage){
        window.TDPF_requestReload();
        return;
      }
      
      $("#td-design-plugin-modal").hide();
      this.supr();
    }
  });
  
  this.openEditDesignDialog = () => new customDesignModal();
  
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
    
    this.css.insert("html[data-td-font] { font-size: "+this.config.fontSize+" !important }");
    this.css.insert(".avatar { border-radius: "+this.config.avatarRadius+"% !important }");
    
    let notificationScrollbarColor = null;
    
    if (this.config.themeColorTweaks){
      switch(TD.settings.getTheme()){
        case "dark":
          this.css.insert(".app-content, .app-columns-container { background-color: #444448 !important }");
          this.css.insert(".column-drag-handle { opacity: 0.5 !important }");
          this.css.insert(".column-drag-handle:hover { opacity: 1 !important }");
          this.css.insert(".scroll-styled-v:not(.scroll-alt)::-webkit-scrollbar-thumb, .scroll-styled-h:not(.scroll-alt)::-webkit-scrollbar-thumb { background-color: #666 !important }");
          notificationScrollbarColor = "666";
          break;

        case "light":
          this.css.insert(".scroll-styled-v:not(.scroll-alt)::-webkit-scrollbar-thumb, .scroll-styled-h:not(.scroll-alt)::-webkit-scrollbar-thumb { background-color: #d2d6da !important }");
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
    
    if (this.config.increaseQuoteTextSize){
      this.css.insert(".quoted-tweet { font-size: 1em !important }");
    }
    
    if (this.config.smallComposeTextSize){
      this.css.insert(".compose-text { font-size: 12px !important; height: 120px !important }");
    }
    
    if (this.config.revertIcons){
      let iconData = [
        [ ".icon-twitter-bird", "00" ],
        [ ".icon-mention", "01" ],
        [ ".icon-following", "02" ],
        [ ".icon-message", "03" ],
        [ ".icon-home", "04" ],
        [ ".icon-hashtag", "05" ],
        [ ".icon-reply", "06" ],
        [ ".icon-favorite", "55" ],
        [ ".icon-retweet", "08" ],
        [ ".icon-drafts", "09" ],
        [ ".icon-search", "0a" ],
        [ ".icon-trash", "0c" ],
        [ ".icon-close", "0d" ],
        [ ".icon-arrow-r:before,.Icon--caretRight", "0e" ],
        [ ".icon-arrow-l:before,.Icon--caretLeft", "0f" ],
        [ ".icon-protected", "13" ],
        [ ".icon-list", "14" ],
        [ ".icon-camera", "15" ],
        [ ".icon-more", "16" ],
        [ ".icon-settings", "18" ],
        [ ".icon-notifications", "19" ],
        [ ".icon-user-dd", "1a" ],
        [ ".icon-activity", "1c" ],
        [ ".icon-trending", "1d" ],
        [ ".icon-minus", "1e" ],
        [ ".icon-plus", "1f" ],
        [ ".icon-geo", "20" ],
        [ ".icon-check", "21" ],
        [ ".icon-schedule", "22" ],
        [ ".icon-dot", "23" ],
        [ ".icon-user", "24" ],
        [ ".icon-content", "25" ],
        [ ".icon-arrow-d:before,.Icon--caretDown", "26" ],
        [ ".icon-arrow-u", "27" ],
        [ ".icon-share", "28" ],
        [ ".icon-info", "29" ],
        [ ".icon-verified", "2a" ],
        [ ".icon-translator", "2b" ],
        [ ".icon-blocked", "2c" ],
        [ ".icon-constrain", "2d" ],
        [ ".icon-play-video", "2e" ],
        [ ".icon-empty", "2f" ],
        [ ".icon-clear-input", "30" ],
        [ ".icon-compose", "31" ],
        [ ".icon-mark-read", "32" ],
        [ ".icon-arrow-r-double", "33" ],
        [ ".icon-arrow-l-double", "34" ],
        [ ".icon-follow", "35" ],
        [ ".icon-image", "36" ],
        [ ".icon-popout", "37" ],
        [ ".icon-move", "39" ],
        [ ".icon-compose-grid", "3a" ],
        [ ".icon-compose-minigrid", "3b" ],
        [ ".icon-compose-list", "3c" ],
        [ ".icon-edit", "40" ],
        [ ".icon-clear-timeline", "41" ],
        [ ".icon-sliders", "42" ],
        [ ".icon-custom-timeline", "43" ],
        [ ".icon-compose-dm", "44" ],
        [ ".icon-bg-dot", "45" ],
        [ ".icon-user-team-mgr", "46" ],
        [ ".icon-user-switch", "47" ],
        [ ".icon-conversation", "48" ],
        [ ".icon-dataminr", "49" ],
        [ ".icon-link", "4a", ],
        [ ".icon-flash", "50" ],
        [ ".icon-pointer-u", "51" ],
        [ ".icon-analytics", "54" ],
        [ ".icon-heart", "55" ],
        [ ".icon-calendar", "56" ],
        [ ".icon-attachment", "57" ],
        [ ".icon-play", "58" ],
        [ ".icon-bookmark", "59" ],
        [ ".icon-play-badge", "60" ],
        [ ".icon-gif-badge", "61" ],
        [ ".icon-poll", "62" ],
        
        [ ".icon-heart-filled", "55" ],
        [ ".icon-retweet-filled", "08" ],
        [ ".icon-list-filled", "14" ],
        [ ".icon-user-filled", "35" ],
      ];
      
      this.icons = document.createElement("style");
      this.icons.innerHTML = `
@font-face {
  font-family: '_of';
  src: url("https://ton.twimg.com/tweetdeck-web/web/assets/fonts/tweetdeck-regular-webfont.5f4ea87976.woff") format("woff");
  font-weight: normal;
  font-style: normal;
}

${iconData.map(entry => `#tduck ${entry[0]}:before{content:\"\\f0${entry[1]}\";font-family:_of!important}`).join("")}

.drawer .btn .icon, .app-header .btn .icon { line-height: 1em !important }
.column-header .column-type-icon { bottom: 26px !important }
.is-options-open .column-type-icon { bottom: 25px !important }

.tweet-action-item .icon-favorite-toggle { font-size: 16px !important; }
.tweet-action-item .heartsprite { top: -260% !important; left: -260% !important; transform: scale(0.4, 0.39) translateY(0.5px) !important; }
.tweet-footer { margin-top: 6px !important }`;
      
      document.head.appendChild(this.icons);
    }
    
    if (this.config.columnWidth[0] === '/'){
      let cols = this.config.columnWidth.slice(1);
      
      this.css.insert(".column { width: calc((100vw - 205px) / "+cols+" - 6px) !important }");
      this.css.insert(".is-condensed .column { width: calc((100vw - 55px) / "+cols+" - 6px) !important }");
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

${notificationScrollbarColor ? `
.scroll-styled-v::-webkit-scrollbar-thumb, .scroll-styled-h::-webkit-scrollbar-thumb { background-color: #${notificationScrollbarColor} !important }
` : ``}
</style>`);
  };
  
  this.uiShowActionsMenuEvent = () => {
    if (this.config.moveTweetActionsToRight){
      $(".js-dropdown.pos-r").toggleClass("pos-r pos-l");
    }
  };
}

ready(){
  // optimization events
  $(window).on("focus", this.onWindowFocusEvent);
  $(window).on("blur", this.onWindowBlurEvent);
  
  // layout events
  $(document).on("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  
  // modal
  $("[data-action='settings-menu']").on("click", this.onSettingsMenuClickedEvent);
  $(".js-app").append('<div id="td-design-plugin-modal" class="js-modal settings-modal ovl scroll-v scroll-styled-v"></div>');
  
  // global settings override
  var me = this;
  
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
      me.openEditDesignDialog();
    }
    else{
      return me.prevFuncSettingsSwitchTab.apply(this, arguments);
    }
  };
}

disabled(){
  if (this.css){
    this.css.remove();
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
  
  $(document).off("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  $(window).off("focus", this.onWindowFocusEvent);
  $(window).off("blur", this.onWindowBlurEvent);
  
  TD.components.GlobalSettings.prototype.getInfo = this.prevFuncSettingsGetInfo;
  TD.components.GlobalSettings.prototype.switchTab = this.prevFuncSettingsSwitchTab;
  
  $("[data-action='settings-menu']").off("click", this.onSettingsMenuClickedEvent);
  $("#td-design-plugin-modal").remove();
}
