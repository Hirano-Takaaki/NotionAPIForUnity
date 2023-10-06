using System;
using System.Globalization;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class Text
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
            [NonSerialized]
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
    public class Date : ISerializationCallbackReceiver
    {
        public string start;
        public string end;
        [NonSerialized]
        public bool isISOChanged = false;

        public void OnAfterDeserialize()
        {
            // ïœçXÇÇªÇÃÇ‹Ç‹Ç…Ç∑ÇÈÇΩÇﬂNotionå`éÆÇ™ï€éùÇ≥ÇÍÇ»Ç¢
        }

        public void OnBeforeSerialize()
        {
            isISOChanged = true;
            if (start != null && start != string.Empty)
            {
                start = DateTime.Parse(start, null, DateTimeStyles.RoundtripKind).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            }
            if (end != null && end != string.Empty)
            {
                end = DateTime.Parse(start, null, DateTimeStyles.RoundtripKind).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
            }
        }
    }
}