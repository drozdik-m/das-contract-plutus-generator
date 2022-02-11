using System;
using System.Collections.Generic;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusRecordTests
    {
        [Test]
        public void PlutusRecord()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>()
            {
                new PlutusRecordMember("a", "BuiltinByteString"),
                new PlutusRecordMember("b", "POSIXTime"),
            }, new List<string>());

            Assert.AreEqual("data Record = Record {" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "a :: BuiltinByteString," + PlutusCode.NewLineString +
                PlutusLine.IndentString + "b :: POSIXTime" + PlutusCode.NewLineString +
                "}" + PlutusCode.NewLineString, record.InString());
        }

        [Test]
        public void EmptyPlutusRecord()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>(), new List<string>());

            Assert.AreEqual("data Record = Record {" + PlutusCode.NewLineString +
                "}" + PlutusCode.NewLineString, record.InString());
        }

        [Test]
        public void PlutusRecordDerivings()
        {
            var record = new PlutusRecord("Record", new List<PlutusRecordMember>()
            {
                new PlutusRecordMember("a", "BuiltinByteString"),
                new PlutusRecordMember("b", "POSIXTime"),
            }, new List<string>()
            {
                "Show",
                "Generic",
                "FromJSON",
                "ToJSON"
            });

            Assert.AreEqual("data Record = Record {" + PlutusCode.NewLineString +
                PlutusLine.IndentString + "a :: BuiltinByteString," + PlutusCode.NewLineString +
                PlutusLine.IndentString + "b :: POSIXTime" + PlutusCode.NewLineString +
                "} deriving (Show, Generic, FromJSON, ToJSON)" + PlutusCode.NewLineString,
                record.InString());
        }

    }
}
