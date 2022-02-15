using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusAlgebraicTypeConstructor : PlutusLine, INamable
    {
        public PlutusAlgebraicTypeConstructor(string name, IEnumerable<INamable> types, bool isLast = false): base(1)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Plutus record name can not be empty");

            Name = name;
            Types = types;
            IsLast = isLast;
        }

        public string Name { get; }

        public IEnumerable<INamable> Types { get; }

        public bool IsLast { get; }

        public PlutusAlgebraicTypeConstructor ToLast()
        {
            return new PlutusAlgebraicTypeConstructor(Name, Types, true);  
        }

        public override string InString()
        {
            return base.InString() + Name +
                (Types.Count() == 0 ? string.Empty : " ") + 
                string.Join(" ", Types.Select(e => e.Name)) + 
                (IsLast ? string.Empty : " |");
        }
    }
}
