using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotionAPIForUnity.Editor
{
    using UnityEngine;
    using System.IO;
    using UnityEditor.AssetImporters;
    using NotionAPIForUnity.Runtime;

    [ScriptedImporter(1, "nojson")]
    public class NojsonImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var json = File.ReadAllText(ctx.assetPath);

            var so = ScriptableObject.CreateInstance<NotionSerializedObject>();
            so.jsonText = json;

            ctx.AddObjectToAsset("NoJson", so);
            ctx.SetMainObject(so);
        }
    }
}
