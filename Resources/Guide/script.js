document.addEventListener("DOMContentLoaded", function(){
  if (!("open" in document.getElementsByTagName("details")[0])){
    var elements = document.getElementsByTagName("details");
    
    var onClick = function(e){
      var summary = e.target;
      var parent = e.target.parentElement;
      var contents = e.target.nextElementSibling;
      
      if (parent.hasAttribute("open")){
        parent.removeAttribute("open");
        summary.setAttribute("aria-expanded", "false");
        contents.style.display = "none";
      }
      else{
        parent.setAttribute("open", "");
        summary.setAttribute("aria-expanded", "true");
        contents.style.display = "block";
      }
    };
    
    var onKey = function(e){
      if (e.keyCode === 13 || e.keyCode === 32){
        onClick(e);
      }
    };
    
    for(var index = 0; index < elements.length; index++){
      var ele = elements[index];
      
      if (ele.childElementCount === 2){
        var summary = ele.children[0];
        var contents = ele.children[1];
        
        ele.style.display = "block";
        ele.setAttribute("role", "group");
        
        summary.setAttribute("role", "button");
        summary.setAttribute("aria-expanded", "false");
        summary.setAttribute("tabindex", 0);
        summary.addEventListener("click", onClick);
        summary.addEventListener("keydown", onKey);
        
        contents.style.display = "none";
      }
    }
  }
});
