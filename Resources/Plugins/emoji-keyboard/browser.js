enabled(){
  this.selectedSkinTone = "";
  this.currentKeywords = [];
  
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
  
  this.emojiData1 = []; // no skin tones, prepended
  this.emojiData2 = {}; // contains emojis with skin tones
  this.emojiData3 = []; // no skin tones, appended
  this.emojiNames = [];
  
  var me = this;
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 15.35em; background-color: white; border-radius: 1px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard-list { height: 10.14em; padding: 0.1em; box-sizing: border-box; overflow-y: auto }");
  this.css.insert(".emoji-keyboard-list .separator { height: 26px }");
  this.css.insert(".emoji-keyboard-list img { padding: 0.1em !important; width: 1em; height: 1em; vertical-align: -0.1em; cursor: pointer }");
  this.css.insert(".emoji-keyboard-search { height: auto; padding: 4px 10px 8px; background-color: #292f33; border-radius: 1px 1px 0 0 }");
  this.css.insert(".emoji-keyboard-search input { width: 100%; border-radius: 1px; }");
  this.css.insert(".emoji-keyboard-skintones { height: 1.3em; text-align: center; background-color: #292f33; border-radius: 0 0 1px 1px }");
  this.css.insert(".emoji-keyboard-skintones div { width: 0.8em; height: 0.8em; margin: 0.25em 0.1em; border-radius: 50%; display: inline-block; box-sizing: border-box; cursor: pointer }");
  this.css.insert(".emoji-keyboard-skintones .sel { border: 2px solid rgba(0, 0, 0, 0.35); box-shadow: 0 0 2px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.4) inset }");
  this.css.insert(".emoji-keyboard-skintones :hover { border: 2px solid rgba(0, 0, 0, 0.25); box-shadow: 0 0 1px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.25) inset }");
  this.css.insert("#emoji-keyboard-tweet-input { padding: 2px 0 !important; line-height: 18px }");
  this.css.insert("#emoji-keyboard-tweet-input img { padding: 0.1em !important; width: 1em; height: 1em; vertical-align: -0.25em }");
  this.css.insert("#emoji-keyboard-tweet-input:empty::before { content: \"What's happening?\"; display: inline-block; color: #ced8de }");
  this.css.insert(".js-docked-compose .js-compose-text:not(.debug) { position: absolute; z-index: -9999; left: 0; opacity: 0 }");
  
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
  this.currentSpanner = null;
  
  var hideKeyboard = () => {
    $(this.currentKeyboard).remove();
    this.currentKeyboard = null;
    
    $(this.currentSpanner).remove();
    this.currentSpanner = null;
    
    this.currentKeywords = [];
    
    this.composePanelScroller.trigger("scroll");
    
    $(".emoji-keyboard-popup-btn").removeClass("is-selected");
    $(".js-compose-text").first().focus();
  };
  
  var generateEmojiHTML = skinTone => {
    let index = 0;
    let html = [ "<p style='font-size:13px;color:#444;margin:4px;text-align:center'>Please, note that most emoji will not show up properly in the text box above, but they will display in the tweet.</p>" ];
    
    for(let array of [ this.emojiData1, this.emojiData2[skinTone], this.emojiData3 ]){
      for(let emoji of array){
        if (emoji === "___"){
          html.push("<div class='separator'></div>");
        }
        else{
          html.push(TD.util.cleanWithEmoji(emoji).replace(' class="emoji" draggable="false"', ''));
          index++;
        }
      }
    }
    
    return html.join("");
  };
  
  var updateFilters = () => {
    let keywords = this.currentKeywords;
    let container = $(this.currentKeyboard.children[1]);
    
    let emoji = container.children("img");
    let info = container.children("p:first");
    let separators = container.children("div");
    
    if (keywords.length === 0){
      info.css("display", "block");
      separators.css("display", "block");
      emoji.css("display", "inline");
    }
    else{
      info.css("display", "none");
      separators.css("display", "none");
      
      emoji.css("display", "none");
      emoji.filter(index => keywords.every(kw => me.emojiNames[index].includes(kw))).css("display", "inline");
    }
  };
  
  var selectSkinTone = skinTone => {
    let selectedEle = this.currentKeyboard.children[2].querySelector("[data-tone='"+this.selectedSkinTone+"']");
    selectedEle && selectedEle.classList.remove("sel");
    
    this.selectedSkinTone = skinTone;
    this.currentKeyboard.children[1].innerHTML = generateEmojiHTML(skinTone);
    this.currentKeyboard.children[2].querySelector("[data-tone='"+this.selectedSkinTone+"']").classList.add("sel");
    updateFilters();
  };
  
  this.generateKeyboard = (left, top) => {
    var outer = document.createElement("div");
    outer.classList.add("emoji-keyboard");
    outer.style.left = left+"px";
    outer.style.top = top+"px";
    
    var keyboard = document.createElement("div");
    keyboard.classList.add("emoji-keyboard-list");
    
    keyboard.addEventListener("click", function(e){
      let ele = e.target;
      
      if (ele.tagName === "IMG"){
        insertEmoji(ele.getAttribute("src"), ele.getAttribute("alt"));
      }
      
      e.stopPropagation();
    });
    
    var search = document.createElement("div");
    search.innerHTML = "<input type='text' placeholder='Search...'>";
    search.classList.add("emoji-keyboard-search");
    
    var skintones = document.createElement("div");
    skintones.innerHTML = me.skinToneData.map(entry => "<div data-tone='"+entry[0]+"' style='background-color:"+entry[1]+"'></div>").join("");
    skintones.classList.add("emoji-keyboard-skintones");
    
    outer.appendChild(search);
    outer.appendChild(keyboard);
    outer.appendChild(skintones);
    $(".js-app").append(outer);
    
    skintones.addEventListener("click", function(e){
      if (e.target.hasAttribute("data-tone")){
        selectSkinTone(e.target.getAttribute("data-tone") || "");
      }
      
      e.stopPropagation();
    });
    
    search.addEventListener("click", function(e){
      e.stopPropagation();
    });
    
    var searchInput = search.children[0];
    searchInput.focus();
    
    searchInput.addEventListener("input", function(e){
      me.currentKeywords = e.target.value.split(" ").filter(kw => kw.length > 0).map(kw => kw.toLowerCase());
      updateFilters();
      e.stopPropagation();
    });
    
    searchInput.addEventListener("focus", function(){
      $(this).select();
    });
    
    this.currentKeyboard = outer;
    selectSkinTone(this.selectedSkinTone);
    
    this.currentSpanner = document.createElement("div");
    this.currentSpanner.style.height = ($(this.currentKeyboard).height()-10)+"px";
    $(".emoji-keyboard-popup-btn").parent().after(this.currentSpanner);
    
    this.composePanelScroller.trigger("scroll");
  };
  
  var getKeyboardTop = () => {
    let button = $(".emoji-keyboard-popup-btn");
    return button.offset().top+button.outerHeight()+me.composePanelScroller.scrollTop()+8;
  };
  
  var focusWithCaretAtEnd = () => {
    let range = document.createRange();
    range.selectNodeContents(me.composeInputNew[0]);
    range.collapse(false);
    
    let sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);
  };
  
  var insertEmoji = (src, alt) => {
    let sel = window.getSelection();
    
    if (!sel.anchorNode || $(sel.anchorNode).closest("#emoji-keyboard-tweet-input").length === 0){
      focusWithCaretAtEnd();
    }
    
    document.execCommand("insertHTML", false, `<img src="${src}" alt="${alt}">`);
  };
  
  // event handlers
  
  this.emojiKeyboardButtonClickEvent = function(e){
    if (me.currentKeyboard){
      hideKeyboard();
      $(this).blur();
    }
    else{
      me.generateKeyboard($(this).offset().left, getKeyboardTop());
      $(this).addClass("is-selected");
    }
    
    e.stopPropagation();
  };
  
  this.composerScrollEvent = function(e){
    if (me.currentKeyboard){
      me.currentKeyboard.style.marginTop = (-$(this).scrollTop())+"px";
    }
  };
  
  this.composerSendingEvent = function(e){
    hideKeyboard();
  };
  
  this.documentClickEvent = function(e){
    if (me.currentKeyboard && !e.target.classList.contains("compose-text")){
      hideKeyboard();
    }
  };
  
  this.documentKeyEvent = function(e){
    if (me.currentKeyboard && e.keyCode === 27){ // escape
      hideKeyboard();
      e.stopPropagation();
    }
  };
  
  this.uploadFilesEvent = function(e){
    if (me.currentKeyboard){
      me.currentKeyboard.style.top = getKeyboardTop()+"px";
    }
  };
  
  var prevOldInputVal = "";
  
  this.composeOldInputFocusEvent = function(){
    let ele = $(this);
    let val = ele.val();
    
    if (val != prevOldInputVal){
      prevOldInputVal = val;
      
      setTimeout(function(){
        let selStart = ele[0].selectionStart;
        let selEnd = ele[0].selectionEnd;
        
        me.composeInputNew.text(val);
        me.composeInputNew.focus();
        
        let sel = window.getSelection();
        let range = document.createRange();
        range.setStart(me.composeInputNew[0].firstChild, selStart);
        range.setEnd(me.composeInputNew[0].firstChild, selEnd);
        
        sel.removeAllRanges();
        sel.addRange(range);
      }, 1);
    }
    else{
      focusWithCaretAtEnd();
    }
  };
  
  var allowedShortcuts = [ 65 /* A */, 67 /* C */, 86 /* V */, 89 /* Y */, 90 /* Z */ ];
  
  this.composeInputKeyEvent = function(e){
    if (e.type === "keydown" && (e.ctrlKey || e.metaKey)){
      if (e.keyCode === 13){
        $(".js-send-button", ".js-docked-compose").click();
      }
      else if (e.keyCode >= 48 && !allowedShortcuts.includes(e.keyCode)){
        e.preventDefault();
        return;
      }
    }
    
    e.stopPropagation();
  };
  
  this.composeInputUpdateEvent = function(){
    let clone = $(this).clone();
    
    clone.children("div").each(function(){
      let ele = $(this)[0];
      ele.outerHTML = "\n"+ele.innerHTML;
    });
    
    clone.children("img").each(function(){
      let ele = $(this)[0];
      ele.outerHTML = ele.getAttribute("alt");
    });
    
    me.composeInputOrig.val(prevOldInputVal = clone.text());
    me.composeInputOrig.trigger("change");
  };
  
  this.composeInputPasteEvent = function(e){
    e.preventDefault();
    
    let text = e.originalEvent.clipboardData.getData("text/plain");
    text && document.execCommand("insertText", false, text);
  };
}

