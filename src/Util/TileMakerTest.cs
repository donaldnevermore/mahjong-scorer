// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Linq;
using NUnit.Framework;
using MahjongSharp.Domain;

namespace MahjongSharp.Util {
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
