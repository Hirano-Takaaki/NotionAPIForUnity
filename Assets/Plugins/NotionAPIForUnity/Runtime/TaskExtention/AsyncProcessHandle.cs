using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace NotionAPIForUnity.Runtime
{
    internal interface IAsyncProcessHandleController
    {
        void Complete(object result);

        void Error(Exception ex);
    }

    public class AsyncProcessHandle : CustomYieldInstruction, IAsyncProcessHandleController
    {
        private readonly TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        public AsyncProcessHandle(int id)
        {
            Id = id;
        }

        public int Id { get; }

        public object Result { get; private set; }

        public bool IsTerminated { get; private set; }

        public Exception Exception { get; private set; }

        public Task<object> Task => _tcs.Task;

        public bool HasError => Exception != null;

        public override bool keepWaiting => !IsTerminated;

        void IAsyncProcessHandleController.Complete(object result)
        {
            Result = result;
            IsTerminated = true;
            OnTerminate?.Invoke();
            _tcs.SetResult(result);
        }

        void IAsyncProcessHandleController.Error(Exception ex)
        {
            Exception = ex;
            IsTerminated = true;
            OnTerminate?.Invoke();
            _tcs.SetException(ex);
        }

        public event Action OnTerminate;
    }
}
