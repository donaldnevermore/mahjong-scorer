// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Point;
using NUnit.Framework;

namespace MahjongScorer {
    [TestFixture]
    public class ScorerTest {
        [Test]
        public void Test() {
            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true, Ippatsu = true };
            var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 2 };
            var rule = new RuleConfig();

            var p = Scorer.GetScore("23440556m23489s", "7s", "",
                handConfig, round, rule);
            Console.WriteLine(p);

            Assert.AreEqual(4, p.Han);
            Assert.AreEqual(30, p.Fu);
            if (p is NonDealerTsumo r) {
                Assert.AreEqual(7900, r.BaseGain);
            }
            else {
                Assert.Fail();
            }
        }

        [Test]
        public void Test2() {
            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi };
            var round = new RoundConfig { SeatWind = Wind.West, RiichiBets = 1 };
            var rule = new RuleConfig();

            var p = Scorer.GetScore("22789m067p34789s", "2s", "",
                handConfig, round, rule);
            Console.WriteLine(p);

            Assert.AreEqual(3, p.Han);
            Assert.AreEqual(30, p.Fu);
            if (p is Ron r) {
                Assert.AreEqual(3900, r.BaseGain);
            }
            else {
                Assert.Fail();
            }
        }

        [Test]
        public void Test3() {
            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi };
            var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 3, Honba = 1 };
            var rule = new RuleConfig();

            var p = Scorer.GetScore("2245689m456p789s", "7m", "",
                handConfig, round, rule);
            Console.WriteLine(p);

            Assert.AreEqual(1, p.Han);
            Assert.AreEqual(40, p.Fu);
            if (p is Ron r) {
                Assert.AreEqual(1300, r.BaseGain);
            }
            else {
                Assert.Fail();
            }
        }

        [Test]
        public void Test4() {
            var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true };
            var round = new RoundConfig { RoundWind = Wind.South, SeatWind = Wind.North, RiichiBets = 1 };
            var rule = new RuleConfig();

            var p = Scorer.GetScore("344056m789p2245s", "3s", "",
                handConfig, round, rule);
            Console.WriteLine(p);

            Assert.AreEqual(4, p.Han);
            Assert.AreEqual(20, p.Fu);
            if (p is NonDealerTsumo r) {
                Assert.AreEqual(5200, r.BaseGain);
            }
            else {
                Assert.Fail();
            }
        }
    }
}
