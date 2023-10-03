using System.IO;
using System.Text;
using UnityEngine;
using BennyKok.NotionAPI.Editor.SimpleJSON;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using NotionAPIForUnity.Runtime;
using System.Linq;
using System.Reflection;

namespace NotionAPIForUnity.Editor
{
    // Schemaからクラス生成
    [CustomEditor(typeof(DatabaseSchemaObject))]
    public class DatabaseSchemaEditor : UnityEditor.Editor
    {
        static readonly string PropertyNameSpace = "NotionAPIForUnity.Runtime";

        private bool busy;

        private DatabaseSchemaObject Target => target as DatabaseSchemaObject;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool isUpdateSchemaButtonDisable = busy || string.IsNullOrEmpty(Target.apiKey) || string.IsNullOrEmpty(Target.databaseId);
            bool isCreateSchemaClassButtonDisable = busy || Target.fieldNames == null || Target.fieldNames.Count == 0;

            using (EditorGUI.DisabledGroupScope scope = new(isUpdateSchemaButtonDisable))
            {
                if (GUILayout.Button("Fetch Schema"))
                {
                    FetchSchema();
                }
            }

            using (EditorGUI.DisabledGroupScope scope = new(isCreateSchemaClassButtonDisable))
            {
                if (GUILayout.Button("Create Schema Class"))
                {
                    CreateCodeSchemaFile(Target);
                }
            }
        }

        internal async Awaitable FetchSchema()
        {
            var api = new NotionApi(Target.apiKey, true);

            busy = true;

            var json = await api.GetDatabaseJSON(Target.databaseId);
            Debug.Log(json);
            var parseJson = JSON.Parse(json);

            Target.fieldTypes.Clear();
            Target.fieldNames.Clear();
            foreach (var node in parseJson["properties"])
            {
                Target.fieldTypes.Add(node.Value["type"]);
                Target.fieldNames.Add(node.Key);
            }
            EditorUtility.SetDirty(Target);
            busy = false;
        }

        public void CreateCodeSchemaFile(DatabaseSchemaObject target)
        {
            var sb = new StringBuilder();
            var className = RemoveWhitespace(target.name);
            for (var i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyTypeFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;

                var field = target.fieldNames[i];
                sb.Append("    ");
                sb.Append("public ");
                sb.Append(notionType.Name);
                sb.Append(" ");
                sb.Append(field);
                sb.Append(";");
                sb.Append(Environment.NewLine);
            }

            string result =
$@"// 自動生成されたファイルです。手動で書き換えないでください。
using {PropertyNameSpace};
using System;

[Serializable]
public class {className}
{{
{sb}
}}
";

            var path = Directory.GetParent(AssetDatabase.GetAssetPath(target));
            var scriptPath = Path.Combine(path.FullName, className + ".cs");
            using (var writer = File.CreateText(scriptPath))
            {
                writer.Write(result);
            }

            scriptPath = "Assets" + scriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(scriptPath);
        }

        public Type GetPropertyTypeFromNotionType(string notionType)
        {
            switch (notionType)
            {
                case "number": return typeof(NumberProperty);
                case "title": return typeof(TitleProperty);
                case "rich_text": return typeof(RichTextProperty);
                case "multi_select": return typeof(MultiSelectProperty);
                case "select": return typeof(SelectProperty);
                case "checkbox": return typeof(CheckboxProperty);
                case "date": return typeof(DateProperty);
                case "formula": return typeof(FormulaStringProperty);
                case "people": return typeof(PeopleProperty);
            }
            return null;
        }

        private string RemoveWhitespace(string text)
        {
            return Regex.Replace(text, @"\s", "");
        }
    }
}