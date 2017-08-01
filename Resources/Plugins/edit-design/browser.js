enabled(){
  // elements & data
  this.css = null;
  this.icons = null;
  this.htmlModal = null;
  this.config = null;
  
  this.defaultConfig = {
    columnWidth: "310px",
    fontSize: "12px",
    hideTweetActions: true,
    moveTweetActionsToRight: true,
    themeColorTweaks: true,
    revertIcons: true,
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
      this.config = $.extend(this.defaultConfig, this.tmpConfig);
      this.tmpConfig = null;
      this.reinjectAll();
      
      if (this.firstTimeLoad){
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
    if (this.htmlModal === null || this.config === null){
      return;
    }
    
    setTimeout(() => {
      let menu = $(".js-dropdown-content").children("ul").first();
      if (menu.length === 0)return;
      
      let itemTD = menu.children("[data-std]").first();
      if (itemTD.length === 0)return;
      
      if (!itemTD.prev().hasClass("drp-h-divider")){
        itemTD.before('<li class="drp-h-divider"></li>');
      }
      
      let itemEditDesign = $('<li class="is-selectable"><a href="#" data-action>Edit layout &amp; design</a></li>');
      itemTD.after(itemEditDesign);
      
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
        if (!item.val(me.config[key]).val()){
          let custom = item.find("option[value='custom']");
          
          if (custom.length === 1){
            item.val("custom");
            custom.text(getTextForCustom(key));
          }
        }
        
        item.change(function(){ // TODO change doesn't fire when Custom is already selected
          let val = item.val();
          
          if (val === "custom"){
            val = prompt("Enter custom value:");
            
            if (val){
              updateKey(key, val);
              item.find("option[value='custom']").text(getTextForCustom(key));
            }
          }
          else{
            updateKey(key, item.val());
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
      setTimeout(function(){
        TD.settings.setTheme($(this).attr("data-td-theme"));
        $(document).trigger("uiToggleTheme");
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
    
    this.css.insert(".txt-base-smallest:not(.icon), .txt-base-largest:not(.icon) { font-size: "+this.config.fontSize+" !important }");
    this.css.insert(".avatar { border-radius: "+this.config.avatarRadius+"% !important }");
    
    if (this.config.themeColorTweaks){
      switch(TD.settings.getTheme()){
        case "dark":
          this.css.insert(".app-content, .app-columns-container { background-color: #444448 }");
          this.css.insert(".column-drag-handle { opacity: 0.5 }");
          this.css.insert(".column-drag-handle:hover { opacity: 1 }");
          break;

        case "light":
          this.css.insert(".scroll-styled-v::-webkit-scrollbar-thumb, .scroll-styled-h::-webkit-scrollbar-thumb { background-color: #d2d6da }");
          this.css.insert(".app-columns-container.scroll-styled-h::-webkit-scrollbar-thumb:not(:hover) { background-color: #a5aeb5 }");
          break;
      }
    }
    
    if (this.config.hideTweetActions){
      this.css.insert(".tweet-action { opacity: 0; }");
      this.css.insert(".tweet-actions.is-visible .tweet-action { opacity: 0.5; }");
      this.css.insert(".is-favorite .tweet-action, .is-retweet .tweet-action { opacity: 0.5; visibility: visible !important }");
      this.css.insert(".tweet:hover .tweet-action, .tweet-action.is-selected, .is-favorite .tweet-action[rel='favorite'], .is-retweet .tweet-action[rel='retweet'] { opacity: 1 !important; visibility: visible !important }");
    }
    
    if (this.config.moveTweetActionsToRight){
      this.css.insert(".tweet-actions { float: right !important; width: auto !important }");
      this.css.insert(".tweet-actions > li:nth-child(4) { margin-right: 2px !important }");
    }
    
    if (this.config.smallComposeTextSize){
      this.css.insert(".compose-text { font-size: 12px !important; height: 120px !important }");
    }
    
    if (this.config.revertIcons){
      this.icons = document.createElement("style");
      this.icons.innerHTML = `
@font-face {
  font-family: 'tweetdeckold';
  src: url("https://ton.twimg.com/tweetdeck-web/web/assets/fonts/tweetdeck-regular-webfont.5f4ea87976.woff") format("woff");
  font-weight: normal;
  font-style: normal;
}

.icon-twitter-bird:before{content:"\\f000";font-family:tweetdeckold}
.icon-mention:before{content:"\\f001";font-family:tweetdeckold}
.icon-following:before{content:"\\f002";font-family:tweetdeckold}
.icon-message:before{content:"\\f003";font-family:tweetdeckold}
.icon-home:before{content:"\\f004";font-family:tweetdeckold}
.icon-hashtag:before{content:"\\f005";font-family:tweetdeckold}
.icon-reply:before{content:"\\f006";font-family:tweetdeckold}
.icon-favorite:before{content:"\\f055";font-family:tweetdeckold}
.icon-retweet:before{content:"\\f008";font-family:tweetdeckold}
.icon-drafts:before{content:"\\f009";font-family:tweetdeckold}
.icon-search:before{content:"\\f00a";font-family:tweetdeckold}
.icon-trash:before{content:"\\f00c";font-family:tweetdeckold}
.icon-close:before{content:"\\f00d";font-family:tweetdeckold}
.icon-arrow-r:before,.Icon--caretRight:before{content:"\\f00e";font-family:tweetdeckold}
.icon-arrow-l:before,.Icon--caretLeft:before{content:"\\f00f";font-family:tweetdeckold}
.icon-protected:before{content:"\\f013";font-family:tweetdeckold}
.icon-list:before{content:"\\f014";font-family:tweetdeckold}
.icon-camera:before{content:"\\f015";font-family:tweetdeckold}
.icon-more:before{content:"\\f016";font-family:tweetdeckold}
.icon-settings:before{content:"\\f018";font-family:tweetdeckold}
.icon-notifications:before{content:"\\f019";font-family:tweetdeckold}
.icon-user-dd:before{content:"\\f01a";font-family:tweetdeckold}
.icon-activity:before{content:"\\f01c";font-family:tweetdeckold}
.icon-trending:before{content:"\\f01d";font-family:tweetdeckold}
.icon-minus:before{content:"\\f01e";font-family:tweetdeckold}
.icon-plus:before{content:"\\f01f";font-family:tweetdeckold}
.icon-geo:before{content:"\\f020";font-family:tweetdeckold}
.icon-check:before{content:"\\f021";font-family:tweetdeckold}
.icon-schedule:before{content:"\\f022";font-family:tweetdeckold}
.icon-dot:before{content:"\\f023";font-family:tweetdeckold}
.icon-user:before{content:"\\f024";font-family:tweetdeckold}
.icon-content:before{content:"\\f025";font-family:tweetdeckold}
.icon-arrow-d:before,.Icon--caretDown:before{content:"\\f026";font-family:tweetdeckold}
.icon-arrow-u:before{content:"\\f027";font-family:tweetdeckold}
.icon-share:before{content:"\\f028";font-family:tweetdeckold}
.icon-info:before{content:"\\f029";font-family:tweetdeckold}
.icon-verified:before{content:"\\f02a";font-family:tweetdeckold}
.icon-translator:before{content:"\\f02b";font-family:tweetdeckold}
.icon-blocked:before{content:"\\f02c";font-family:tweetdeckold}
.icon-constrain:before{content:"\\f02d";font-family:tweetdeckold}
.icon-play-video:before{content:"\\f02e";font-family:tweetdeckold}
.icon-empty:before{content:"\\f02f";font-family:tweetdeckold}
.icon-clear-input:before{content:"\\f030";font-family:tweetdeckold}
.icon-compose:before{content:"\\f031";font-family:tweetdeckold}
.icon-mark-read:before{content:"\\f032";font-family:tweetdeckold}
.icon-arrow-r-double:before{content:"\\f033";font-family:tweetdeckold}
.icon-arrow-l-double:before{content:"\\f034";font-family:tweetdeckold}
.icon-follow:before{content:"\\f035";font-family:tweetdeckold}
.icon-image:before{content:"\\f036";font-family:tweetdeckold}
.icon-popout:before{content:"\\f037";font-family:tweetdeckold}
.icon-move:before{content:"\\f039";font-family:tweetdeckold}
.icon-compose-grid:before{content:"\\f03a";font-family:tweetdeckold}
.icon-compose-minigrid:before{content:"\\f03b";font-family:tweetdeckold}
.icon-compose-list:before{content:"\\f03c";font-family:tweetdeckold}
.icon-edit:before{content:"\\f040";font-family:tweetdeckold}
.icon-clear-timeline:before{content:"\\f041";font-family:tweetdeckold}
.icon-sliders:before{content:"\\f042";font-family:tweetdeckold}
.icon-custom-timeline:before{content:"\\f043";font-family:tweetdeckold}
.icon-compose-dm:before{content:"\\f044";font-family:tweetdeckold}
.icon-bg-dot:before{content:"\\f045";font-family:tweetdeckold}
.icon-user-team-mgr:before{content:"\\f046";font-family:tweetdeckold}
.icon-user-switch:before{content:"\\f047";font-family:tweetdeckold}
.icon-conversation:before{content:"\\f048";font-family:tweetdeckold}
.icon-dataminr:before{content:"\\f049";font-family:tweetdeckold}
.icon-link:before{content:"\\f04a";font-family:tweetdeckold}
.icon-flash:before{content:"\\f050";font-family:tweetdeckold}
.icon-pointer-u:before{content:"\\f051";font-family:tweetdeckold}
.icon-analytics:before{content:"\\f054";font-family:tweetdeckold}
.icon-heart:before{content:"\\f055";font-family:tweetdeckold}
.icon-calendar:before{content:"\\f056";font-family:tweetdeckold}
.icon-attachment:before{content:"\\f057";font-family:tweetdeckold}
.icon-play:before{content:"\\f058";font-family:tweetdeckold}
.icon-bookmark:before{content:"\\f059";font-family:tweetdeckold}
.icon-play-badge:before{content:"\\f060";font-family:tweetdeckold}
.icon-gif-badge:before{content:"\\f061";font-family:tweetdeckold}
.icon-poll:before{content:"\\f062";font-family:tweetdeckold}

.icon-heart-filled:before{content:"\\f055";font-family:tweetdeckold}
.icon-retweet-filled:before{content:"\\f008";font-family:tweetdeckold}
.icon-list-filled:before{content:"\\f014";font-family:tweetdeckold}
.icon-user-filled:before{content:"\\f035";font-family:tweetdeckold}

.column-type-icon { bottom: 26px !important }
.is-options-open .column-type-icon { bottom: 25px !important }
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
.txt-base-smallest:not(.icon), .txt-base-largest:not(.icon) { font-size: ${this.config.fontSize} !important }
.avatar { border-radius: ${this.config.avatarRadius}% !important }

${this.config.revertIcons ? `
@font-face { font-family: 'tweetdeckold'; src: url(\"https://ton.twimg.com/tweetdeck-web/web/assets/fonts/tweetdeck-regular-webfont.5f4ea87976.woff\") format(\"woff\"); font-weight: normal; font-style: normal }
.icon-reply:before{content:"\\f006";font-family:tweetdeckold}
.icon-heart-filled:before{content:"\\f055";font-family:tweetdeckold}
.icon-retweet-filled:before{content:"\\f008";font-family:tweetdeckold}
.icon-list-filled:before{content:"\\f014";font-family:tweetdeckold}
.icon-user-filled:before{content:"\\f035";font-family:tweetdeckold}
.icon-user-dd:before{content:"\\f01a";font-family:tweetdeckold}
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
