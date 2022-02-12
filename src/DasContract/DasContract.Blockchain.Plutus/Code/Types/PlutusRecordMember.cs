using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusRecordMember : PlutusLine
    {
        public PlutusRecordMember(string name, INamable dataType, bool isLast = false): base(1)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Plutus record name can not be empty");

            if (string.IsNullOrEmpty(dataType.Name))
                throw new Exception("Plutus record type can not be empty");

            Name = name;
            DataType = dataType;
            IsLast = isLast;
        }

        public string Name { get; }

        public INamable DataType { get; }

        public bool IsLast { get; }

        public PlutusRecordMember ToLast()
        {
            return new PlutusRecordMember(Name, DataType, true);  
        }

        public override string InString()
        {
            return base.InString() + $"{Name} :: {DataType.Name}" + (IsLast ? string.Empty : ",");
        }
    }
}
