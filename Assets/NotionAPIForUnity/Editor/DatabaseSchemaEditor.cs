using System.IO;
using System.Text;
using UnityEngine;
using BennyKok.NotionAPI.Editor.SimpleJSON;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using System;
using UnityEditor;
using NotionAPIForUnity.Runtime;
using System.Collections;
using System.Collections.Generic;

namespace NotionAPIForUnity.Editor
{
    // Schemaからクラス生成
    // need Editor Coroutines Package
    [CustomEditor(typeof(DatabaseSchemaObject))]
    public class DatabaseSchemaEditor : UnityEditor.Editor
    {
        static readonly string PropertyNameSpace = "NotionAPIForUnity.Runtime";

        private bool busy;
        private EditorCoroutine curentCoroutine;
        private JSONNode justJsonNode = null;

        private DatabaseSchemaObject Target => target as DatabaseSchemaObject;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool isUpdateSchemaButtonDisable = busy || string.IsNullOrEmpty(Target.apiKey) || string.IsNullOrEmpty(Target.databaseId);
            bool isCreateSchemaClassButtonDisable = busy || Target.fieldNames == null || Target.fieldNames.Count == 0;

            using (var scope = new EditorGUI.DisabledGroupScope(isUpdateSchemaButtonDisable))
            {
                if (GUILayout.Button("Fetch Schema"))
                {
                    curentCoroutine = EditorCoroutineUtility.StartCoroutine(FetchSchema(), this);
                }
            }

            using (var scope = new EditorGUI.DisabledGroupScope(isCreateSchemaClassButtonDisable))
            {
                if (GUILayout.Button("Create Schema Class"))
                {
                    CreateCodeSchemaFile(Target);
                }
            }
        }

        private void OnDisable()
        {
            if (curentCoroutine == null) { return; }
            EditorCoroutineUtility.StopCoroutine(curentCoroutine);
            curentCoroutine = null;
        }

        internal IEnumerator FetchSchema()
        {
            var api = new NotionApi(Target);

            busy = true;
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return api.GetDatabaseJSON(SetJson);
            Debug.Log(json);
            justJsonNode = JSON.Parse(json);

            Target.fieldTypes.Clear();
            Target.fieldNames.Clear();
            foreach (var node in justJsonNode["properties"])
            {
                Target.fieldTypes.Add(node.Value["type"]);
                Target.fieldNames.Add(node.Key);
            }
            EditorUtility.SetDirty(Target);
            busy = false;
        }

        public void CreateCodeSchemaFile(DatabaseSchemaObject target)
        {
            var classStringBuilder = new StringBuilder();
            var className = RemoveWhitespace(target.name);
            for (var i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyTypeFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;

                var field = target.fieldNames[i];
                classStringBuilder.Append("    ");
                classStringBuilder.Append("public ");
                classStringBuilder.Append(notionType.Name);
                classStringBuilder.Append(" ");
                classStringBuilder.Append(field);
                classStringBuilder.Append(";");
                classStringBuilder.Append(Environment.NewLine);
            }

            var enumStringBuilder = new StringBuilder();
            for (var i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyTypeFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;
                if (notionType == typeof(SelectProperty))
                {
                    enumStringBuilder.Append("[Serializable]");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append("public enum ");
                    enumStringBuilder.Append($"{target.fieldNames[i]}_Enum");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append("{");
                    enumStringBuilder.Append(Environment.NewLine);
                    List<string> enumTypes = new List<string>();
                    if (justJsonNode != null)
                    {
                        foreach (var node in justJsonNode["properties"]["select"]["select"]["options"])
                        {
                            enumTypes.Add(node.Value["name"].Value);
                        }
                        foreach (var item in enumTypes)
                        {
                            enumStringBuilder.Append("    ");
                            enumStringBuilder.Append(item);
                            enumStringBuilder.Append(",");
                            enumStringBuilder.Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("キャッシュがクリアされています。再度Fetchしてください。");
                    }
                    enumStringBuilder.Append("}");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append(Environment.NewLine);

                }
                else if (notionType == typeof(MultiSelectProperty))
                {
                    enumStringBuilder.Append("[Serializable]");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append("public enum ");
                    enumStringBuilder.Append($"{char.ToUpper(target.fieldNames[i][0])}{target.fieldNames[i].Substring(1)}_Enum");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append("{");
                    enumStringBuilder.Append(Environment.NewLine);
                    HashSet<string> enumTypes = new HashSet<string>();
                    if (justJsonNode != null)
                    {
                        foreach (var node in justJsonNode["properties"]["mulitiSelect"]["multi_select"]["options"])
                        {
                            enumTypes.Add(node.Value["name"].Value);
                        }
                        foreach (var item in enumTypes)
                        {
                            enumStringBuilder.Append("    ");
                            enumStringBuilder.Append(item);
                            enumStringBuilder.Append(",");
                            enumStringBuilder.Append(Environment.NewLine);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("キャッシュがクリアされています。再度Fetchしてください。");
                    }
                    enumStringBuilder.Append("}");
                    enumStringBuilder.Append(Environment.NewLine);
                    enumStringBuilder.Append(Environment.NewLine);
                }
            }

            string generateClassStr =
$@"// 自動生成されたファイルです。手動で書き換えないでください。
using {PropertyNameSpace};
using System;

[Serializable]
public partial class {className} : Schema
{{
{classStringBuilder}
}}

{enumStringBuilder}
";
            string classStr =
$@"using {PropertyNameSpace};
using System;

public partial class {className}
{{

}}
";

            var path = Directory.GetParent(AssetDatabase.GetAssetPath(target));

            var generateClassScriptPath = Path.Combine(path.FullName, className + "_Generate" + ".cs");
            using (var writer = File.CreateText(generateClassScriptPath))
            {
                writer.Write(generateClassStr);
            }
            generateClassScriptPath = "Assets" + generateClassScriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(generateClassScriptPath);

            var classScriptPath = Path.Combine(path.FullName, className + ".cs");
            using (var writer = File.CreateText(classScriptPath))
            {
                writer.Write(classStr);
            }
            classScriptPath = "Assets" + classScriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(classScriptPath);
        }

        public Type GetPropertyTypeFromNotionType(string notionType)
        {
            switch (notionType)
            {
                case "number": return typeof(NumberProperty);
                case "title": return typeof(TitleProperty);
                case "rich_text": return typeof(TextProperty);
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