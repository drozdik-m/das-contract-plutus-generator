using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types.Premade
{
    public class PlutusContractMonad : PlutusPremadeType
    {
        public PlutusContractMonad(INamable share, INamable schema, INamable error, INamable producedValue)
        {
            Share = share;
            Schema = schema;
            Error = error;
            ProducedValue = producedValue;
        }

        public INamable Share { get; }
        public INamable Schema { get; }
        public INamable Error { get; }
        public INamable ProducedValue { get; }

        public override string Name => $"Contract {PlutusCode.ProperlyBracketed(Share.Name)} {PlutusCode.ProperlyBracketed(Schema.Name)} {PlutusCode.ProperlyBracketed(Error.Name)} {PlutusCode.ProperlyBracketed(ProducedValue.Name)}";


        public static PlutusContractMonad Type(INamable share, INamable schema, INamable error, INamable producedValue) 
            => new PlutusContractMonad(share, schema, error, producedValue);
        
    }
}
