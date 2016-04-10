(function($,$TD){
  //
  // Variable: Says whether TweetDick events was initialized.
  //
  var isInitialized = false;
  
  //
  // Function: Initializes TweetDick events. Called after the website app is loaded.
  //
  var initializeTweetDick = function(){
    isInitialized = true;
  };
  
  //
  // Block: Observe the app <div> element and initialize TweetDick whenever possible.
  //
  var app = $("body").children(".js-app");
  
  new MutationObserver(function(mutations){
    if (mutations.some(mutation => mutation.attributeName === "class")){
      if (isInitialized && app.hasClass("is-hidden")){
        isInitialized = false;
      }
      else if (!isInitialized && !app.hasClass("is-hidden")){
        initializeTweetDick();
      }
    }
  }).observe(app[0],{
    attributes: true
  });
})($,$TD);
