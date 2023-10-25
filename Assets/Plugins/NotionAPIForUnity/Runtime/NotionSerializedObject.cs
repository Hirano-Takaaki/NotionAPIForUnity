using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    public class NotionSerializedObject : ScriptableObject
    {
        public string jsonText;

        public DatabaseQuery<T> GetInstance<T>() where T : Schema
        {
            return JsonUtility.FromJson<DatabaseQuery<T>>(jsonText);
        }
    }
}