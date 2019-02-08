using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WasabiRealFeeCalc
{

    public class Channel<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly Queue<TaskCompletionSource<T>> _waitingQueue = new Queue<TaskCompletionSource<T>>();

        public void Send(T item)
        {
            TaskCompletionSource<T> tcs = null;
            lock (_queue)
            {
                if (_waitingQueue.Count > 0) tcs = _waitingQueue.Dequeue();
                else _queue.Enqueue(item);
            }
            if (tcs != null) tcs.TrySetResult(item);
        }

        public Task<T> TakeAsync()
        {
            lock (_queue)
            {
                if (_queue.Count > 0)
                {
                    return Task.FromResult(_queue.Dequeue());
                }
                else
                {
                    var tcs = new TaskCompletionSource<T>();
                    _waitingQueue.Enqueue(tcs);
                    return tcs.Task;
                }
            }
        }
    }
}