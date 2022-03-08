using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusStateMachine : PlutusPremadeType
    {
        public PlutusStateMachine(INamable datum, INamable redeemer)
        {
            Datum = datum;
            Redeemer = redeemer;
        }

        
        public INamable Datum { get; }
        public INamable Redeemer { get; }

        public override string Name => $"StateMachine {PlutusCode.ProperlyBracketed(Datum.Name)} {PlutusCode.ProperlyBracketed(Redeemer.Name)}";


        public static PlutusStateMachine Type(INamable datum, INamable redeemer) 
            => new PlutusStateMachine(datum, redeemer);
        
    }
}
