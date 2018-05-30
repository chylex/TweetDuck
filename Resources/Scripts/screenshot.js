(function($TD){
  let ele = document.getElementsByTagName("article")[0];
  ele.style.width = "{width}px";
  
  ele.style.position = "absolute";
  let contentHeight = ele.offsetHeight;
  ele.style.position = "static";
  
  let avatar = ele.querySelector(".tweet-avatar");
  let avatarBottom = avatar ? avatar.getBoundingClientRect().bottom : 0;
  
  $TD.setHeight(Math.floor(Math.max(contentHeight, avatarBottom+9))).then(() => {
    let framesLeft = {frames}; // basic render is done in 1 frame, large media take longer
    
    let onNextFrame = function(){
      if (--framesLeft < 0){
        $TD.triggerScreenshot();
      }
      else{
        requestAnimationFrame(onNextFrame);
      }
    };
    
    onNextFrame();
  });
})($TD_NotificationScreenshot);
