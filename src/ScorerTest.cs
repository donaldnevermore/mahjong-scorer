// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Point;
using NUnit.Framework;

[TestFixture]
public class ScorerTest {
    [Test]
    public void Test1() {
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true, Ippatsu = true };
        var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 2 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("23440556m23489s", "7s", "",
            "8p,7m", hand, round, rule);
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
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi };
        var round = new RoundConfig { SeatWind = Wind.West, RiichiBets = 1 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("22789m067p34789s", "2s", "",
            "3z,5m", hand, round, rule);
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
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi };
        var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 3, Honba = 1 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("2245689m456p789s", "7m", "",
            "9s,7p", hand, round, rule);
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
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true };
        var round = new RoundConfig { RoundWind = Wind.South, SeatWind = Wind.North, RiichiBets = 1 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("344056m789p2245s", "3s", "",
            "8s,4p", hand, round, rule);
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

    [Test]
    public void Test5() {
        var hand = new HandConfig();
        var round = new RoundConfig { RoundWind = Wind.South };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("789m67p77s", "8p", "666zo,678so",
            "9m9p", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(1, p.Han);
        Assert.AreEqual(30, p.Fu);
        if (p is Ron r) {
            Assert.AreEqual(1500, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void Test6() {
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true };
        var round = new RoundConfig { RoundWind = Wind.South, RiichiBets = 1, Honba = 1 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("34445m222p23467s", "5s", "",
            "1z,3m", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(6, p.Han);
        Assert.AreEqual(30, p.Fu);
        if (p is DealerTsumo r) {
            Assert.AreEqual(18000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void Test7() {
        var hand = new HandConfig();
        var round = new RoundConfig {
            SeatWind = Wind.West,
            RoundWind = Wind.South,
            RiichiBets = 1,
            Honba = 2
        };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("678m333567p2s", "2s", "067so",
            "2m", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(2, p.Han);
        Assert.AreEqual(30, p.Fu);
        if (p is Ron r) {
            Assert.AreEqual(2000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void Test8() {
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi };
        var round = new RoundConfig {
            SeatWind = Wind.West,
            RoundWind = Wind.South,
            RiichiBets = 2
        };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("1233344066m123s", "5m", "",
            "0s9m5p,2m9p2s", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(9, p.Han);
        Assert.AreEqual(30, p.Fu);
        if (p is Ron r) {
            Assert.AreEqual(16000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestYakuman1() {
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi, Under = true, Tsumo = true };
        var round = new RoundConfig { SeatWind = Wind.West, RiichiBets = 1 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("789p05577s11177z", "7z", "",
            "6s,4s", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(13, p.Han);
        Assert.AreEqual(50, p.Fu);
        if (p is NonDealerTsumo r) {
            Assert.AreEqual(32000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestYakuman2() {
        var hand = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true };
        var round = new RoundConfig { RoundWind = Wind.South, SeatWind = Wind.South, RiichiBets = 2 };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("33444p33777s", "3s", "9999p",
            "7p8s,4s2z", hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(1, p.YakumanCount);
        if (p is NonDealerTsumo r) {
            Assert.AreEqual(32000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestYakuman3() {
        var hand = new HandConfig();
        var round = new RoundConfig();
        var rule = new RuleConfig();

        var p = Scorer.GetScore("0599s", "9s",
            "5555zo,6666zo,7777zo", "5p4m8m0m",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(1, p.YakumanCount);
        if (p is Ron r) {
            Assert.AreEqual(48000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestYakuman4() {
        var hand = new HandConfig { Blessing = true };
        var round = new RoundConfig();
        var rule = new RuleConfig();

        var p = Scorer.GetScore("1112223334445z", "5z", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(6, p.YakumanCount);
        if (p is DealerTsumo r) {
            Assert.AreEqual(288000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestGetActualPoint() {
        var got = Scorer.GetActualPoint(new[] { 47300, 24100, 20300, 8300 },
            new[] { 20, 10, -10, -20 }, 30000);
        var want = new[] { 57.3, 4.1, -19.7, -41.7 };

        for (var i = 0; i < 4; i++) {
            Assert.AreEqual(want[i], got[i], 0.000001);
        }
    }

    [Test]
    public void Test7Pairs() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("112244m5566p778s", "8s", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(2, p.Han);
        Assert.AreEqual(25, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(1600, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestPureStraight() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("123456789m1234p", "1p", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(2, p.Han);
        Assert.AreEqual(40, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(2600, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestTripleTriplets() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222m222p2224567s", "7s", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(5, p.Han);
        Assert.AreEqual(50, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(8000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestMixedTripleSequence() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("123m123p1234456s", "4s", "", "",
            hand, round, rule);
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
    public void TestFullFlush() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("1235677891134m", "5m", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(7, p.Han);
        Assert.AreEqual(30, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(12000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestHalfFlush() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("12356778934m11z", "5m", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(3, p.Han);
        Assert.AreEqual(40, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(5200, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestConcealedTriplets1() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222333m33p77s", "3p", "456so", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(1, p.Han);
        Assert.AreEqual(30, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(1000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestConcealedTriplets2() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222333m333p7s", "7s", "456so", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(3, p.Han);
        Assert.AreEqual(40, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(5200, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestConcealedTriplets3() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222333m333p7s", "7s", "444so", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(5, p.Han);
        Assert.AreEqual(40, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(8000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestConcealedTriplets4() {
        var hand = new HandConfig();
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222333m333p4455s", "4s", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(5, p.Han);
        Assert.AreEqual(50, p.Fu);

        if (p is Ron r) {
            Assert.AreEqual(8000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }

    [Test]
    public void TestConcealedTriplets5() {
        var hand = new HandConfig { Tsumo = true };
        var round = new RoundConfig { SeatWind = Wind.North };
        var rule = new RuleConfig();

        var p = Scorer.GetScore("222333m333p4455s", "4s", "", "",
            hand, round, rule);
        Console.WriteLine(p);

        Assert.AreEqual(1, p.YakumanCount);

        if (p is NonDealerTsumo r) {
            Assert.AreEqual(32000, r.BaseGain);
        }
        else {
            Assert.Fail();
        }
    }
}
