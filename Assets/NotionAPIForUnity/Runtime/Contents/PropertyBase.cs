using System;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class PropertyBase
    {
        public string type;
        public string id;
    }

    [Serializable]
    public class OptionBase
    {
        public string id;
        public string name;
        public string color;
    }

    [Serializable]
    public class Options
    {
        public OptionBase[] options;
    }

    [Serializable]
    public class SelectProperty : PropertyBase
    {
        public OptionBase select;
        public OptionBase Value => select;
    }

    [Serializable]
    public class MultiSelectProperty : PropertyBase
    {
        public OptionBase[] multi_select;
        public OptionBase[] Value => multi_select;
    }

    [Serializable]
    public class SelectPropertyDefinition : PropertyBase
    {
        public Options select;
    }

    [Serializable]
    public class TitleProperty : PropertyBase
    {
        public NotionText[] title;
        public string Value
        => (title != null && title.Length > 0) ? title[0].text : null;
    }

    [Serializable]
    public class TextPropertyDefinition : PropertyBase
    {
        public NotionText[] text;
        public string Value
        => (text != null && text.Length > 0) ? text[0].plain_text : null;
    }

    [Serializable]
    public class RichTextProperty : PropertyBase
    {
        public NotionText[] rich_text;
        public string Value
        => (rich_text != null && rich_text.Length > 0) ? rich_text[0].plain_text : null;
    }

    [Serializable]
    public class FormulaStringProperty : PropertyBase
    {
        public FormulaString formula;
        public string Value => formula.value;

        [Serializable]
        public class FormulaString
        {
            public string type;
            public string value;
        }
    }

    [Serializable]
    public class NumberProperty : PropertyBase
    {
        public float number;
        public float Value => number;

        public override string ToString() => number.ToString();
    }

    [Serializable]
    public class CheckboxProperty : PropertyBase
    {
        public bool checkbox;
        public bool Value => checkbox;

        public override string ToString() => checkbox.ToString();
    }

    [Serializable]
    public class DateProperty : PropertyBase
    {
        public Date date;
    }

    [Serializable]
    public class Person
    {
        public string email;
    }

    [Serializable]
    public class UserObject : PropertyBase
    {
        public string name;
        public string avatar_url;
        public Person person;
    }

    [Serializable]
    public class PeopleProperty : PropertyBase
    {
        public UserObject[] people;
        public UserObject[] Value => people;
    }
}