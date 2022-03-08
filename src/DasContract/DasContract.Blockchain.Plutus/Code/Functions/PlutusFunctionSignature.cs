using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Comments
{
    /// <summary>
    /// Signature of a plutus function
    /// </summary>
    public class PlutusFunctionSignature : PlutusLine, INamable
    {
        public PlutusFunctionSignature(int indent, string name, IEnumerable<INamable> types): base(indent)
        {
            if (types.Count() == 0)
                throw new ArgumentException("A function signature can not have zero types");

            Name = name;
            Types = types;
        }

        public string Name { get; }

        public IEnumerable<INamable> Types { get; }


        /// <inheritdoc/>
        public override string InString()
        {
            return base.InString() + $"{Name} :: {string.Join(" -> ", Types.Select(e => e.Name))}";
        }
    }
}
