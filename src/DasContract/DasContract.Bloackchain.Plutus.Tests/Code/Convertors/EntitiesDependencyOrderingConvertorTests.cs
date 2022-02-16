using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasContract.Blockchain.Plutus.Code.Convertors;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Editor.Entities.DataModels.Entities.Properties.Reference;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Convertors
{
    public class EntitiesDependencyOrderingConvertorTests
    {
        [Test]
        public void EntitiesDependencyOrderingConvertorComplicatedTest()
        {
            var entity1 = new ContractEntity()
            {
                Id = "1"
            };

            var entity2 = new ContractEntity()
            {
                Id = "2",
                ReferenceProperties = new ReferenceContractProperty[]
                {
                    new ReferenceContractProperty()
                    {
                        Entity = entity1
                    }
                }
            };

            var entity3 = new ContractEntity()
            {
                Id = "3",
                ReferenceProperties = new ReferenceContractProperty[]
                {
                    new ReferenceContractProperty()
                    {
                        Entity = entity1
                    }
                }
            };

            var entity4 = new ContractEntity()
            {
                Id = "4",
                ReferenceProperties = new ReferenceContractProperty[]
                {
                    new ReferenceContractProperty()
                    {
                        Entity = entity2
                    }
                }
            };

            var entity5 = new ContractEntity()
            {
                Id = "5",
                ReferenceProperties = new ReferenceContractProperty[]
                {
                    new ReferenceContractProperty()
                    {
                        Entity = entity2,
                    },
                    new ReferenceContractProperty()
                    {
                        Entity = entity4,
                    },
                    new ReferenceContractProperty()
                    {
                        Entity = entity3,
                    }
                }
            };

            var entities = new ContractEntity[]
            {
                entity5,
                entity4,
                entity2,
                entity3,
                entity1
            };

            var convertor = new EntitiesDependencyOrderingConvertor();
            var result = string.Join(", ", convertor.Convert(entities).Select(e => e.Id));
            Assert.AreEqual("1, 3, 2, 4, 5", result);
        }
    }
}
