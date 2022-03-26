using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using DasContract.Blockchain.Plutus.Utils;

namespace DasContract.Blockchain.Plutus.Code.Instances
{
    /// <summary>
    /// Instance for equality
    /// </summary>
    public class PlutusEq : PlutusCode
    {
        public PlutusEq(PlutusRecord record)
            : base(GetLinesOfCode(record))
        {

        }

        public PlutusEq(PlutusAlgebraicType type)
            : base(GetLinesOfCode(type))
        {

        }

        /// <summary>
        /// Lines of code for a plutus record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusRecord record)
        {
            return GetDeclaration(record)
                .Append(GetEqualityLine(record, record.Members.Count()));
        }

        /// <summary>
        /// Lines of code for an algebraic type
        /// </summary>
        /// <param name="algType"></param>
        /// <returns></returns>
        static IEnumerable<IPlutusLine> GetLinesOfCode(PlutusAlgebraicType algType)
        {
            return GetDeclaration(algType)
                .Concat(GetEqualityLines(algType))
                .Concat(GetOtherwise());
        }

        /// <summary>
        /// Equality lines for an algebraic type
        /// </summary>
        /// <param name="algType"></param>
        /// <returns></returns>
        static IEnumerable<IPlutusLine> GetEqualityLines(PlutusAlgebraicType algType)
        {
            var result = new List<IPlutusLine>();

            foreach (var ctor in algType.Constructors)
                result.Add(GetEqualityLine(ctor, ctor.Types.Count()));

            return result;
        }

        /// <summary>
        /// The equality line for an item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="memberCount"></param>
        /// <returns></returns>
        static IPlutusLine GetEqualityLine(INamable item, int memberCount)
        {
            if (memberCount == 0)
                return new PlutusRawLine(1, $"{item.Name} == {item.Name} = True");

            else if (memberCount == 1)
                return new PlutusRawLine(1, $"{item.Name} a == {item.Name} a' = a == a'");

            var generator = new StableVariableNameGenerator();
            var leftSide = $"{item.Name} ";
            var rightSide = $"{item.Name} ";
            const string ComparisonInit = "= ";
            var comparison = ComparisonInit;
            for (var i = 0; i < memberCount; i++)
            {
                var randLetter = generator.GetNext();
                leftSide += $"{randLetter} ";
                rightSide += $"{randLetter}' ";
                if (comparison == ComparisonInit)
                    comparison += $"({randLetter} == {randLetter}')";
                else
                    comparison += $" && ({randLetter} == {randLetter}')";
            }

            return new PlutusRawLine(1, leftSide + "== " + rightSide + comparison);
        }

        /// <summary>
        /// Returns the instance declaration
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static IEnumerable<IPlutusLine> GetDeclaration(INamable item)
        {
            return new List<IPlutusLine>()
            {
                new PlutusRawLine(0, $"instance Eq {item.Name} where"),
                new PlutusPragma(1, "INLINABLE (==)")
            };
        }

        static IEnumerable<IPlutusLine> GetOtherwise()
        {
            return new List<IPlutusLine>()
            {
                new PlutusRawLine(1, $"_ == _ = False"),
            };
        }
    }
}
