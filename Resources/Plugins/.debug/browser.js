enabled(){
	this.isDebugging = false;
	
	this.onKeyDown = (e) => {
		
		// ==========================
		// F4 key - toggle debug mode
		// ==========================
		
		if (e.keyCode === 115) {
			this.isDebugging = !this.isDebugging;
			$(".nav-user-info").first().css("background-color", this.isDebugging ? "#5a6b75" : "#292f33");
		}
		
		else if (this.isDebugging) {
			e.preventDefault();
			
			// ===================================
			// N key - simulate popup notification
			// S key - simulate sound notification
			// ===================================
			
			if (e.keyCode === 78 || e.keyCode === 83) {
				let col = TD.controller.columnManager.getAllOrdered()[0];
				let model = col.model;
				
				let prevPopup = model.getHasNotification();
				let prevSound = model.getHasSound();
				
				model.setHasNotification(e.keyCode === 78);
				model.setHasSound(e.keyCode === 83);
				
				$.publish("/notifications/new", [ {
					column: col,
					items: [
						col.updateArray[Math.floor(Math.random() * col.updateArray.length)]
					]
				} ]);
				
				setTimeout(function() {
					model.setHasNotification(prevPopup);
					model.setHasSound(prevSound);
				}, 1);
			}
				
				// ========================
				// D key - trigger debugger
			// ========================
			
			else if (e.keyCode === 68) {
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
