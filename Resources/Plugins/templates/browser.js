enabled(){
  let me = this;
  
  // configuration
  
  this.config = {
    templates: {} // identifier: { name, contents }
  };
  
  const configFile = "config.json";
  
  $TDP.checkFileExists(this.$token, configFile).then(exists => {
    if (!exists){
      $TDP.writeFile(this.$token, configFile, JSON.stringify(this.config));
    }
    else{
      $TDP.readFile(this.$token, configFile, true).then(contents => {
        try{
          $.extend(true, this.config, JSON.parse(contents));
        }catch(err){
          // why :(
        }
      }).catch(err => {
        $TD.alert("error", "Problem loading configuration for the template plugin: "+err.message);
      });
    }
  });
  
  this.saveConfig = () => {
    $TDP.writeFile(this.$token, configFile, JSON.stringify(this.config)).catch(err => {
      $TD.alert("error", "Problem saving configuration for the template plugin: "+err.message);
    });
  };
  
  // setup
  
  this.htmlModal = null;
  
  $TDP.readFileRoot(this.$token, "modal.html").then(contents => {
    this.htmlModal = contents;
  }).catch(err => {
    $TD.alert("error", "Problem loading data for the template plugin: "+err.message);
  });
  
  // button
  
  let buttonHTML = '<button class="manage-templates-btn needsclick btn btn-on-blue full-width txt-left margin-b--12 padding-v--6 padding-h--12"><i class="icon icon-bookmark"></i><span class="label padding-ls">Manage templates</span></button>';
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  window.TDPF_injectMustache("compose/docked_compose.mustache", "prepend", '<div class="js-tweet-type-button">', buttonHTML);
  
  let dockedComposePanel = $(".js-docked-compose");
  
  if (dockedComposePanel.length){
    dockedComposePanel.find(".js-tweet-type-button").first().before(buttonHTML);
  }
  
  // template implementation
  
  const readTemplateTokens = (contents, tokenData) => {
    let startIndex = -1;
    let endIndex = -1;
    
    let data = [];
    let tokenNames = Object.keys(tokenData);
    
    for(let currentIndex = 0; currentIndex < contents.length; currentIndex++){
      if (contents[currentIndex] === '\\'){
        contents = contents.substring(0, currentIndex)+contents.substring(currentIndex+1);
        continue;
      }
      else if (contents[currentIndex] !== '{'){
        continue;
      }
      
      startIndex = currentIndex+1;
      
      for(; startIndex < contents.length; startIndex++){
        if (!tokenNames.some(name => contents[startIndex] === name[startIndex-currentIndex-1])){
          break;
        }
      }
      
      endIndex = startIndex;
      
      let token = contents.substring(currentIndex+1, startIndex);
      let replacement = tokenData[token] || "";
      
      let entry = [ token, currentIndex ];
      
      if (contents[endIndex] === '#'){
        ++endIndex;
        
        let bracketCount = 1;
        
        for(; endIndex < contents.length; endIndex++){
          if (contents[endIndex] === '{'){
            ++bracketCount;
          }
          else if (contents[endIndex] === '}'){
            if (--bracketCount === 0){
              entry.push(contents.substring(startIndex+1, endIndex));
              break;
            }
          }
          else if (contents[endIndex] === '#'){
            entry.push(contents.substring(startIndex+1, endIndex));
            startIndex = endIndex;
          }
          else if (contents[endIndex] === '\\'){
            contents = contents.substring(0, endIndex)+contents.substring(endIndex+1);
          }
        }
      }
      else if (contents[endIndex] !== '}'){
        continue;
      }
      
      data.push(entry);
      
      contents = contents.substring(0, currentIndex)+replacement+contents.substring(endIndex+1);
      currentIndex += replacement.length;
    }
    
    return [ contents, data ];
  };
  
  const doAjaxRequest = (index, url, evaluator) => {
    return new Promise((resolve, reject) => {
      if (!url){
        resolve([ index, "{ajax}" ]);
        return;
      }
      
      $TD.makeGetRequest(url, function(data){
        if (evaluator){
          resolve([ index, eval(evaluator.replace(/\$/g, "'"+data.replace(/(["'\\\n\r\u2028\u2029])/g, "\\$1")+"'"))]);
        }
        else{
          resolve([ index, data ]);
        }
      }, function(err){
        resolve([ index, "" ]);
        $TD.alert("error", "Error executing AJAX request: "+err);
      });
    });
  };
  
  const useTemplate = (contents, append) => {
    let ele = $(".js-compose-text");
    if (ele.length === 0) {
      return;
    }
    
    let value = append ? ele.val()+contents : contents;
    let prevLength = value.length;
    
    let tokens = null;
    
    [value, tokens] = readTemplateTokens(value, {
      "cursor": "",
      "ajax": "(...)"
    });
    
    ele.val(value);
    ele.trigger("change");
    ele.focus();
    
    ele[0].selectionStart = ele[0].selectionEnd = value.length;
    
    let promises = [];
    let indexOffset = 0;
    
    for(let token of tokens){
      switch(token[0]){
        case "cursor":
          let [, index1, length ] = token;
          ele[0].selectionStart = index1;
          ele[0].selectionEnd = index1+(length | 0 || 0);
          break;
          
        case "ajax":
          let [, index2, evaluator, url ] = token;
          
          if (!url){
            url = evaluator;
            evaluator = null;
          }
          
          promises.push(doAjaxRequest(index2, url, evaluator));
          break;
      }
    }
    
    if (promises.length > 0){
      let selStart = ele[0].selectionStart;
      let selEnd = ele[0].selectionEnd;
      
      ele.prop("disabled", true);
      
      Promise.all(promises).then(values => {
        const placeholderLen = 5; // "(...)".length
        let indexOffset = 0;
        
        for(let value of values){
          let diff = value[1].length-placeholderLen;
          let realIndex = indexOffset+value[0];
          
          let val = ele.val();
          ele.val(val.substring(0, realIndex)+value[1]+val.substring(realIndex+placeholderLen));
          
          indexOffset += diff;
        }
        
        ele.prop("disabled", false);
        ele.trigger("change");
        ele.focus();
        
        ele[0].selectionStart = selStart+indexOffset;
        ele[0].selectionEnd = selEnd+indexOffset;
      });
    }
    
    if (!append){
      hideTemplateModal();
    }
  };
  
  // modal dialog
  
  this.editingTemplate = null;
  
  const showTemplateModal = () => {
    $(".js-app-content").prepend(this.htmlModal);
    
    /* TODO possibly implement this later
    
    <li>{paste}</li>
    <li>Paste text or an image from clipboard</li>
    <li>{paste#text}</li>
    <li>Paste only if clipboard has text</li>
    <li>{paste#image}</li>
    <li>Paste only if clipboard has an image</li>
    
    */
    
    let ele = $("#templates-modal-wrap").first();
    
    ele.on("click", "li[data-template]", function(e){
      let template = me.config.templates[$(this).attr("data-template")];
      useTemplate(template.contents, e.shiftKey);
    });
    
    ele.on("click", "li[data-template] i[data-action]", function(e){
      let identifier = $(this).closest("li").attr("data-template");
      
      switch($(this).attr("data-action")){
        case "edit-template":
          let editor = $("#template-editor");
          
          if (editor.hasClass("invisible")){
            toggleEditor();
          }
          
          let template = me.config.templates[identifier];
          $("[name='template-name']", editor).val(template.name);
          $("[name='template-contents']", editor).val(template.contents);
          
          me.editingTemplate = identifier;
          break;
          
        case "delete-template":
          delete me.config.templates[identifier];
          onTemplatesUpdated(true);
          
          if (me.editingTemplate === identifier){
            me.editingTemplate = null;
          }
          
          break;
      }
      
      e.stopPropagation();
    });
    
    ele.on("click", ".template-editor-tips-button", function(e){
      $(this).children(".icon").toggleClass("icon-arrow-d icon-arrow-u");
      ele.find(".template-editor-tips").toggle();
    });
    
    ele.on("click", "button", function(e){
      switch($(this).attr("data-action")){
        case "new-template":
        case "editor-cancel":
          toggleEditor();
          break;
          
        case "editor-confirm":
          let editor = $("#template-editor");
          
          if (me.editingTemplate !== null){
            delete me.config.templates[me.editingTemplate];
          }
          
          let name = $("[name='template-name']", editor).val();
          let identifier = name.toLowerCase().replace(/[^a-z0-9]/g, "")+"-"+(Math.random().toString(36).substring(2, 7));
          
          if (name.trim().length === 0){
            alert("Please, include a name for your template.");
            $("[name='template-name']", editor).focus();
            return;
          }
          
          me.config.templates[identifier] = {
            name: name,
            contents: $("[name='template-contents']", editor).val()
          };
          
          toggleEditor();
          onTemplatesUpdated(true);
          break;
        
        case "close":
          hideTemplateModal();
          break;
      }
      
      $(this).blur();
    });
    
    onTemplatesUpdated(false);
  };
  
  const hideTemplateModal = () => {
    $("#templates-modal-wrap").remove();
  };
  
  const toggleEditor = () => {
    let editor = $("#template-editor");
    $("[name]", editor).val("");
    
    if ($("button[data-action='new-template']", "#template-list").add(editor).toggleClass("invisible").hasClass("invisible")){
      me.editingTemplate = null;
    }
  };
  
  const onTemplatesUpdated = (save) => {
    let eles = [];
    
    for(let identifier of Object.keys(this.config.templates)){
      eles.push(`<li data-template="${identifier}">
<span class="template-name">${this.config.templates[identifier].name}</span>
<span class="template-actions"><i class="icon icon-edit" data-action="edit-template"></i><i class="icon icon-close" data-action="delete-template"></i></span>
</li>`);
    }
    
    if (eles.length === 0){
      eles.push("<li>No templates available</li>");
    }
    
    $("#template-list").children("ul").html(eles.join(""));
    
    if (save){
      this.saveConfig();
    }
  };
  
  // event handlers
  
  this.manageTemplatesButtonClickEvent = function(e){
    if ($("#templates-modal-wrap").length){
      hideTemplateModal();
    }
    else{
      showTemplateModal();
    }
    
    $(this).blur();
  };
  
  this.drawerToggleEvent = function(e, data){
    if (typeof data === "undefined" || data.activeDrawer !== "compose"){
      hideTemplateModal();
    }
  };
}

ready(){
  $(".js-drawer[data-drawer='compose']").on("click", ".manage-templates-btn", this.manageTemplatesButtonClickEvent);
  $(document).on("uiDrawerActive", this.drawerToggleEvent);
  $(document).on("click", ".js-new-composer-opt-in", this.drawerToggleEvent);
}

disabled(){
  $(".manage-templates-btn").remove();
  $("#templates-modal-wrap").remove();
  
  $(".js-drawer[data-drawer='compose']").off("click", ".manage-templates-btn", this.manageTemplatesButtonClickEvent);
  $(document).off("uiDrawerActive", this.drawerToggleEvent);
  $(document).off("click", ".js-new-composer-opt-in", this.drawerToggleEvent);
  
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
