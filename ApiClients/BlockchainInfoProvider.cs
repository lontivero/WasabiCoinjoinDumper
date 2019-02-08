using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.BitcoinCore;

namespace WasabiRealFeeCalc
{
    interface IBlockProvider
    {
        Task<Block> GetBlockByIdAsync(uint256 id);
        Task<uint256> GetTipAsync();
    }

    class BlockchainInfoClient : ITransactionProvider, IBlockProvider
    {
        public async Task<Transaction> GetTransactionByIdAsync(uint256 id)
        {
            var content = await GetBlockchainInfoAsync($"/tx/{id}?format=hex");
            return Transaction.Parse(content, Network.Main);
        }

        public async Task<Block> GetBlockByIdAsync(uint256 id)
        {
            var content = await GetBlockchainInfoAsync($"/block/{id}?format=hex");
            return Block.Parse(content, Network.Main);
        }

        public async Task<uint256> GetTipAsync()
        {
            var content = await GetBlockchainInfoAsync("/latestblock");
            var id = content.Substring(content.IndexOf('0'), 64);
            return uint256.Parse(id);
        }

        private static async Task<string> GetBlockchainInfoAsync(string path)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://blockchain.info");

                using (var response = await httpClient.GetAsync(path, HttpCompletionOption.ResponseContentRead))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new HttpRequestException(response.StatusCode.ToString());
                    string responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }
        }
    }
}
