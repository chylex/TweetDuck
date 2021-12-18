(function(){
  //
  // Class: Abstract plugin base class.
  //
  window.PluginBase = class{
    constructor(pluginSettings){
      this.$requiresReload = !!(pluginSettings && pluginSettings.requiresPageReload);
    }
    
    enabled(){}
    ready(){}
    disabled(){}
  };
  
  //
  // Variable: Main object for containing and managing plugins.
  //
  window.TD_PLUGINS = new class{
    constructor(){
      this.installed = [];
      this.disabled = [];
      this.waitingForFeatures = [];
      this.waitingForReady = [];
      this.areFeaturesLoaded = false;
    }
    
    isDisabled(plugin){
      return this.disabled.includes(plugin.id);
    }
    
    findObject(identifier){
      return this.installed.find(plugin => plugin.id === identifier);
    }
    
    install(plugin){
      this.installed.push(plugin);
      
      if (typeof plugin.obj.configure === "function"){
        $TDP.setConfigurable(plugin.obj.$token);
      }
      
      if (!this.isDisabled(plugin)){
        this.runWhenFeaturesLoaded(plugin);
        this.runWhenReady(plugin);
      }
    }
    
    runWhenFeaturesLoaded(plugin){
      if (this.areFeaturesLoaded){
        plugin.obj.enabled();
      }
      else{
        this.waitingForFeatures.push(plugin);
      }
    }
    
    runWhenReady(plugin){
      if (TD.ready){
        plugin.obj.ready();
      }
      else{
        this.waitingForReady.push(plugin);
      }
    }
    
    setState(plugin, enable){
      let reloading = plugin.obj.$requiresReload;
      
      if (enable && this.isDisabled(plugin)){
        if (reloading){
          window.TDPF_requestReload();
        }
        else{
          this.disabled.splice(this.disabled.indexOf(plugin.id), 1);
          plugin.obj.enabled();
          this.runWhenReady(plugin);
        }
      }
      else if (!enable && !this.isDisabled(plugin)){
        if (reloading){
          window.TDPF_requestReload();
        }
        else{
          this.disabled.push(plugin.id);
          plugin.obj.disabled();
          
          for(let key of Object.keys(plugin.obj)){
            if (key[0] !== '$'){
              delete plugin.obj[key];
            }
          }
        }
      }
    }
    
    onFeaturesLoaded(){
      window.TDPF_getColumnName = window.TDGF_getColumnName;
      window.TDPF_reloadColumns = window.TDGF_reloadColumns;
      window.TDPF_prioritizeNewestEvent = window.TDGF_prioritizeNewestEvent;
      window.TDPF_injectMustache = window.TDGF_injectMustache;
      window.TDPF_registerPropertyUpdateCallback = window.TDGF_registerPropertyUpdateCallback;
      window.TDPF_playVideo = function(urlOrObject, username){
        if (typeof urlOrObject === "string"){
          window.TDGF_playVideo(urlOrObject, null, username);
        }
        else{
          window.TDGF_playVideo(urlOrObject.videoUrl, urlOrObject.tweetUrl, urlOrObject.username);
        }
      };
      
      this.waitingForFeatures.forEach(plugin => plugin.obj.enabled());
      this.waitingForFeatures = [];
      this.areFeaturesLoaded = true;
    }
    
    onReady(){
      this.waitingForReady.forEach(plugin => plugin.obj.ready());
      this.waitingForReady = [];
    }
  };
  
  //
  // Block: Setup a function to change plugin state.
  //
  window.TDPF_setPluginState = function(identifier, enable){
    window.TD_PLUGINS.setState(window.TD_PLUGINS.findObject(identifier), enable);
  };
  
  //
  // Block: Setup a function to trigger plugin configuration.
  //
  window.TDPF_configurePlugin = function(identifier){
    window.TD_PLUGINS.findObject(identifier).obj.configure();
  };
  
  //
  // Block: Setup a function to reload the page.
  //
  (function(){
    let isReloading = false;
    
    window.TDPF_requestReload = function(){
      if (!isReloading){
        window.setTimeout(window.TDGF_reload, 1);
        isReloading = true;
      }
    };
  })();
  
  #import "scripts/plugins.base.js"
})();
