// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace MahjongScorer.Util;

[TestFixture]
public class NumberUtilTest {
    [Test]
    public void Test() {
        var n = NumberUtil.RoundUpToNextUnit(1920, 100);
        Assert.AreEqual(2000, n);

        var n2 = NumberUtil.RoundUpToNextUnit(1985, 10);
        Assert.AreEqual(1990, n2);
    }
}
