enabled(){
  // add a stylesheet to change tweet actions
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert(".tweet-actions { float: right !important; width: auto !important; }");
  this.css.insert(".tweet-action { opacity: 0; }");
  this.css.insert(".is-favorite .tweet-action, .is-retweet .tweet-action { opacity: 0.5; visibility: visible !important; }");
  this.css.insert(".tweet:hover .tweet-action, .is-favorite .tweet-action[rel='favorite'], .is-retweet .tweet-action[rel='retweet'] { opacity: 1; visibility: visible !important; }");
  this.css.insert(".tweet-actions > li:nth-child(4) { margin-right: 2px !important; }");

  // revert small links around the tweet
  this.prevFooterMustache = TD.mustaches["status/tweet_single_footer.mustache"];

  var footerLayout = TD.mustaches["status/tweet_single_footer.mustache"];
  footerLayout = footerLayout.replace('txt-mute txt-size--12', 'txt-mute txt-small');
  footerLayout = footerLayout.replace('{{#inReplyToID}}', '{{^inReplyToID}} <a class="pull-left margin-txs txt-mute txt-small is-vishidden-narrow" href="#" rel="viewDetails">{{_i}}Details{{/i}}</a> <a class="pull-left margin-txs txt-mute txt-small is-vishidden is-visshown-narrow" href="#" rel="viewDetails">{{_i}}Open{{/i}}</a> {{/inReplyToID}} {{#inReplyToID}}');
  footerLayout = footerLayout.replace('<span class="link-complex-target"> {{_i}}View Conversation{{/i}}', '<i class="icon icon-conversation icon-small-context"></i> <span class="link-complex-target"> <span class="is-vishidden-wide is-vishidden-narrow">{{_i}}View{{/i}}</span> <span class="is-vishidden is-visshown-wide">{{_i}}Conversation{{/i}}</span>');
  TD.mustaches["status/tweet_single_footer.mustache"] = footerLayout;

  // fix layout for right-aligned actions menu
  this.uiShowActionsMenuEvent = function(){
    $(".js-dropdown.pos-r").toggleClass("pos-r pos-l");
  };
}

ready(){
  $(document).on("uiShowActionsMenu", this.uiShowActionsMenuEvent);
}

disabled(){
  this.css.remove();
  
  $(document).off("uiShowActionsMenu", this.uiShowActionsMenuEvent);
  TD.mustaches["status/tweet_single_footer.mustache"] = this.prevFooterMustache;
}