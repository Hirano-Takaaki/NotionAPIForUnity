﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NotionAPIForUnity.Runtime;
using System.Text;

namespace NotionAPIForUnity.Editor
{
    [CustomEditor(typeof(NotionSerializedObject))]
    public class NotionSerializedObjectDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            NotionSerializedObject noJson = target as NotionSerializedObject;

            /* -- カスタム表示 -- */
            EditorGUILayout.TextArea(ToReadable(noJson.jsonText));
        }

        public string ToReadable(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            int i = 0;
            int indent = 0;
            int quoteCount = 0;
            int position = -1;
            var sb = new StringBuilder();
            int lastindex = 0;

            while (true)
            {
                if (i > 0 && json[i] == '"' && json[i - 1] != '\\') quoteCount++;

                if (quoteCount % 2 == 0)
                {
                    if (json[i] == '{' || json[i] == '[')
                    {
                        indent++;
                        position = 1;
                    }
                    else if (json[i] == '}' || json[i] == ']')
                    {
                        indent--;
                        position = 0;
                    }
                    else if (json.Length > i && json[i] == ',' && json[i + 1] == '"')
                    {
                        position = 1;
                    }
                    if (position >= 0)
                    {
                        sb.AppendLine(json.Substring(lastindex, i + position - lastindex));
                        sb.Append(new string(' ', indent * 4));
                        lastindex = i + position;
                        position = -1;
                    }
                }

                i++;

                if (json.Length <= i)
                {
                    sb.Append(json.Substring(lastindex));
                    break;
                }

            }
            return sb.ToString();
        }
    }
}
