using System;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    internal class TransactionProviderBuilder
    {
        public Repository<Transaction> TransactionRepository { get; private set; }
        public ITransactionProvider TransactionProvider { get; private set; }
        public IBlockProvider BlockProvider { get; private set; }

        internal async Task BuildAsync()
        {
            var dataProvider = new BlockchainInfoClient();

            TransactionRepository = new Repository<Transaction>(new TransactionSerializer());
            var transactionsMemoryCache = new MemoryCache<Transaction>();
            var transactionsPersistentCache = new PersistentCache<Transaction>(TransactionRepository);
            var transactionsPersistentCachedProvider = new CachedTransactionProvider(dataProvider, transactionsPersistentCache);
            var transactionsMemoryCachedProvider = new CachedTransactionProvider(transactionsPersistentCachedProvider, transactionsMemoryCache);

            foreach(var tx in TransactionRepository.Enumerate())
            {
                await transactionsMemoryCache.SaveAsync(tx.GetHash(), tx);
            }

            TransactionProvider = transactionsMemoryCachedProvider;

            var blocksRepository = new Repository<Block>(new BlockSerializer());
            var blocksPersistentCache = new PersistentCache<Block>(blocksRepository);
            var blocksPersistenceCachedProvider = new CachedBlockProvider(dataProvider, blocksPersistentCache);

            BlockProvider = blocksPersistenceCachedProvider;
        }
    }
}