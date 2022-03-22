using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Processes;
using DasContract.Abstraction.Processes.Events;
using DasContract.Abstraction.Processes.Tasks;
using DasContract.Abstraction.UserInterface;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.Forms;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Activities;
using DasContract.Blockchain.Plutus.Data.Processes.Process.Events;
using DasContract.Blockchain.Plutus.Data.Processes.Process.MultiInstances;
using DasContract.Blockchain.Plutus.Data.Users;
using DasContract.Blockchain.Plutus.Data.Utils.String;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.Processes.Activities.MultiInstance
{
    public class ContractUserActivityConvertor : IConvertor<UserTask, ContractUserActivity>
    {
        private readonly IConvertor<Task, ContractMultiInstance> multiInstanceConvertor;
        private readonly IConvertor<UserForm, ContractForm> userFormConvertor;

        public ContractUserActivityConvertor(
            IConvertor<Task, ContractMultiInstance> multiInstanceConvertor,
            IConvertor<UserForm, ContractForm> userFormConvertor)
        {
            this.multiInstanceConvertor = multiInstanceConvertor;
            this.userFormConvertor = userFormConvertor;
        }

        public ContractUserActivity Convert(UserTask source)
        {
            var result = new ContractUserActivity()
            {
                Id = source.Id.FirstCharToUpperCase(),
                AssigneeId = source.Assignee is null ? string.Empty : source.Assignee.Name,
                CandidateRoleIds = source.CandidateRoles.Select(e => e.Name).ToList(),
                CandidateUserIds = source.CandidateUsers.Select(e => e.Name).ToList(),
                Code = source.ValidationScript,
                Form = !string.IsNullOrEmpty(source.FormDefinition) ? 
                    userFormConvertor.Convert(UserForm.DeserializeFormScript(source.FormDefinition)) :
                    new ContractForm(),
            };

            result.MultiInstance = multiInstanceConvertor.Convert(source);

            return result;
        }

        public static ContractUserActivity Bind(ContractUserActivity userActivity,
            IEnumerable<ContractUser> users,
            IEnumerable<ContractRole> roles)
        {
            //Assignee
            if (!string.IsNullOrWhiteSpace(userActivity.AssigneeId))
            {
                var suitableAssignee = users
                    .Where(e => e.Id == userActivity.AssigneeId);
                userActivity.Assignee = suitableAssignee.SingleOrDefault();
            }

            //Candidate users
            var suitableCandidateUsers = users
                .Where(e => userActivity.CandidateUserIds.Contains(e.Name));
            userActivity.CandidateUsers = suitableCandidateUsers.ToList();

            //Candidate roles
            var suitableCandidateRoles = roles
                .Where(e => userActivity.CandidateRoleIds.Contains(e.Name));
            userActivity.CandidateRoles = suitableCandidateRoles.ToList();

            return userActivity;
        }
    }
}
