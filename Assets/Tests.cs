using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tests : MonoBehaviour
{
    [SerializeField]
    DatabaseSchemaObject schemaObject;

    NotionApi api;

    // Start is called before the first frame update
    async void Start()
    {
        api = new NotionApi(schemaObject, true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
