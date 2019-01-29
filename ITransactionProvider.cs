using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    interface ITransactionProvider
    {
        Task<Transaction> GetTransactionByIdAsync(uint256 txid);
    }
}
