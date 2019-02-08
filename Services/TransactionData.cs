using System;
using System.Text;
using NBitcoin;
using Newtonsoft.Json;

namespace WasabiRealFeeCalc
{
    class TransactionMetadata
    {
		[JsonConverter(typeof(Uint256JsonConverter))]
        public uint256 TransactionId { get; set; }
        public uint BlockNumber { get; set; }
        public DateTimeOffset FirstTimeSeen { get; set; }
        public DateTimeOffset Time { get; set; }
        public uint Size { get; set; }
        public uint VirtualSize { get; set; }
    }

    class RawData
    {
        public string BlockNumber {get; set;}
        public string Time {get; set;}
        public string FirstTimeSeen {get; set;}
        public string TransactionId {get; set;}
        public string Size {get; set;}
        public string VirtualSize {get; set;}
        public string Index {get; set;}
        public string Value {get; set;}
        public string Address {get; set;}

        public override bool Equals(object obj)
        {
            var other = obj as RawData;
            if(other == null) return false;
            return other.ToString() == this.ToString();
        }

        private string _internal = null;

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            if(_internal == null)
            {
                var sb = new StringBuilder();
                sb.Append(BlockNumber); sb.Append(", ");
                sb.Append(Time); sb.Append(", ");
                sb.Append(FirstTimeSeen); sb.Append(", ");
                sb.Append(TransactionId); sb.Append(", ");
                sb.Append(Size); sb.Append(", ");
                sb.Append(VirtualSize); sb.Append(", ");
                sb.Append(Index); sb.Append(", ");
                sb.Append(Value); sb.Append(", ");
                sb.Append(Address); 
                _internal = sb.ToString();
            }
            return _internal;
        }

        public static string GetHeadersString()
        {
            var sb = new StringBuilder();
            sb.Append("BlockNumber"); sb.Append(", ");
            sb.Append("Time"); sb.Append(", ");
            sb.Append("FirstTimeSeen"); sb.Append(", ");
            sb.Append("TransactionId"); sb.Append(", ");
            sb.Append("Size"); sb.Append(", ");
            sb.Append("VirtualSize"); sb.Append(", ");
            sb.Append("Index"); sb.Append(", ");
            sb.Append("Value"); sb.Append(", ");
            sb.Append("Address");
            return sb.ToString();
        } 
    }
}