using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    class DataDirectoryWatcher : ServiceBase
    {
        private readonly Channel<RawData[]> _processor;
        private readonly Repository<Transaction> _transactionRepository;
        private readonly Repository<TransactionMetadata> _transactionMetadataRepository;

        private IEnumerable<uint256> _lastTransactionFiles = Enumerable.Empty<uint256>();

        public DataDirectoryWatcher(Repository<Transaction> transactionRepository, Repository<TransactionMetadata> transactionMetadataRepository, Channel<RawData[]> processor)
        {
            _processor = processor;
            _transactionRepository = transactionRepository;
            _transactionMetadataRepository = transactionMetadataRepository;
        }

        public override async Task DoProcess()
        {
            var currentTransactions = _transactionMetadataRepository.GetIds().Intersect(_transactionRepository.GetIds());

            var newTransactions = currentTransactions.Except(_lastTransactionFiles);
            _lastTransactionFiles = currentTransactions;

            foreach(var txId in newTransactions)
            {
                ProcessTransaction(txId);
            }
            await Task.Delay(10);
        }

        public async void ProcessTransaction(uint256 txId)
        {
            var tx = await _transactionRepository.GetAsync(txId);
            if(!tx.IsWasabiCoinJoin())
                return;

            var meta = await _transactionMetadataRepository.GetAsync(txId);

            var i = 0;
            var arr = new RawData[tx.Outputs.Count()];
            foreach(var output in tx.Outputs)
            {
                try{
                    var rawData = new RawData {
                        BlockNumber = meta.BlockNumber.ToString(),
                        Time = meta.Time.ToUniversalTime().ToString(),
                        FirstTimeSeen = meta.FirstTimeSeen.ToUniversalTime().ToString(),
                        TransactionId = meta.TransactionId.ToString(),
                        Size = meta.Size.ToString(),
                        VirtualSize = meta.VirtualSize.ToString(),
                        Index = (i++).ToString(),
                        Value = output.Value.ToString(),    
                        Address = output.ScriptPubKey.GetDestinationAddress(Network.Main).ToString()
                    };
                arr[i-1] = rawData;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            _processor.Send(arr);
        }
    }
}