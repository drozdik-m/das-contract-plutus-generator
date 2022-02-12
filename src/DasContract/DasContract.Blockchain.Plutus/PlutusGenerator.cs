using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Data;

namespace DasContract.Blockchain.Plutus
{
    public class PlutusGenerator
    {
        /// <summary>
        /// Generates Plutus code 
        /// </summary>
        /// <param name="contract">The contract data model to translate</param>
        /// <returns>Plutus code</returns>
        public IPlutusCode GeneratePlutusContract(PlutusContract contract)
        {
            //--- Pragma -----------------------------------------
            var pragmas = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusPragma(0, "LANGUAGE DataKinds"),
                new PlutusPragma(0, "LANGUAGE DeriveAnyClass"),
                new PlutusPragma(0, "LANGUAGE DeriveGeneric"),
                new PlutusPragma(0, "LANGUAGE FlexibleContexts"),
                new PlutusPragma(0, "LANGUAGE MultiParamTypeClasses"),
                new PlutusPragma(0, "LANGUAGE NoImplicitPrelude"),
                new PlutusPragma(0, "LANGUAGE OverloadedStrings"),
                new PlutusPragma(0, "LANGUAGE ScopedTypeVariables"),
                new PlutusPragma(0, "LANGUAGE TemplateHaskell"),
                new PlutusPragma(0, "LANGUAGE TypeApplications"),
                new PlutusPragma(0, "LANGUAGE TypeFamilies"),
                new PlutusPragma(0, "LANGUAGE TypeOperators"),
                new PlutusEmptyLine(),
                new PlutusPragma(0, "OPTIONS_GHC -fno-warn-unused-imports"),
                new PlutusEmptyLine(),
            });

            //--- Module -----------------------------------------
            var module = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusRawLine(0, "module PlutusContract"),
                    new PlutusRawLine(1, "(module PlutusContract)"),
                    new PlutusRawLine(1, "where"),

                new PlutusEmptyLine(),
                new PlutusEmptyLine(),
            });


            //--- Imports ----------------------------------------
            var imports = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusImport(0, "Control.Monad hiding (fmap)"),
                new PlutusImport(0, "Data.Aeson (ToJSON, FromJSON)"),
                new PlutusImport(0, "Data.Map as Map"),
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
                new PlutusEmptyLine(),
            });

            //----------------------------------------------------
            //--                 DATA MODELS                   ---
            //----------------------------------------------------
            IPlutusCode dataModels = new PlutusSectionComment(0, "DATA MODELS");

            // --- Timer event -----------------------------------
            dataModels = dataModels
                .Append(new PlutusEmptyLine())
                .Append(new PlutusSubsectionComment(0, "Timer event"));
            var timerEventData = new PlutusAlgebraicType("TimerEvent", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("InTime", new List<string>()),
                new PlutusAlgebraicTypeConstructor("TimedOut", new List<string>())
            }, new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(timerEventData)
                .Append(new PlutusMakeLift(timerEventData))
                .Append(new PlutusUnstableMakeIsData(timerEventData))
                .Append(new PlutusEmptyLine())
                .Append(new PlutusEq(timerEventData));


            // -- Sequential multi instance ----------------------
            dataModels = dataModels
                .Append(new PlutusEmptyLine())
                .Append(new PlutusSubsectionComment(0, "Sequential multi instance"));
            var sequentialMultiInstanceData = new PlutusAlgebraicType("SequentialMultiInstance", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("ToLoop", new List<string>{ "Integer" }),
                new PlutusAlgebraicTypeConstructor("LoopEnded", new List<string>())
            }, new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });
            dataModels = dataModels
                .Append(sequentialMultiInstanceData)
                .Append(new PlutusMakeLift(sequentialMultiInstanceData))
                .Append(new PlutusUnstableMakeIsData(sequentialMultiInstanceData))
                .Append(new PlutusEmptyLine())
                .Append(new PlutusEq(sequentialMultiInstanceData));


            //Result
            return pragmas
                .Append(module)
                .Append(imports)
                .Append(dataModels);
        }
    }
}
