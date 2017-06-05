constructor(){
  super({
    requiresPageReload: true
  })
}

enabled(){
  // elements & data
  this.css = null;
  this.htmlModal = null;
  this.config = null;
  
  this.defaultConfig = {
    columnWidth: "310px",
    fontSize: "12px",
    hideTweetActions: true,
    moveTweetActionsToRight: true,
    revertReplies: false,
    themeColorTweaks: true,
    roundedScrollBars: false,
    smallComposeTextSize: false,
    optimizeAnimations: true,
    avatarRadius: 10
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
    this.injectDeciderReplyHook(obj && obj.revertReplies);
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
      
      itemEditDesign.on("click", "a", function(){
        new customDesignModal();
      });
      
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
  
  // decider injections
  this.injectDeciderReplyHook = enable => {
    let prevFunc = TD.decider.updateFromBackend;
    
    TD.decider.updateFromBackend = function(data){
      data["simplified_replies"] = !enable;
      return prevFunc.apply(this, arguments);
    };
    
    TD.decider.updateForGuestId();
    this.$requiresReload = enable;
  };
  
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
    this.css.insert("#general_settings .divider-bar::after { display: inline-block; padding-top: 10px; line-height: 17px; content: 'Use the new | Edit layout & design | option in the Settings to modify TweetDeck theme, column width, font size, and other features.' }");
    
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
      this.css.insert(".is-favorite .tweet-action, .is-retweet .tweet-action { opacity: 0.5; visibility: visible !important }");
      this.css.insert(".tweet:hover .tweet-action, .is-favorite .tweet-action[rel='favorite'], .is-retweet .tweet-action[rel='retweet'] { opacity: 1; visibility: visible !important }");
    }
    
    if (this.config.moveTweetActionsToRight){
      this.css.insert(".tweet-actions { float: right !important; width: auto !important }");
      this.css.insert(".tweet-actions > li:nth-child(4) { margin-right: 2px !important }");
    }
    
    if (this.config.smallComposeTextSize){
      this.css.insert(".compose-text { font-size: 12px !important; height: 120px !important }");
    }
    
    if (!this.config.roundedScrollBars){
      this.css.insert(".scroll-styled-v:not(.antiscroll-inner)::-webkit-scrollbar { width: 8px }");
      this.css.insert(".scroll-styled-h:not(.antiscroll-inner)::-webkit-scrollbar { height: 8px }");
      this.css.insert(".scroll-styled-v::-webkit-scrollbar-thumb { border-radius: 0 }");
      this.css.insert(".scroll-styled-h::-webkit-scrollbar-thumb { border-radius: 0 }");
      this.css.insert(".antiscroll-scrollbar { border-radius: 0 }");
      this.css.insert(".antiscroll-scrollbar-vertical { margin-top: 0 }");
      this.css.insert(".antiscroll-scrollbar-horizontal { margin-left: 0 }");
      this.css.insert(".app-columns-container::-webkit-scrollbar { height: 9px !important }");
    }
    
    if (this.config.revertReplies){
      this.css.insert(".activity-header + .tweet .tweet-context { margin-left: -35px }");
      this.css.insert(".activity-header + .tweet .tweet-context .obj-left { margin-right: 5px }");
    }
    
    if (this.config.columnWidth[0] === '/'){
      let cols = this.config.columnWidth.slice(1);
      
      this.css.insert(".column { width: calc((100vw - 205px) / "+cols+" - 8px) !important }");
      this.css.insert(".is-condensed .column { width: calc((100vw - 55px) / "+cols+" - 8px) !important }");
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
    
    $TDP.injectIntoNotificationsBefore(this.$token, "css", "</head>", [
      "<style type='text/css'>",
      ".txt-base-smallest:not(.icon), .txt-base-largest:not(.icon) { font-size: "+this.config.fontSize+" !important }",
      ".avatar { border-radius: "+this.config.avatarRadius+"% !important }",
      (this.config.revertReplies ? ".activity-header + .tweet .tweet-context { margin-left: -35px } .activity-header + .tweet .tweet-context .obj-left { margin-right: 5px }" : ""),
      "</style>"
    ].join(""));
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
}

disabled(){
  if (this.css){
    this.css.remove();
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
  
  $("[data-action='settings-menu']").off("click", this.onSettingsMenuClickedEvent);
  $("#td-design-plugin-modal").remove();
}
