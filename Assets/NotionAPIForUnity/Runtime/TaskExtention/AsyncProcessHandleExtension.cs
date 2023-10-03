using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{

    public static class AsyncProcessHandleExtension
    {
        public static AsyncProcessHandle ToAsyncProcessHandle(this IEnumerator enumerator)
        {
            return CustomCoroutineManager.Instance.StartCoroutine(enumerator);
        }
    }
}
