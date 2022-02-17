using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties
{
    public interface IContractPropertyVisitor<T>
    {
        T Visit(PrimitiveContractProperty property);

        T Visit(ReferenceContractProperty property);

        T Visit(DictionaryContractProperty property);
    }
}
