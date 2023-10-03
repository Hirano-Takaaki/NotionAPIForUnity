using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotionAPIForUnity.Example
{
    public class ExsampleDatabaseManager : MonoBehaviour
    {
        [SerializeField]
        DatabaseSchemaObject schemaObject;

        NotionApi api = null;

        private void Start()
        {
            api = new NotionApi(schemaObject.apiKey, true);
        }

        public async void ShowQueryDatabase()
        {
            // Schemaクラスをいれる
            var result = await api.QueryDatabase<object>(schemaObject.databaseId);
            for (int i = 0; i < result.results.Length; i++)
            {
                // 配下に生成した変数がいる
                Debug.Log(result.results[i].properties);
            }
        }
    }
}
