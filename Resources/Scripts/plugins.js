(function($TDP){
  //
  // Block: Setup a simple JavaScript object configuration loader.
  //
  window.TDPF_loadConfigurationFile = function(pluginObject, fileName, onSuccess, onFailure){
    $TDP.readFile(pluginObject.$token, fileName, true).then(contents => {
      var obj;

      try{
        obj = eval("("+contents+")");
      }catch(err){
        if (!(onFailure && onFailure(err.message))){
          alert("Problem loading '"+fileName+"' file for '"+pluginObject.$id+"' plugin, the JavaScript syntax is invalid: "+err.message);
        }

        return;
      }

      onSuccess && onSuccess(obj);
    }).catch(err => {
      if (!(onFailure && onFailure(err))){
        alert("Problem loading '"+fileName+"' file for '"+pluginObject.$id+"' plugin: "+err);
      }
    });
  };
})($TDP);