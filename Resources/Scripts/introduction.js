(function($, $TD){
  $(document).one("TD.ready", function(){
    let css = $(`
<style>
#td-introduction-modal {
  display: block;
}

#td-introduction-modal .mdl {
  width: 90%;
  max-width: 830px;
  height: 328px;
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

#td-introduction-modal .main-menu {
  float: left;
  width: 187px;
  height: 124px;
  margin-right: 12px;
  box-shadow: 2px 2px 3px rgba(0, 0, 0, 0.33);
  background: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAALsAAAB8CAMAAAAGozFUAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAACZUExURQAAAAAANgAAYQA2YQA2iABhYQBhrDYAADYANjYAYTY2ADY2NjY2YTY2iDZhrDaIiDaIz2EAAGEANmEAYWE2NmE2iGFhNmFhYWGs8og2AIg2Nog2YYiIYYisiIjPrIjPz4jP8qxhAKxhNqzPiKzyrKzy8szMzM+INs/PiM/yrM/yz8/y8tfX1/Dw8PKsYfLPiPLyrPLyz/Ly8nG9GIQAAARDSURBVHja7ZyPd9IwEMdDxbI5cemcWJjbWrWrDi2Q//+PM7m7pOGHfcORlD6v7w1ok2wfwqXkvrs78TTcQzypoR6DZ//dcTA7szM7s8dgX2dCiJxGrG+Kvd+C1w619M+uoZqkGiy7AxvivI+XajURSWVO9AtjQ6UQqTIn18R+PxFSP82SCnuUOYxdfdjMzSmNn9mPMJa9p/QWpH5cZ5rjgqbanNQjOMnGS315nWl+7NGkqrmSqpb6hRuvm+PO++qygtkWKZ2YOW2EGBWr6dK3mTKHmcYeuu3r/fR5UawmUrXjY9tMLRFItez6abPYYd/MffbN4uHT89033UPVInfje1ir+Glbm7msGjKR1mbg/UFv7KFqYzGfwUhq6cbHZle1Jp3QZ65fjMwcize3hbEcu1ZnZkVCb+wBq8L86D4JGl1sm+E9AbMzO7MzO7MzO7Mze586QQqdcav7ggO3juMfF8Whlsj+6hXuzx17x0aWmswO/nBLbPaZ8RqatzdHsO/36In94WOltAOnXSZwoR7A+Qa3H5q/CEH+aA5+OfaQ6ASCYGAuZq3nAi4KjML2oH6T9qxX05+W3T5pt5+WQ5NUpA1s99jMpZt05zpiPxzVpOF1gukSFACPDN3+1p0l/3qbHf1rVBQO9gMFISz7ZnE/Xe6wX3oqXzc7KgoH+4GCEFoXA8VL20CuGprCTG6xkzZwyGZIUTjUDxSEwOybu4regzC3mxLXqm8zVhsoYUXai2aF5qQolL7KQB1QQeDvVWZndmZndmZn9nNn/3WuB9sMszM7szM7sw+WXXvEQgYTRYOyNxA9IEMJiyHZjSijWi1pUOxIDcqWiao6uSgalN0E70BYT7YncJ1CFI0074Uvg55KFI1i715U1SlF0bD3maRSGH4nfbn0VKJo+Pt77qKqTi2KRvlePZObI7PzXozZmZ3ZmZ3Zmf1/YjdO3qjY3xH0rBW8PN9jvDyv3cyR+R7DZvdCNq7pQk8iwfE243l7NUTR9CYSHLNWk8qPRsKcJu9CdJHgmHlXqos9vkhwPLsNuWptpi+R4Hh2G3KF+Vgte3yR4FXfq620Orw9QTleDpLd3M6Tvqed95HMzuzMzuzMzuw9sJep6v7XQSnEXsyBd0AZmN3WVzrAL2Rfvb8t/vK3OgukKM/ddelpkdlraVyLV7LvbZujsLuyG1CkxcURtGlalsQVbaHELfP43YYfeP9WRmGBfPWw7Bpc47dFWnaTsBy7K9ri6r3Q4w47BrSsbx7neeh5NwZTp7ZIy34ik2P3iraYxK22aguyb+c46RfvZGibgZsElfzRc9fB7oq2uJysHXvfZs+u0tDspl4RhqBgOAHGEXhpWu3Ks0VbqF6Lq9piC7vQIGszRSkDs+MfqFMbTkBJWF6almO3RVsocQsfS0yihOxXGgQpW2D4/+728p6A2Zmd2Zmd2c+SfaCHZh/u8QdspaeBZ15I7gAAAABJRU5ErkJggg==);
}

#td-introduction-modal .main-menu + p {
  margin-top: 15px;
}

#td-introduction-modal footer {
  padding: 10px 0;
}

#td-introduction-modal button {
  margin-left: 8px;
}

#td-introduction-modal .anondata {
  float: left;
  margin: 5px 7px;
}

#td-introduction-modal .anondata input {
  vertical-align: -10%;
}

#td-introduction-modal .anondata label {
  cursor: pointer;
  display: inline-block;
  font-size: 14px;
}
</style>`).appendTo(document.head);
    
    let ele = $(`
<div id="td-introduction-modal" class="ovl">
  <div class="mdl is-inverted-dark">
    <header class="mdl-header">
      <h3 class="mdl-header-title">Welcome to TweetDuck</h3>
      <a href="#" class="mdl-dismiss link-normal-dark"><i class="icon icon-close"></i></a>
    </header>
    <div class="mdl-inner">
      <div class="mdl-content">
        <p>Thank you for downloading TweetDuck!</p>
        <p><a id="td-introduction-follow" href="#">Follow @TryTweetDuck</a> for latest news and updates about the app.</p>
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
        <button class="btn btn-positive" data-guide><span class="label">Show Guide</span></button>
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
        TD.controller.clients.getPreferredClient().getUsersByIds([ "731137856052269056" ], users => onSuccess(users[0]), onError);
      }
    };
    
    loadTweetDuckUser(user => tdUser = user);
    
    ele.find("#td-introduction-follow").click(function(){
      loadTweetDuckUser(user => {
        $(document).trigger("uiShowFollowFromOptions", { userToFollow: user });
        
        $(".js-modals-container").find("header a[rel='user']").each(function(){
          this.outerHTML = this.innerText;
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
