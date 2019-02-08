using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    internal class TransactionMetadataDownloader : ServiceBase
    {
        public Channel<TransactionMetadata> Inbox { get; } = new Channel<TransactionMetadata>();

        private Repository<TransactionMetadata> _metadataRepository;

        public TransactionMetadataDownloader(Repository<TransactionMetadata> metadataRepository)
        {
            _metadataRepository = metadataRepository;
        }

        public override async Task DoProcess()
        {
            var txData = await Inbox.TakeAsync();
            if(await _metadataRepository.ContainsAsync(txData.TransactionId))
                return;
            
            await _metadataRepository.SaveAsync(txData.TransactionId, txData);
        }
    }

}
