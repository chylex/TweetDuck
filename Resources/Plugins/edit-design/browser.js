enabled(){
  // elements & data
  this.css = null;
  this.htmlModal = null;
  this.config = null;
  
  this.defaultConfig = {
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
  
  var loadConfigObject = obj => {
    this.config = obj;
    this.reinjectAll();
  };
  
  $TDP.checkFileExists(this.$token, configFile).then(exists => {
    if (!exists){
      loadConfigObject(this.defaultConfig);
    }
    else{
      $TDP.readFile(this.$token, configFile, true).then(contents => {
        try{
          loadConfigObject($.extend(this.defaultConfig, JSON.parse(contents)));
        }catch(err){
          loadConfigObject(this.defaultConfig);
        }
      }).catch(err => {
        loadConfigObject(this.defaultConfig);
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
  
  var customDesignModal = TD.components.BaseModal.extend(function(){
    let modal = $("#td-design-plugin-modal");
    this.setAndShowContainer(modal, false);
    
    modal.find("[data-td-key]").each(function(){
      let item = $(this);
      let key = item.attr("data-td-key");
      let value = item.attr("data-td-value");
      
      if (value == me.config[key]){
        item.addClass("selected");
      }
      
      item.click(function(){ console.profile("aaa");
        modal.find("[data-td-key='"+key+"']").removeClass("selected");
        item.addClass("selected");
        
        me.config[key] = value;
        
        setTimeout(function(){
          me.saveConfig();
          me.reinjectAll();
        }, 1); // delays the slight lag caused by saving and reinjection
      });
    });
  }).methods({
    _render: () => $(this.htmlModal),
    destroy: function(){
      $("#td-design-plugin-modal").hide();
      this.supr();
    }
  });
  
  // css and layout injection
  this.resetLayout = function(){
  };
  
  this.resetDesign = function(){
    if (this.css){
      this.css.remove();
    }
    
    this.css = window.TDPF_createCustomStyle(this);
  };
  
  this.reinjectAll = function(){
    this.resetLayout();
    this.resetDesign();
    
    // TODO
  };
}

ready(){
  $("[data-action='settings-menu']").on("click", this.onSettingsMenuClickedEvent);
  $(".js-app").append('<div id="td-design-plugin-modal" class="js-modal settings-modal ovl scroll-v scroll-styled-v"></div>');
}

disabled(){
  this.resetLayout();
  
  if (this.css){
    this.css.remove();
  }
  
  $("[data-action='settings-menu']").off("click", this.onSettingsMenuClickedEvent);
  $("#td-design-plugin-modal").remove();
}
