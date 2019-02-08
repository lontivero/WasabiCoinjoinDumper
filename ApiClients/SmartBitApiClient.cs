using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NBitcoin;
using Newtonsoft.Json.Linq;

namespace WasabiRealFeeCalc
{
    internal class SmartBitApiClient
    {
        private string _nextToken = string.Empty;

        public async Task<JObject> GetAddressTransactionsAsync(BitcoinAddress address)
        {
            var nextLink = $"blockchain/address/{address}" + (string.IsNullOrEmpty(_nextToken) ? "" : $"?next={_nextToken}");
            var content = await GetSmartBitApiClientAsync(nextLink);
            var json = JObject.Parse(content);
            _nextToken = json["address"]["transaction_paging"]["next"]?.ToString();
            return json;
        }

        private static async Task<string> GetSmartBitApiClientAsync(string path)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://api.smartbit.com.au/v1/");

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
