(function($, $TD, $TDX, TD){
  var isDebugging = false;
  
  $(document).keydown(function(e){
    
    // ==========================
    // F4 key - toggle debug mode
    // ==========================
    
    if (e.keyCode === 115){
      isDebugging = !isDebugging;
      $(".app-title").first().css("background-color", isDebugging ? "#5A6B75" : "#292F33");
    }
    
    // Debug mode handling
    
    else if (isDebugging){
      e.preventDefault();
      
      // ===================================
      // N key - simulate popup notification
      // ===================================
      
      if (e.keyCode === 78){
        var col = TD.controller.columnManager.getAllOrdered()[0];

        $.publish("/notifications/new",[{
          column: col,
          items: [
            col.updateArray[Math.floor(Math.random()*col.updateArray.length)]
          ]
        }]);
      }
      
      // ===================================
      // S key - simulate sound notification
      // ===================================
      
      else if (e.keyCode === 83){
        if ($TDX.hasCustomNotificationSound){
          $TD.onTweetSound();
        }
        else{
          document.getElementById("update-sound").play();
        }
      }
    }
  });
})($, $TD, $TDX, TD);
