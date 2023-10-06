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

        public static Text DefaultText => new Text()
        {
            type = "",
            text = new TextContent()
            {
                content = ""
            },
            annotations = new Annotations()
            {
                bold = false,
                italic = false,
                strikethrough = false,
                underline = false,
                code = false,
                color = ""
            },
            plain_text = "",
            href = ""
        };

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
            if (isISOChanged) { return; }
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