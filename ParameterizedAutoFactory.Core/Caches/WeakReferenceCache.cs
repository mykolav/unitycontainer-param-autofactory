using System;
using System.Collections.Generic;

namespace Unity.ParameterizedAutoFactory.Core.Caches
{
    /// <summary>
    /// This is a simple in-memory cache.
    /// It is basically a <see cref="Dictionary{TKey,TValue}" /> of <see cref="WeakReference{TValue}" />.
    ///
    /// Using <see cref="WeakReference{TValue}" /> for caching is discouraged.
    /// For example, see <a href="https://stackoverflow.com/q/7755954/818321">this StackOverlow's question</a>
    ///
    /// This class exists for experimental purposes.
    /// </summary>
    public class WeakReferenceCache<TKey, TValue> : ICache<TKey, TValue> 
        where TValue : class
    {
        private readonly Dictionary<TKey, WeakReference<TValue>> _entries = 
            new Dictionary<TKey, WeakReference<TValue>>();

        public void AddOrReplace(TKey key, TValue value)
        {
            _entries[key] = new WeakReference<TValue>(value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);

            if (!_entries.TryGetValue(key, out var valueReference) ||
                !valueReference.TryGetTarget(out var target))
            {
                return false;
            }

            value = target;
            return true;
        }
    }
}
