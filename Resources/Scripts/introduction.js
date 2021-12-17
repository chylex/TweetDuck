(function(){
  $(document).one("TD.ready", function(){
    const css = $(`<style>#import "styles/introduction.css"</style>`).appendTo(document.head);
    const ele = $(`#import "markup/introduction.html"`).appendTo(".js-app");
    
    let tdUser = null;
    
    const loadTweetDuckUser = (onSuccess, onError) => {
      if (tdUser !== null){
        onSuccess(tdUser);
      }
      else{
        TD.controller.clients.getPreferredClient().getUsersByIds([ "957608948189880320" ], users => onSuccess(users[0]), onError);
      }
    };
    
    loadTweetDuckUser(user => tdUser = user);
    
    ele.find("#td-introduction-follow").click(function(){
      loadTweetDuckUser(user => {
        $(document).trigger("uiShowFollowFromOptions", { userToFollow: user });
        
        $(".js-modals-container").find("header a[rel='user']").each(function(){
          this.outerHTML = "TweetDuck";
        });
      }, () => {
        alert("An error occurred when retrieving the account information.");
      });
    });
    
    ele.find("button, a.mdl-dismiss").click(function(){
      const showGuide = $(this)[0].hasAttribute("data-guide");
      
      ele.fadeOut(200, function(){
        $TD.onIntroductionClosed(showGuide);
        ele.remove();
        css.remove();
      });
    });
  });
})();
