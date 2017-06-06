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
  
  this.css.insert(".template-editor { height: 100%; flex: 0 0 320px; overflow-y: auto; background-color: #485865; }");
  this.css.insert(".template-editor-form { flex: 1 1 auto; padding: 12px 16px; font-size: 14px; }");
  this.css.insert(".template-editor-form .compose-text-title { margin: 16px 0 9px; }");
  this.css.insert(".template-editor-form .compose-text-title:first-child { margin-top: 0; }");
  this.css.insert(".template-editor-form input, .template-editor-form textarea { color: #111; background-color: #fff; border: none; border-radius: 0; }");
  this.css.insert(".template-editor-form input:focus, .template-editor-form textarea:focus { box-shadow: inset 0 1px 3px rgba(17, 17, 17, 0.1), 0 0 8px rgba(80, 165, 230, 0.6); }");
  this.css.insert(".template-editor-form input { margin-bottom: 16px; }");
  this.css.insert(".template-editor-form textarea { height: 130px; font-size: 14px; padding: 10px; resize: none; }");
  this.css.insert(".template-editor-form .template-editor-tips-button { cursor: pointer; }");
  this.css.insert(".template-editor-form .template-editor-tips-button .icon { vertical-align: -10%; margin-left: 4px; }");
  this.css.insert(".template-editor-form .template-editor-tips { display: none; }");
  
  // modal dialog
  
  var showTemplateModal = () => {
    var html = `
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
        <input type="text">
        
        <div class="compose-text-title">Contents</div>
        <textarea class="compose-text txt-size--14 scroll-v scroll-styled-v scroll-styled-h scroll-alt padding-a--0"></textarea>

        <div class="compose-text-title template-editor-tips-button">Advanced <i class="icon icon-small icon-arrow-d"></i></div>
        <div class="template-editor-tips">
          <p>You can use the following tokens: TODO</p>
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
