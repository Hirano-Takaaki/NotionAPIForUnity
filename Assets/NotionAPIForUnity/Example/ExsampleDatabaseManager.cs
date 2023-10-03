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
            // Schema�N���X�������
            var queryResponse = await api.QueryDatabase<ExampleSchema>(schemaObject.databaseId).ToAsync<DatabaseQueryResponse<ExampleSchema>>() ;
            for (int i = 0; i < queryResponse.results.Length; i++)
            {
                // �z���ɐ��������ϐ�������
                Debug.Log(queryResponse.results[i].properties.name.Value);
                Debug.Log(queryResponse.results[i].properties.num.number);
                Debug.Log(queryResponse.results[i].properties.discription.rich_text.FirstOrDefault().text.content);
            }
        }
    }
}
