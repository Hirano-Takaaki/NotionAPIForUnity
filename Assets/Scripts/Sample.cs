using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    [SerializeField]
    DatabaseSchemaObject schemaObject;

    NotionApi api;
    PlayerSchema cache = null;

    [SerializeField]
    Button button1;

    [SerializeField]
    Button button2;

    [SerializeField]
    UnityEngine.UI.Text text;

    // Start is called before the first frame update
    void Start()
    {
        api = new NotionApi(schemaObject.apiKey, true);

        button1.onClick.AddListener(async () =>
        {
            var response = await api.GetQueryDatabase<PlayerSchema>(schemaObject.databaseId).ToAsync<DatabaseQuery<PlayerSchema>>();
            cache ??= response.results[0].properties;
            text.text = response.results[Random.Range(0, response.results.Length)].properties.name.GetMainValue();
        });

        button2.onClick.AddListener(async () =>
        {
            cache.name.SetMainValue("Hoge");
            _ = await api.PostPageDatabase<PlayerSchema>(new DatabasePage<PlayerSchema>()
            {
                parent = new Parent()
                {
                    database_id = schemaObject.databaseId
                },
                properties = cache
            }).ToAsync<DatabasePage<PlayerSchema>>();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
