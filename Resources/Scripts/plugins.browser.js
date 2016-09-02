(function(){
  //
  // Class: Abstract plugin base class.
  //
  window.PluginBase = class{
    constructor(pluginSettings){
      this.$pluginSettings = pluginSettings || {};
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
      this.waiting = [];
    }
    
    isDisabled(plugin){
      return this.disabled.includes(plugin.id);
    }
    
    findObject(identifier){
      return this.installed.find(plugin => plugin.id === identifier);
    }
    
    install(plugin){
      this.installed.push(plugin);
      
      if (!this.isDisabled(plugin)){
        plugin.obj.enabled();
        this.runWhenReady(plugin);
      }
    }
    
    runWhenReady(plugin){
      if (window.TD_APP_READY){
        plugin.obj.ready();
      }
      else{
        this.waiting.push(plugin);
      }
    }
    
    setState(plugin, enable){
      if (enable && this.isDisabled(plugin)){
        this.disabled.splice(this.disabled.indexOf(plugin.id), 1);
        plugin.obj.enabled();
        this.runWhenReady(plugin);
      }
      else if (!enable && !this.isDisabled(plugin)){
        this.disabled.push(plugin.id);
        plugin.obj.disabled();
      }
      else return;

      if (plugin.obj.$pluginSettings.requiresPageReload){
        window.location.reload();
      }
    }
    
    onReady(){
      this.waiting.forEach(plugin => plugin.obj.ready());
      this.waiting = [];
    }
  };
  
  //
  // Block: Setup global function to change plugin state.
  //
  window.TDPF_setPluginState = function(identifier, enable){
    window.TD_PLUGINS.setState(window.TD_PLUGINS.findObject(identifier), enable);
  };
})();