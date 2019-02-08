namespace WasabiRealFeeCalc
{
    public interface ISerializer<T>
    {
        byte[] Serialize(T item);
        T Deserialize(byte[] content);
    }
}
