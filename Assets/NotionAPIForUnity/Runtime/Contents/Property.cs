using System;
using System.Linq;

namespace NotionAPIForUnity.Runtime
{
    [Serializable]
    public class Property
    {
        public string type;
        public string id;

        public static Property DefaultProperty => new Property()
        {
            type = "",
            id = "",
        };
    }

    [Serializable]
    public class OptionEntry
    {
        public string id;
        public string name;
        public string color;

        public T GetEnumValue<T>() where T : struct
        {
            return Enum.Parse<T>(name);
        }

        public static OptionEntry DefaultOptionEntry => new OptionEntry()
        {
            id = "",
            name = "",
            color = "",
        };
    }

    [Serializable]
    public class Options
    {
        public OptionEntry[] options;

        public static Options DefaultOptions => new Options()
        {
            options = new OptionEntry[1]
            {
                OptionEntry.DefaultOptionEntry,
            }
        };
    }

    [Serializable]
    public class MultiSelectPropertyDefinition : Property
    {
        public Options multi_select;

        public static Options DefaultMultiSelectPropertyDefinition => new Options()
        {
            options = new OptionEntry[1]
            {
                OptionEntry.DefaultOptionEntry,
            }
        };
    }

    [Serializable]
    public class SelectProperty : Property
    {
        public OptionEntry select;
        public OptionEntry Value => select;

        public T GetMainValue<T>() where T : struct
        {
            return select.GetEnumValue<T>();
        }
        public void SetMainValue<T>(T value)
        {
            select.name = value.ToString();
        }

        public static SelectProperty DefaultSelectProperty => new SelectProperty()
        {
            select = OptionEntry.DefaultOptionEntry
        };
    }

    [Serializable]
    public class MultiSelectProperty : Property
    {
        public OptionEntry[] multi_select;
        public OptionEntry[] Value => multi_select;

        public T[] GetMainValue<T>() where T : struct
        {
            return multi_select.Select(entry => entry.GetEnumValue<T>()).ToArray();
        }
        public void SetMainValue<T>(T[] value) where T : struct
        {
            for (int i = 0; i < value.Length; i++)
            {
                multi_select[i].name = value[i].ToString();
            }
        }

        public static MultiSelectProperty DefaultMultiSelectProperty => new MultiSelectProperty()
        {
            multi_select = new OptionEntry[1]
            {
                OptionEntry.DefaultOptionEntry
            }
        };
    }

    [Serializable]
    public class SelectPropertyDefinition : Property
    {
        public Options select;

        public static SelectPropertyDefinition DefaultSelectPropertyDefinition => new SelectPropertyDefinition()
        {
            select = Options.DefaultOptions
        };
    }

    [Serializable]
    public class TitleProperty : Property
    {
        public Text[] title;
        public string Value
        => (title != null && title.Length > 0) ? title[0].text : null;

        public string GetMainValue()
        {
            return title.FirstOrDefault()?.text.content;
        }
        public void SetMainValue(string value)
        {
            title.FirstOrDefault().text.content = value;
        }

        public static TitleProperty DefaultTitleProperty => new TitleProperty()
        {
            title = new Text[1]
            {
                Text.DefaultText
            }
        };
    }

    [Serializable]
    public class TextPropertyDefinition : Property
    {
        public Text[] text;
        public string Value
        => (text != null && text.Length > 0) ? text[0].plain_text : null;

        public static TextPropertyDefinition DefaultTextPropertyDefinition => new TextPropertyDefinition()
        {
            text = new Text[1]
            {
                Text.DefaultText
            }
        };
    }

    [Serializable]
    public class TextProperty : Property
    {
        public Text[] rich_text;
        public string Value
        => (rich_text != null && rich_text.Length > 0) ? rich_text[0].plain_text : null;

        public string GetMainValue()
        {
            return rich_text.FirstOrDefault()?.text.content;
        }
        public void SetMainValue(string value)
        {
            rich_text.FirstOrDefault().text.content = value;
        }

        public static TextProperty DefaultTextProperty => new TextProperty()
        {
            rich_text = new Text[1]
            {
                Text.DefaultText
            }
        };
    }

    [Serializable]
    public class FormulaStringProperty : Property
    {
        public FormulaString formula;
        public string Value
        => formula.@string;

        public string GetMainValue()
        {
            return formula.@string;
        }
        public void SetMainValue(string value)
        {
            formula.@string = value;
        }

        public static FormulaStringProperty DefaultFormulaStringProperty => new FormulaStringProperty()
        {
            formula = new FormulaString()
            {
                type = "",
                @string = "",
            }
        };

        [Serializable]
        public class FormulaString
        {
            public string type;
            public string @string;
        }
    }

    [Serializable]
    public class NumberProperty : Property
    {
        public float number;
        public float Value => number;

        public override string ToString() => number.ToString();

        public float GetMainValue()
        {
            return number;
        }
        public void SetMainValue(float value)
        {
            number = value;
        }

        public static NumberProperty DefaultNumberProperty => new NumberProperty()
        {
            number = 0
        };
    }

    [Serializable]
    public class CheckboxProperty : Property
    {
        public bool checkbox;
        public bool Value => checkbox;

        public override string ToString() => checkbox.ToString();

        public bool GetMainValue()
        {
            return checkbox;
        }
        public void SetMainValue(bool value)
        {
            checkbox = value;
        }

        public static CheckboxProperty DefaultCheckboxProperty => new CheckboxProperty()
        {
            checkbox = false
        };
    }

    [Serializable]
    public class DateProperty : Property
    {
        public Date date;

        public Date GetMainValue()
        {
            return date;
        }
        public void SetMainValue(Date value)
        {
            date = value;
        }

        public static DateProperty DefaultDateProperty => new DateProperty()
        {
            date = default
        };
    }

    [Serializable]
    public class Person
    {
        public string email;

        public static Person DefaultPerson => new Person()
        {
            email = ""
        };
    }

    [Serializable]
    public class UserObject : Property
    {
        public string name;
        public string avatar_url;
        public Person person;

        public static UserObject DefaultUserObject => new UserObject()
        {
            name = "",
            avatar_url = "",
            person = Person.DefaultPerson
        };
    }

    [Serializable]
    public class PeopleProperty : Property
    {
        public UserObject[] people;
        public UserObject[] Value => people;

        public UserObject[] GetMainValue()
        {
            return people;
        }
        public void SetMainValue(UserObject[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                people[i] = value[i];
            }
        }

        public static PeopleProperty DefaultPeopleProperty => new PeopleProperty()
        {
            people = new UserObject[1]
            {
                UserObject.DefaultUserObject
            }
        };
    }
}