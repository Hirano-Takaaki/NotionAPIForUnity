# NotionAPIForUnity
Notionデータベース化計画

# 使い方
## Notionのデータベースと同期するスキーマオブジェクトの作成
1. Create->NotionAPIForUnity->DatabaseSchema
2. DatabaseSchemaオブジェクトに命名
3. APIKeyとDatabaseIDを設定
4. Fetch Schemaボタンを押し、変数の種類と名前を取得
5. Create Schem Classボタンを押し、クラスを生成
6. 再生成時は4から行う

## Notionのデータをロード
サンプルコード
```c#:NotionLoad
var response = await api.GetQueryDatabase<PlayerSchema>(schemaObject.databaseId).ToAsync<DatabaseQuery<PlayerSchema>>();
```

## Notionのデータに書き込み
サンプルコード
propertiesを編集したのちに呼び出すとNotion側が更新される
```c#:NotionLoad
var response = await api.PatchPageDatabase<PlayerSchema>(pageId, new DatabasePage<PlayerSchema>()
        {
            parent = new Parent()
            {
                database_id = databaseId
            },
            properties = properties
        }).ToAsync<DatabasePage<PlayerSchema>>();
```

## Notionのデータを追加
サンプルコード
propertiesを生成したのちに呼び出すとNotion側で生成される
※生成は他のプロパティをコピーし生成するほうが早い
```c#:NotionLoad
var response = await api.PostPageDatabase<PlayerSchema>(new DatabasePage<PlayerSchema>()
            {
                parent = new Parent()
                {
                    database_id = databaseId
                },
                properties = properties
            }).ToAsync<DatabasePage<PlayerSchema>>();
```
