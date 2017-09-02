(function($, $TD){
  $(document).one("TD.ready", function(){
    let css = $(`
<style>
#td-introduction-modal {
  display: block;
}

#td-introduction-modal .mdl {
  width: 90%;
  max-width: 680px;
  height: 296px;
}

#td-introduction-modal .mdl-header-title {
  cursor: default;
}

#td-introduction-modal .mdl-content {
  padding: 4px 16px;
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
        <p>Right-click anywhere or click <strong>Settings &ndash; TweetDuck</strong> to access the main menu.</p>
        <p>If you are using TweetDuck for the first time, check out the <strong>guide</strong> that showcases many great features TweetDuck offers and answers some common questions.</p>
        <p>You can also view the guide by opening the main menu, going to <strong>About TweetDuck</strong> and clicking the help button.</p>
      </div>
      <footer class="txt-right">
        <button class="btn btn-positive" data-guide><span class="label">Show Guide</span></button>
        <button class="btn btn-positive"><span class="label">Close</span</button>
      </footer>
    </div>
  </div>
</div>`).appendTo(".js-app");
    
    ele.find("button, a").click(function(){
      let showGuide = $(this)[0].hasAttribute("data-guide");
      
      ele.fadeOut(200, function(){
        $TD.onIntroductionClosed(showGuide);
        ele.remove();
        css.remove();
      });
    });
  });
})($, $TD);
