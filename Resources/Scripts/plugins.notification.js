(function(){
  //
  // Class: Abstract plugin base class.
  //
  window.PluginBase = class{
    constructor(pluginSettings){
      this.$pluginSettings = pluginSettings || {};
    }

    run(){}
  };
  
  //
  // Variable: Main object for containing and managing plugins.
  //
  window.TD_PLUGINS = new class{
    constructor(){
      this.installed = [];
      this.disabled = [];
    }
    
    isDisabled(plugin){
      return this.disabled.includes(plugin.id);
    }
    
    install(plugin){
      this.installed.push(plugin);
      
      if (!this.isDisabled(plugin)){
        plugin.obj.run();
      }
    }
  };
})();