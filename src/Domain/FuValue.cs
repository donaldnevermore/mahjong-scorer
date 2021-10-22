// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;

namespace MahjongScorer.Domain {
    public record FuValue : IComparable<FuValue> {
        public FuType Name { get; }
        public int Value { get; }

        public FuValue(FuType name, int value) {
            Name = name;
            Value = value;
        }

        public int CompareTo(FuValue? other) {
            if (other is null) {
                throw new ArgumentNullException(nameof(other));
            }

            return Value.CompareTo(other.Value);
        }

        public override string ToString() {
            return $"{Name}: {Value}";
        }
    }
}
