using System;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class IDObject
    {
        public string id;
        [NonSerialized]
        public string created_time;
        [NonSerialized]
        public string last_edited_time;
    }

    [Serializable]
    public class Database<T> : IDObject where T : Schema
    {
        public Text[] title;
        public T properties;
    }

    // Json用
    [Serializable]
    public class Parent
    {
        public string database_id;

        public Parent(string database_id)
        {
            this.database_id = database_id ?? throw new ArgumentNullException(nameof(database_id));
        }
    }

    [Serializable]
    public class DatabaseQuery<T> where T : Schema
    {
        public DatabasePage<T>[] results;

        public DatabasePage<T>[] ToDatabase() => results;
    }


    [Serializable]
    public class DatabasePage<T> : IDObject where T : Schema
    {
        public T properties;
        public Parent parent;

        public string DatabaseId => parent.database_id;

        public T DatabaseData => properties;
    }

    [Serializable]
    public class DatabaseUsers
    {
        public UserObject[] results;
    }
}