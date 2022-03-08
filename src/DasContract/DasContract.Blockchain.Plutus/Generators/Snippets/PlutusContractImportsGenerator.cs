using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;

namespace DasContract.Blockchain.Plutus.Generators.Snippets
{
    public class PlutusContractImportsGenerator : ICodeGenerator
    {
        public IPlutusCode Generate()
        {
            var imports = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusImport(0, "Control.Monad hiding (fmap)"),
                new PlutusImport(0, "Data.Aeson (ToJSON, FromJSON)"),
                new PlutusImport(0, "Data.Map as Map"),
                new PlutusImport(0, "Data.Default"),
                new PlutusImport(0, "Data.Text (Text, pack)"),
                new PlutusImport(0, "Data.Void (Void)"),
                new PlutusImport(0, "Data.Monoid (Last (..))"),
                new PlutusImport(0, "GHC.Generics (Generic)"),
                new PlutusImport(0, "Plutus.Contract"),
                new PlutusImport(0, "Plutus.Contract.StateMachine"),
                new PlutusImport(0, "Plutus.Contract.StateMachine.ThreadToken"),
                new PlutusImport(0, "PlutusTx (Data (..))"),
                new PlutusImport(0, "qualified PlutusTx"),
                new PlutusImport(0, "PlutusTx.Prelude hiding (Semigroup(..), unless)"),
                new PlutusImport(0, "Ledger hiding (singleton)"),
                new PlutusImport(0, "Ledger.Typed.Tx"),
                new PlutusImport(0, "Ledger.Constraints as Constraints"),
                new PlutusImport(0, "qualified Ledger.Typed.Scripts as Scripts"),
                new PlutusImport(0, "Ledger.Ada as Ada"),
                new PlutusImport(0, "Prelude (IO, Semigroup (..), Show (..), String)"),
                new PlutusImport(0, "Text.Printf (printf)"),
                new PlutusImport(0, "qualified PlutusTx.IsData as PlutusTx"),
                new PlutusImport(0, "Plutus.V1.Ledger.Value"),
                new PlutusImport(0, "Data.Char (GeneralCategory(CurrencySymbol))"),
                new PlutusImport(0, "qualified Ledger.Constraints.TxConstraints as Constraints"),
                new PlutusImport(0, "Plutus.V1.Ledger.Bytes (fromHex)"),
                new PlutusImport(0, "Data.String"),
                PlutusLine.Empty,
            });

            return imports;
        }
    }
}
