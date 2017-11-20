var init = function(){
  if (!("open" in document.getElementsByTagName("details")[0])){
    var elements = document.getElementsByTagName("details");
    
    var onClick = function(e){
      var summary = e.target;
      var parent = e.target.parentElement;
      var contents = e.target.nextElementSibling;
      
      if (parent.hasAttribute("open")){
        parent.removeAttribute("open");
        summary.setAttribute("aria-expanded", "false");
        contents.setAttribute("aria-hidden", "true");
        contents.style.display = "none";
      }
      else{
        parent.setAttribute("open", "");
        summary.setAttribute("aria-expanded", "true");
        contents.setAttribute("aria-hidden", "false");
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
        
        contents.setAttribute("aria-hidden", "true");
        contents.style.display = "none";
      }
    }
  }
  else if ("WebkitAppearance" in document.documentElement.style){
    var elements = document.getElementsByTagName("summary");
    
    var onMouseDown = function(e){
      e.target.classList.add("webkit-workaround");
    };
    
    var onMouseUp = function(e){
      e.target.classList.remove("webkit-workaround");
      e.target.blur();
    };
    
    for(var index = 0; index < elements.length; index++){
      elements[index].addEventListener("mousedown", onMouseDown);
      elements[index].addEventListener("mouseup", onMouseUp);
    }
  }
  
  if (location.hash.length > 1){
    var element = document.getElementById(location.hash.substring(1));
    
    if (element && element.tagName === "SUMMARY"){
      element.click();
      element.blur();
      element.scrollIntoView(true);
      
      if (window.innerWidth === 0){
        var ffs = function(){
          element.scrollIntoView(true);
          window.removeEventListener("resize", ffs);
        };
        
        window.addEventListener("resize", ffs);
      }
    }
  }
};

if (document.readyState !== "loading"){
  init();
}
else{
  document.addEventListener("DOMContentLoaded", init);
}
