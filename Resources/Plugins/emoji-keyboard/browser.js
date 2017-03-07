enabled(){  
  this.emojiHTML = "";
  
  // styles and layout
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 15.35em; height: 11.2em; background-color: white; overflow-y: auto; padding: 0.1em; box-sizing: border-box; border-radius: 2px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard .separator { height: 26px; }");
  this.css.insert(".emoji-keyboard .emoji { padding: 0.1em !important; cursor: pointer }");
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  TD.mustaches["compose/docked_compose.mustache"] = TD.mustaches["compose/docked_compose.mustache"].replace('<div class="cf margin-t--12 margin-b--30">', '<div class="cf margin-t--12 margin-b--30"><button class="needsclick btn btn-on-blue txt-left margin-b--12 padding-v--9 emoji-keyboard-popup-btn"><i class="icon icon-heart"></i></button>');
  
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
  
  // event handlers
  
  var me = this;
  
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
    let generated = [
      "<p style='font-size:13px;color:#444;margin:4px;text-align:center'>Please, note that most emoji will not show up properly in the text box above, but they will display in the tweet.</p>"
    ];
    
    let lines = contents.split("\n");
    
    for(let line of lines){
      if (line[0] === '#'){
        continue;
      }
      else if (line[0] === '@'){
        generated.push("<div class='separator'></div>");
        continue;
      }
      
      let decl = line.substring(0, line.indexOf(";"));
      let emoji = decl.split(" ").map(pt => convUnicode(parseInt(pt, 16))).join("");
      
      generated.push(TD.util.cleanWithEmoji(emoji));
    }
    
    this.emojiHTML = generated.join("");
  }).catch(err => {
    $TD.alert("error", "Problem loading emoji keyboard: "+err.message);
  });
}

disabled(){
  this.css.remove();
  
  $(".emoji-keyboard-popup-btn").off("click", this.emojiKeyboardButtonClickEvent);
  $(document).off("click", this.documentClickEvent);
  $(document).off("keydown", this.documentKeyEvent);
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
