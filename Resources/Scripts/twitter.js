//
// Block: Inject custom CSS and layout into the page.
//
setTimeout(function(){
  var style = document.createElement("style");
  document.head.appendChild(style);
  
  style.sheet.insertRule("body { overflow: hidden !important; }", 0); // remove scrollbar
  style.sheet.insertRule(".topbar { display: none !important; }", 0); // hide top bar
  style.sheet.insertRule(".page-canvas, .buttons, .btn, input { border-radius: 0 !important; }", 0); // sharpen borders
  style.sheet.insertRule("input { padding: 5px 8px 4px !important; }", 0); // tweak input padding
  
  style.sheet.insertRule("#doc { width: 100%; height: 100%; margin: 0; position: absolute; display: table; }", 0); // center everything
  style.sheet.insertRule("#page-outer { display: table-cell; vertical-align: middle; }", 0); // center everything
  style.sheet.insertRule("#page-container { padding: 0 20px !important; width: 100% !important; box-sizing: border-box !important; }", 0); // center everything
  style.sheet.insertRule(".page-canvas { margin: 0 auto !important; }", 0); // center everything
  
  if (location.pathname === "/logout"){
    style.sheet.insertRule(".page-canvas { width: auto !important; max-width: 888px; }", 0); // fix min width
    style.sheet.insertRule(".signout-wrapper { width: auto !important; }", 0); // fix min width
    style.sheet.insertRule(".btn { margin: 0 4px !important; }", 0); // add margin around buttons
    style.sheet.insertRule(".btn.cancel { border: 1px solid #bbc1c5 !important; }", 0); // add border to cancel button
    style.sheet.insertRule(".aside p { display: none; }", 0); // hide text below the logout dialog
  }
}, 1);
