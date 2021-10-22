// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using NUnit.Framework;

namespace MahjongScorer {
    [TestFixture]
    public class ScorerTest {
        [Test]
        public void TestDealerRiichi() {
            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi };
            var round = new RoundConfig();
            var rule = new RuleConfig();

            var pt = Scorer.GetScore("33345m23455p678s", "5p",
                Array.Empty<string>(), handConfig, round, rule);

            Assert.AreEqual(2, pt.Han);
            Assert.AreEqual(40, pt.Fu);
            Assert.AreEqual(3900, pt.Ron?.BaseGain);
        }

        [Test]
        public void TestNonDealerTsumo() {
            var dora = new DoraInfo { RedDora = 1 };
            var hand = new HandConfig { DoraInfo = dora, Tsumo = true, Under = true };
            var round = new RoundConfig { SeatWind = Wind.North };
            var rule = new RuleConfig();

            var pt = Scorer.GetScore("33456789m77p678s", "3m",
                Array.Empty<string>(), hand, round, rule);

            Assert.AreEqual(3, pt.Han);
            Assert.AreEqual(30, pt.Fu);
            Assert.AreEqual(4000, pt.NonDealerTsumo?.BaseGain);
        }
    }
}
