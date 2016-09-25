constructor(){
  super({
    requiresPageReload: true
  });
}

enabled(){
  // prepare variables and functions
  var clearColumn = (columnName) => {
    TD.controller.columnManager.get(columnName).clear();
    TD.controller.stats.columnActionClick("clear");
  };
  
  var resetColumn = (columnName) => {
    var col = TD.controller.columnManager.get(columnName);
    col.model.setClearedTimestamp(0);
    col.reloadTweets();
  };
  
  var forEachColumn = (func) => {
    Object.keys(TD.controller.columnManager.getAll()).forEach(func);
  };
  
  var replaceMustache = (key, search, replace) => {
    TD.mustaches[key] = TD.mustaches[key].replace(search, replace);
  };
  
  var wasShiftPressed = false;
  
  var updateShiftState = (pressed) => {
    if (pressed != wasShiftPressed){
      wasShiftPressed = pressed;
      
      if (pressed){
        $(document).on("mousemove", this.eventKeyUp);
      }
      else{
        $(document).off("mousemove", this.eventKeyUp);
      }
      
      $("#clear-columns-btn-all").text(pressed ? "Reset all" : "Clear all");
    }
  };
  
  // prepare event handlers
  this.eventClickSingle = function(e){
    var name = $(this).closest(".js-column").attr("data-column");
    e.shiftKey ? resetColumn(name) : clearColumn(name);
  };
  
  this.eventClickAll = function(e){
    forEachColumn(e.shiftKey ? resetColumn : clearColumn);
  };
  
  this.eventKeyDown = function(e){
    if (!(document.activeElement === null || document.activeElement === document.body)){
      return;
    }
    
    updateShiftState(e.shiftKey);
    
    if (e.keyCode === 46){ // 46 = delete
      if (e.altKey){
        forEachColumn(e.shiftKey ? resetColumn : clearColumn);
      }
      else{
        var focusedColumn = $(".js-column.is-focused");
        
        if (focusedColumn.length){
          var name = focusedColumn.attr("data-column");
          e.shiftKey ? resetColumn(name) : clearColumn(name);
        }
      }
    }
  };
  
  this.eventKeyUp = function(e){
    if (!e.shiftKey){
      updateShiftState(false);
    }
  };
  
  // add column buttons and keyboard shortcut info to UI
  replaceMustache("column/column_header.mustache", "</header>", [
    '{{^isTemporary}}',
    '<a class="column-header-link" href="#" data-action="td-clearcolumns-dosingle" style="right:34px">',
    '<i class="icon icon-clear-timeline"></i>',
    '</a>',
    '{{/isTemporary}}',
    '</header>'
  ].join(""));
  
  replaceMustache("keyboard_shortcut_list.mustache", "</dl>  <dl", [
    '<dd class="keyboard-shortcut-definition" style="white-space:nowrap">',
    '<span class="text-like-keyboard-key">1</span> … <span class="text-like-keyboard-key">9</span> + <span class="text-like-keyboard-key">Del</span> Clear column 1－9',
    '</dd><dd class="keyboard-shortcut-definition">',
    '<span class="text-like-keyboard-key">Alt</span> + <span class="text-like-keyboard-key">Del</span> Clear all',
    '</dd></dl><dl'
  ].join(""));
  
  // load custom style
  var css = window.TDPF_createCustomStyle(this);
  css.insert(".column-title { margin-right: 60px !important; }");
  css.insert(".column-type-message .column-title { margin-right: 115px !important; }");
  css.insert(".mark-all-read-link { right: 59px !important; }");
  css.insert(".open-compose-dm-link { right: 90px !important; }");
  css.insert("button[data-action='clear'].btn-options-tray { display: none !important; }");
}

ready(){
  // setup events
  $(document).on("click", "[data-action='td-clearcolumns-dosingle']", this.eventClickSingle);
  $(document).on("click", "[data-action='td-clearcolumns-doall']", this.eventClickAll);
  $(document).on("keydown", this.eventKeyDown);
  $(document).on("keyup", this.eventKeyUp);
  
  // add clear all button
  $("nav.app-navigator").first().append([
    '<a class="link-clean cf app-nav-link padding-hl" data-title="Clear all" data-action="td-clearcolumns-doall">',
    '<div class="obj-left"><i class="icon icon-large icon-clear-timeline"></i></div>',
    '<div id="clear-columns-btn-all" class="nbfc padding-ts hide-condensed">Clear all</div>',
    '</a></nav>'
  ].join(""));
}

disabled(){
  // not needed, plugin reloads the page when enabled or disabled
}
