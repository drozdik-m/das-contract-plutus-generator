using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class EnumPropertyToTypeConvertor : IConvertor<EnumContractProperty, INamable>
    {
        /// <inheritdoc/>
        public INamable Convert(EnumContractProperty source)
        {
            var type = PlutusUnspecifiedDataType.Type(source.Name);
            INamable cardinalizedType = new TypeToCardinalizedType(source.Cardinality)
                .Convert(type);
            INamable mandatorizedType = new TypeToMaybeType(source.IsMandatory)
                .Convert(cardinalizedType);

            return mandatorizedType;
        }
    }
}
