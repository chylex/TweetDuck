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
    let col = TD.controller.columnManager.get(columnName);
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
      
      $("#clear-columns-btn-all-1,#clear-columns-btn-all-2").text(pressed ? "Restore columns" : "Clear columns");
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
    return if !(document.activeElement === null || document.activeElement === document.body);
    
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
    '<a class="column-header-link td-clear-column-shortcut" href="#" data-action="td-clearcolumns-dosingle" style="right:34px">',
    '<i class="icon icon-clear-timeline js-show-tip" data-placement="bottom" data-original-title="Clear column (hold Shift to restore)"></i>',
    '</a>',
    '{{/isTemporary}}',
    '</header>'
  ].join(""));
  
  replaceMustache("keyboard_shortcut_list.mustache", "</dl>  <dl", [
    '<dd class="keyboard-shortcut-definition" style="white-space:nowrap">',
    '<span class="text-like-keyboard-key">1</span> … <span class="text-like-keyboard-key">9</span> + <span class="text-like-keyboard-key">Del</span> Clear column 1－9',
    '</dd><dd class="keyboard-shortcut-definition">',
    '<span class="text-like-keyboard-key">Alt</span> + <span class="text-like-keyboard-key">Del</span> Clear all columns',
    '</dd></dl><dl'
  ].join(""));
  
  // load custom style
  var css = window.TDPF_createCustomStyle(this);
  css.insert(".js-app-add-column.is-hidden + #clear-columns-btn-all-parent-1 { display: none; }");
  css.insert(".column-navigator-overflow #clear-columns-btn-all-parent-2 { display: none; }");
  css.insert(".column-title { margin-right: 60px !important; }");
  css.insert(".mark-all-read-link { right: 59px !important; }");
  css.insert(".open-compose-dm-link { right: 90px !important; }");
  css.insert("button[data-action='clear'].btn-options-tray { display: none !important; }");
  css.insert("[data-td-icon='icon-message'] .column-title { margin-right: 115px !important; }");
  css.insert("[data-td-icon='icon-schedule'] .td-clear-column-shortcut { display: none; }");
  css.insert("[data-td-icon='icon-custom-timeline'] .td-clear-column-shortcut { display: none; }");
}

ready(){
  // setup events
  $(document).on("click", "[data-action='td-clearcolumns-dosingle']", this.eventClickSingle);
  $(document).on("click", "[data-action='td-clearcolumns-doall']", this.eventClickAll);
  $(document).on("keydown", this.eventKeyDown);
  $(document).on("keyup", this.eventKeyUp);
  
  // add clear all button
  const generateButton = (idParent, idButton) => {
    return `
<a id="${idParent}" class="js-header-action link-clean cf app-nav-link padding-h--10" data-title="Clear columns (hold Shift to restore)" data-action="td-clearcolumns-doall">
  <div class="obj-left margin-l--2"><i class="icon icon-medium icon-clear-timeline"></i></div>
  <div id="${idButton}" class="nbfc padding-ts hide-condensed txt-size--16 app-nav-link-text">Clear columns</div>
</a>`;
  };
  
  $(".js-app-add-column").first().after(generateButton("clear-columns-btn-all-parent-1", "clear-columns-btn-all-1"));
  $(".js-column-nav-list").first().append(generateButton("clear-columns-btn-all-parent-2", "clear-columns-btn-all-2"));
  
  // setup tooltip handling
  var tooltipEvents = $._data($(".js-header-action")[0]).events;
  
  if (tooltipEvents.mouseover && tooltipEvents.mouseover.length && tooltipEvents.mouseout && tooltipEvents.mouseout.length){
    $("#clear-columns-btn-all-parent-1,#clear-columns-btn-all-parent-2").on("mouseover", tooltipEvents.mouseover[0].handler).on("mouseout", tooltipEvents.mouseout[0].handler);
  }
}

disabled(){
  // not needed, plugin reloads the page when enabled or disabled
}
