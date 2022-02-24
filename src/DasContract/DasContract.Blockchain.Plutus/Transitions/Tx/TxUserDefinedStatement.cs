using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Transitions.Tx
{
    internal enum TxUserDefinedStatement
    {
        UserDefinedTransition,
        UserDefinedExpectedValue,
        UserDefinedNewValue,
        UserDefinedConstraints
    }
}
