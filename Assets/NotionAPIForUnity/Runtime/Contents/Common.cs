using System;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class IDObject
    {
        public string id;
        public string createdTime;
        public string lastEditedTime;
    }

    [Serializable]
    public class Database<T> : IDObject
    {
        public NotionText[] title;
        public T properties;
    }

    [Serializable]
    public class Page<T> : IDObject
    {
        public T properties;
    }

    [Serializable]
    public class DatabaseQueryResponse<T>
    {
        public Page<T>[] results;
    }

    [Serializable]
    public class DatabaseUsers
    {
        public UserObject[] results;
    }
}