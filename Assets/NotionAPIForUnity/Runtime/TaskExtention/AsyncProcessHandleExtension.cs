using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{

    public static class AsyncProcessHandleExtension
    {
        public async static Task<T> ToAsync<T>(this IEnumerator enumerator) where T : class
        {
            var result = await CustomCoroutineManager.Instance.StartCoroutine(enumerator).Task;
            return result as T;
        }

        public async static Task<object> ToAsync(this IEnumerator enumerator)
        {
            return await CustomCoroutineManager.Instance.StartCoroutine(enumerator).Task;
        }
    }
}
