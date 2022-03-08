using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Transitions
{
    /// <summary>
    /// Every process element has a transition type.
    /// This transition type represents if transition INTO the elements' state requires a transaction.
    /// - Tx = transition into the element does not require a transaction
    /// - NonTx = transition into the element does require a transaction
    /// - Implicit = This state cannot be transitioned to and its type is implicitly set. Currently only the start event.
    /// </summary>
    public enum TxType
    {
        Tx,
        NonTx,
        Implicit
    }
}


