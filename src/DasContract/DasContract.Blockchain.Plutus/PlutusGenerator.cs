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
            });

            //--- Module -----------------------------------------
            var module = new PlutusCode(new List<IPlutusLine>()
            {
                new PlutusRawLine(0, "module PlutusContract"),
                    new PlutusRawLine(1, "(module PlutusContract)"),
                    new PlutusRawLine(1, "where"),
            });

            //----------------------------------------------------
            //--                 DATA MODELS                   ---
            //----------------------------------------------------
            IPlutusCode dataModels = new PlutusSectionComment(0, "DATA MODELS");

            // --- Timer event ---
            dataModels = dataModels.Append(new PlutusSubsectionComment(0, "Timer event"));
            var timerEventData = new PlutusAlgebraicType("TimerEvent", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("IntTime", new List<string>()),
                new PlutusAlgebraicTypeConstructor("TimedOut", new List<string>())
            }, new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON",
                "ToSchema"
            });
            dataModels = dataModels.Append(timerEventData);
            dataModels = dataModels.Append(new PlutusMakeLift(timerEventData));
            dataModels = dataModels.Append(new PlutusUnstableMakeIsData(timerEventData));


            //Result
            return pragmas
                .Append(module)
                .Append(dataModels);
        }
    }
}
