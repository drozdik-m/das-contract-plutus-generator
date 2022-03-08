using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors
{
    public class TypeToCardinalizedType : IConvertor<INamable, INamable>
    {
        public TypeToCardinalizedType(ContractPropertyCardinality cardinality)
        {
            Cardinality = cardinality;
        }

        public ContractPropertyCardinality Cardinality { get; }

        /// <inheritdoc/>
        public INamable Convert(INamable source)
        {
            INamable cardinalizedType;
            if (Cardinality == ContractPropertyCardinality.Single)
                cardinalizedType = source;
            else if (Cardinality == ContractPropertyCardinality.Collection)
                cardinalizedType = PlutusList.Type(source);
            else
                throw new Exception("Unknown cardinality");

            return cardinalizedType;
        }
    }
}
