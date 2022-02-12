using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Code.Types
{
    public class PlutusAlgebraicTypeConstructor : PlutusLine, INamable
    {
        public PlutusAlgebraicTypeConstructor(string name, IEnumerable<string> types, bool isLast = false): base(1)
        {
            if (string.IsNullOrEmpty(name))
                throw new Exception("Plutus record name can not be empty");

            Name = name;
            Types = types;
            IsLast = isLast;
        }

        public string Name { get; }

        public IEnumerable<string> Types { get; }

        public bool IsLast { get; }

        public PlutusAlgebraicTypeConstructor ToLast()
        {
            return new PlutusAlgebraicTypeConstructor(Name, Types, true);  
        }

        public override string InString()
        {
            return base.InString() + Name +
                (Types.Count() == 0 ? string.Empty : " ") + 
                string.Join(" ", Types) + 
                (IsLast ? string.Empty : " |");
        }
    }
}
