using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;

namespace DasContract.Blockchain.Plutus.Code.Types.Determined
{
    public class PlutusUserActivityForm : PlutusPremadeType
    {
        public PlutusUserActivityForm(ContractUserActivity userActivity)
        {
            UserActivity = userActivity;
        }

        public ContractUserActivity UserActivity { get; }

        public override string Name => UserActivity.Name + FormPostfix;

        public const string FormPostfix = "Form";

        public static PlutusUserActivityForm Type(ContractUserActivity userActivity) 
            => new PlutusUserActivityForm(userActivity);
        
    }
}
