using System;
using System.Collections.Generic;
using System.Text;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Gateways;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;

namespace DasContract.Blockchain.Plutus.Data.Processes.Process
{
    public interface IContractProcessElementVisitor<T>
    {
        T Visit(ContractExclusiveGateway element);

        T Visit(ContractMergingExclusiveGateway element);
        
        T Visit(ContractStartEvent element);

        T Visit(ContractEndEvent element);

        T Visit(ContractCallActivity element);

        T Visit(ContractUserActivity element);

        T Visit(ContractScriptActivity element);

        T Visit(ContractTimerBoundaryEvent contractTimerBoundaryEvent);
    }
}
