(function($TDP){
  //
  // Block: Setup a simple JavaScript object configuration loader.
  //
  window.TDPF_loadConfigurationFile = function(pluginObject, fileNameUser, fileNameDefault, onSuccess, onFailure){
    var identifier = pluginObject.$id;
    var token = pluginObject.$token;
    
    $TDP.checkFileExists(token, fileNameUser).then(exists => {
      var fileName = exists ? fileNameUser : fileNameDefault;
      
      (exists ? $TDP.readFile(token, fileName, true) : $TDP.readFileRoot(token, fileName)).then(contents => {
        var obj;
        
        try{
          obj = eval("("+contents+")");
        }catch(err){
          if (!(onFailure && onFailure(err))){
            $TD.alert("warning", "Problem loading '"+fileName+"' file for '"+identifier+"' plugin, the JavaScript syntax is invalid: "+err.message);
          }
          
          return;
        }
        
        onSuccess && onSuccess(obj);
      }).catch(err => {
        if (!(onFailure && onFailure(err))){
          $TD.alert("warning", "Problem loading '"+fileName+"' file for '"+identifier+"' plugin: "+err.message);
        }
      });
    }).catch(err => {
      if (!(onFailure && onFailure(err))){
        $TD.alert("warning", "Problem checking '"+fileNameUser+"' file for '"+identifier+"' plugin: "+err.message);
      }
    });
  };
  
  //
  // Block: Setup a function to add/remove custom CSS.
  //
  window.TDPF_createCustomStyle = function(pluginObject){
    var element = document.createElement("style");
    element.id = "plugin-"+pluginObject.$id+"-"+Math.random().toString(36).substring(2, 7);
    document.head.appendChild(element);
    
    return {
      insert: (rule) => element.sheet.insertRule(rule, 0),
      remove: () => element.remove(),
      element: element
    };
  };
})($TDP);
