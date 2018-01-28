(function($, $TD){
  $(document).one("TD.ready", function(){
    let css = $(`
<style>
#td-introduction-modal {
  display: block;
}

#td-introduction-modal .mdl {
  width: 90%;
  max-width: 626px;
  height: 244px;
}

#td-introduction-modal .mdl-header-title {
  cursor: default;
}

#td-introduction-modal .mdl-content {
  padding: 4px 16px 0;
  overflow-y: auto;
}

#td-introduction-modal p {
  margin: 12px 0;
  font-size: 1.4rem;
}

#td-introduction-modal p strong {
  font-weight: normal;
  text-shadow: 0 0 #000;
}

#td-introduction-modal footer {
  padding: 10px 0;
}
</style>`).appendTo(document.head);
    
    let ele = $(`
<div id="td-introduction-modal" class="ovl">
  <div class="mdl is-inverted-dark">
    <header class="mdl-header">
      <h3 class="mdl-header-title">Quick message</h3>
      <a href="#" class="mdl-dismiss link-normal-dark"><i class="icon icon-close"></i></a>
    </header>
    <div class="mdl-inner">
      <div class="mdl-content">
        <p>Hi! Unfortunately the old <strong>@TryTweetDuck</strong> account was suspended.</p>
        <p>If you were following it before, or if you want to keep up with the latest news and updates about TweetDuck, please <a id="td-introduction-follow" href="#">follow @TryMyAwesomeApp</a>.</p>
        <p>Thanks for your support!</p>
      </div>
      <footer class="txt-right">
        <button class="btn btn-positive"><span class="label">Close</span</button>
      </footer>
    </div>
  </div>
</div>`).appendTo(".js-app");
    
    let tdUser = null;
    let loadTweetDuckUser = (onSuccess, onError) => {
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
      ele.fadeOut(200, function(){
        $TD.onIntroductionClosed(false, false);
        ele.remove();
        css.remove();
      });
    });
  });
})($, $TD);
