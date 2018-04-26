enabled(){
  this.isDebugging = false;
  
  this.onKeyDown = (e) => {
    // ==========================
    // F4 key - toggle debug mode
    // ==========================
    
    if (e.keyCode === 115){
      this.isDebugging = !this.isDebugging;
      $(".nav-user-info").first().css("background-color", this.isDebugging ? "#5A6B75" : "#292F33");
    }
    
    // Debug mode handling
    
    else if (this.isDebugging){
      e.preventDefault();
      
      // ===================================
      // N key - simulate popup notification
      // S key - simulate sound notification
      // ===================================
      
      if (e.keyCode === 78 || e.keyCode === 83){
        var col = TD.controller.columnManager.getAllOrdered()[0];
        
        var prevPopup = col.model.getHasNotification();
        var prevSound = col.model.getHasSound();
        
        col.model.setHasNotification(e.keyCode === 78);
        col.model.setHasSound(e.keyCode === 83);
        
        $.publish("/notifications/new",[{
          column: col,
          items: [
            col.updateArray[Math.floor(Math.random()*col.updateArray.length)]
          ]
        }]);
        
        setTimeout(function(){
          col.model.setHasNotification(prevPopup);
          col.model.setHasSound(prevSound);
        }, 1);
      }
      
      // ========================
      // D key - trigger debugger
      // ========================
      
      else if (e.keyCode === 68){
        debugger;
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

configure(){
  alert("Configure triggered");
}
