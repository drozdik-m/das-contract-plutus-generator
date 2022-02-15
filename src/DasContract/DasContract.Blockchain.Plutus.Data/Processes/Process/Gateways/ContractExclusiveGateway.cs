using System.Collections.Generic;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways
{
    public class ContractExclusiveGateway : ContractGateway
    {
        //public ContractProcessElement Incoming { get; set; }

        public ICollection<ContractProcessElement> Outgoing { get; set; } = new List<ContractProcessElement>();

        public override void CollectSuccessors(ref Dictionary<string, ContractProcessElement> collector)
        {
            base.CollectSuccessors(ref collector);
            foreach(var outgoing in Outgoing)
                outgoing.CollectSuccessors(ref collector);
        }
    }
}
