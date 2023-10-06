using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            // SchemaƒNƒ‰ƒX‚ð‚¢‚ê‚é
            var queryResponse = await api.GetQueryDatabase<ExampleSchema>(schemaObject.databaseId).ToAsync<DatabaseQuery<ExampleSchema>>();


            queryResponse.results[0].properties.num.number = 999;

            _ = await api.PostPageDatabase(new DatabasePage<ExampleSchema>()
            {
                parent = new Parent()
                {
                    database_id = schemaObject.databaseId
                },
                properties = queryResponse.results[0].properties
            }).ToAsync<ExampleSchema>();
        }
    }
}
