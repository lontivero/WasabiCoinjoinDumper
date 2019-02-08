using System.Text;
using NBitcoin;
using Newtonsoft.Json;

namespace WasabiRealFeeCalc
{
    class TransactionMetadataSerializer : ISerializer<TransactionMetadata>
    {
        public byte[] Serialize(TransactionMetadata item)
        {
            var json = JsonConvert.SerializeObject(item);
            return Encoding.UTF8.GetBytes(json);
        }
        public TransactionMetadata Deserialize(byte[] content)
        {
            var json = Encoding.UTF8.GetString(content);
            return JsonConvert.DeserializeObject<TransactionMetadata>(json);
        }
    }
}
