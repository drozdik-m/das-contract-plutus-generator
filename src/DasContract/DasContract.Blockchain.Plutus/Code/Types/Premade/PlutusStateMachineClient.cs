using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusStateMachineClient : PlutusPremadeType
    {
        public PlutusStateMachineClient(INamable datum, INamable redeemer)
        {
            Datum = datum;
            Redeemer = redeemer;
        }

        
        public INamable Datum { get; }
        public INamable Redeemer { get; }

        public override string Name => $"StateMachineClient {PlutusCode.ProperlyBracketed(Datum.Name)} {PlutusCode.ProperlyBracketed(Redeemer.Name)}";


        public static PlutusStateMachineClient Type(INamable datum, INamable redeemer) 
            => new PlutusStateMachineClient(datum, redeemer);
        
    }
}
