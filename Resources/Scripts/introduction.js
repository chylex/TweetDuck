(function($, $TD){
  $(document).one("TD.ready", function(){
    let css = $(`
<style>
#td-introduction-modal {
  display: block;
}

#td-introduction-modal .mdl {
  width: 90%;
  max-width: 800px;
  height: 372px;
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
        <p><strong>Right-click anywhere</strong> or click <strong>Settings – TweetDuck</strong> in the left panel to open the main menu, where you can access the <strong>Options</strong>, <strong>Plugins</strong>, and more.</p>
        <p>You can also right-click links, media, tweets, notifications, etc. to access their context menu.</p>
        <p>If you're using TweetDuck for the first time, check out the <strong>guide</strong> that answers common questions and showcases important features. You can open the main menu, select <strong>About TweetDuck</strong>, and click the help button to view the guide later.</p>
        <p>Before you continue, please consider helping development by allowing TweetDuck to send anonymous data in the future. You can always disable it in <strong>Options – Feedback</strong>.</p>
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
