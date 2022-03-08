using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Abstraction.Data;
using DasContract.Abstraction.UserInterface;
using DasContract.Abstraction.UserInterface.FormFields;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Forms;
using DasContract.String.Utils;

namespace DasContract.Blockchain.Plutus.Data.DasContractConversion.DataModels
{
    

    public class ContractFormConvertor : IConvertor<UserForm, ContractForm>
    {
        private readonly IConvertor<Field, PrimitiveContractPropertyType> primitiveFieldConvertor;

        public ContractFormConvertor(IConvertor<Field, PrimitiveContractPropertyType> primitiveFieldConvertor)
        {
            this.primitiveFieldConvertor = primitiveFieldConvertor;
        }

        /// <inheritdoc/>
        public ContractForm Convert(UserForm source)
        {
            //Gather all form fields
            var formFields = source.FieldGroups
                .Aggregate(new List<Field>(), (acc, item) =>
                {
                    acc.AddRange(item.Fields);
                    return acc;
                })
                .Where(e => !e.ReadOnly);

            var resultForm = new ContractForm()
            {
                Fields = formFields.Select(e => new PrimitiveContractProperty()
                {
                    Id = e.Label
                             .FirstCharToLowerCase()
                             .Replace(" ", ""),
                    IsMandatory = true,
                    Cardinality = e.Multiple ? ContractPropertyCardinality.Collection : ContractPropertyCardinality.Single,
                    Type = primitiveFieldConvertor.Convert(e),

                }).ToList(),
            }; 

            return resultForm;
        }
    }
}
