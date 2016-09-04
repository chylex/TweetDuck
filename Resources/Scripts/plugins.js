(function($TDP){
  //
  // Block: Setup a simple JavaScript object configuration loader.
  //
  (function(){
    var continueLoading = function(token, identifier, fileName, onSuccess, onFailure){
      $TDP.readFile(token, fileName, true).then(contents => {
        var obj;

        try{
          obj = eval("("+contents+")");
        }catch(err){
          if (!(onFailure && onFailure(err.message))){
            alert("Problem loading '"+fileName+"' file for '"+identifier+"' plugin, the JavaScript syntax is invalid: "+err.message);
          }

          return;
        }

        onSuccess && onSuccess(obj);
      }).catch(err => {
        if (!(onFailure && onFailure(err))){
          alert("Problem loading '"+fileName+"' file for '"+identifier+"' plugin: "+err);
        }
      });
    };
    
    window.TDPF_loadConfigurationFile = function(pluginObject, fileNameUser, fileNameDefault, onSuccess, onFailure){
      var identifier = pluginObject.$id;
      var token = pluginObject.$token;

      $TDP.checkFileExists(token, fileNameUser).then(exists => {
        if (!exists){
          $TDP.readFile(token, fileNameDefault, true).then(contents => {
            $TDP.writeFile(token, fileNameUser, contents);
            continueLoading(token, identifier, fileNameUser, onSuccess, onFailure);
          }).catch(err => {
            if (!(onFailure && onFailure(err))){
              alert("Problem generating '"+fileNameUser+"' file for '"+identifier+"' plugin: "+err);
            }
          });
        }
        else{
          continueLoading(token, identifier, fileNameUser, onSuccess, onFailure);
        }
      }).catch(err => {
        if (!(onFailure && onFailure(err))){
          alert("Problem checking '"+fileNameUser+"' file for '"+identifier+"' plugin: "+err);
        }
      });
    };
  })();
})($TDP);