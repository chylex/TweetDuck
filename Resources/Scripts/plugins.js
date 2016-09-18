(function($TDP){
  //
  // Block: Setup a simple JavaScript object configuration loader.
  //
  window.TDPF_loadConfigurationFile = function(pluginObject, fileNameUser, fileNameDefault, onSuccess, onFailure){
    var identifier = pluginObject.$id;
    var token = pluginObject.$token;

    $TDP.checkFileExists(token, fileNameUser).then(exists => {
      var fileName = exists ? fileNameUser : fileNameDefault;

      $TDP.readFile(token, fileName, true).then(contents => {
        var obj;

        try{
          obj = eval("("+contents+")");
        }catch(err){
          if (!(onFailure && onFailure(err.message))){
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
})($TDP);