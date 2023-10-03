using NotionAPIForUnity.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            POST
        }

        private bool busy = false;
        private readonly bool debugMode;
        private readonly string apiKey;
        private readonly CancellationTokenSource source = new CancellationTokenSource();

        private static List<string> apiKeys = new List<string>();

        // Notion固有
        private readonly static float updateThrottleSecond = 1;
        private readonly static string versionLavel = "v1";
        // NOTE: 自動取得ではないのでNotion側がUpdateしたら動かない。
        // 参考 https://developers.notion.com/reference/versioning
        private readonly static string notionVersionLavel = "2022-06-28";
        private readonly static string queryLavel = "query";
        private readonly static string rootUrl = $"https://api.notion.com/{versionLavel}";
        private readonly static string urlDB = rootUrl + "/databases";
        private readonly static string urlUsers = rootUrl + "/users";

        public static IReadOnlyList<string> VaildApiKeys => apiKeys.AsReadOnly();

        public NotionApi(string apiKey, bool isDebug = false)
        {
            this.apiKey = apiKey;
            apiKeys.Add(apiKey);
            debugMode = isDebug;
        }

        private UnityWebRequest WebRequestWithAuth(string url, RequestType requestType, WWWForm form = null, KeyValuePair<string, string>[] requestHeaders = null)
        {
            UnityWebRequest request = null;
            switch (requestType)
            {
                case RequestType.GET:
                    request = UnityWebRequest.Get(url);
                    break;
                case RequestType.POST:
                    request = UnityWebRequest.Post(url, form);
                    break;
            }

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

        internal IEnumerator GetJsonAsync(string url, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("GET Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.GET))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                yield return WaitBusyWhile();
                busy = true;
                yield return request.SendWebRequest();
                WaitBusyRealTime();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
            }
        }

        internal IEnumerator PostJsonAsync(string url, WWWForm form, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("POST Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.POST, form))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                yield return WaitBusyWhile();
                busy = true;
                yield return request.SendWebRequest();
                WaitBusyRealTime();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
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
            if (debugMode)
            {
                Debug.Log(json);
            }
            var database = JsonUtility.FromJson<Database<T>>(json);
            callback?.Invoke(database);
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
        }

        public IEnumerator QueryDatabase<T>(string databaseId, Action<DatabaseQueryResponse<T>> callback = null)
        {
            var url = $"{urlDB}/{databaseId}/{queryLavel}";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return QueryDatabaseJSON(databaseId, null, SetJson);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var queryResponse = JsonUtility.FromJson<DatabaseQueryResponse<T>>(json);
            callback?.Invoke(queryResponse);
        }

        internal IEnumerator QueryDatabaseJSON(string databaseId, WWWForm form = null, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}/{queryLavel}";
            var json = "";

            void SetJson(string val)
            {
                json = val;
            }
            yield return PostJsonAsync(url, form, SetJson);
            callback?.Invoke(json);
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
            if (debugMode)
            {
                Debug.Log(json);
            }
            var users = JsonUtility.FromJson<DatabaseUsers>(json);
            callback?.Invoke(users);
        }

        private IEnumerator WaitBusyRealTime()
        {
            yield return new WaitForSecondsRealtime(updateThrottleSecond);
            busy = false;
        }

        private IEnumerator WaitBusyWhile()
        {
            yield return new WaitUntil(() => !busy);
        }

        public void Dispose()
        {
            source.Cancel();
            apiKeys.Remove(apiKey);
        }
    }
}