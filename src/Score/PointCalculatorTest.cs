// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongSharp.Domain;
using NUnit.Framework;
using MahjongSharp.Util;

namespace MahjongSharp.Score {
    [TestFixture]
    public class PointCalculatorTest {
        [Test]
        public void TestDealerRiichi() {
            var handInfo = HandMaker.GetHandInfo("33345m23455p678s", "5p", Array.Empty<string>());

            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi };
            var round = new RoundConfig();
            var rule = new RuleConfig();

            var b = PointCalculator.HasWin(handInfo);
            Assert.IsTrue(b);

            var pc = new PointCalculator(handInfo, handConfig, round, rule);
            var got = pc.GetPoints();
            Assert.AreEqual(2, got.Han);
            Assert.AreEqual(40, got.Fu);
            Assert.AreEqual(3900, got.Ron?.BaseGain);
        }
    }
}
