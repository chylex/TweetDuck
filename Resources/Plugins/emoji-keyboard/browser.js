enabled(){
  this.ENABLE_CUSTOM_KEYBOARD = false;
  
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
  
  const me = this;
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".emoji-keyboard { position: absolute; width: 15.35em; background-color: white; border-radius: 1px; font-size: 24px; z-index: 9999 }");
  this.css.insert(".emoji-keyboard-popup-btn { height: 36px !important }");
  this.css.insert(".emoji-keyboard-popup-btn .icon { vertical-align: -4px !important }");
  
  this.css.insert(".emoji-keyboard-list { height: 10.14em; padding: 0.1em; box-sizing: border-box; overflow-y: auto }");
  this.css.insert(".emoji-keyboard-list .separator { height: 26px }");
  this.css.insert(".emoji-keyboard-list img { padding: 0.1em !important; width: 1em; height: 1em; vertical-align: -0.1em; cursor: pointer }");
  
  this.css.insert(".emoji-keyboard-search { height: auto; padding: 4px 10px 8px; background-color: #292f33; border-radius: 1px 1px 0 0 }");
  this.css.insert(".emoji-keyboard-search input { width: 100%; border-radius: 1px; }");
  
  this.css.insert(".emoji-keyboard-skintones { height: 1.3em; text-align: center; background-color: #292f33; border-radius: 0 0 1px 1px }");
  this.css.insert(".emoji-keyboard-skintones div { width: 0.8em; height: 0.8em; margin: 0.25em 0.1em; border-radius: 50%; display: inline-block; box-sizing: border-box; cursor: pointer }");
  this.css.insert(".emoji-keyboard-skintones .sel { border: 2px solid rgba(0, 0, 0, 0.35); box-shadow: 0 0 2px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.4) inset }");
  this.css.insert(".emoji-keyboard-skintones :hover { border: 2px solid rgba(0, 0, 0, 0.25); box-shadow: 0 0 1px 0 rgba(255, 255, 255, 0.65), 0 0 1px 0 rgba(255, 255, 255, 0.25) inset }");
  
  this.css.insert(".js-compose-text { font-family: \"Twitter Color Emoji\", Helvetica, Arial, Verdana, sans-serif; }");
  
  // layout
  
  let buttonHTML = '<button class="needsclick btn btn-on-blue txt-left padding-v--6 padding-h--8 emoji-keyboard-popup-btn"><i class="icon icon-heart"></i></button>';
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  window.TDPF_injectMustache("compose/docked_compose.mustache", "append", '<div class="cf margin-t--12 margin-b--30">', buttonHTML);
  
  this.getDrawerInput = () => {
    return $(".js-compose-text", me.composeDrawer);
  };
  
  this.getDrawerScroller = () => {
    return $(".js-compose-scroller > .scroll-v", me.composeDrawer);
  };
  
  // keyboard generation
  
  this.currentKeyboard = null;
  this.currentSpanner = null;
  
  let wasSearchFocused = false;
  let lastEmojiKeyword, lastEmojiPosition, lastEmojiLength;
  
  const hideKeyboard = (refocus) => {
    $(this.currentKeyboard).remove();
    this.currentKeyboard = null;
    
    $(this.currentSpanner).remove();
    this.currentSpanner = null;
    
    this.currentKeywords = [];
    
    this.getDrawerScroller().trigger("scroll");
    
    $(".emoji-keyboard-popup-btn").removeClass("is-selected");
    
    if (refocus){
      this.getDrawerInput().focus();
      
      if (lastEmojiKeyword && lastEmojiPosition === 0){
        document.execCommand("insertText", false, lastEmojiKeyword);
      }
    }
    
    lastEmojiKeyword = null;
  };
  
  const generateEmojiHTML = skinTone => {
    let index = 0;
    let html = [ "<p style='font-size:13px;color:#444;margin:4px;text-align:center'>Please, note that some emoji may not show up correctly in the text box above, but they will display in the tweet.</p>" ];
    
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
  
  const updateFilters = () => {
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
  
  const selectSkinTone = skinTone => {
    let selectedEle = this.currentKeyboard.children[2].querySelector("[data-tone='"+this.selectedSkinTone+"']");
    selectedEle && selectedEle.classList.remove("sel");
    
    this.selectedSkinTone = skinTone;
    this.currentKeyboard.children[1].innerHTML = generateEmojiHTML(skinTone);
    this.currentKeyboard.children[2].querySelector("[data-tone='"+this.selectedSkinTone+"']").classList.add("sel");
    updateFilters();
  };
  
  this.generateKeyboard = (left, top) => {
    let outer = document.createElement("div");
    outer.classList.add("emoji-keyboard");
    outer.style.left = left+"px";
    outer.style.top = top+"px";
    
    let keyboard = document.createElement("div");
    keyboard.classList.add("emoji-keyboard-list");
    
    keyboard.addEventListener("click", function(e){
      let ele = e.target;
      
      if (ele.tagName === "IMG"){
        insertEmoji(ele.getAttribute("src"), ele.getAttribute("alt"));
      }
      
      e.stopPropagation();
    });
    
    let search = document.createElement("div");
    search.innerHTML = "<input type='text' placeholder='Search...'>";
    search.classList.add("emoji-keyboard-search");
    
    let skintones = document.createElement("div");
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
    
    let searchInput = search.children[0];
    searchInput.focus();
    
    wasSearchFocused = false;
    
    searchInput.addEventListener("input", function(e){
      me.currentKeywords = e.target.value.split(" ").filter(kw => kw.length > 0).map(kw => kw.toLowerCase());
      updateFilters();
      
      wasSearchFocused = $(this).val().length > 0;
      e.stopPropagation();
    });
    
    searchInput.addEventListener("keydown", function(e){
      if (e.keyCode === 13 && $(this).val().length){ // enter
        let ele = $(".emoji-keyboard-list").children("img").filter(":visible").first();
        
        if (ele.length > 0){
          insertEmoji(ele[0].getAttribute("src"), ele[0].getAttribute("alt"));
        }
        
        e.preventDefault();
      }
    });
    
    searchInput.addEventListener("click", function(){
      $(this).select();
    });
    
    searchInput.addEventListener("focus", function(){
      wasSearchFocused = true;
    });
    
    this.currentKeyboard = outer;
    selectSkinTone(this.selectedSkinTone);
    
    this.currentSpanner = document.createElement("div");
    this.currentSpanner.style.height = ($(this.currentKeyboard).height()-10)+"px";
    $(".emoji-keyboard-popup-btn").parent().after(this.currentSpanner);
    
    this.getDrawerScroller().trigger("scroll");
  };
  
  const getKeyboardTop = () => {
    let button = $(".emoji-keyboard-popup-btn");
    return button.offset().top + button.outerHeight() + me.getDrawerScroller().scrollTop() + 8;
  };
  
  const insertEmoji = (src, alt) => {
    let input = this.getDrawerInput();
    
    let val = input.val();
    let posStart = input[0].selectionStart;
    let posEnd = input[0].selectionEnd;
    
    input.val(val.slice(0, posStart)+alt+val.slice(posEnd));
    input.trigger("change");
    
    input[0].selectionStart = posStart+alt.length;
    input[0].selectionEnd = posStart+alt.length;
    
    lastEmojiKeyword = null;
    
    if (wasSearchFocused){
      $(".emoji-keyboard-search").children("input").focus();
    }
    else{
      input.focus();
    }
  };
  
  // general event handlers
  
  this.emojiKeyboardButtonClickEvent = function(e){
    if (me.currentKeyboard){
      $(this).blur();
      hideKeyboard(true);
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
  
  this.composeInputKeyDownEvent = function(e){
    if (lastEmojiKeyword && (e.keyCode === 8 || e.keyCode === 27)){ // backspace, escape
      let ele = $(this)[0];
      
      if (ele.selectionStart === lastEmojiPosition){
        ele.selectionStart -= lastEmojiLength; // selects the emoji
        document.execCommand("insertText", false, lastEmojiKeyword);
        
        e.preventDefault();
        e.stopPropagation();
      }
      
      lastEmojiKeyword = null;
    }
  };
  
  this.composeInputKeyPressEvent = function(e){
    if (String.fromCharCode(e.which) === ':'){
      let ele = $(this);
      let val = ele.val();
      
      let firstColon = val.lastIndexOf(':', ele[0].selectionStart);
      if (firstColon === -1) {
        return;
      }
      
      let search = val.substring(firstColon+1, ele[0].selectionStart).toLowerCase();
      if (!/^[a-z_]+$/.test(search)) {
        return;
      }
      
      let keywords = search.split("_").filter(kw => kw.length > 0).map(kw => kw.toLowerCase());
      if (keywords.length === 0) {
        return;
      }
      
      let foundNames = me.emojiNames.filter(name => keywords.every(kw => name.includes(kw)));
      
      if (foundNames.length === 0){
        return;
      }
      else if (foundNames.length > 1 && foundNames.includes(search)){
        foundNames = [ search ];
      }
      
      lastEmojiKeyword = `:${search}:`;
      lastEmojiPosition = lastEmojiLength = 0;
      
      if (foundNames.length === 1){
        let foundIndex = me.emojiNames.indexOf(foundNames[0]);
        let foundEmoji;
        
        for(let array of [ me.emojiData1, me.emojiData2[me.selectedSkinTone], me.emojiData3 ]){
          let realArray = array.filter(ele => ele !== "___");
          
          if (foundIndex >= realArray.length){
            foundIndex -= realArray.length;
          }
          else{
            foundEmoji = realArray[foundIndex];
            break;
          }
        }
        
        if (foundEmoji){
          e.preventDefault();
          
          ele.val(val.substring(0, firstColon)+foundEmoji+val.substring(ele[0].selectionStart));
          ele[0].selectionEnd = ele[0].selectionStart = firstColon+foundEmoji.length;
          ele.trigger("change");
          ele.focus();
          
          lastEmojiPosition = firstColon+foundEmoji.length;
          lastEmojiLength = foundEmoji.length;
        }
      }
      else if (foundNames.length > 1 && $(".js-app-content").is(".is-open")){
        e.preventDefault();
        ele.val(val.substring(0, firstColon)+val.substring(ele[0].selectionStart));
        ele[0].selectionEnd = ele[0].selectionStart = firstColon;
        ele.trigger("change");
        
        if (!me.currentKeyboard){
          $(".emoji-keyboard-popup-btn").click();
        }
        
        $(".emoji-keyboard-search").children("input").focus().val("");
        document.execCommand("insertText", false, keywords.join(" "));
      }
    }
    else{
      lastEmojiKeyword = null;
    }
  };
  
  this.composeInputFocusEvent = function(e){
    wasSearchFocused = false;
  };
  
  this.composerSendingEvent = function(e){
    hideKeyboard();
  };
  
  this.composerActiveEvent = function(e){
    $(".emoji-keyboard-popup-btn", me.composeDrawer).on("click", me.emojiKeyboardButtonClickEvent);
    $(".js-docked-compose .js-compose-scroller > .scroll-v", me.composeDrawer).on("scroll", me.composerScrollEvent);
  };
  
  this.documentClickEvent = function(e){
    if (me.currentKeyboard && $(e.target).closest(".compose-text-container").length === 0){
      hideKeyboard();
    }
  };
  
  this.documentKeyEvent = function(e){
    if (me.currentKeyboard && e.keyCode === 27){ // escape
      hideKeyboard(true);
      e.stopPropagation();
    }
  };
  
  this.uploadFilesEvent = function(e){
    if (me.currentKeyboard){
      me.currentKeyboard.style.top = getKeyboardTop()+"px";
    }
  };
  
  // re-enabling
  
  let maybeDockedComposePanel = $(".js-docked-compose");
  
  if (maybeDockedComposePanel.length){
    maybeDockedComposePanel.find(".cf.margin-t--12.margin-b--30").first().append(buttonHTML);
    this.composerActiveEvent();
  }
}

ready(){
  this.composeDrawer = $(".js-drawer[data-drawer='compose']");
  this.composeSelector = ".js-compose-text,.js-reply-tweetbox";
  
  $(document).on("click", this.documentClickEvent);
  $(document).on("keydown", this.documentKeyEvent);
  $(document).on("tduckOldComposerActive", this.composerActiveEvent);
  $(document).on("uiComposeImageAdded", this.uploadFilesEvent);
  
  this.composeDrawer.on("uiComposeTweetSending", this.composerSendingEvent);
  
  $(document).on("keydown", this.composeSelector, this.composeInputKeyDownEvent);
  $(document).on("keypress", this.composeSelector, this.composeInputKeyPressEvent);
  $(document).on("focus", this.composeSelector, this.composeInputFocusEvent);
  
  // HTML generation
  
  const convUnicode = function(codePt){
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
  
  $(".emoji-keyboard-popup-btn").remove();
  
  $(document).off("click", this.documentClickEvent);
  $(document).off("keydown", this.documentKeyEvent);
  $(document).off("tduckOldComposerActive", this.composerActiveEvent);
  $(document).off("uiComposeImageAdded", this.uploadFilesEvent);
  
  this.composeDrawer.off("uiComposeTweetSending", this.composerSendingEvent);
  
  $(document).off("keydown", this.composeSelector, this.composeInputKeyDownEvent);
  $(document).off("keypress", this.composeSelector, this.composeInputKeyPressEvent);
  $(document).off("focus", this.composeSelector, this.composeInputFocusEvent);
  
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
