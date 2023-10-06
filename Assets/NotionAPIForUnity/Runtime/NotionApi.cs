using NotionAPIForUnity.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[assembly: InternalsVisibleTo("NotionAPIForUnity.Editor")]
namespace NotionAPIForUnity.Runtime
{

    public class NotionApi : IDisposable
    {
        enum RequestType
        {
            GET,
            POST,
            PATCH
        }

        private DateTime timeStamp = DateTime.Now;
        private readonly bool debugMode;
        private readonly int timeout;
        private readonly string apiKey;
        private readonly CancellationTokenSource source = new CancellationTokenSource();

        private static List<string> apiKeys = new List<string>();

        // Notion固有
        private readonly static bool isActiveUpdateThrottle = false;
        private readonly static int updateThrottleSecond = 1;
        private readonly static string versionLavel = "v1";
        // NOTE: 自動取得ではないのでNotion側がUpdateしたら動かない。
        // 参考 https://developers.notion.com/reference/versioning
        private readonly static string notionVersionLavel = "2022-06-28";
        private readonly static string queryLavel = "query";
        private readonly static string rootUrl = $"https://api.notion.com/{versionLavel}";
        private readonly static string urlDB = rootUrl + "/databases";
        private readonly static string urlPages = rootUrl + "/pages";
        private readonly static string urlUsers = rootUrl + "/users";

        public static IReadOnlyList<string> VaildApiKeys => apiKeys.AsReadOnly();

        public NotionApi(string apiKey, bool isDebug = false, int timeout = 30)
        {
            this.apiKey = apiKey;
            apiKeys.Add(apiKey);
            debugMode = isDebug;
            this.timeout = timeout;
        }

        private UnityWebRequest WebRequestWithAuth(string uri, RequestType requestType, byte[] bodyData = null, KeyValuePair<string, string>[] requestHeaders = null)
        {
            UnityWebRequest request = null;
            switch (requestType)
            {
                case RequestType.GET:
                    request = UnityWebRequest.Get(uri);
                    break;
                case RequestType.POST:
                    request = UnityWebRequest.Post(uri, formData: null);
                    break;
                case RequestType.PATCH:
                    request = UnityWebRequest.Put(uri, bodyData);
                    request.method = "PATCH";
                    break;
            }

            request.timeout = timeout;
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Notion-Version", $"{notionVersionLavel}");
            request.SetRequestHeader("Content-Type", "application/json");
            if (requestHeaders != null)
            {
                for (int i = 0; i < requestHeaders.Length; i++)
                {
                    request.SetRequestHeader(requestHeaders[i].Key, requestHeaders[i].Value);
                }
            }
            return request;
        }

