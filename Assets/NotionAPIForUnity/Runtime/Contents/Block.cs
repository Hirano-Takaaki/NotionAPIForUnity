using System;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class NotionText
    {
        public string type;
        public TextContent text;
        public Annotations annotations;
        public string plain_text;
        public string href;


        [Serializable]
        public class TextContent
        {
            public string content;
            public string link;

            public static implicit operator string(TextContent text)
            {
                if (text == null) return null;
                return !string.IsNullOrEmpty(text.content) ? text.content : text.link;
            }
        }

        [Serializable]
        public class Annotations
        {
            public bool bold;
            public bool italic;
            public bool strikethrough;
            public bool underline;
            public bool code;
            public string color;
        }
    }

    [Serializable]
    public class Date
    {
        public string start;
        public string end;
    }
}