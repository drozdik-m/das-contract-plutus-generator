using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.UserInterface;
using DasContract.Abstraction.UserInterface.FormFields;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Forms;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractFieldTypeConvertor : IConvertor<Field, PrimitiveContractPropertyType>
    {

        /// <inheritdoc/>
        public PrimitiveContractPropertyType Convert(Field source)
        {
            if (source is AddressField) return PrimitiveContractPropertyType.Address;
            if (source is BoolField) return PrimitiveContractPropertyType.Bool;
            if (source is DateField) return PrimitiveContractPropertyType.POSIXTime;
            if (source is DecimalField) return PrimitiveContractPropertyType.Integer;
            if (source is DropdownField) return PrimitiveContractPropertyType.BuiltinByteString;
            if (source is EnumField) return PrimitiveContractPropertyType.BuiltinByteString;
            if (source is IntField) return PrimitiveContractPropertyType.Integer;
            if (source is SingleLineField) return PrimitiveContractPropertyType.BuiltinByteString;

            throw new Exception("Unknown type of Field: " + source.GetType().Name);
        }
    }
}
