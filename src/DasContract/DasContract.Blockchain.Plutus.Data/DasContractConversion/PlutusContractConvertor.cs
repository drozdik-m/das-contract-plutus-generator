using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes;
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
    }
}
