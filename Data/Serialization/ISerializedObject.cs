namespace TweetDuck.Data.Serialization{
    interface ISerializedObject{
        bool OnReadUnknownProperty(string property, string value);
    }
}
