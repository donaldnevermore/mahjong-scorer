// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain;

public record FuValue {
    public FuType Name { get; }
    public int Value { get; }

    public FuValue(FuType name, int value) {
        Name = name;
        Value = value;
    }

    public override string ToString() {
        return $"{Name}: {Value}";
    }
}
