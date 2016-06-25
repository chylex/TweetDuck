class PluginBase{
  constructor(pluginSettings){
    this.$pluginSettings = pluginSettings || {};
  }

  enabled(){}
  ready(){}
  disabled(){}
}

var PLUGINS = {
  installed: [],
  disabled: [],
  waiting: [],

  isDisabled: plugin => PLUGINS.disabled.includes(plugin.id),
  findObject: identifier => PLUGINS.installed.find(plugin => plugin.id === identifier),

  load: function(){
    PLUGINS.installed.forEach(plugin => {
      if (!PLUGINS.isDisabled(plugin)){
        plugin.obj.enabled();
        PLUGINS.runWhenReady(plugin);
      }
    });
  },

  onReady: function(){
    PLUGINS.waiting.forEach(plugin => plugin.obj.ready());
    PLUGINS.waiting = [];
  },

  runWhenReady: function(plugin){
    if (window.TD_APP_READY){
      plugin.obj.ready();
    }
    else{
      PLUGINS.waiting.push(plugin);
    }
  },

  setState: function(identifier, enable){
    var plugin = PLUGINS.findObject(identifier);

    if (enable && PLUGINS.isDisabled(plugin)){
      PLUGINS.disabled.splice(PLUGINS.disabled.indexOf(identifier),1);
      plugin.obj.enabled();
      PLUGINS.runWhenReady(plugin);
    }
    else if (!enable && !PLUGINS.isDisabled(plugin)){
      PLUGINS.disabled.push(identifier);
      plugin.obj.disabled();
    }
    else return;

    if (plugin.obj.$pluginSettings.requiresPageReload){
      window.location.reload();
    }
  }
};

window.TD_PLUGINS = PLUGINS;