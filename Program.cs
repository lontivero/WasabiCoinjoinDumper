using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{

    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = new TransactionProviderBuilder();
            await builder.BuildAsync();

            var csv = new CsvUpdater("WasabiCoinjoins.csv");
            var watcher = new DataDirectoryWatcher(builder.TransactionRepository, builder.TransactionMetadataRepository, csv.Inbox);
            var crawlerService = new TransactionCrawler(builder.TransactionProvider, builder.TransactionRepository, builder.TransactionMetadataRepository);
            csv.Start();
            crawlerService.Start();
            watcher.Start();

            var i = 0;
            while(i<10000)
            {
                await Task.Delay(1000);
                i++;
            }
        }
    }
}
