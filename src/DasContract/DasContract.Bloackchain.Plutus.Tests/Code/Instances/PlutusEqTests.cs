using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusEqTests
    {
        class TestType : INamable
        {
            public TestType(string name)
            {
                Name = name;
            }

            public string Name { get; }

        }

        [Test]
        public void PlutusEqRecord()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>()
            {
                new PlutusRecordMember("a", new TestType("BuiltinByteString")),
                new PlutusRecordMember("b", new TestType("POSIXTime")),
            }, new List<string>());

            Assert.AreEqual("instance Eq Record where" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "{-# INLINABLE (==) #-}" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "Record a b == Record a' b' = (a == a') && (b == b')" + PlutusCode.NewLineString, new PlutusEq(record).InString());
        }

        [Test]
        public void PlutusEqRecordOneMember()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>()
            {
                new PlutusRecordMember("a", new TestType("BuiltinByteString")),
            }, new List<string>());

            Assert.AreEqual("instance Eq Record where" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "{-# INLINABLE (==) #-}" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "Record a == Record a' = a == a'" + PlutusCode.NewLineString, new PlutusEq(record).InString());
        }

        [Test]
        public void PlutusEqRecordNoMembers()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>()
            {
                
            }, new List<string>());

            Assert.AreEqual("instance Eq Record where" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "{-# INLINABLE (==) #-}" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "Record == Record = True" + PlutusCode.NewLineString, new PlutusEq(record).InString());
        }


        [Test]
        public void PlutusEqAlgType()
        {
            var type = new PlutusAlgebraicType("AlgType", new List<PlutusAlgebraicTypeConstructor>()
            {
                new PlutusAlgebraicTypeConstructor("A", new List<INamable>{ new TestType("Type"), new TestType("AndAnotherOne") }),
                new PlutusAlgebraicTypeConstructor("B", new List<INamable>()),
                new PlutusAlgebraicTypeConstructor("C", new List<INamable>{ new TestType("Type") }),
            }, new List<string>());

            Assert.AreEqual("instance Eq AlgType where" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "{-# INLINABLE (==) #-}" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "A a b == A a' b' = (a == a') && (b == b')" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "B == B = True" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "C a == C a' = a == a'" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "_ == _ = False" + PlutusCode.NewLineString,
                new PlutusEq(type).InString());
        }

        

    }
}
