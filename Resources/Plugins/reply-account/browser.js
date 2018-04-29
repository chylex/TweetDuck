enabled(){
  var configuration = { defaultAccount: "#preferred" };
  
  window.TDPF_loadConfigurationFile(this, "configuration.js", "configuration.default.js", obj => configuration = obj);
  
  this.lastSelectedAccount = null;
  
  this.uiComposeTweetEvent = (e, data) => {
    return if !(data.type === "reply" || (data.type === "tweet" && "quotedTweet" in data)) || data.popFromInline || !("element" in data);
    
    var query;
    
    if (configuration.useAdvancedSelector){
      if (configuration.customSelector){
        let customSelectorDef = configuration.customSelector.toString();
        
        if (customSelectorDef.startsWith("function (column){")){
          $TD.alert("warning", "Plugin reply-account has invalid configuration: customSelector needs to be updated due to TweetDeck changes, please read the default configuration file for the updated guide");
          return;
        }
        else if (customSelectorDef.startsWith("function (type,")){
          $TD.alert("warning", "Plugin reply-account has invalid configuration: the type parameter is no longer present due to TweetDeck changes, please read the default configuration file for the updated guide");
          return;
        }
        
        var section = data.element.closest("section.js-column");
        
        var column = TD.controller.columnManager.get(section.attr("data-column"));
        var header = $(".column-title", section);
        var title = header.children(".column-head-title");
        
        var columnTitle, columnAccount;
        
        if (title.length){
          columnTitle = title.text();
          columnAccount = header.children(".attribution").text();
        }
        else{
          columnTitle = header.children(".column-title-edit-box").val();
          columnAccount = "";
        }
        
        try{
          query = configuration.customSelector(columnTitle, columnAccount, column, section.hasClass("column-temp"));
        }catch(e){
          $TD.alert("warning", "Plugin reply-account has invalid configuration: customSelector threw an error: "+e.message);
          return;
        }
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
        return if this.lastSelectedAccount === null;
        
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
  for(let event of [ "uiInlineComposeTweet", "uiDockedComposeTweet" ]){
    $(document).on(event, this.uiComposeTweetEvent);
    window.TDPF_prioritizeNewestEvent(document, event);
  }
  
  $(document).on("click", ".js-account-list .js-account-item", this.onSelectedAccountChanged);
}

disabled(){
  $(document).off("uiInlineComposeTweet", this.uiComposeTweetEvent);
  $(document).off("uiDockedComposeTweet", this.uiComposeTweetEvent);
  $(document).off("click", ".js-account-list .js-account-item", this.onSelectedAccountChanged);
}
