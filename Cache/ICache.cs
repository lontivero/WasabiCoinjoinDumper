using System.Threading.Tasks;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    interface ICache<T>
    {
        Task<(bool, T)> TryGetAsync(uint256 id);
        Task SaveAsync(uint256 id, T item);
    }
}
