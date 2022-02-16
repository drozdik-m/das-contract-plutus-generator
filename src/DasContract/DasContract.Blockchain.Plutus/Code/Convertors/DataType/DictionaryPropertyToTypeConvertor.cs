using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Code.Types.Temporary;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class DictionaryPropertyToTypeConvertor : IConvertor<DictionaryContractProperty, INamable>
    {
        public DictionaryPropertyToTypeConvertor(
            IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> primitiveConvertor,
            IConvertor<PrimitiveContractProperty, INamable> primitivePropConvertor,
            IConvertor<ReferenceContractProperty, INamable> referencePropConvertor
            )
        {
            PrimitiveConvertor = primitiveConvertor;
            PrimitivePropConvertor = primitivePropConvertor;
            ReferencePropConvertor = referencePropConvertor;
        }

        public IConvertor<PrimitiveContractPropertyType, PlutusPremadeType> PrimitiveConvertor { get; }

        public IConvertor<PrimitiveContractProperty, INamable> PrimitivePropConvertor { get; }

        public IConvertor<ReferenceContractProperty, INamable> ReferencePropConvertor { get; }

        /// <inheritdoc/>
        public INamable Convert(DictionaryContractProperty source)
        {
            var keyType = PrimitiveConvertor.Convert(source.KeyType);
            INamable valueType;
            if (source.ValueType is PrimitiveContractProperty primitiveProp)
                valueType = PrimitivePropConvertor.Convert(primitiveProp);
            else if (source.ValueType is ReferenceContractProperty refProp)
                valueType = ReferencePropConvertor.Convert(refProp);
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