ready(){
  this.composeDrawer = $("[data-drawer='compose']");
  
  this.composePanelScroller = $(".js-compose-scroller", ".js-docked-compose").first().children().first();
  this.composePanelScroller.on("scroll", this.composerScrollEvent);
  
  $(".emoji-keyboard-popup-btn").on("click", this.emojiKeyboardButtonClickEvent);
  $(document).on("click", this.documentClickEvent);
  $(document).on("keydown", this.documentKeyEvent);
  $(document).on("uiComposeImageAdded", this.uploadFilesEvent);
  this.composeDrawer.on("uiComposeTweetSending", this.composerSendingEvent);
  
  // Editor
  
  this.composeInputOrig = $(".js-compose-text", ".js-docked-compose").first();
  this.composeInputNew = $('<div id="emoji-keyboard-tweet-input" contenteditable="true" class="compose-text txt-size--14 scroll-v scroll-styled-v scroll-styled-h scroll-alt td-detect-image-paste"></div>').insertAfter(this.composeInputOrig);
  
  this.composeInputOrig.on("focus", this.composeOldInputFocusEvent);
  
  this.composeInputNew.on("keydown keypress keyup", this.composeInputKeyEvent);
  this.composeInputNew.on("input", this.composeInputUpdateEvent);
  this.composeInputNew.on("paste", this.composeInputPasteEvent);
  
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
    for(let skinTone of this.skinToneList){
      this.emojiData2[skinTone] = [];
    }
    
    // declaration inserters
    
    let mapUnicode = pt => convUnicode(parseInt(pt, 16));
    
    let addDeclaration1 = decl => {
      this.emojiData1.push(decl.split(" ").map(mapUnicode).join(""));
    };
    
    let addDeclaration2 = (tone, decl) => {
      let gen = decl.split(" ").map(mapUnicode).join("");
      
      if (tone === null){
        for(let skinTone of this.skinToneList){
          this.emojiData2[skinTone].push(gen);
        }
      }
      else{
        this.emojiData2[tone].push(gen);
      }
    };
    
    let addDeclaration3 = decl => {
      this.emojiData3.push(decl.split(" ").map(mapUnicode).join(""));
    };
    
    // line reading
                      
    let skinToneState = 0;
    
    for(let line of contents.split("\n")){
      if (line[0] === '@'){
        switch(skinToneState){
          case 0: this.emojiData1.push("___"); break;
          case 1: this.skinToneList.forEach(skinTone => this.emojiData2[skinTone].push("___")); break;
          case 2: this.emojiData3.push("___"); break;
        }
        
        continue;
      }
      else if (line[0] === '#'){
        if (line[1] === '1'){
          skinToneState = 1;
        }
        else if (line[1] === '2'){
          skinToneState = 2;
        }
        
        continue;
      }
      
      let semicolon = line.indexOf(';');
      let decl = line.slice(0, semicolon);
      let desc = line.slice(semicolon+1).toLowerCase();
      
      if (skinToneState === 1){
        let skinIndex = decl.indexOf('$');

        if (skinIndex !== -1){
          let declPre = decl.slice(0, skinIndex);
          let declPost = decl.slice(skinIndex+1);
          
          for(let skinTone of this.skinToneNonDefaultList){
            this.emojiData2[skinTone].pop();
            addDeclaration2(skinTone, declPre+skinTone+declPost);
          }
        }
        else{
          addDeclaration2(null, decl);
          this.emojiNames.push(desc);
        }
      }
      else if (skinToneState === 2){
        addDeclaration3(decl);
        this.emojiNames.push(desc);
      }
      else if (skinToneState === 0){
        addDeclaration1(decl);
        this.emojiNames.push(desc);
      }
    }
  }).catch(err => {
    $TD.alert("error", "Problem loading emoji keyboard: "+err.message);
  });
}

disabled(){
  this.css.remove();
  
  if (this.currentKeyboard){
    $(this.currentKeyboard).remove();
  }
  
  if (this.currentSpanner){
    $(this.currentSpanner).remove();
  }
  
  this.composeInputNew.remove();
  
  this.composeInputOrig.off("focus", this.composeOldInputFocusEvent);
  this.composePanelScroller.off("scroll", this.composerScrollEvent);
  
  $(".emoji-keyboard-popup-btn").off("click", this.emojiKeyboardButtonClickEvent);
  $(".emoji-keyboard-popup-btn").remove();
  
  $(document).off("click", this.documentClickEvent);
  $(document).off("keydown", this.documentKeyEvent);
  $(document).off("uiComposeImageAdded", this.uploadFilesEvent);
  this.composeDrawer.off("uiComposeTweetSending", this.composerSendingEvent);
  
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
