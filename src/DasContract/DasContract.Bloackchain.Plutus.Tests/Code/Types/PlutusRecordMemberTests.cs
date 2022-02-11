﻿using System;
using DasContract.Blockchain.Plutus.Code;
using DasContract.Blockchain.Plutus.Code.Comments;
using DasContract.Blockchain.Plutus.Code.Types;
using NUnit.Framework;

namespace DasContract.Bloackchain.Plutus.Tests
{
    public class PlutusRecordMemberTests
    {
        [Test]
        public void PlutusRecordMember()
        {
            var member = new PlutusRecordMember("a", "BuiltinByteString");
            Assert.AreEqual(PlutusLine.IndentString + 
                "a :: BuiltinByteString,", member.InString());
        }

        [Test]
        public void PlutusRecordMemberLast()
        {
            var member = new PlutusRecordMember("bcd", "POSIXTime", true);
            Assert.AreEqual(PlutusLine.IndentString +
                "bcd :: POSIXTime", member.InString());
        }

        [Test]
        public void PlutusRecordMemberEmptyName()
        {
            try
            {
                var member = new PlutusRecordMember("", "POSIXTime", true);
                Assert.Fail();
            }
            catch
            {

            }
        }
    }
}
