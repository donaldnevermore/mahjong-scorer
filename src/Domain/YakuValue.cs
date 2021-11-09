// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain;

public record YakuValue {
    public YakuType Name { get; }
    public int Value { get; }
    public bool IsYakuman { get; }

    public YakuValue(YakuType name, int value, bool isYakuman = false) {
        Name = name;
        Value = value;
        IsYakuman = isYakuman;
    }

    public override string ToString() {
        return $"{Name}: {Value}";
    }
}
