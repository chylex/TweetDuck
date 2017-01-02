(function($, $TD, TD){
  $(document).keydown(function(e){
    
    // ==============================
    // F4 key - simulate notification
    // ==============================
    
    if (e.keyCode === 115){
      var col = TD.controller.columnManager.getAllOrdered()[0];
      
      $.publish("/notifications/new",[{
        column: col,
        items: [
          col.updateArray[Math.floor(Math.random()*col.updateArray.length)]
        ]
      }]);
    }
  });
})($, $TD, TD);
