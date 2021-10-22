// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahjongScorer.Domain {
    public class Meld : IComparable<Meld> {
        public static readonly IEqualityComparer<Meld> MeldIgnoreColorEqualityComparer =
            new MeldIgnoreColorEqualityComparerImpl();

        public MeldType Type { get; }
        public Tile[] Tiles { get; }
        public bool IsOpen { get; }
        public Suit Suit => Tiles[0].Suit;

        public Meld(bool isOpen, Tile tile) {
            IsOpen = isOpen;
            Type = MeldType.Single;
            Tiles = new[] { tile };
        }

        public Meld(bool isOpen, Tile tile1, Tile tile2) {
            if (!tile1.EqualsIgnoreColor(tile2)) {
                throw new ArgumentException("Invalid meld composition.");
            }

            IsOpen = isOpen;
            Type = MeldType.Pair;
            Tiles = new[] { tile1, tile2 };
        }

        public Meld(bool isOpen, Tile tile1, Tile tile2, Tile tile3) {
            if (tile1.EqualsIgnoreColor(tile2) && tile1.EqualsIgnoreColor(tile3)) {
                IsOpen = isOpen;
                Type = MeldType.Triplet;
                Tiles = new[] { tile1, tile2, tile3 };
            }
            else {
                if (tile1.IsHonor) {
                    throw new ArgumentException("Suit of Z cannot form sequences.");
                }
                if (tile1.Suit != tile2.Suit || tile1.Suit != tile3.Suit) {
                    throw new ArgumentException("Invalid meld composition.");
                }
                if (tile1.Rank + 1 != tile2.Rank || tile1.Rank + 2 != tile3.Rank) {
                    throw new ArgumentException("Invalid meld composition.");
                }

                IsOpen = isOpen;
                Type = MeldType.Sequence;
                Tiles = new[] { tile1, tile2, tile3 };
            }
        }

        public Meld(bool isOpen, Tile tile1, Tile tile2, Tile tile3, Tile tile4) {
            if (!tile1.EqualsIgnoreColor(tile2) ||
                !tile1.EqualsIgnoreColor(tile3) ||
                !tile1.EqualsIgnoreColor(tile4)) {
                throw new ArgumentException("Invalid meld composition.");
            }

            IsOpen = isOpen;
            Type = MeldType.Quad;
            Tiles = new[] { tile1, tile2, tile3, tile4 };
        }

        /// <summary>
        /// Terminal/Honor tiles, e.g. 19m19p19s1234567z
        /// </summary>
        public bool HasYaochuu => Type != MeldType.Single && (Tiles[0].IsYaochuu || Tiles[^1].IsYaochuu);
        public bool IsYaochuu => Type != MeldType.Single && Tiles[0].IsYaochuu && Tiles[^1].IsYaochuu;

        /// <summary>
        /// Terminal tiles, e.g. 19m19p19s
        /// </summary>
        public bool HasTerminal => Type != MeldType.Single && (Tiles[0].IsRoto || Tiles[^1].IsRoto);
        public bool IsTerminal => Type != MeldType.Single && Tiles[0].IsRoto && Tiles[^1].IsRoto;

        /// <summary>
        /// Honor tiles, e.g. 1234567z
        /// </summary>
        public bool IsHonor => Tiles[0].IsHonor;

        public int CompareTo(Meld? other) {
            if (other is null) {
                throw new ArgumentException(nameof(other));
            }

            if (!Tiles[0].EqualsIgnoreColor(other.Tiles[0])) {
                return Tiles[0].CompareTo(other.Tiles[0]);
            }
            if (Type != other.Type) {
                return Type - other.Type;
            }

            var hasRed = Tiles.Any(tile => tile.IsRed);
            var otherHasRed = other.Tiles.Any(tile => tile.IsRed);
            if (hasRed && !otherHasRed) {
                return 1;
            }
            if (!hasRed && otherHasRed) {
                return -1;
            }

            return 0;
        }

        public override string ToString() {
            var builder = new StringBuilder();
            foreach (var tile in Tiles) {
                builder.Append(tile.ToStringIgnoreColor());
            }

            if (IsOpen) {
                builder.Append("open");
            }

            return builder.ToString();
        }

        public override bool Equals(object? obj) {
            if (obj is Meld other) {
                if (Type != other.Type || IsOpen != other.IsOpen) {
                    return false;
                }
                if (Tiles.Length != other.Tiles.Length) {
                    return false;
                }
                for (var i = 0; i < Tiles.Length; i++) {
                    if (!Tiles[i].EqualsIgnoreColor(other.Tiles[i])) {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public bool IsDoubleSidedIgnoreColor(Tile tile) {
            if (Type != MeldType.Sequence) {
                return false;
            }
            if (!Tiles[0].EqualsIgnoreColor(tile) && !Tiles[^1].EqualsIgnoreColor(tile)) {
                return false;
            }
            if (tile.Rank != 3 && tile.Rank != 7) {
                return true;
            }
            if (tile.Rank == 3 && Tiles[0].Rank == 1) {
                return false;
            }
            if (tile.Rank == 7 && Tiles[0].Rank == 9) {
                return false;
            }

            return true;
        }

        public bool ContainsIgnoreColor(Tile tile) {
            if (Suit != tile.Suit) {
                return false;
            }
            return tile.Rank >= Tiles[0].Rank && tile.Rank <= Tiles[^1].Rank;
        }

        private struct MeldIgnoreColorEqualityComparerImpl : IEqualityComparer<Meld> {
            public bool Equals(Meld? x, Meld? y) {
                if (x is null && y is null) {
                    return true;
                }
                if (x is null || y is null) {
                    return false;
                }

                if (x.Type != y.Type || x.IsOpen != y.IsOpen) {
                    return false;
                }
                if (x.Tiles.Length != y.Tiles.Length) {
                    return false;
                }
                for (var i = 0; i < x.Tiles.Length; i++) {
                    if (!x.Tiles[i].EqualsIgnoreColor(y.Tiles[i])) {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(Meld obj) {
                return obj.ToString().GetHashCode();
            }
        }
    }
}
