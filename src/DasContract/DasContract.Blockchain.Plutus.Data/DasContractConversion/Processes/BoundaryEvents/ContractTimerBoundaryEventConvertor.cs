using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.Processes.Events;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    public class ContractTimerBoundaryEventConvertor : IConvertor<TimerBoundaryEvent, ContractTimerBoundaryEvent>
    {
        /// <inheritdoc/>
        public ContractTimerBoundaryEvent Convert(TimerBoundaryEvent timerBoundaryEvent)
        {
            //Timer definition
            var timerDefinition = timerBoundaryEvent.TimerDefinition;
            timerDefinition = timerDefinition.Trim();
            
            if (string.IsNullOrWhiteSpace(timerDefinition))
                throw new Exception($"Timer boundary {timerBoundaryEvent.Id} has empty timer definition");

            if (timerBoundaryEvent.TimerDefinitionType != TimerDefinitionType.Date)
                throw new Exception($"Timer boundary {timerBoundaryEvent.Id} has timer type not set to \"Date\". Unfortunately, Plutus blockchain safely supports only this timer type.");

            if (timerDefinition.StartsWith("${") && timerDefinition.EndsWith("}"))
            {
                timerDefinition = timerDefinition[2..^(-1)];
                timerDefinition = timerDefinition.Trim();
            }
            else if (DateTime.TryParse(timerDefinition, out DateTime timerDefinitionDate))
            {
                TimeSpan timeSpan = timerDefinitionDate - new DateTime(1970, 1, 1, 0, 0, 0);
                timerDefinition = (timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
            }

            //Boundary event
            var resTimerBoundaryEvent = new ContractTimerBoundaryEvent()
            {
                Id = timerBoundaryEvent.Id,
                TimerDefinition = timerDefinition
            };
            
            return resTimerBoundaryEvent;
        }
    }
}
