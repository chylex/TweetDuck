(function(){
  if (!("$TDP" in window)){
    console.error("Missing $TDP");
  }
  
  const validatePluginObject = function(pluginObject){
    if (!("$token" in pluginObject)){
      throw "Invalid plugin object.";
    }
  };
  
  //
  // Block: Setup a simple JavaScript object configuration loader.
  //
  window.TDPF_loadConfigurationFile = function(pluginObject, fileNameUser, fileNameDefault, onSuccess, onFailure){
    validatePluginObject(pluginObject);
    
    let identifier = pluginObject.$id;
    let token = pluginObject.$token;
    
    $TDP.checkFileExists(token, fileNameUser).then(exists => {
      let fileName = exists ? fileNameUser : fileNameDefault;
      
      if (fileName === null){
        onSuccess && onSuccess({});
        return;
      }
      
      (exists ? $TDP.readFile(token, fileName, true) : $TDP.readFileRoot(token, fileName)).then(contents => {
        let obj;
        
        try{
          obj = eval("(" + contents + ")");
        }catch(err){
          if (!(onFailure && onFailure(err))){
            $TD.alert("warning", "Problem loading '" + fileName + "' file for '" + identifier + "' plugin, the JavaScript syntax is invalid: " + err.message);
          }
          
          return;
        }
        
        onSuccess && onSuccess(obj);
      }).catch(err => {
        if (!(onFailure && onFailure(err))){
          $TD.alert("warning", "Problem loading '" + fileName + "' file for '" + identifier + "' plugin: " + err.message);
        }
      });
    }).catch(err => {
      if (!(onFailure && onFailure(err))){
        $TD.alert("warning", "Problem checking '" + fileNameUser + "' file for '" + identifier + "' plugin: " + err.message);
      }
    });
  };
  
  //
  // Block: Setup a function to add/remove custom CSS.
  //
  window.TDPF_createCustomStyle = function(pluginObject){
    validatePluginObject(pluginObject);
    
    let element = document.createElement("style");
    element.id = "plugin-" + pluginObject.$id + "-"+Math.random().toString(36).substring(2, 7);
    document.head.appendChild(element);
    
    return {
      insert: (rule) => element.sheet.insertRule(rule, 0),
      remove: () => element.remove(),
      element: element
    };
  };
  
  //
  // Block: Setup a function to mimic a Storage object that will be saved in the plugin.
  //
  window.TDPF_createStorage = function(pluginObject, onReady){
    validatePluginObject(pluginObject);
    
    if ("$storage" in pluginObject){
      if (pluginObject.$storage !== null){ // set to null while the file is still loading
        onReady(pluginObject.$storage);
      }
      
      return;
    }
    
    class Storage{
      get length(){
        return Object.keys(this).length;
      }
      
      key(index){
        return Object.keys(this)[index];
      }
      
      getItem(key){
        return this[key] || null;
      }
      
      setItem(key, value){
        this[key] = value;
        updateFile();
      }
      
      removeItem(key){
        delete this[key];
        updateFile();
      }
      
      clear(){
        for(key of Object.keys(this)){
          delete this[key];
        }
        
        updateFile();
      }
      
      replace(obj, silent){
        for(let key of Object.keys(this)){
          delete this[key];
        }
        
        for(let key in obj){
          this[key] = obj[key];
        }
        
        if (!silent){
          updateFile();
        }
      }
    };
    
    var storage = new Proxy(new Storage(), {
      get: function(obj, prop, receiver){
        const value = obj[prop];
        return typeof value === "function" ? value.bind(obj) : value;
      },
      
      set: function(obj, prop, value){
        obj.setItem(prop, value);
        return true;
      },
      
      deleteProperty: function(obj, prop){
        obj.removeItem(prop);
        return true;
      },
      
      enumerate: function(obj){
        return Object.keys(obj);
      }
    });
    
    var delay = -1;
    
    const updateFile = function(){
      window.clearTimeout(delay);
      
      delay = window.setTimeout(function(){
        $TDP.writeFile(pluginObject.$token, ".storage", JSON.stringify(storage));
      }, 0);
    };
    
    pluginObject.$storage = null;
    
    window.TDPF_loadConfigurationFile(pluginObject, ".storage", null, function(obj){
      storage.replace(obj, true);
      onReady(pluginObject.$storage = storage);
    }, function(){
      onReady(pluginObject.$storage = storage);
    });
  };
})();
