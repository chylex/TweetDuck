enabled(){
  this.css = window.TDPF_createCustomStyle(this);
  this.prevFooterMustache = TD.mustaches["status/tweet_single_footer.mustache"];
  
  // load configuration
  window.TDPF_loadConfigurationFile(this, "configuration.js", "configuration.default.js", config => {
    if (config.hideTweetActions){
      this.css.insert(".tweet-action { opacity: 0; }");
      this.css.insert(".is-favorite .tweet-action, .is-retweet .tweet-action { opacity: 0.5; visibility: visible !important; }");
      this.css.insert(".tweet:hover .tweet-action, .is-favorite .tweet-action[rel='favorite'], .is-retweet .tweet-action[rel='retweet'] { opacity: 1; visibility: visible !important; }");
    }
    
    if (config.moveTweetActionsToRight){
      this.css.insert(".tweet-actions { float: right !important; width: auto !important; }");
      this.css.insert(".tweet-actions > li:nth-child(4) { margin-right: 2px !important; }");
    }
    
    if (config.smallComposeTextSize){
      this.css.insert(".compose-text { font-size: 12px !important; height: 120px !important; }");
    }
    
    if (config.revertConversationLinks){
      var footer = TD.mustaches["status/tweet_single_footer.mustache"];
      footer = footer.replace('txt-mute txt-size--12', 'txt-mute txt-small');
      footer = footer.replace('{{#inReplyToID}}', '{{^inReplyToID}} <a class="pull-left margin-txs txt-mute txt-small is-vishidden-narrow" href="#" rel="viewDetails">{{_i}}Details{{/i}}</a> <a class="pull-left margin-txs txt-mute txt-small is-vishidden is-visshown-narrow" href="#" rel="viewDetails">{{_i}}Open{{/i}}</a> {{/inReplyToID}} {{#inReplyToID}}');
      footer = footer.replace('<span class="link-complex-target"> {{_i}}View Conversation{{/i}}', '<i class="icon icon-conversation icon-small-context"></i> <span class="link-complex-target"> <span class="is-vishidden-wide is-vishidden-narrow">{{_i}}View{{/i}}</span> <span class="is-vishidden is-visshown-wide">{{_i}}Conversation{{/i}}</span>');
      TD.mustaches["status/tweet_single_footer.mustache"] = footer;
    }
    
    if (config.moveTweetActionsToRight){
      $(document).on("uiShowActionsMenu", this.uiShowActionsMenuEvent);
    }
  });
  
  // fix layout for right-aligned actions menu
  this.uiShowActionsMenuEvent = function(){
    $(".js-dropdown.pos-r").toggleClass("pos-r pos-l");
  };
}

disabled(){
  this.css.remove();
  TD.mustaches["status/tweet_single_footer.mustache"] = this.prevFooterMustache;
  
  $(document).off("uiShowActionsMenu", this.uiShowActionsMenuEvent);
}
