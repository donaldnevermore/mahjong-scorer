// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MahjongScorer.Domain {
    public class Tile : IComparable<Tile> {
        public static readonly IEqualityComparer<Tile>
            TileIgnoreColorEqualityComparer = new TileIgnoreColorEqualityComparerImpl();

        public Suit Suit { get; }
        public int Rank { get; }
        public bool IsRed { get; }

        public bool IsHonor => Suit == Suit.Z;
        public bool IsRoto => Rank == 1 || Rank == 9;
        public bool IsYaochuu => IsHonor || IsRoto;

        public Tile(Suit suit, int rank, bool isRed = false) {
            if (rank <= 0 || rank > 9) {
                throw new ArgumentException("Index must be within the range of 1 and 9.");
            }
            if (suit == Suit.Z && Rank > 7) {
                throw new ArgumentException("Index of tiles in Suit of Z must be within the range of 1 and 7.");
            }

            Suit = suit;
            Rank = rank;
            IsRed = isRed;
        }

        public static Tile GetTile(int index) {
            var suit = (Suit)(index / 9);
            var rank = index % 9 + 1;
            return new Tile(suit, rank);
        }

        public static int GetIndex(Tile tile) {
            var start = (int)tile.Suit * 9;
            var offset = tile.Rank - 1;
            return start + offset;
        }

        public int CompareTo(Tile? other) {
            if (other is null) {
                throw new ArgumentNullException(nameof(other));
            }

            if (Suit != other.Suit) {
                return Suit - other.Suit;
            }
            if (Rank != other.Rank) {
                return Rank - other.Rank;
            }
            if (IsRed && !other.IsRed) {
                return 1;
            }
            if (!IsRed && other.IsRed) {
                return -1;
            }

            return 0;
        }

        public string ToStringIgnoreColor() {
            var suffix = GetSuffix();
            return $"{Rank}{suffix}";
        }

        public string GetSuffix() {
            var suffix = "";

            switch (Suit) {
            case Suit.M:
                suffix = "m";
                break;
            case Suit.P:
                suffix = "p";
                break;
            case Suit.S:
                suffix = "s";
                break;
            case Suit.Z:
                suffix = "z";
                break;
            }

            return suffix;
        }

        public override string ToString() {
            var suffix = GetSuffix();
            return IsRed ? $"0{suffix}" : $"{Rank}{suffix}";
        }

        public override bool Equals(object? obj) {
            if (obj is Tile other) {
                return Suit == other.Suit && Rank == other.Rank && IsRed == other.IsRed;
            }

            return false;
        }

        public bool EqualsIgnoreColor(Tile other) {
            return Suit == other.Suit && Rank == other.Rank;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public int GetHashCode(Tile obj) {
            var hash = (int)obj.Suit;
            hash = hash * 31 + obj.Rank;
            hash = hash * 31 + (obj.IsRed ? 1 : 0);
            return hash;
        }

        private struct TileIgnoreColorEqualityComparerImpl : IEqualityComparer<Tile> {
            public bool Equals(Tile? x, Tile? y) {
                if (x is null && y is null) {
                    return true;
                }
                if (x is null || y is null) {
                    return false;
                }

                return x.EqualsIgnoreColor(y);
            }

            public int GetHashCode(Tile obj) {
                var hash = (int)obj.Suit;
                hash = hash * 31 + obj.Rank;
                return hash;
            }
        }
    }
}
