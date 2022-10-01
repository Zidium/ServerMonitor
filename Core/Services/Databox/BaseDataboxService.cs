using System;

namespace ZidiumServerMonitor
{
    internal abstract class BaseDataboxService<T> where T : new()
    {
        private T _value = new T();

        private object lockObject = new object();

        public T GetAndReset()
        {
            lock (lockObject)
            {
                var result = Copy(_value);
                _value = new T();
                return result;
            }
        }

        protected abstract T Copy(T value);

        protected void Update(Action<T> action)
        {
            lock (lockObject)
            {
                action(_value);
            }
        }
    }
}
