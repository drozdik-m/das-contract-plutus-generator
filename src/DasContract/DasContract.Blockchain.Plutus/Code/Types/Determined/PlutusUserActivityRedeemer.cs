using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;

namespace DasContract.Blockchain.Plutus.Code.Types.Determined
{
    public class PlutusUserActivityRedeemer : PlutusPremadeType
    {
        public PlutusUserActivityRedeemer(ContractUserActivity userActivity)
        {
            UserActivity = userActivity;
        }

        public ContractUserActivity UserActivity { get; }

        public override string Name => UserActivity.Name + RedeemerPostfix;

        public const string RedeemerPostfix = "Redeemer";

        public static PlutusUserActivityRedeemer Type(ContractUserActivity userActivity) 
            => new PlutusUserActivityRedeemer(userActivity);
        
    }
}
