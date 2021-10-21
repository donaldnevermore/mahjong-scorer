// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;

namespace MahjongSharp.Domain {
    public record YakuValue : IComparable<YakuValue> {
        public YakuType Name { get; }
        public int Value { get; }
        public bool IsYakuman { get; }

        public YakuValue(YakuType name, int value, bool isYakuman = false) {
            Name = name;
            Value = value;
            IsYakuman = isYakuman;
        }

        public int CompareTo(YakuValue? other) {
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
