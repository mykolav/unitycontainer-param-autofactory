namespace Unity.ParameterizedAutoFactory.Core.Caches
{
    internal interface ICache<in TKey, TValue>
        where TValue : class
    {
        void AddOrReplace(TKey key, TValue value);
        bool TryGetValue(TKey key, out TValue value);
    }
}