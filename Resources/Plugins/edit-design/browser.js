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
    smallComposeTextSize: false,
    roundedScrollBars: false,
    optimizeAnimations: true,
    avatarRadius: 10
  };
  
  // modal dialog loading
  $TDP.readFileRoot(this.$token, "modal.html").then(contents => {
    this.htmlModal = contents;
  }).catch(err => {
    $TD.alert("error", "Problem loading data for the design edit plugin: "+err.message);
  });
  
  // configuration
  const configFile = "config.json";
  this.tmpConfig = null;
  
  var loadConfigObject = obj => {
    this.tmpConfig = obj || {};
    
    if (window.TD_APP_READY){
      this.onAppReady();
    }
  };
  
  this.onAppReady = () => {
    if (this.tmpConfig !== null){
      this.config = $.extend(this.defaultConfig, this.tmpConfig);
      this.tmpConfig = null;
      this.reinjectAll();
    }
  };
  
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
      }, 1);
    });
  }).methods({
    _render: () => $(this.htmlModal),
    destroy: function(){
      $("#td-design-plugin-modal").hide();
      this.supr();
    }
  });
  
  // css and layout injection
  this.resetDesign = function(){
    if (this.css){
      this.css.remove();
    }
    
    this.css = window.TDPF_createCustomStyle(this);
  };
  
  this.reinjectAll = function(){
    this.resetDesign();
    
    this.css.insert(".txt-base-smallest:not(.icon), .txt-base-largest:not(.icon) { font-size: "+this.config.fontSize+" !important }");
    this.css.insert(".avatar { border-radius: "+this.config.avatarRadius+"% !important }");
    
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
    }
    
    if (this.config.optimizeAnimations){
      this.css.insert(".app-content { will-change: transform }");
      this.css.insert(".column-holder { will-change: transform }");
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
  };
  
  this.uiShowActionsMenuEvent = () => {
    if (this.config.moveTweetActionsToRight){
      $(".js-dropdown.pos-r").toggleClass("pos-r pos-l");
    }
  };
}

ready(){
  // configuration
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
  
  this.onAppReady();
  
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
  
  $(document).off("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  
  $("[data-action='settings-menu']").off("click", this.onSettingsMenuClickedEvent);
  $("#td-design-plugin-modal").remove();
}
