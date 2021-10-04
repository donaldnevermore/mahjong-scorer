using System;

namespace MahjongSharp {
    public record YakuValue : IComparable<YakuValue> {
        public string Name { get; init; } = "";
        public int Value { get; init; } = 0;
        public YakuType Type { get; init; } = YakuType.Normal;

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
