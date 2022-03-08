using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance;
using DasContract.Blockchain.Plutus.Data.DataModels;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes;
using DasContract.Blockchain.Plutus.Data.Users;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class PlutusContractConvertor : IConvertor<Contract, PlutusContract>
    {
        private readonly IConvertor<(IEnumerable<Entity>, IEnumerable<DasContract.Abstraction.Data.Enum>), ContractDataModel> contractDataModelConvertor;
        private readonly IConvertor<(IEnumerable<ProcessUser>, IEnumerable<ProcessRole>), ContractUsers> usersConvertor;
        private readonly IConvertor<IEnumerable<Process>, ContractProcesses> processesConvertor;

        public PlutusContractConvertor(
            IConvertor<(IEnumerable<Entity>, IEnumerable<DasContract.Abstraction.Data.Enum>), ContractDataModel> contractDataModelConvertor,
            IConvertor<(IEnumerable<ProcessUser>, IEnumerable<ProcessRole>), ContractUsers> usersConvertor,
            IConvertor<IEnumerable<Process>, ContractProcesses> processesConvertor)
        {
            this.contractDataModelConvertor = contractDataModelConvertor;
            this.usersConvertor = usersConvertor;
            this.processesConvertor = processesConvertor;
        }

        /// <inheritdoc/>
        public PlutusContract Convert(Contract source)
        {
            var result = new PlutusContract
            {
                Id = source.Id,
                //TODO GlobalValidationCode,
                DataModel = contractDataModelConvertor.Convert((source.Entities, source.Enums)),
                Identities = usersConvertor.Convert((source.Users, source.Roles)),
                Processes = processesConvertor.Convert(source.Processes),
            };

            foreach (var process in result.Processes.AllProcesses)
                ContractProcessConvertor.Bind(
                    process,
                    result.Identities.Users, 
                    result.Identities.Roles);

            return result;
        }

        /// <summary>
        /// Returns the ready-to-go default DasContract to Plutus contract convertor
        /// </summary>
        public static PlutusContractConvertor Default
        {
            get
            {
                //Data type convertors
                var propertyCardinalityConvertor = new PropertyCardinalityConvertor();
                var propertyTypeConvertor = new PrimitivePropertyTypeConvertor();
                var primitivePropertyConvertor = new PrimitivePropertyConvertor(
                    propertyTypeConvertor,
                    propertyCardinalityConvertor);
                var referencePropertyConvertor = new ReferencePropertyConvertor(
                    propertyCardinalityConvertor);
                var dictionaryPropertyConvertor = new DictionaryPropertyConvertor(
                    propertyTypeConvertor);
                var enumPropertyConvertor = new EnumPropertyConvertor(
                    propertyCardinalityConvertor);
                var propertyConvertor = new ContractPropertyConvertor(
                    primitivePropertyConvertor,
                    referencePropertyConvertor,
                    dictionaryPropertyConvertor,
                    enumPropertyConvertor);
                var entityConvertor = new ContractEntityConvertor(
                    propertyConvertor);
                var contractEnumConvertor = new EnumConvertor();
                var plutusDataModelConvertor = new ContractDataModelConvertor(
                    entityConvertor,
                    contractEnumConvertor);

                //User convertor
                var roleConvertor = new ContractRoleConvertor();
                var userConvertor = new ContractUserConvertor();
                var usersConvertor = new ContractUsersConvertor(
                    roleConvertor,
                    userConvertor);

                //Process convertor
                var multiInstanceConvertor = new ContractSequentialMultiInstanceConvertor();
                var scriptActivityConvertor = new ContractScriptActivityConvertor(
                    multiInstanceConvertor);
                var contractFieldTypeConvertor = new ContractFieldTypeConvertor();
                var userFormConvertor = new ContractFormConvertor(
                    contractFieldTypeConvertor);
                var userActivityConvertor = new ContractUserActivityConvertor(
                    multiInstanceConvertor,
                    userFormConvertor);
                var callActivityConvertor = new ContractCallActivityConvertor(
                    multiInstanceConvertor);
                var timerBoundaryConvertor = new ContractTimerBoundaryEventConvertor();
                var mergingExclusiveGatewayConvertor = new ContractMergingExclusiveGatewayConvertor();
                var exclusiveGatewayConvertor = new ContractExclusiveGatewayConvertor();
                var processConvertor = new ContractProcessConvertor(
                    scriptActivityConvertor,
                    userActivityConvertor,
                    callActivityConvertor,
                    timerBoundaryConvertor,
                    exclusiveGatewayConvertor,
                    mergingExclusiveGatewayConvertor);
                var processesConvertor = new ContractProcessesConvertor(
                    processConvertor);

                //Contract convertor
                var plutusContractConvertor = new PlutusContractConvertor(
                    plutusDataModelConvertor,
                    usersConvertor,
                    processesConvertor);

                return plutusContractConvertor;
            }
        }
    }
}
