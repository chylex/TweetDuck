namespace TweetDck.Migration{
    enum MigrationDecision{
        /// <summary>
        /// Copies the important files and then deletes the TweetDeck folder.
        /// </summary>
        Migrate,
        
        /// <summary>
        /// Does exactly what <see cref="Migrate"/> does, but also silently uninstalls TweetDeck.
        /// </summary>
        MigratePurge,

        /// <summary>
        /// Does not copy any files and does not ask the user about data migration again.
        /// </summary>
        Ignore,
        
        /// <summary>
        /// Does not copy any files but asks the user again when the program is re-ran.
        /// </summary>
        AskLater
    }
}
