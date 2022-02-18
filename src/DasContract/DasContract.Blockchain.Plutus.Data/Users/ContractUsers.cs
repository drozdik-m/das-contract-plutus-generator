using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Users
{
    public class ContractUsers
    {
        public ICollection<ContractUser> Users { get; set; } = new List<ContractUser>();    
    }
}
