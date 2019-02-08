using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    internal class CsvUpdater : ServiceBase
    {
        public Channel<RawData> Inbox { get; } = new Channel<RawData>();

        private string _fileName;
        private bool _initialized = false;
        private IDictionary<RawData,bool> _cache = new Dictionary<RawData,bool>(); 

        public CsvUpdater(string fileName)
        {
            _fileName = fileName;
        }

        public override async Task DoProcess()
        {
            await EnsureCsvFileExistsAsync();
            var rawData = await Inbox.TakeAsync();

            if(_cache.ContainsKey(rawData)) return;

            _cache.Add(rawData, true);
            await File.AppendAllLinesAsync(_fileName,  new[]{rawData.ToString()});
        }

        private async Task EnsureCsvFileExistsAsync()
        {
            if(_initialized) return;

            if(!File.Exists(_fileName))
            {
                await File.AppendAllLinesAsync(_fileName,  new[]{RawData.GetHeadersString()});
            }
            else
            {
                foreach(var line in (await File.ReadAllLinesAsync(_fileName)).Skip(1))
                {
                    var lineParts = line.Split(',');
                    var rawData = new RawData{
                        BlockNumber  =lineParts[0],
                        Time=lineParts[1],
                        FirstTimeSeen=lineParts[2],
                        TransactionId=lineParts[3],
                        Size=lineParts[4],
                        VirtualSize=lineParts[5],
                        Index=lineParts[6],
                        Value=lineParts[7],
                        Address=lineParts[8]
                    };
                    if(_cache.ContainsKey(rawData)) continue;;

                    _cache.Add(rawData, true);
                }
            }
            _initialized=true;
        }
    }
}
