using System;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace WasabiRealFeeCalc
{
    internal class TransactionDiscoverer : ServiceBase
    {
        private SmartBitApiClient _client = new SmartBitApiClient();
        private Channel<TransactionMetadata> _processorChannel;

        public TransactionDiscoverer(Channel<TransactionMetadata> processorChannel)
        {
            _processorChannel = processorChannel;
        }

        public override async Task DoProcess()
        {
            try
            {
                Console.WriteLine("Checking for new coinjoin transactions....");
                var json = await _client.GetAddressTransactionsAsync(Constants.CoordinatorAddress);
                var jsonTransactions = json["address"]["transactions"];
                foreach (var transaction in jsonTransactions)
                {
                    var confirmations = transaction.Value<int>("confirmations");
                    if(confirmations == 0) continue;

                    var txData = new TransactionMetadata {
                        TransactionId = uint256.Parse(transaction.Value<string>("txid")),
                        BlockNumber = transaction.Value<uint>("block"),
                        FirstTimeSeen = Utils.UnixTimeToDateTime(transaction.Value<ulong>("first_seen")),
                        Time = Utils.UnixTimeToDateTime(transaction.Value<ulong>("time")),
                        Size = transaction.Value<uint>("size"),
                        VirtualSize = transaction.Value<uint>("vsize")
                    };

                    _processorChannel.Send(txData);
                }
            }
            catch(Exception)
            {
                // If fail it will retry
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        }
    }
}