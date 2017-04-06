enabled(){
  this.selectedSkinTone = "";
  
  this.skinToneList = [
    "", "1F3FB", "1F3FC", "1F3FD", "1F3FE", "1F3FF"
  ];
  
  this.skinToneNonDefaultList = [
    "1F3FB", "1F3FC", "1F3FD", "1F3FE", "1F3FF"
  ];
  
  this.skinToneData = [
    [ "", "#FFDD67" ],
    [ "1F3FB", "#FFE1BD" ],
    [ "1F3FC", "#FED0AC" ],
    [ "1F3FD", "#D6A57C" ],
    [ "1F3FE", "#B47D56" ],
    [ "1F3FF", "#8A6859" ],
  ];
  
  this.emojiHTML1 = ""; // no skin tones, prepended
  this.emojiHTML2 = {}; // contains emojis with skin tones
  this.emojiHTML3 = ""; // no skin tones, appended
  
  var me = this;
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 15.35em; background-color: white; border-radius: 2px 2px 3px 3px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard-list { height: 10.14em; padding: 0.1em; box-sizing: border-box; overflow-y: auto }");
  this.css.insert(".emoji-keyboard-list .separator { height: 26px }");
  this.css.insert(".emoji-keyboard-list .emoji { padding: 0.1em !important; cursor: pointer }");
  this.css.insert(".emoji-keyboard-skintones { height: 1.3em; text-align: center; background-color: #292f33; border-radius: 0 0 2px 2px }");
  this.css.insert(".emoji-keyboard-skintones div { width: 0.8em; height: 0.8em; margin: 0.25em 0.1em; border-radius: 50%; display: inline-block; box-sizing: border-box; cursor: pointer }");
  this.css.insert(".emoji-keyboard-skintones .sel { border: 2px solid rgba(0, 0, 0, 0.35); box-shadow: 0 0 2px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.4) inset }");
  this.css.insert(".emoji-keyboard-skintones :hover { border: 2px solid rgba(0, 0, 0, 0.25); box-shadow: 0 0 1px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.25) inset }");
  
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
    $(".js-compose-text").first().focus();
  };
  
  var generateEmojiHTML = skinTone => {
    return this.emojiHTML1+this.emojiHTML2[skinTone]+this.emojiHTML3;
  };
  
  var selectSkinTone = skinTone => {
    let selectedEle = this.currentKeyboard.children[1].querySelector("[data-tone='"+this.selectedSkinTone+"']");
    selectedEle && selectedEle.classList.remove("sel");
    
    this.selectedSkinTone = skinTone;
    this.currentKeyboard.children[0].innerHTML = generateEmojiHTML(skinTone);
    this.currentKeyboard.children[1].querySelector("[data-tone='"+this.selectedSkinTone+"']").classList.add("sel");
  };
  
  this.generateKeyboard = (input, left, top) => {
    var outer = document.createElement("div");
    outer.classList.add("emoji-keyboard");
    outer.style.left = left+"px";
    outer.style.top = top+"px";
    
    var keyboard = document.createElement("div");
    keyboard.classList.add("emoji-keyboard-list");
    
    keyboard.addEventListener("click", function(e){
      if (e.target.tagName === "IMG"){
        var val = input.val();
        var inserted = e.target.getAttribute("alt");
        var posStart = input[0].selectionStart;
        var posEnd = input[0].selectionEnd;
        
        input.val(val.slice(0, posStart)+inserted+val.slice(posStart));
        input.trigger("change");
        input.focus();
        
        input[0].selectionStart = posStart+inserted.length;
        input[0].selectionEnd = posEnd+inserted.length;
      }
      
      e.stopPropagation();
    });
    
    var skintones = document.createElement("div");
    skintones.innerHTML = me.skinToneData.map(entry => "<div data-tone='"+entry[0]+"' style='background-color:"+entry[1]+"'></div>").join("");
    skintones.classList.add("emoji-keyboard-skintones");
    
    skintones.addEventListener("click", function(e){
      if (e.target.hasAttribute("data-tone")){
        selectSkinTone(e.target.getAttribute("data-tone") || "");
      }
      
      e.stopPropagation();
    });
    
    outer.appendChild(keyboard);
    outer.appendChild(skintones);
    document.body.appendChild(outer);
    
    this.currentKeyboard = outer;
    selectSkinTone(this.selectedSkinTone);
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
      me.generateKeyboard($(".js-compose-text").first(), pos.left, pos.top+$(this).outerHeight()+8);
      
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
    let generated1 = [];
    let generated2 = {};
    let generated3 = [];
    
    for(let skinTone of this.skinToneList){
      generated2[skinTone] = [];
    }
    
    // declaration inserters
    
    let addDeclaration1 = decl => {
      generated1.push(decl.split(" ").map(pt => convUnicode(parseInt(pt, 16))).join(""));
    };
    
    let addDeclaration2 = (tone, decl) => {
      let gen = decl.split(" ").map(pt => convUnicode(parseInt(pt, 16))).join("");
      
      if (tone === null){
        for(let skinTone of this.skinToneList){
          generated2[skinTone].push(gen);
        }
      }
      else{
        generated2[tone].push(gen);
      }
    };
    
    let addDeclaration3 = decl => {
      generated3.push(decl.split(" ").map(pt => convUnicode(parseInt(pt, 16))).join(""));
    };
    
    // line reading
                      
    let skinToneState = 0;
    
    for(let line of contents.split("\n")){
      if (line[0] === '@'){
        switch(skinToneState){
          case 0: generated1.push("___"); break;
          case 1: this.skinToneList.forEach(skinTone => generated2[skinTone].push("___")); break;
          case 2: generated3.push("___"); break;
        }
        
        if (line[1] === '1'){
          skinToneState = 1;
        }
        else if (line[1] === '2'){
          skinToneState = 2;
        }
      }
      else if (skinToneState === 1){
        let decl = line.slice(0, line.indexOf(';'));
        let skinIndex = decl.indexOf('$');

        if (skinIndex !== -1){
          let declPre = decl.slice(0, skinIndex);
          let declPost = decl.slice(skinIndex+1);
          
          for(let skinTone of this.skinToneNonDefaultList){
            generated2[skinTone].pop();
            addDeclaration2(skinTone, declPre+skinTone+declPost);
          }
        }
        else{
          addDeclaration2(null, decl);
        }
      }
      else if (skinToneState === 2){
        addDeclaration3(line.slice(0, line.indexOf(';')));
      }
      else if (skinToneState === 0){
        addDeclaration1(line.slice(0, line.indexOf(';')));
      }
    }
    
    // final processing
    
    let replaceSeparators = str => str.replace(/___/g, "<div class='separator'></div>");
    
    let start = "<p style='font-size:13px;color:#444;margin:4px;text-align:center'>Please, note that most emoji will not show up properly in the text box above, but they will display in the tweet.</p>";
    this.emojiHTML1 = start+replaceSeparators(TD.util.cleanWithEmoji(generated1.join("")));
    
    for(let skinTone of this.skinToneList){
      this.emojiHTML2[skinTone] = replaceSeparators(TD.util.cleanWithEmoji(generated2[skinTone].join("")));
    }
    
    this.emojiHTML3 = replaceSeparators(TD.util.cleanWithEmoji(generated3.join("")));
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
