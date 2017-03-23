run(){
  var extend = function(target, source){
    for(let prop in source){
      target[prop] = source[prop];
    }
    
    return target;
  };
  
  const configFile = "config.json";
  
  $TDP.readFile(this.$token, configFile, true).then(contents => {
    try{
      loadConfigObject(extend(this.defaultConfig, JSON.parse(contents)));
    }catch(err){
      loadConfigObject(this.defaultConfig);
    }
  }).catch(err => {
    loadConfigObject(this.defaultConfig);
  });
  
  // config handling
  this.defaultConfig = {
    fontSize: "12px",
    avatarRadius: 10
  };
  
  var loadConfigObject = config => {
    let css = window.TDPF_createCustomStyle(this);
    css.insert(".txt-base-smallest, .txt-base-largest { font-size: "+config.fontSize+" !important }");
    css.insert(".avatar { border-radius: "+config.avatarRadius+"% !important }");
  };
}
