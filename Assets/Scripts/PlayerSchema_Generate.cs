// 自動生成されたファイルです。手動で書き換えないでください。
using NotionAPIForUnity.Runtime;
using System;

[Serializable]
public partial class PlayerSchema : Schema
{
    public TextProperty discription;
    public SelectProperty select;
    public NumberProperty playerId;
    public MultiSelectProperty mulitiSelect;
    public TitleProperty name;

}

[Serializable]
public enum select_Enum
{
    Bar,
    Foo,
}

[Serializable]
public enum MulitiSelect_Enum
{
    Piyo,
    Fuga,
    Hoge,
}


