using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    public static class NotionApiExtension
    {
        /// <summary>
        /// データベースの型情報取得
        /// ※中身は取得できない
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async static Task<Database<T>> GetDatabaseAsync<T>(this NotionApi api, Action<Database<T>> callback = null) where T : Schema
        {
            return await api.GetDatabase(callback).ToAsync<Database<T>>();
        }

        /// <summary>
        /// データベースのコンテンツの取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async static Task<DatabaseQuery<T>> GetQueryDatabaseAsync<T>(this NotionApi api, Action<DatabaseQuery<T>> callback = null) where T : Schema
        {
            return await api.GetQueryDatabase(callback).ToAsync<DatabaseQuery<T>>();
        }

        public async static Task<DatabasePage<T>[]> ToDatabesePagesAsync<T>(this Task<DatabaseQuery<T>> query) where T : Schema
        {
            return (await query).results;
        }

        /// <summary>
        /// データベースのコンテンツの更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="pageId"></param>
        /// <param name="updatePage"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async static Task<DatabasePage<T>> PatchPageDatabaseAsync<T>(this NotionApi api, DatabasePage<T> updatePage, Action<DatabasePage<T>> callback = null) where T : Schema
        {
            return await api.PatchPageDatabase(updatePage, callback).ToAsync<DatabasePage<T>>();
        }

        /// <summary>
        /// データベースのコンテンツの作成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="api"></param>
        /// <param name="createPage"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async static Task<DatabasePage<T>> PostPageDatabaseAsync<T>(this NotionApi api, DatabasePage<T> createPage, Action<DatabasePage<T>> callback = null) where T : Schema
        {
            return await api.PostPageDatabase(createPage, callback).ToAsync<DatabasePage<T>>();
        }
    }
}
