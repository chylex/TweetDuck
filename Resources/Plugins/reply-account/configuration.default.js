{
  /*
   * Simple way of configuring the plugin
   * ------------------------------------
   *
   * Set value of 'defaultAccount' to one of the following values:
   *
   *   "#preferred"  to use your preferred TweetDeck account (the one used to log into TweetDeck)
   *   "#last"       to specify the account that was selected last time (only updates if a single account is selected)
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
   * 2. Replace the example code in 'customSelector' function with your desired behavior
   *
   * The 'customSelector' function should return a string in one of the formats supported by 'defaultAccount'.
   * If it returns anything else (for example, false or undefined), it falls back to 'defaultAccount' behavior.
   *
   * When writing a custom function, you can install Dev Tools to access the browser console:
   *   https://tweetduck.chylex.com/guide/#dev-tools
   *
   *
   * The 'type' parameter is TweetDeck column type. Here is the full list of column types, note that some are
   * unused and have misleading names (for example, Home columns are 'col_timeline' instead of 'col_home'):
   *   col_timeline, col_interactions, col_mentions, col_followers, col_search, col_list,
   *   col_customtimeline, col_messages, col_usertweets, col_favorites, col_activity,
   *   col_dataminr, col_home, col_me, col_inbox, col_scheduled, col_unknown
   *
   * If you want to see your current column types, run this in your browser console:
   *   TD.controller.columnManager.getAllOrdered().map(obj => obj.getColumnType());
   *
   *
   * The 'title' parameter is the column title. Some are fixed (such as 'Home' or 'Notifications'),
   * some contain specific information (for example, Search columns contain the search query).
   *
   *
   * The 'account' parameter is the account name displayed next to the column title (including the @).
   * This parameter is empty for some columns (such as Messages, or Notifications for all accounts) or can
   * contain other text (for example, the Scheduled column contains the string 'All accounts').
   *
   *
   * The 'column' parameter is a TweetDeck column object. If you want to see the object structure,
   * run the following code in the console for an array containing all of your column objects:
   *   TD.controller.columnManager.getAllOrdered()
   *
   *
   * The 'isTemporary' parameter is true if the column is not attached to the main column list,
   * for example when clicking on a profile and viewing their tweets in a modal dialog.
   *
   */
  
  useAdvancedSelector: false,
  
  customSelector: function(type, title, account, column, isTemporary){
    console.info(arguments); // Prints all arguments into the console
    
    if (type === "col_search" && title === "TweetDuck"){
      // This is a search column that looks for 'TweetDuck' in the tweets,
      // search columns are normally linked to the preferred account
      // so this forces the @TryTweetDuck account to be used instead
      return "@TryTweetDuck";
    }
    else if (type === "col_timeline" && account === "@chylexcz"){
      // This is a Home column of my test account @chylexcz,
      // but I want to reply to tweets from my official account
      return "@chylexmc";
    }
    
    // otherwise returns 'undefined' which falls back to 'defaultAccount' behavior
  }
}