using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class PrimitivePropertyToTypeConvertor : IConvertor<PrimitiveContractProperty, INamable>
    {
        public PrimitivePropertyToTypeConvertor(IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> primitiveTypeConvertor)
        {
            PrimitiveTypeConvertor = primitiveTypeConvertor;
        }

        public IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> PrimitiveTypeConvertor { get; }

        /// <inheritdoc/>
        public INamable Convert(PrimitiveContractProperty source)
        {
            var type = PrimitiveTypeConvertor.Convert(source.Type);
            INamable cardinalizedType = new TypeToCardinalizedType(source.Cardinality)
                .Convert(type);
            INamable mandatorizedType = new TypeToMaybeType(source.IsMandatory)
                .Convert(cardinalizedType);

            return mandatorizedType;
        }
    }
}
