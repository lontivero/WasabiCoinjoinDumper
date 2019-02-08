using System.IO;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class PersistentCache<T> : ICache<T> where T: class
    {
        private readonly string DirectoryName = Path.Combine("Cache", typeof(T).Name);
        private readonly Repository<T> _repository;

        public PersistentCache(Repository<T> repository)
        {
            _repository = repository;
        }

        public async Task SaveAsync(uint256 id,  T item)
        {
            await _repository.SaveAsync(id, item);
        }

        public async Task<(bool, T)> TryGetAsync(uint256 id)
        {
            var item = await _repository.GetAsync(id);
            return (item != default(T), item);
        }
    }
}
