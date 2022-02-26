using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    public class PrimitivePropertyTypeToPlutusConvertor : IConvertor<PrimitiveContractPropertyType, PlutusPremadeType>
    {
        /// <inheritdoc/>
        public PlutusPremadeType Convert(PrimitiveContractPropertyType source)
        {
            return source switch
            {
                PrimitiveContractPropertyType.Integer => PlutusInteger.Type,
                PrimitiveContractPropertyType.Address => PlutusPubKeyHash.Type,
                PrimitiveContractPropertyType.Bool => PlutusBool.Type,
                PrimitiveContractPropertyType.ByteString => PlutusBuiltinByteString.Type,
                PrimitiveContractPropertyType.POSIXTime => PlutusPOSIXTime.Type,
                _ => throw new Exception("No suitable string found"),
            };
        }
    }
}
