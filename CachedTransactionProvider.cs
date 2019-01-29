using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class CachedTransactionProvider : ITransactionProvider
    {
        private readonly ITransactionProvider _inner;
        private readonly ICache<Transaction> _cache;

        public CachedTransactionProvider()
        {
        }

        public CachedTransactionProvider(ITransactionProvider inner, ICache<Transaction> cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<Transaction> GetTransactionByIdAsync(uint256 txid)
        {
            var (found, tx) = await _cache.TryGetAsync(txid);
            if(found) return tx;

            tx = await _inner.GetTransactionByIdAsync(txid);
            await _cache.SaveAsync(txid, tx);
            return tx;
        }
    }

    class CachedBlockProvider : IBlockProvider
    {
        private readonly IBlockProvider _inner;
        private readonly ICache<Block> _cache;

        public CachedBlockProvider(IBlockProvider inner, ICache<Block> cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public async Task<Block> GetBlockByIdAsync(uint256 blkid)
        {
            var (found, blk) = await _cache.TryGetAsync(blkid);
            if(found) return blk;

            blk = await _inner.GetBlockByIdAsync(blkid);
            await _cache.SaveAsync(blkid, blk);
            return blk;
        }

        public async Task<uint256> GetTipAsync()
        {
            return await _inner.GetTipAsync();
        }
    }    
}