        internal IEnumerator GetJsonAsync(string url, Action<string> callback = null, KeyValuePair<string, string>[] requestHeaders = null)
        {
            if (debugMode)
            {
                Debug.Log("GET Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.GET, null, requestHeaders))
            {
                if (isActiveUpdateThrottle)
                {
                    bool IsVaild()
                    {
                        var span = DateTime.Now - timeStamp;
                        return span.TotalSeconds > updateThrottleSecond;
                    }
                    yield return new WaitUntil(IsVaild);
                    timeStamp = DateTime.Now;
                }
                yield return request.SendWebRequest();
                var jsonData = request.downloadHandler.text;
                if (debugMode)
                {
                    Debug.Log("GET downloadHandler: " + request.downloadHandler.text);
                }
                callback?.Invoke(jsonData);
                yield return jsonData;
            }
        }

        internal IEnumerator PostJsonAsync(string url, Action<string> callback = null, KeyValuePair<string, string>[] requestHeaders = null, UploadHandler handler = null)
        {
            if (debugMode)
            {
                Debug.Log("POST Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.POST, null, requestHeaders))
            {
                if (isActiveUpdateThrottle)
                {
                    bool IsVaild()
                    {
                        var span = DateTime.Now - timeStamp;
                        return span.TotalSeconds > updateThrottleSecond;
                    }
                    yield return new WaitUntil(IsVaild);
                    timeStamp = DateTime.Now;
                }

                if (handler != null)
                {
                    request.uploadHandler = handler;
                }

                yield return request.SendWebRequest();
                var jsonData = request.downloadHandler.text;
                if (debugMode)
                {
                    Debug.Log("POST downloadHandler: " + request.downloadHandler.text);
                }
                callback?.Invoke(jsonData);
                yield return jsonData;
            }
        }

        internal IEnumerator PatchJsonAsync(string url, byte[] bodyData, Action<string> callback = null, KeyValuePair<string, string>[] requestHeaders = null)
        {
            if (debugMode)
            {
                Debug.Log("PATCH Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.PATCH, bodyData, requestHeaders))
            {
                if (isActiveUpdateThrottle)
                {
                    bool IsVaild()
                    {
                        var span = DateTime.Now - timeStamp;
                        return span.TotalSeconds > updateThrottleSecond;
                    }
                    yield return new WaitUntil(IsVaild);
                    timeStamp = DateTime.Now;
                }
                yield return request.SendWebRequest();
                var jsonData = request.downloadHandler.text;
                if (debugMode)
                {
                    Debug.Log("PATCH downloadHandler: " + request.downloadHandler.text);
                }
                callback?.Invoke(jsonData);
                yield return jsonData;
            }
        }

        public IEnumerator GetDatabase<T>(string databaseId, Action<Database<T>> callback = null)
        {
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return GetDatabaseJSON(databaseId, SetJson);
            var database = JsonUtility.FromJson<Database<T>>(json);
            callback?.Invoke(database);
            yield return database;
        }

        internal IEnumerator GetDatabaseJSON(string databaseId, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return GetJsonAsync(url, SetJson);
            callback?.Invoke(json);
            yield return json;
        }

        public IEnumerator GetQueryDatabase<T>(string databaseId, Action<DatabaseQuery<T>> callback = null)
        {
            var json = "";
            void SetJson(string val)
            {
                json = val;
            }
            yield return GetQueryDatabaseJson(databaseId, SetJson);
            var queryResponse = JsonUtility.FromJson<DatabaseQuery<T>>(json);
            callback?.Invoke(queryResponse);
            yield return queryResponse;
        }

        internal IEnumerator GetQueryDatabaseJson(string databaseId, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}/{queryLavel}";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return PostJsonAsync(url, SetJson);
            callback?.Invoke(json);
            yield return json;
        }

        public IEnumerator PatchPageDatabase<T>(string pageId, DatabasePage<T> updateData, Action<DatabasePage<T>> callback = null) where T : class
        {
            var json = "";
            var updateDataJson = JsonUtility.ToJson(updateData);

            void SetJson(string val)
            {
                json = val;
            }
            yield return PatchPageDatabaseJson(pageId, Encoding.UTF8.GetBytes(updateDataJson), SetJson);
            var queryResponse = JsonUtility.FromJson<DatabasePage<T>>(json);
            callback?.Invoke(queryResponse);
            yield return queryResponse;
        }

        internal IEnumerator PatchPageDatabaseJson(string pageId, byte[] bodyData = null, Action<string> callback = null)
        {
            var url = $"{urlPages}/{pageId}";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return PatchJsonAsync(url, bodyData, SetJson);
            callback?.Invoke(json);
            yield return json;
        }

        public IEnumerator PostPageDatabase<T>(DatabasePage<T> updateData, Action<DatabasePage<T>> callback = null) where T : class
        {
            var json = "";
            var updateDataJson = JsonUtility.ToJson(updateData);

            Debug.Log(updateDataJson);

            void SetJson(string val)
            {
                json = val;
            }
            yield return PostPageDatabaseJson(updateDataJson, SetJson);
            var queryResponse = JsonUtility.FromJson<DatabasePage<T>>(json);
            callback?.Invoke(queryResponse);
            yield return queryResponse;
        }

        internal IEnumerator PostPageDatabaseJson(string updateDataJson = null, Action<string> callback = null)
        {
            var url = $"{urlPages}";
            var json = "";

            var bodyData = Encoding.UTF8.GetBytes(updateDataJson);

            void SetJson(string val)
            {
                json = val;
            }
            yield return PostJsonAsync(url, SetJson, handler: new UploadHandlerRaw(bodyData));
            callback?.Invoke(json);
            yield return json;
        }

        public IEnumerator GetUsers(Action<DatabaseUsers> callback = null)
        {
            var url = $"{urlUsers}/";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return GetJsonAsync(url, SetJson);
            var users = JsonUtility.FromJson<DatabaseUsers>(json);
            callback?.Invoke(users);
            yield return users;
        }

        public void Dispose()
        {
            source.Cancel();
            apiKeys.Remove(apiKey);
        }
    }
}