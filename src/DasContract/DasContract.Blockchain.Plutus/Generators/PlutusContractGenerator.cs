using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Convertors;
using DasContract.Blockchain.Plutus.Code.Convertors.DataType;
using DasContract.Blockchain.Plutus.Code.Keywords;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Determined;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.Blockchain.Plutus.Generators.Snippets;
using DasContract.Blockchain.Plutus.Transitions;
using DasContract.Blockchain.Plutus.Transitions.NonTx;
using DasContract.Blockchain.Plutus.Utils;

namespace DasContract.Blockchain.Plutus.Generators
{
    /// <summary>
    /// A generator that is capable of creating plutus code from a plutus contract data model
    /// </summary>
    public class PlutusContractGenerator
    {
        private readonly ICodeGenerator pragmaGenerator;
        private readonly ICodeGenerator moduleGenerator;
        private readonly ICodeGenerator importsGenerator;
        private readonly ICodeGenerator dataModelsGenerator;
        private readonly ICodeGenerator onChainGenerator;
        private readonly ICodeGenerator offChainGenerator;

        public PlutusContractGenerator(
            ICodeGenerator pragmaGenerator,
            ICodeGenerator moduleGenerator,
            ICodeGenerator importsGenerator,
            ICodeGenerator dataModelsGenerator,
            ICodeGenerator onChainGenerator,
            ICodeGenerator offChainGenerator)
        {
            this.pragmaGenerator = pragmaGenerator;
            this.moduleGenerator = moduleGenerator;
            this.importsGenerator = importsGenerator;
            this.dataModelsGenerator = dataModelsGenerator;
            this.onChainGenerator = onChainGenerator;
            this.offChainGenerator = offChainGenerator;
        }

        /// <summary>
        /// Generates plutus code from a plutus contract
        /// </summary>
        /// <param name="contract">The contract data model to translate</param>
        /// <returns>Plutus code</returns>
        public IPlutusCode Generate()
        {
            //--- Pragma -----------------------------------------
            var pragmas = pragmaGenerator.Generate();

            //--- Module -----------------------------------------
            var module = moduleGenerator.Generate();

            //--- Imports ----------------------------------------
            var imports = importsGenerator.Generate();

            //--- Data models ------------------------------------
            var dataModels = dataModelsGenerator.Generate();

            //--- On chain ---------------------------------------
            var onChain = onChainGenerator.Generate();

            //--- Off chain --------------------------------------
            var offChain = offChainGenerator.Generate();

            //Result
            return pragmas
                .Append(module)
                .Append(imports)
                .Append(dataModels)
                .Append(onChain)
                .Append(offChain);
        }

        /// <summary>
        /// Returns ready-to-go default/standard plutus contract generator
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static PlutusContractGenerator Default(PlutusContract contract)
        {
            return new PlutusContractGenerator(
                new PlutusContractPragmaGenerator(),
                new PlutusContractModuleGenerator(),
                new PlutusContractImportsGenerator(),
                new PlutusContractDataModelGenerator(contract),
                new PlutusContractOnChainGenerator(contract),
                new PlutusContractOffChainGenerator(contract));
        }
        
    }
}
