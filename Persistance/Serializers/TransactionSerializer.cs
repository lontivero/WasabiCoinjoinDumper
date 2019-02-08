using NBitcoin;

namespace WasabiRealFeeCalc
{
    class TransactionSerializer : ISerializer<Transaction>
    {
        public TransactionSerializer()
        {
        }

        public byte[] Serialize(Transaction item)
        {
            return item.ToBytes();
        }
        public Transaction Deserialize(byte[] content)
        {
            return Transaction.Load(content, Network.Main);
        }
    }
}
