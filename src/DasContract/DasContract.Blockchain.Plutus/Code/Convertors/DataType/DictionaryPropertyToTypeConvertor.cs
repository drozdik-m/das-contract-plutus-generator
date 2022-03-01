using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Enum;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Reference;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class DictionaryPropertyToTypeConvertor : IConvertor<DictionaryContractProperty, INamable>
    {
        private IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> primitiveConvertor;
        private IConvertor<PrimitiveContractProperty, INamable> primitivePropConvertor;
        private IConvertor<ReferenceContractProperty, INamable> referencePropConvertor;
        private IConvertor<EnumContractProperty, INamable> enumPropConvertor;

        public DictionaryPropertyToTypeConvertor(
            IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> primitiveConvertor,
            IConvertor<PrimitiveContractProperty, INamable> primitivePropConvertor,
            IConvertor<ReferenceContractProperty, INamable> referencePropConvertor,
            IConvertor<EnumContractProperty, INamable> enumPropConvertor
            )
        {
            this.primitiveConvertor = primitiveConvertor;
            this.primitivePropConvertor = primitivePropConvertor;
            this.referencePropConvertor = referencePropConvertor;
            this.enumPropConvertor = enumPropConvertor;
        }

        /// <inheritdoc/>
        public INamable Convert(DictionaryContractProperty source)
        {
            var keyType = primitiveConvertor.Convert(source.KeyType);
            INamable valueType;
            if (source.ValueType is PrimitiveContractProperty primitiveProp)
                valueType = primitivePropConvertor.Convert(primitiveProp);
            else if (source.ValueType is ReferenceContractProperty refProp)
                valueType = referencePropConvertor.Convert(refProp);
            else if (source.ValueType is EnumContractProperty enumProp)
                valueType = enumPropConvertor.Convert(enumProp);
            else if (source.ValueType is DictionaryContractProperty dictProp)
                valueType = Convert(dictProp);
            else
                throw new Exception("Unknown type of a ValueType: " + source.ValueType.GetType().FullName);

            var dictionaryType = PlutusDictionary.Type(keyType, valueType);

            INamable cardinalizedType = new TypeToCardinalizedType(source.Cardinality)
                .Convert(dictionaryType);
            INamable mandatorizedType = new TypeToMaybeType(source.IsMandatory)
                .Convert(cardinalizedType);

            return mandatorizedType;
        }
    }
}
