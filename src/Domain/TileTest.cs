// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain;

using NUnit.Framework;

[TestFixture]
public class TileTest {
    [Test]
    public void TestGetNextTile() {
        var m1 = new Tile(Suit.M, 1);
        var m2 = new Tile(Suit.M, 2);
        var m9 = new Tile(Suit.M, 9);

        var z1 = new Tile(Suit.Z, 1);
        var z7 = new Tile(Suit.Z, 7);

        var gotM2 = Tile.GetNextTile(m1);
        Assert.IsTrue(gotM2.EqualsIgnoreColor(m2));

        var gotM1 = Tile.GetNextTile(m9);
        Assert.IsTrue(gotM1.EqualsIgnoreColor(m1));

        var gotZ1 = Tile.GetNextTile(z7);
        Assert.IsTrue(gotZ1.EqualsIgnoreColor(z1));
    }
}
