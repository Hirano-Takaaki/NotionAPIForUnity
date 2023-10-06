using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NotionAPIForUnity.Example
{
    public class ExampleDatabaseManager : MonoBehaviour
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
            // �f�[�^�x�[�X�擾
            var queryResponse = await api.GetQueryDatabase<ExampleSchema>(schemaObject.databaseId).ToAsync<DatabaseQuery<ExampleSchema>>();

            // �����̃f�[�^�̏�������
            queryResponse.results[0].properties.name.title.FirstOrDefault().text.content = "HogeName";
            _ = await api.PatchPageDatabase(queryResponse.results[0].id, new DatabasePage<ExampleSchema>()
            {
                parent = new Parent()
                {
                    database_id = schemaObject.databaseId
                },
                properties = queryResponse.results[0].properties
            }).ToAsync<ExampleSchema>();

            // �V�K���ڂ̒ǉ�
            var baseProp = new DatabasePage<ExampleSchema>()
            {
                parent = new Parent()
                {
                    database_id = schemaObject.databaseId
                },
                properties = queryResponse.results[0].properties
            };

            baseProp.properties.name.SetMainValue("PiyoPiyo");
            baseProp.properties.num.SetMainValue(99.9f);
            _ = await api.PostPageDatabase(baseProp).ToAsync<ExampleSchema>();
        }
    }
}
