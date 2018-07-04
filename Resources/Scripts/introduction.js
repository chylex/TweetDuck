(function($, $TD){
  $(document).one("TD.ready", function(){
    let css = $(`<style>#import "styles/introduction.css"</style>`).appendTo(document.head);
    
    let ele = $(`
<div id="td-introduction-modal" class="ovl scroll-v scroll-styled-v">
  <div class="mdl is-inverted-dark">
    <header class="mdl-header">
      <h3 class="mdl-header-title">Welcome to TweetDuck</h3>
      <a href="#" class="mdl-dismiss link-normal-dark"><i class="icon icon-close"></i></a>
    </header>
    <div class="mdl-inner">
      <div class="mdl-content">
        <p>Thank you for downloading TweetDuck!</p>
        <p><a id="td-introduction-follow" href="#">Follow @TryMyAwesomeApp</a> for latest news and updates about the app.</p>
        <div class="main-menu"></div>
        <p><strong>Right-click anywhere</strong> or click <strong>Settings&nbsp;–&nbsp;TweetDuck</strong> in the left panel to open the main menu. You can also right-click links, tweets, images and videos, and desktop notifications to access their respective context menus.</p>
        <p>Click <strong>Show Guide</strong> to see awesome features TweetDuck offers, or view the guide later by going to <strong>About TweetDuck</strong> and clicking the help button on top.</p>
      </div>
      <footer class="txt-right">
        <div class="anondata">
          <input id="td-anonymous-data" type="checkbox" checked>
          <label for="td-anonymous-data">Send anonymous usage data</label>
          <label>&nbsp;(<a href="https://github.com/chylex/TweetDuck/wiki/Send-anonymous-data" rel="nofollow">learn more</a>)</label>
        </div>
        <button class="Button--primary" data-guide><span class="label">Show Guide</span></button>
        <button class="Button--secondary"><span class="label">Close</span</button>
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
      let showGuide = $(this)[0].hasAttribute("data-guide");
      let allowDataCollection = $("#td-anonymous-data").is(":checked");
      
      ele.fadeOut(200, function(){
        $TD.onIntroductionClosed(showGuide, allowDataCollection);
        ele.remove();
        css.remove();
      });
    });
  });
})($, $TD);
