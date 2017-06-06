enabled(){
  
  // button
  
  var buttonHTML = '<button class="manage-templates-btn needsclick btn btn-on-blue full-width txt-left margin-b--12 padding-v--9"><i class="icon icon-bookmark"></i><span class="label padding-ls">Manage templates</span></button>';
  
  this.prevComposeMustache = TD.mustaches["compose/docked_compose.mustache"];
  TD.mustaches["compose/docked_compose.mustache"] = TD.mustaches["compose/docked_compose.mustache"].replace('<div class="js-tweet-type-button">', buttonHTML+'<div class="js-tweet-type-button">');
  
  var dockedComposePanel = $(".js-docked-compose");
  
  if (dockedComposePanel.length){
    dockedComposePanel.find(".js-tweet-type-button").first().before(buttonHTML);
  }
  
  // css
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".templates-modal-wrap { width: 100%; height: 100%; padding: 49px; position: absolute; z-index: 999; box-sizing: border-box; background-color: rgba(0, 0, 0, 0.5); }");
  this.css.insert(".templates-modal { width: 100%; height: 100%; background-color: #fff; display: flex; }"); // TODO fix things when the columns are scrolled to left
  this.css.insert(".templates-modal > div { display: flex; flex-direction: column; }");
  this.css.insert(".templates-modal-bottom { flex: 0 0 auto; padding: 16px; text-align: right; }");
  this.css.insert(".templates-modal-bottom button { margin-left: 4px; }");
  
  this.css.insert(".template-list { height: 100%; flex: 1 1 auto; }");
  this.css.insert(".template-list ul { list-style-type: none; font-size: 24px; color: #222; flex: 1 1 auto; padding: 12px; overflow-y: auto; }");
  this.css.insert(".template-list li { display: block; width: 100%; padding: 4px 8px; box-sizing: border-box; cursor: pointer; }");
  this.css.insert(".template-list li:hover { background-color: #d8d8d8; }");
  this.css.insert(".template-list li span { white-space: nowrap; }");
  this.css.insert(".template-list li .icon { opacity: 0.6; margin-left: 4px; padding: 3px; }");
  this.css.insert(".template-list li .icon:hover { opacity: 1; }");
  this.css.insert(".template-list li .template-actions { float: right; }");
  
  this.css.insert(".template-editor { height: 100%; flex: 0 0 auto; width: 25vw; min-width: 150px; max-width: 400px; background-color: #485865; }");
  this.css.insert(".template-editor-form { flex: 1 1 auto; padding: 12px 16px; font-size: 14px; overflow-y: auto; }");
  this.css.insert(".template-editor-form .compose-text-title { margin: 24px 0 9px; }");
  this.css.insert(".template-editor-form .compose-text-title:first-child { margin-top: 0; }");
  this.css.insert(".template-editor-form input, .template-editor-form textarea { color: #111; background-color: #fff; border: none; border-radius: 0; }");
  this.css.insert(".template-editor-form input:focus, .template-editor-form textarea:focus { box-shadow: inset 0 1px 3px rgba(17, 17, 17, 0.1), 0 0 8px rgba(80, 165, 230, 0.6); }");
  this.css.insert(".template-editor-form textarea { height: 146px; font-size: 14px; padding: 10px; resize: none; }");
  this.css.insert(".template-editor-form .template-editor-tips-button { cursor: pointer; }");
  this.css.insert(".template-editor-form .template-editor-tips-button .icon { font-size: 12px; vertical-align: -5%; margin-left: 4px; }");
  this.css.insert(".template-editor-form .template-editor-tips { display: none; }");
  this.css.insert(".template-editor-form .template-editor-tips p { margin: 0 0 10px; }");
  this.css.insert(".template-editor-form .template-editor-tips li:nth-child(2n+1) { margin-top: 5px; padding-left: 6px; font-family: monospace; }");
  this.css.insert(".template-editor-form .template-editor-tips li:nth-child(2n) { margin-top: 1px; padding-left: 14px; opacity: 0.66; }");
  
  // modal dialog
  
  var showTemplateModal = () => {
    let html = `
<div class="templates-modal-wrap">
  <div class="templates-modal">
    <div class="template-list">
      <ul>
        <li><span class="template-name">Test template 1</span><span class="template-actions"><i class="icon icon-edit"></i><i class="icon icon-close"></i></span></li>
        <li><span class="template-name">Test template 2</span><span class="template-actions"><i class="icon icon-edit"></i><i class="icon icon-close"></i></span></li>
      </ul>
      
      <div class="templates-modal-bottom">
        <button class="btn btn-positive"><i class="icon icon-plus icon-small padding-rs"></i><span class="label">New Template</span></button>
      </div>
    </div>

    <div class="template-editor">
      <div class="template-editor-form">
        <div class="compose-text-title">Template Name</div>
        <input name="template-name" type="text">
        
        <div class="compose-text-title">Contents</div>
        <textarea name="template-contents" class="compose-text scroll-v scroll-styled-v scroll-styled-h scroll-alt"></textarea>

        <div class="compose-text-title template-editor-tips-button">Advanced <i class="icon icon-arrow-d"></i></div>
        <div class="template-editor-tips">
          <p>You can use the following tokens. All tokens except for <span style="font-family: monospace">{ajax}</span> can only be used once.</p>
          <ul>
            <li>{cursor}</li>
            <li>Location where the cursor is placed</li>
            <li>{cursor#&lt;selectionlength&gt;}</li>
            <li>Places cursor and selects a set amount of characters</li>
            <li>{paste}</li>
            <li>Paste text or an image from clipboard</li>
            <li>{paste#text}</li>
            <li>Paste only if clipboard has text</li>
            <li>{paste#image}</li>
            <li>Paste only if clipboard has an image</li>
            <li>{ajax#&lt;url&gt;}</li>
            <li>Replaced with the result of an ajax request</li>
            <li>{ajax#&lt;eval&gt;#&lt;url&gt;}</li>
            <li>Allows parsing the ajax request using <span style="font-family: monospace">$</span> as the placeholder for the result<br>Example: <span style="font-family: monospace">$.substring(0,5)</span></li>
          </ul>
        </div>
      </div>
      
      <div class="templates-modal-bottom">
        <button class="btn"><i class="icon icon-close icon-small padding-rs"></i><span class="label">Cancel</span></button>
        <button class="btn btn-positive"><i class="icon icon-check icon-small padding-rs"></i><span class="label">Confirm</span></button>
      </div>
    </div>
  </div>
</div>`;
    
    $(".js-app-content").prepend(html);
    
    let ele = $(".templates-modal-wrap").first();
    
    ele.on("click", ".template-editor-tips-button", function(e){
      $(this).children(".icon").toggleClass("icon-arrow-d icon-arrow-u");
      ele.find(".template-editor-tips").toggle();
    });
  };
  
  // event handlers
  
  this.manageTemplatesButtonClickEvent = function(e){
    let wrap = $(".templates-modal-wrap");
    
    if (wrap.length){
      wrap.remove();
    }
    else{
      showTemplateModal();
    }
    
    $(this).blur();
  };
}

ready(){
  $(".manage-templates-btn").on("click", this.manageTemplatesButtonClickEvent);
}

disabled(){
  this.css.remove();
  
  $(".manage-templates-btn").remove();
  $(".templates-modal-wrap").remove();
  
  TD.mustaches["compose/docked_compose.mustache"] = this.prevComposeMustache;
}
