using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties
{
    public interface IContractPropertyVisitor<T>
    {
        T Visit(PrimitiveContractProperty property);

        T Visit(ReferenceContractProperty property);

        T Visit(DictionaryContractProperty property);

        T Visit(EnumContractProperty property);
    }
}
