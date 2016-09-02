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
  
  var clearAllColumns = () => {
    Object.keys(TD.controller.columnManager.getAll()).forEach(key => clearColumn(key));
  };
  
  var replaceMustache = (key, search, replace) => {
    TD.mustaches[key] = TD.mustaches[key].replace(search, replace);
  };
  
  // prepare event handlers
  this.eventClearSingle = function(){
    clearColumn($(this).closest(".js-column").attr("data-column"));
  };
  
  this.eventClearAll = function(){
    clearAllColumns();
  };
  
  this.eventKeys = function(e){
    if (e.keyCode === 46 && (document.activeElement === null || document.activeElement === document.body)){ // 46 = delete
      if (e.altKey){
        clearAllColumns();
      }
      else{
        var focusedColumn = $(".js-column.is-focused");
        
        if (focusedColumn.length){
          clearColumn(focusedColumn.attr("data-column"));
        }
      }
    }
  };
  
  // add column buttons and keyboard shortcut info to UI
  replaceMustache("column/column_header.mustache", "</header>", [
    '<a class="column-header-link" href="#" data-action="td-clearcolumns-dosingle" style="right:34px">',
    '<i class="icon icon-clear-timeline"></i>',
    '</a></header>'
  ].join(""));
  
  replaceMustache("keyboard_shortcut_list.mustache", "</dl>  <dl", [
    '<dd class="keyboard-shortcut-definition" style="white-space:nowrap">',
    '<span class="text-like-keyboard-key">1</span> … <span class="text-like-keyboard-key">9</span> + <span class="text-like-keyboard-key">Del</span> Clear column 1－9',
    '</dd><dd class="keyboard-shortcut-definition">',
    '<span class="text-like-keyboard-key">Alt</span> + <span class="text-like-keyboard-key">Del</span> Clear all',
    '</dd></dl><dl'
  ].join(""));
}

ready(){
  // setup events
  $(document).on("click", "[data-action='td-clearcolumns-dosingle']", this.eventClearSingle);
  $(document).on("click", "[data-action='td-clearcolumns-doall']", this.eventClearAll);
  $(document).on("keydown", this.eventKeys);
  
  // add clear all button
  $("nav.app-navigator").first().append([
    '<a class="link-clean cf app-nav-link padding-hl" data-title="Clear all" data-action="td-clearcolumns-doall">',
    '<div class="obj-left"><i class="icon icon-large icon-clear-timeline"></i></div>',
    '<div class="nbfc padding-ts hide-condensed">Clear all</div>',
    '</a></nav>'
  ].join(""));
}

disabled(){
  // not needed, plugin reloads the page when enabled or disabled
}
