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
            api = new(schemaObject.apiKey, true);
        }

        public async void ShowQueryDatabase()
        {
            // Schema�N���X�������
            var result = await api.QueryDatabase<object>(schemaObject.databaseId);
            for (int i = 0; i < result.results.Length; i++)
            {
                // �z���ɐ��������ϐ�������
                Debug.Log(result.results[i].properties);
            }
        }
    }
}
