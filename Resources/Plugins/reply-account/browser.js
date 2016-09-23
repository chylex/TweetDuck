enabled(){
  var configuration = { defaultAccount: "#preferred" };
  
  window.TDPF_loadConfigurationFile(this, "configuration.js", "configuration.default.js", obj => configuration = obj);
  
  this.lastSelectedAccount = null;
  
  this.uiComposeTweetEvent = (e, data) => {
    if (data.type !== "reply"){
      return;
    }
    
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
    else if (query[0] !== '@' && query[0] !== '#'){
      $TD.alert("warning", "Plugin reply-account has invalid configuration: the requested account does not begin with @ or #: "+query);
      return;
    }
    
    var identifier = null;
    
    switch(query){
      case "#preferred":
        identifier = TD.storage.clientController.client.getDefaultAccount();
        break;
      
      case "#last":
        if (this.lastSelectedAccount === null){
          return;
        }
        
        identifier = this.lastSelectedAccount;
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
  
  this.onSelectedAccountChanged = () => {
    var selected = $(".js-account-item.is-selected", ".js-account-list");
    this.lastSelectedAccount = selected.length === 1 ? selected.attr("data-account-key") : null;
  };
}

ready(){
  var events = $._data(document, "events");
  
  for(var event of [ "uiInlineComposeTweet", "uiDockedComposeTweet" ]){
    $(document).on(event, this.uiComposeTweetEvent);
    
    var handlers = events[event];
    var newHandler = handlers[handlers.length-1];
    
    for(var index = handlers.length-1; index > 0; index--){
      handlers[index] = handlers[index-1];
    }
    
    handlers[0] = newHandler;
  }
  
  $(document).on("click", ".js-account-list .js-account-item", this.onSelectedAccountChanged);
}

disabled(){
  $(document).off("uiInlineComposeTweet", this.uiComposeTweetEvent);
  $(document).off("uiDockedComposeTweet", this.uiComposeTweetEvent);
  $(document).off("click", ".js-account-list .js-account-item", this.onSelectedAccountChanged);
}