using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    internal class CustomCoroutineManager : MonoBehaviour
    {
        private static CustomCoroutineManager instance;

        private readonly Dictionary<int, Coroutine> runningCoroutines = new Dictionary<int, Coroutine>();

        private int currentId;

        public static CustomCoroutineManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var gameObj = new GameObject($"{nameof(CustomCoroutineManager)}");
                    DontDestroyOnLoad(gameObj);
                    instance = gameObj.AddComponent<CustomCoroutineManager>();
                }

                return instance;
            }
        }

        public bool ThrowException { get; set; } = true;

        public new AsyncProcessHandle StartCoroutine(IEnumerator routine)
        {
            if (routine == null)
            {
                throw new ArgumentNullException(nameof(routine));
            }

            var id = currentId++;
            var handle = new AsyncProcessHandle(id);
            var handleSetter = (IAsyncProcessHandleController)handle;

            void OnComplete(object result)
            {
                handleSetter.Complete(result);
            }

            void OnError(Exception ex)
            {
                handleSetter.Error(ex);
            }

            void OnTerminate()
            {
                runningCoroutines.Remove(id);
            }

            var coroutine = StartCoroutineInternal(routine, ThrowException, OnComplete,
                OnError, OnTerminate);
            runningCoroutines.Add(id, coroutine);
            return handle;
        }

        public void StopCoroutine(AsyncProcessHandle handle)
        {
            var coroutine = runningCoroutines[handle.Id];
            StopCoroutine(coroutine);
            runningCoroutines.Remove(handle.Id);
        }

        private Coroutine StartCoroutineInternal(IEnumerator routine, bool throwException = true,
            Action<object> onComplete = null, Action<Exception> onError = null, Action onTerminate = null)
        {
            return base.StartCoroutine(ProcessRoutine(routine, throwException, onComplete, onError, onTerminate));
        }

        private IEnumerator ProcessRoutine(IEnumerator routine, bool throwException = true,
            Action<object> onComplete = null, Action<Exception> onError = null, Action onTerminate = null)
        {
            object current = null;
            while (true)
            {
                Exception ex = null;
                try
                {
                    if (!routine.MoveNext())
                    {
                        break;
                    }

                    current = routine.Current;
                }
                catch (Exception e)
                {
                    ex = e;
                    onError?.Invoke(e);
                    onTerminate?.Invoke();
                    if (throwException)
                    {
                        throw;
                    }
                }

                if (ex != null)
                {
                    yield return ex;
                    yield break;
                }

                yield return current;
            }

            onComplete?.Invoke(current);
            onTerminate?.Invoke();
        }
    }
}
