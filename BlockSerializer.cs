using NBitcoin;

namespace WasabiRealFeeCalc
{
    class BlockSerializer : ISerializer<Block>
    {
        public BlockSerializer()
        {
        }

        public byte[] Serialize(Block item)
        {
            return item.ToBytes();
        }
        public Block Deserialize(byte[] content)
        {
            return Block.Load(content, Network.Main);
        }
    }
}
