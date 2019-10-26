using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace WasabiRealFeeCalc
{

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var client = new SmartBitApiClient();

            while(true)
            {
                JObject json = null;
                while(json == null)
                {
                    try
                    {
                        json = await client.GetAddressTransactionsAsync(Constants.CoordinatorAddress);
                    }
                    catch(Exception)
                    {
                        await Task.Delay(2000);
                    }
                }

                var jsonTransactions = json["address"]["transactions"];
                foreach (var transaction in jsonTransactions)
                {
                    var confirmations = transaction.Value<int>("confirmations");
                    if(confirmations == 0) continue;

                    var txId = uint256.Parse(transaction.Value<string>("txid"));
                    var blockNumber = transaction.Value<uint>("block");
                    var firstTimeSeen = Utils.UnixTimeToDateTime(transaction.Value<ulong>("first_seen"));
                    var time = Utils.UnixTimeToDateTime(transaction.Value<ulong>("time"));

                    var outputs = transaction["outputs"];
                    foreach (var output in outputs)
                    {
                        var address = output["addresses"].First();
                        if(address.ToString() != Constants.CoordinatorAddress.ToString()) 
                            continue;

                        var index = output.Value<int>("n");
                        var value = output.Value<decimal>("value");
//                        Console.WriteLine($"{blockNumber}, {time}, {firstTimeSeen}, {txId}, {index}, {value}, {address}");
                        Console.WriteLine($"{firstTimeSeen}, {txId}, {index}, {value}");
                    }
                }
            }
        }
    }
}
