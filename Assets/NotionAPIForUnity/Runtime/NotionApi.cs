#if NOTION_API_FOR_UNITY_UNITASK_SUPPORT
using Cysharp.Threading.Tasks;
using NotionAPIForUnity.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private readonly CancellationTokenSource source = new();

        private static List<string> apiKeys = new();

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

        private UnityWebRequest WebRequestWithAuth(string url, RequestType requestType, WWWForm form = null)
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
            return request;
        }

        internal async UniTask<string> GetJSON(string url, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("GET Requesting: " + url);
            }

            using (var request = WebRequestWithAuth(url, RequestType.GET))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                await WaitBusyWhile();
                busy = true;
                await request.SendWebRequest();
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
                return jsonData;
            }
        }

        internal async UniTask<string> PostJSON(string url, WWWForm form, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("POST Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.POST, form))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                await WaitBusyWhile();
                busy = true;
                await request.SendWebRequest();
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
                return jsonData;
            }
        }

        public async UniTask<Database<T>> GetDatabase<T>(string databaseId, Action<Database<T>> callback = null)
        {
            var json = await GetDatabaseJSON(databaseId);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var database = JsonUtility.FromJson<Database<T>>(json);
            callback?.Invoke(database);
            return database;
        }

        internal async UniTask<string> GetDatabaseJSON(string databaseId, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}";
            var json = await GetJSON(url, callback);
            callback?.Invoke(json);
            return json;
        }

        public async UniTask<DatabaseQueryResponse<T>> QueryDatabase<T>(string databaseId, Action<DatabaseQueryResponse<T>> callback = null)
        {
            var json = await QueryDatabaseJSON(databaseId);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var queryResponse = JsonUtility.FromJson<DatabaseQueryResponse<T>>(json);
            callback?.Invoke(queryResponse);
            return queryResponse;
        }

        internal async UniTask<string> QueryDatabaseJSON(string databaseId, WWWForm form = null, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}/{queryLavel}";
            var json = await PostJSON(url, form, callback);
            return json;
        }

        //public async Awaitable<DatabaseQueryResponse<T>> PostDatabase<T>()
        //{

        //}

        public async UniTask<DatabaseUsers> GetUsers(Action<DatabaseUsers> callback = null)
        {
            var url = $"{urlUsers}/";
            var json = await GetJSON(url);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var users = JsonUtility.FromJson<DatabaseUsers>(json);
            callback?.Invoke(users);
            return users;
        }

        private async void WaitBusyUpdate()
        {
            await UniTask.Delay((int)updateThrottleSecond * 1000, cancellationToken: source.Token);
            busy = false;
        }

        public async Awaitable WaitBusyWhile()
        {
            await UniTask.WaitUntil(() => busy, cancellationToken: source.Token);
        }

        public void Dispose()
        {
            source.Cancel();
            apiKeys.Remove(apiKey);
        }
    }
}
#elif UNITY_2023_1_OR_NEWER
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
        private readonly CancellationTokenSource source = new();

        private static List<string> apiKeys = new();

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

        private UnityWebRequest WebRequestWithAuth(string url, RequestType requestType, WWWForm form = null)
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
            return request;
        }

        internal async Awaitable<string> GetJSON(string url, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("GET Requesting: " + url);
            }

            using (var request = WebRequestWithAuth(url, RequestType.GET))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                await WaitBusyWhile();
                busy = true;
                await request.SendWebRequest();
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
                return jsonData;
            }
        }

        internal async Awaitable<string> PostJSON(string url, WWWForm form, Action<string> callback = null)
        {
            if (debugMode)
            {
                Debug.Log("POST Requesting: " + url);
            }
            using (var request = WebRequestWithAuth(url, RequestType.POST, form))
            {
                // NOTE: Awaitableに対応するため,UnityのAPI更新後変更
                await WaitBusyWhile();
                busy = true;
                await request.SendWebRequest();
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
                return jsonData;
            }
        }

        public async Awaitable<Database<T>> GetDatabase<T>(string databaseId, Action<Database<T>> callback = null)
        {
            var json = await GetDatabaseJSON(databaseId);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var database = JsonUtility.FromJson<Database<T>>(json);
            callback?.Invoke(database);
            return database;
        }

        internal async Awaitable<string> GetDatabaseJSON(string databaseId, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}";
            var json = await GetJSON(url, callback);
            callback?.Invoke(json);
            return json;
        }

        public async Awaitable<DatabaseQueryResponse<T>> QueryDatabase<T>(string databaseId, Action<DatabaseQueryResponse<T>> callback = null)
        {
            var json = await QueryDatabaseJSON(databaseId);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var queryResponse = JsonUtility.FromJson<DatabaseQueryResponse<T>>(json);
            callback?.Invoke(queryResponse);
            return queryResponse;
        }

        internal async Awaitable<string> QueryDatabaseJSON(string databaseId, WWWForm form = null, Action<string> callback = null)
        {
            var url = $"{urlDB}/{databaseId}/{queryLavel}";
            var json = await PostJSON(url, form, callback);
            return json;
        }

        //public async Awaitable<DatabaseQueryResponse<T>> PostDatabase<T>()
        //{

        //}

        public async Awaitable<DatabaseUsers> GetUsers(Action<DatabaseUsers> callback = null)
        {
            var url = $"{urlUsers}/";
            var json = await GetJSON(url);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var users = JsonUtility.FromJson<DatabaseUsers>(json);
            callback?.Invoke(users);
            return users;
        }

        private async void WaitBusyUpdate()
        {
            await Awaitable.WaitForSecondsAsync(updateThrottleSecond);
            busy = false;
        }

        public async Awaitable WaitBusyWhile()
        {
            while (true)
            {
                if (busy)
                {
                    await Awaitable.NextFrameAsync(source.Token);
                }
                else
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            source.Cancel();
            apiKeys.Remove(apiKey);
        }
    }
}
#else
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
    // 実装していない
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
        private readonly CancellationTokenSource source = new();

        private static List<string> apiKeys = new();

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

        private UnityWebRequest WebRequestWithAuth(string url, RequestType requestType, WWWForm form = null)
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
            return request;
        }

        internal IEnumerator GetJSON(string url, Action<string> callback = null)
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
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
            }
        }

        internal IEnumerator PostJSON(string url, WWWForm form, Action<string> callback = null)
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
                WaitBusyUpdate();
                var jsonData = request.downloadHandler.text;
                callback?.Invoke(jsonData);
            }
        }

        public IEnumerator GetDatabase<T>(string databaseId, Action<Database<T>> callback = null)
        {
            var json = "";
            yield return GetDatabaseJSON(databaseId, (val) => json = val);
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
            callback += val => json = val;
            yield return GetJSON(url, callback);
            callback?.Invoke(json);
        }

        public IEnumerator QueryDatabase<T>(string databaseId, Action<DatabaseQueryResponse<T>> callback = null)
        {
            var json = "";
            yield return QueryDatabaseJSON(databaseId, null, val => json = val);
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
            callback += val => json = val;
            yield return PostJSON(url, form, callback);
        }

        //public async Awaitable<DatabaseQueryResponse<T>> PostDatabase<T>()
        //{

        //}

        public IEnumerator GetUsers(Action<DatabaseUsers> callback = null)
        {
            var url = $"{urlUsers}/";
            var json = "";
            yield return GetJSON(url, val => json = val);
            if (debugMode)
            {
                Debug.Log(json);
            }
            var users = JsonUtility.FromJson<DatabaseUsers>(json);
            callback?.Invoke(users);
        }

        private IEnumerator WaitBusyUpdate()
        {
            yield return new WaitForSeconds(updateThrottleSecond);
            busy = false;
        }

        public IEnumerator WaitBusyWhile()
        {
            yield return new WaitUntil(() => busy);
        }

        public void Dispose()
        {
            apiKeys.Remove(apiKey);
        }
    }
}
#endif