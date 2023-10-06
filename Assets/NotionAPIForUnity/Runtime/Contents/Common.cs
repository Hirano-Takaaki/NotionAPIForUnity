using System;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class IDObject
    {
        public string id;
        public string created_time;
        public string last_edited_time;
    }

    [Serializable]
    public class Database<T> : IDObject
    {
        public Text[] title;
        public T properties;
    }

    [Serializable]
    public class Parent 
    {
        public string database_id;
    }

    [Serializable]
    public class Page<T> : IDObject
    {
        public T properties;
    }

    [Serializable]
    public class DatabaseQuery<T>
    {
        public Page<T>[] results;
    }

    [Serializable]
    public class DatabasePage<T>
    {
        public Parent parent;
        public T properties;
    }

    [Serializable]
    public class DatabaseUsers
    {
        public UserObject[] results;
    }
}