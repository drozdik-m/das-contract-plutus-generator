using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels.Properties.Primitive
{
    public class PrimitivePropertyTypeConvertor : IConvertor<PropertyDataType, PrimitiveContractPropertyType>
    {
        /// <inheritdoc/>
        public PrimitiveContractPropertyType Convert(PropertyDataType source)
        {
            return source switch
            {
                PropertyDataType.Int => PrimitiveContractPropertyType.Integer,
                PropertyDataType.Uint => PrimitiveContractPropertyType.Integer,
                PropertyDataType.Bool => PrimitiveContractPropertyType.Bool,
                PropertyDataType.String => PrimitiveContractPropertyType.BuiltinByteString,
                PropertyDataType.DateTime => PrimitiveContractPropertyType.POSIXTime,
                PropertyDataType.Address => PrimitiveContractPropertyType.Address,
                PropertyDataType.AddressPayable => PrimitiveContractPropertyType.Address,
                PropertyDataType.Data => PrimitiveContractPropertyType.BuiltinByteString,
                _ => throw new Exception("No suitable property type found"),
            };
        }
    }
}
