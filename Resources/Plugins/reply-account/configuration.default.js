{
  /*
   * WARNING
   * -------
   *
   * Make sure you are editing 'configuration.js' and not the default configuration file, as the default one will be replaced with each update.
   *
   */
  
  /*
   * Simple way of configuring the plugin
   * ------------------------------------
   *
   * Set value of 'defaultAccount' to one of the following values:
   *
   *   "#preferred"  to use your preferred TweetDeck account (the one used to log into TweetDeck)
   *   "#last"       to specify the account that was selected last time
   *   "#default"    to fall back to default TweetDeck behavior; useful for advanced configuration below, otherwise disable the plugin instead
   *   "@myAccount"  to specify an account name to use; has to be one of your registered account names
   *
   */
  
  defaultAccount: "#preferred",
  
  /*
   * Advanced way of configuring the plugin
   * --------------------------------------
   *
   * This assumes a basic knowledge of JavaScript and jQuery.
   *
   * 1. Set value of 'useAdvancedSelector' to true
   * 2. Uncomment the 'customSelector' function, and replace the example code with your desired behavior
   *
   * The 'customSelector' function should return a string in one of the formats supported by 'defaultAccount'.
   * If it returns anything else (for example, false or undefined), it falls back to 'defaultAccount' behavior.
   *
   * The 'column' parameter is a TweetDeck column object. If you want to see all properties of the object, open your browser, nagivate to TweetDeck,
   * log in, and run the following code in your browser console, which will return an object containing all of the column objects mapped to their IDs:
   *   TD.controller.columnManager.getAll()
   *
   * The example below shows how to extract the column type, title, and account from the object.
   * Column type is prefixed with col_, and may be one of the following:
   *
   *   col_timeline, col_interactions, col_mentions, col_followers, col_search, col_list,
   *   col_customtimeline, col_messages, col_usertweets, col_favorites, col_activity,
   *   col_dataminr, col_home, col_me, col_inbox, col_scheduled, col_unknown
   *
   * Some of these appear to be unused (for example, Home columns are 'col_timeline' instead of 'col_home').
   *
   * If you want to see your column types, run this in your browser console:
   *   Object.values(TD.controller.columnManager.getAll()).forEach(obj => console.log(obj.getColumnType()));
   * 
   * You can also get the jQuery column object using: $("section.column[data-column='"+column.ui.state.columnKey+"']")
   *
   */
  
  useAdvancedSelector: false,
  
  /*customSelector: function(column){
    var titleObj = $(column.getTitleHTML());
    
    var columnType = column.getColumnType(); // col_timeline
    var columnTitle = titleObj.siblings(".column-head-title").text(); // Home
    var columnAccount = titleObj.siblings(".attribution").text(); // @chylexmc
    
    if (columnType === "col_search" && columnTitle === "TweetDuck"){
      // This is a search column that looks for 'TweetDuck' in the tweets,
      // search columns are normally linked to the preferred account
      // so this forces the @TryTweetDuck account to be used instead.
      return "@TryTweetDuck";
    }
    else if (columnType === "col_timeline" && columnAccount === "@chylexcz"){
      // This is a Home column of my test account @chylexcz,
      // but I want to reply to tweets from my official account.
      return "@chylexmc";
    }
    
    // otherwise returns 'undefined' which falls back to 'defaultAccount' behavior
  }*/
}