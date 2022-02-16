using System;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using DasContract.Blockchain.Plutus.Data.Interfaces;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests.Code.Types
{
    public class PlutusRecordMemberTests
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
        public void PlutusRecordMember()
        {
            var member = new PlutusRecordMember("a", new TestType("BuiltinByteString"));
            Assert.AreEqual(PlutusLine.IndentString +
                "a :: BuiltinByteString,", member.InString());
        }

        [Test]
        public void PlutusRecordMemberLast()
        {
            var member = new PlutusRecordMember("bcd", new TestType("POSIXTime"), true);
            Assert.AreEqual(PlutusLine.IndentString +
                "bcd :: POSIXTime", member.InString());
        }

        [Test]
        public void PlutusRecordMemberEmptyName()
        {
            try
            {
                var member = new PlutusRecordMember("", new TestType("POSIXTime"), true);
                Assert.Fail();
            }
            catch
            {

            }
        }
    }
}
