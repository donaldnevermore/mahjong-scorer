// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Linq;
using MahjongScorer.Domain;
using NUnit.Framework;

namespace MahjongScorer.Util {
    [TestFixture]
    public class TileMakerTest {
        [Test]
        public void TestConvertTiles() {
            var hand = TileMaker.ConvertTiles("234567m23455p67s").ToArray();
            var want = new Tile[] {
                new(Suit.M, 2), new(Suit.M, 3), new(Suit.M, 4), new(Suit.M, 5),
                new(Suit.M, 6), new(Suit.M, 7), new(Suit.P, 2), new(Suit.P, 3),
                new(Suit.P, 4), new(Suit.P, 5), new(Suit.P, 5), new(Suit.S, 6),
                new(Suit.S, 7)
            };
            Assert.AreEqual(want, hand);
        }

        [Test]
        public void TestConvertTile() {
            var t = TileMaker.ConvertTile("2m");
            var want = new Tile(Suit.M, 2);
            Assert.AreEqual(want, t);
        }

        [Test]
        public void TestConvertMelds() {
            var m = TileMaker.ConvertMelds(new[] { "234m" });
            var t1 = new Tile(Suit.M, 2);
            var t2 = new Tile(Suit.M, 3);
            var t3 = new Tile(Suit.M, 4);
            var meld = new Meld(true, t1, t2, t3);
            Assert.AreEqual(meld, m[0]);
        }

        [Test]
        public void TestGetGreenTiles() {
            var got = TileMaker.GetGreenTiles();
            var want = new[] { 19, 20, 21, 23, 25, 32 };
            Assert.AreEqual(want, got);
        }

        [Test]
        public void TestGetFullTiles() {
            var got = TileMaker.GetFullTiles();
            Assert.AreEqual(136, got.Length);
        }
    }
}
