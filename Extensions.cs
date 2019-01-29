using System.Linq;
using NBitcoin;

namespace WasabiRealFeeCalc
{
    static class TransactionExtensions
    {
        public static bool IsWasabiCoinJoin(this Transaction tx)
        {
            var payToWasabi = tx.Outputs.Exists(x=>x.ScriptPubKey == Constants.CoordinatorAddress.ScriptPubKey);
            var nroInputs   = tx.Inputs.Count;
            var nroOutputs = tx.Outputs.Count; 
            var allSegwitInputs = tx.Inputs.Count(x=>x.WitScript!=WitScript.Empty) == tx.Inputs.Count;
            var allSegwitOutputs = tx.Outputs.Count(x=>x.ScriptPubKey.IsWitness) == tx.Outputs.Count;
            var mixingLevels = tx.Outputs
                                .GroupBy(x => x.Value)
                                .ToDictionary(o=>o.Key, o=>o.Count())
                                .Select(x=> new { Denomination = x.Key, AnonymitySet= x.Value})
                                .Where(x=>x.AnonymitySet > 1)
                                .OrderBy(x=>x.Denomination)
                                .ToArray();

            var equalAmountOutputs = mixingLevels.Sum(x=>x.AnonymitySet);

            if(!allSegwitInputs)  return false;
            if(!allSegwitOutputs) return false;
            if( nroInputs < 20)   return false;
            if( nroOutputs < 24)  return false;
            if( nroOutputs < 24)  return false;
            if( equalAmountOutputs < (nroInputs * 0.8)) return false;
            if( mixingLevels.Length > 1 && (mixingLevels[1].Denomination != (2 * mixingLevels[0].Denomination)))
            {
                if(!payToWasabi)
                    return false;
            } 
            return true;
        }
    }
}