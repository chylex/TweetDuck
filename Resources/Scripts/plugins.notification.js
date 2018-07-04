//
// Class: Abstract plugin base class.
//
window.PluginBase = class{
  constructor(){}
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

#import "scripts/plugins.base.js"
