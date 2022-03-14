using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Types.Premade;
using DasContract.Blockchain.Plutus.Data.Abstraction;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties;
using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using DasContract.Blockchain.Plutus.Data.Interfaces;

namespace DasContract.Blockchain.Plutus.Code.Convertors.DataType
{
    /// <summary>
    /// Reorders the contract entity collection by their dependency relations
    /// </summary>
    public class EntitiesDependencyOrderingConvertor : IConvertor<IEnumerable<ContractEntity>, IEnumerable<ContractEntity>>
    {

        /// <inheritdoc/>
        public IEnumerable<ContractEntity> Convert(IEnumerable<ContractEntity> source)
        {
            //Return topologic sort
            return Sort(source, e =>
            {
                var dependencyDictionary = new Dictionary<string, ContractEntity>();
                e.Properties.ToList().ForEach(p => p.CollectDependencies(ref dependencyDictionary));
                return dependencyDictionary.Values;
            });
        }


        //Topological sort
        //Source of the code below: https://www.codeproject.com/articles/869059/topological-sorting-in-csharp

        static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>();
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies,
                           List<T> sorted, Dictionary<T, bool> visited)
        {
            var alreadyVisited = visited.TryGetValue(item, out var inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency between entities found");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}
