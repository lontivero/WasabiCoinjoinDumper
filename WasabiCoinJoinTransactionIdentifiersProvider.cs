using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class TransactionCrawler : ServiceBase
    {
        public Channel<Transaction> Inbox { get; } = new Channel<Transaction>();

        private readonly ITransactionProvider _transactionProvider;
        private readonly Repository<Transaction> _transactionRepository;
        private readonly TransactionDownloader _transactionDownloader;
        private readonly TransactionMetadataDownloader _transactionMetadataDownloader;
        private readonly LatestCoinJoinTransactionFinder _latestCoinJoinTransactionFinder;

        private int _coinjoinTransactionCounter = 0;

        public TransactionCrawler(ITransactionProvider transactionProvider, Repository<Transaction> transactionRepository)
        {
            _transactionProvider = transactionProvider;
            _transactionRepository = transactionRepository;
            _transactionDownloader = new TransactionDownloader(transactionProvider, _transactionRepository, Inbox);
            _transactionMetadataDownloader = new TransactionMetadataDownloader();
            _latestCoinJoinTransactionFinder = new LatestCoinJoinTransactionFinder(_transactionProvider, _transactionDownloader.Inbox);
        }

        public override void Start()
        {
            _transactionDownloader.Start();
            _transactionMetadataDownloader.Start();
            _latestCoinJoinTransactionFinder.Start();
        }

        public override async Task DoProcess()
        {
            var tx = await Inbox.TakeAsync();
            Console.WriteLine($"CoinJoin transaction found ID {tx.GetHash()}.");

            _transactionDownloader.Inbox.Send(tx);
            _transactionMetadataDownloader.Inbox.Send(tx);
            _coinjoinTransactionCounter++;
        }
    }

    class TransactionDownloader : ServiceBase
    {
        public Channel<Transaction> Inbox { get; } = new Channel<Transaction>();
        private readonly Channel<Transaction> _processorInbox;

        private readonly ITransactionProvider _transactionProvider;
        private readonly Repository<Transaction> _transactionRepository;

        public TransactionDownloader(ITransactionProvider transactionProvider, Repository<Transaction> transactionRepository, Channel<Transaction> processorInbox)
        {
            _transactionProvider = transactionProvider;
            _transactionRepository = transactionRepository;
            _processorInbox = processorInbox;
        }

        public override async Task DoProcess()
        {
            var tx = await Inbox.TakeAsync();

            foreach(var prevTxId in tx.Inputs.Select(x=>x.PrevOut.Hash).Distinct())
            {
                if (await _transactionRepository.ContainsAsync(prevTxId)) 
                    continue;

                tx = await _transactionProvider.GetTransactionByIdAsync(prevTxId);
                if(tx.IsWasabiCoinJoin())
                {
                    _processorInbox.Send(tx);
                }
            }
        }

        private async Task<(Block, Transaction)> FindMostRecentCoinJoinTransactionAsync(uint256 tip)
        {
            var tx = await _transactionProvider.GetTransactionByIdAsync(uint256.Parse("d1774d05fda847fd52612ff27ad4c280c172a3607e93c45e15b6ae1193dae7db"));
            return (null, tx);
/*          while(true)
            {
                Console.Write($"Downloading block {tip}");
                var block = await  _blockProvider.GetBlockByIdAsync(tip);
                foreach(var tx in block.Transactions)
                {
                    if(tx.IsWasabiCoinJoin())
                        return (block, tx);
                }
                Console.WriteLine(" CoinJoin not found");
                tip = block.Header.HashPrevBlock;
            }
*/
        }
    }

    class LatestCoinJoinTransactionFinder : ServiceBase
    {
        private uint256 _latestTxId = uint256.Zero;
        private readonly Channel<Transaction> _processorInblox;
        private readonly ITransactionProvider _transactionProvider;

        public LatestCoinJoinTransactionFinder(ITransactionProvider transactionProvider, Channel<Transaction> processorInblox)
        {
            _transactionProvider = transactionProvider;
            _processorInblox = processorInblox;
        }

        public override async Task DoProcess()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://www.wasabiwallet.io");

                using (var response = await httpClient.GetAsync("/unversioned/coinjoins-table.html", HttpCompletionOption.ResponseContentRead))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new HttpRequestException(response.StatusCode.ToString());
                    var responseString = await response.Content.ReadAsStringAsync();
                    var txId = uint256.Parse(responseString.Substring(responseString.IndexOf("tx/")+3, 64));
                    if(txId != _latestTxId)
                    {
                        _latestTxId = txId;
                        var tx = await _transactionProvider.GetTransactionByIdAsync(_latestTxId);
                        _processorInblox.Send(tx);
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(2));
        }
    }

    class TransactionMetadataDownloader : ServiceBase
    {
        public Channel<Transaction> Inbox { get; } = new Channel<Transaction>();

        private ITransactionProvider _transactionProvider;

/*
        public TransactionMetadataDownloader(ITransactionProvider transactionProvider)
        {
            _transactionProvider = transactionProvider;
        }
*/
        public override async Task DoProcess()
        {
            var tx = await Inbox.TakeAsync();
        }
    }

}
