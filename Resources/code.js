(function($,$TD){
  //
  // Variable: Says whether TweetDick events was initialized.
  //
  var isInitialized = false;
  
  //
  // Function: Initializes TweetDick events. Called after the website app is loaded.
  //
  var initializeTweetDick = function(){
    $("[data-action='settings-menu']").click(function(){
      setTimeout(function(){
        var menu = $(".js-dropdown-content").children("ul").first();
        if (menu.length == 0)return;
        
        menu.children(".drp-h-divider").last().after('<li class="is-selectable" data-tweetdick><a href="#" data-action>TweetDick</a></li><li class="drp-h-divider"></li>');
        
        var tweetDickBtn = menu.children("[data-tweetdick]");
        
        tweetDickBtn.on("click","a",function(){
          $TD.openSettingsMenu();
        });
        
        tweetDickBtn.hover(function(){
          $(this).addClass("is-selected");
        },function(){
          $(this).removeClass("is-selected");
        });
      },0);
    });
    
    isInitialized = true;
  };
  
  //
  // Block: Observe the app <div> element and initialize TweetDick whenever possible.
  //
  var app = $("body").children(".js-app");
  
  new MutationObserver(function(mutations){
    if (isInitialized && app.hasClass("is-hidden")){
      isInitialized = false;
    }
    else if (!isInitialized && !app.hasClass("is-hidden")){
      initializeTweetDick();
    }
  }).observe(app[0],{
    attributes: true,
    attributeFilter: [ "class" ]
  });
})($,$TD);
