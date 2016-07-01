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
  window.TD_PLUGINS = {
    install: function(plugin){
      plugin.obj.run();
    }
  };
})();