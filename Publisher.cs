using System.Collections.Concurrent;

namespace razor {
    class Publisher<TFunc, TArg> where TFunc : Delegate {
        private ConcurrentBag<TFunc> callbacks;
        public Publisher() {
            callbacks = new();
        }
        public void Subscribe(TFunc callback) {
            callbacks.Add(callback);
        }
        public void Publish(TArg message) {
            foreach (var callback in callbacks) {
                callback.DynamicInvoke(message);
            }
        }
    }
}