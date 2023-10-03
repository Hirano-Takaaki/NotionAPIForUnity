using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            DatabaseQueryResponse<ExsampleDatabaseSchema> queryResponse = null;
            void SetValue(DatabaseQueryResponse<ExsampleDatabaseSchema> val)
            {
                queryResponse = val;
            }
            await api.QueryDatabase<ExsampleDatabaseSchema>(schemaObject.databaseId, SetValue).ToAsyncProcessHandle().Task;
            for (int i = 0; i < queryResponse.results.Length; i++)
            {
                // 配下に生成した変数がいる
                Debug.Log(queryResponse.results[i].properties.name.Value);
                Debug.Log(queryResponse.results[i].properties.num.number);
                Debug.Log(queryResponse.results[i].properties.discription.rich_text.FirstOrDefault().text.content);
            }
        }
    }
}
