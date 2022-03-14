using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class ReferencePropertyToTypeConvertor : IConvertor<ReferenceContractProperty, INamable>
    {
        /// <inheritdoc/>
        public INamable Convert(ReferenceContractProperty source)
        {
            var type = PlutusFutureDataType.Type(source.Entity.Id);
            INamable cardinalizedType = new TypeToCardinalizedType(source.Cardinality)
                .Convert(type);
            INamable mandatorizedType = new TypeToMaybeType(source.IsMandatory)
                .Convert(cardinalizedType);

            return mandatorizedType;
        }
    }
}
