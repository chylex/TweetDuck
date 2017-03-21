enabled(){
  this.isDebugging = false;
  
  this.onKeyDown = (e) => {
    // ==========================
    // F4 key - toggle debug mode
    // ==========================
    
    if (e.keyCode === 115){
      this.isDebugging = !this.isDebugging;
      $(".app-title").first().css("background-color", this.isDebugging ? "#5A6B75" : "#292F33");
    }
    
    // Debug mode handling
    
    else if (this.isDebugging){
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
  };
}

ready(){
  $(document).on("keydown", this.onKeyDown);
}

disabled(){
  $(document).off("keydown", this.onKeyDown);
}
