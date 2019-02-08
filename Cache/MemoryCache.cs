using System.Collections.Generic;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class MemoryCache<T> : ICache<T>
    {
        private readonly IDictionary<uint256, T> _cache = new Dictionary<uint256, T>();

        public Task SaveAsync(uint256 id,  T item)
        {
            if(!_cache.ContainsKey(id))
                _cache.Add(id, item);
            return Task.CompletedTask;
        }

        public Task<(bool, T)> TryGetAsync(uint256 id)
        {
            if(_cache.ContainsKey(id))
                return Task.FromResult((true, _cache[id]));
            
            return Task.FromResult((false, default(T)));
        }
    }
}
