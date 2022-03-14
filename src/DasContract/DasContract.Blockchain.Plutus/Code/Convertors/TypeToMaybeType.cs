using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors
{
    public class TypeToMaybeType : IConvertor<INamable, INamable>
    {
        public TypeToMaybeType(bool isMandatory)
        {
            IsMandatory = isMandatory;
        }

        public bool IsMandatory { get; }

        /// <inheritdoc/>
        public INamable Convert(INamable source)
        {
            INamable maybeType;
            if (IsMandatory)
                maybeType = source;
            else
                maybeType = PlutusMaybe.Type(source);

            return maybeType;
        }
    }
}
