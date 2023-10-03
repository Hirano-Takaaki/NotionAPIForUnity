using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    [CreateAssetMenu(menuName = "NotionAPIForUnity/DatabaseSchema")]
    public class DatabaseSchemaObject : ScriptableObject
    {
        public string apiKey;
        public string databaseId;
        public List<string> fieldTypes;
        public List<string> fieldNames;
    }
}