using System.Collections.Generic;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways
{
    public class ContractExclusiveGateway : ContractGateway
    {

        /// <summary>
        /// Outgoing connections of this gateway
        /// </summary>
        public ICollection<ContractConditionedConnection> Outgoing { get; set; } 
            = new List<ContractConditionedConnection>();

        /// <inheritdoc/>
        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            base.CollectSuccessors(ref collector);
            foreach(var outgoing in Outgoing)
                outgoing.Target.CollectSuccessors(ref collector);
        }


        /// <inheritdoc/>
        public override T Accept<T>(IContractProcessElementVisitor<T> visitor)
            => visitor.Visit(this);
    }
}
