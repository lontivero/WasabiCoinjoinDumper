using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class TransactionCrawler : ServiceBase
    {
        public Channel<TransactionMetadata> Inbox { get; } = new Channel<TransactionMetadata>();

        private readonly ITransactionProvider _transactionProvider;
        private readonly Repository<Transaction> _transactionRepository;
        private readonly TransactionDownloader _transactionDownloader;
        private readonly TransactionMetadataDownloader _transactionMetadataDownloader;
        private readonly TransactionDiscoverer _transactionDiscoverer;

        private int _coinjoinTransactionCounter = 0;

        public TransactionCrawler(ITransactionProvider transactionProvider, 
            Repository<Transaction> transactionRepository, 
            Repository<TransactionMetadata> transactionMetadataRepository)
        {
            _transactionProvider = transactionProvider;
            _transactionRepository = transactionRepository;
            _transactionDownloader = new TransactionDownloader(transactionProvider, _transactionRepository);
            _transactionDiscoverer = new TransactionDiscoverer(Inbox);
            _transactionMetadataDownloader = new TransactionMetadataDownloader(transactionMetadataRepository);
        }

        public override void Start()
        {
            _transactionDownloader.Start();
            _transactionMetadataDownloader.Start();
            _transactionDiscoverer.Start();
            base.Start();
        }

        public override async Task DoProcess()
        {
            var txData = await Inbox.TakeAsync();
            Console.WriteLine($"Processing transaction ID {txData.TransactionId}.");

            _transactionDownloader.Inbox.Send(txData.TransactionId);
            _transactionMetadataDownloader.Inbox.Send(txData);

            _coinjoinTransactionCounter++;
        }
    }

}
