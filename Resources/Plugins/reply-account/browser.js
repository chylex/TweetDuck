enabled(){
  var configuration = { defaultAccount: "#preferred" };
  
  window.TDPF_loadConfigurationFile(this, "configuration.js", "configuration.default.js", obj => configuration = obj);
  
  this.uiInlineComposeTweetEvent = function(e, data){
    var query;
    
    if (configuration.useAdvancedSelector){
      if (configuration.customSelector){
        var column = TD.controller.columnManager.get(data.element.closest("section.column").attr("data-column"));
        query = configuration.customSelector(column);
      }
      else{
        $TD.alert("warning", "Plugin reply-account has invalid configuration: useAdvancedSelector is true, but customSelector function is missing");
        return;
      }
    }
    else{
      query = configuration.defaultAccount;
      
      if (query === ""){
        query = "#preferred";
      }
      else if (typeof query !== "string"){
        query = "#default";
      }
    }
    
    if (typeof query === "undefined"){
      query = "#preferred";
    }
    
    if (typeof query !== "string"){
      return;
    }
    else if (query.length === 0){
      $TD.alert("warning", "Plugin reply-account has invalid configuration: the requested account is empty");
      return;
    }
    else if (query[0] !== '@' && query[0] !== '&'){
      $TD.alert("warning", "Plugin reply-account has invalid configuration: the requested account does not begin with @ or #: "+query);
      return;
    }
    
    var identifier = null;
    
    switch(query){
      case "#preferred":
        identifier = TD.storage.clientController.client.getDefaultAccount();
        break;
      
      case "#last":
        // TODO
        break;
      
      case "#default":
        return;
      
      default:
        if (query[0] === '@'){
          var obj = TD.storage.accountController.getAccountFromUsername(query.substring(1));
          
          if (obj.length === 0){
            $TD.alert("warning", "Plugin reply-account has invalid configuration: requested account not found: "+query);
            return;
          }
          else{
            identifier = obj[0].privateState.key;
          }
        }
        else{
          $TD.alert("warning", "Plugin reply-account has invalid configuration: unknown requested account query: "+query);
          return;
        }
    }
    
    data.singleFrom = data.from = [ identifier ];
  };
}

ready(){
  var events = $._data(document, "events");
  
  if ("uiInlineComposeTweet" in events){
    $(document).on("uiInlineComposeTweet", this.uiInlineComposeTweetEvent);
    
    var handlers = events["uiInlineComposeTweet"];
    var oldHandler = handlers[0];
    var newHandler = handlers[1];
    
    handlers[0] = newHandler;
    handlers[1] = oldHandler;
  }
  else{
    $(document).on("uiInlineComposeTweet", this.uiInlineComposeTweetEvent);
  }
}

disabled(){
  $(document).off("uiInlineComposeTweet", this.uiInlineComposeTweetEvent);
}