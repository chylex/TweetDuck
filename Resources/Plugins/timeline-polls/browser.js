enabled(){
  this.reloadColumns = () => {
    Object.values(TD.controller.columnManager.getAll()).forEach(column => column.reloadTweets());
  };
  
  // styles
  
  this.css = window.TDPF_createCustomStyle(this);
  this.css.insert("html[data-td-theme='dark'] .quoted-tweet .td-timeline-poll { color: #e1e8ed; }");
  
  // utility functions
  
  var hasPoll = function(tweet){
    return tweet.hasPoll && tweet.hasPoll();
  };
  
  var renderTweetPoll = function(tweet){
    return `<div class='td-timeline-poll'>${TD.ui.template.render("status/poll", $.extend({}, tweet, {
      chirp: tweet
    }))}</div>`;
  };
  
  var renderPollHook = function(tweet, html){
    let ele = null;
    
    if (hasPoll(tweet)){
      (ele || (ele = $(html))).find(".js-tweet-body").first().children("div").last().after(renderTweetPoll(tweet));
    }
    
    if (tweet.quotedTweet && hasPoll(tweet.quotedTweet)){
      (ele || (ele = $(html))).find(".js-quoted-tweet-text").first().after(renderTweetPoll(tweet.quotedTweet));
    }
    
    if (ele){
      ele.find(".js-card-container").css("display", "none");
      return ele.prop("outerHTML");
    }
    else{
      return html;
    }
  };
  
  // hooks
  
  var funcs = {
    TwitterStatus: TD.services.TwitterStatus.prototype.render,
    TwitterActionOnTweet: TD.services.TwitterActionOnTweet.prototype.render,
    TweetDetailView: TD.components.TweetDetailView.prototype._renderChirp
  };
  
  TD.services.TwitterStatus.prototype.render = function(e){
    return renderPollHook(this, funcs.TwitterStatus.apply(this, arguments));
  };
  
  TD.services.TwitterActionOnTweet.prototype.render = function(e){
    return renderPollHook(this.targetTweet, funcs.TwitterActionOnTweet.apply(this, arguments));
  };
  
  TD.components.TweetDetailView.prototype._renderChirp = function(){
    let result = funcs.TweetDetailView.apply(this, arguments);
    
    if (this.mainChirp.quotedTweet && hasPoll(this.mainChirp.quotedTweet)){
      $(this.$tweetDetail).find(".js-quoted-tweet-text").first().after(renderTweetPoll(this.mainChirp.quotedTweet));
    }
    
    return result;
  };
  
  this.prevRenderFuncs = funcs;
  this.reloadColumns();
}

disabled(){
  TD.services.TwitterStatus.prototype.render = this.prevRenderFuncs.TwitterStatus;
  TD.services.TwitterActionOnTweet.prototype.render = this.prevRenderFuncs.TwitterActionOnTweet;
  TD.components.TweetDetailView.prototype._renderChirp = this.prevRenderFuncs.TweetDetailView;
  
  this.css.remove();
  this.reloadColumns();
}