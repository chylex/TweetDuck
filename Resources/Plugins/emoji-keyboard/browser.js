enabled(){  
  this.emojiHTML = "";
  
  var me = this;
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 15.35em; height: 11.2em; background-color: white; overflow-y: auto; padding: 0.1em; box-sizing: border-box; border-radius: 2px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard .separator { height: 26px; }");
  this.css.insert(".emoji-keyboard .emoji { padding: 0.1em !important; cursor: pointer }");
  
  // layout
  
  var buttonHTML = '<button class="needsclick btn btn-on-blue txt-left padding-v--9 emoji-keyboard-popup-btn"><i class="icon icon-heart"></i></button>';
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  TD.mustaches["compose/docked_compose.mustache"] = TD.mustaches["compose/docked_compose.mustache"].replace('<div class="cf margin-t--12 margin-b--30">', '<div class="cf margin-t--12 margin-b--30">'+buttonHTML);
  
  var dockedComposePanel = $(".js-docked-compose");
  
  if (dockedComposePanel.length){
    dockedComposePanel.find(".cf.margin-t--12.margin-b--30").first().append(buttonHTML);
  }
  
  // keyboard generation
  
  this.currentKeyboard = null;
  
  var hideKeyboard = () => {
    $(this.currentKeyboard).remove();
    this.currentKeyboard = null;
    
    $(".emoji-keyboard-popup-btn").removeClass("is-selected");
  };
  
  this.generateKeyboardFor = (input, left, top) => {
    var created = document.createElement("div");
    document.body.appendChild(created);
    
    created.classList.add("emoji-keyboard");
    created.style.left = left+"px";
    created.style.top = top+"px";
    created.innerHTML = this.emojiHTML;
    
    created.addEventListener("click", function(e){
      if (e.target.tagName === "IMG"){
        input.val(input.val()+e.target.getAttribute("alt"));
        input.trigger("change");
        input.focus();
      }
      
      e.stopPropagation();
    });
    
    return created;
  };
  
  this.prevTryPasteImage = window.TDGF_tryPasteImage;
  var prevTryPasteImageF = this.prevTryPasteImage;
  
  window.TDGF_tryPasteImage = function(){
    if (me.currentKeyboard){
      hideKeyboard();
    }
    
    return prevTryPasteImageF.apply(this, arguments);
  };
  
  // event handlers
  
  this.emojiKeyboardButtonClickEvent = function(e){
    if (me.currentKeyboard){
      hideKeyboard();
    }
    else{
      var pos = $(this).offset();
      me.currentKeyboard = me.generateKeyboardFor($(".js-compose-text").first(), pos.left, pos.top+$(this).outerHeight()+8);
      
      $(this).addClass("is-selected");
    }
    
    $(this).blur();
    e.stopPropagation();
  };
  
  this.documentClickEvent = function(e){
    if (me.currentKeyboard && !e.target.classList.contains("js-compose-text")){
      hideKeyboard();
    }
  };
  
  this.documentKeyEvent = function(e){
    if (me.currentKeyboard && e.keyCode === 27){ // escape
      hideKeyboard();
      e.stopPropagation();
    }
  };
  
  /*
   * TODO
   * ----
   * add emoji search if I can be bothered
   * lazy emoji loading
   */
}

ready(){
  $(".emoji-keyboard-popup-btn").on("click", this.emojiKeyboardButtonClickEvent);
  $(document).on("click", this.documentClickEvent);
  $(document).on("keydown", this.documentKeyEvent);
  
  // HTML generation
  
  var convUnicode = function(codePt){
    if (codePt > 0xFFFF){
      codePt -= 0x10000;
      return String.fromCharCode(0xD800+(codePt>>10), 0xDC00+(codePt&0x3FF));
    }
    else{
      return String.fromCharCode(codePt);
    }
  };
  
  $TDP.readFileRoot(this.$token, "emoji-ordering.txt").then(contents => {
    let generated = [];
    
    let addDeclaration = decl => {
      generated.push(decl.split(" ").map(pt => convUnicode(parseInt(pt, 16))).join(""));
    };
    
    let skinTones = [
      "1F3FB", "1F3FC", "1F3FD", "1F3FE", "1F3FF"
    ];
    
    for(let line of contents.split("\n")){
      if (line[0] === '@'){
        generated.push("___");
      }
      else{
        let decl = line.slice(0, line.indexOf(";"));
        let skinIndex = decl.indexOf('$');

        if (skinIndex !== -1){
          let declPre = decl.slice(0, skinIndex);
          let declPost = decl.slice(skinIndex+1);

          skinTones.map(skinTone => declPre+skinTone+declPost).forEach(addDeclaration);
        }
        else{
          addDeclaration(decl);
        }
      }
    }
    
    let start = "<p style='font-size:13px;color:#444;margin:4px;text-align:center'>Please, note that most emoji will not show up properly in the text box above, but they will display in the tweet.</p>";
    this.emojiHTML = start+TD.util.cleanWithEmoji(generated.join("")).replace("___", "<div class='separator'></div>");
  }).catch(err => {
    $TD.alert("error", "Problem loading emoji keyboard: "+err.message);
  });
}

disabled(){
  this.css.remove();
  
  if (this.currentKeyboard){
    $(this.currentKeyboard).remove();
  }
  
  window.TDGF_tryPasteImage = this.prevTryPasteImage;
  
  $(".emoji-keyboard-popup-btn").off("click", this.emojiKeyboardButtonClickEvent);
  $(".emoji-keyboard-popup-btn").remove();
  
  $(document).off("click", this.documentClickEvent);
  $(document).off("keydown", this.documentKeyEvent);
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
