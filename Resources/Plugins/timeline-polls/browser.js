constructor(){
  super({
    requiresPageReload: true
  });
}

enabled(){
  // add a stylesheet
  window.TDPF_createCustomStyle(this).insert(".column-detail .timeline-poll-container { display: none }");
  
  // setup layout injecting
  var injectLayout = (mustache, onlyIfNotFound, search, replace) => {
    if (TD.mustaches[mustache].indexOf(onlyIfNotFound) === -1){
      TD.mustaches[mustache] = TD.mustaches[mustache].replace(search, replace);
    }
  };

  // add poll rendering to tweets
  injectLayout("status/tweet_single.mustache", "status/poll", "{{/quotedTweetMissing}} {{#translation}}", "{{/quotedTweetMissing}} <div class='timeline-poll-container'>{{>duck/tweet_single/poll}}</div> {{#translation}}");
  TD.mustaches["duck/tweet_single/poll.mustache"] = '<div class="js-poll margin-tl"> {{#poll}}  <ul class="margin-b--12"> {{#choices}} <li class="position-rel margin-b--8 height-3"> <div class="poll-bar pin-top height-p--100 br-1 {{#isWinner}}poll-bar--winner{{/isWinner}} {{#hasTimeLeft}}br-left{{/hasTimeLeft}} width-p--{{percentage}}"/>  <div class="poll-label position-rel padding-a--4"> <span class="txt-bold txt-right inline-block width-5 padding-r--4">{{percentage}}%</span> {{{label}}} {{#isSelectedChoice}} <i class="icon icon-check txt-size-variable--11"></i> {{/isSelectedChoice}} </div> </li> {{/choices}} </ul> <span class="inline-block txt-small padding-ls txt-seamful-deep-gray"> {{{prettyCount}}} &middot; {{#hasTimeLeft}} {{{prettyTimeLeft}}} {{/hasTimeLeft}} {{^hasTimeLeft}} {{_i}}Final results{{/i}} {{/hasTimeLeft}} </span> {{/poll}} </div>';
}

disabled(){
  // not needed, plugin reloads the page when enabled or disabled
}