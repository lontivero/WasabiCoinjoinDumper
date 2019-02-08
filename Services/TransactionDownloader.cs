using System;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    internal class TransactionDownloader : ServiceBase
    {
        public Channel<uint256> Inbox { get; } = new Channel<uint256>();
        private readonly ITransactionProvider _transactionProvider;
        private readonly Repository<Transaction> _transactionRepository;

        public TransactionDownloader(ITransactionProvider transactionProvider, Repository<Transaction> transactionRepository)
        {
            _transactionProvider = transactionProvider;
            _transactionRepository = transactionRepository;
        }

        public override async Task DoProcess()
        {
            var txId = await Inbox.TakeAsync();

            try
            {
                if (await _transactionRepository.ContainsAsync(txId)) 
                    return;
                var tx = await _transactionProvider.GetTransactionByIdAsync(txId);
                await _transactionRepository.SaveAsync(txId, tx);
            }
            catch(Exception)
            {
                Inbox.Send(txId); // retry
            }
        }
    }
}
