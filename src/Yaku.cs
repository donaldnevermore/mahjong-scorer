using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MahjongSharp {
    public static class Yaku {
        /// <summary>
        /// Do a Riichi when it's Menzenchin.
        /// </summary>
        public static YakuValue Riichi(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (handStatus.Menzenchin && handStatus.Riichi) {
                return new YakuValue { Name = "Riichi", Value = 1 };
            }
            if (handStatus.Menzenchin && handStatus.DoubleRiichi) {
                return new YakuValue { Name = "DoubleRiichi", Value = 2 };
            }

            return new YakuValue();
        }

        /// <summary>
        /// One shot.
        /// </summary>
        public static YakuValue Ippatsu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var menzenRiichi = handStatus.Menzenchin && (handStatus.Riichi || handStatus.DoubleRiichi);
            if (menzenRiichi && handStatus.Ippatsu) {
                return new YakuValue { Name = "Ippatsu", Value = 1 };
            }

            return new YakuValue();
        }

        /// <summary>
        /// Tsumo at Menzenchin status.
        /// </summary>
        public static YakuValue MenzenTsumo(IList<Meld> decompose, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (handStatus.Menzenchin && handStatus.Tsumo) {
                return new YakuValue { Name = "MenzenTsumo", Value = 1 };
            }

            return new YakuValue();
        }

        /// <summary>
        /// Menzenchin only.
        /// 4 Sequences + 1 non-Yakuhai pair, and the winning tile must be 2-sided.
        /// </summary>
        public static YakuValue Pinfu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.Menzenchin) {
                return new YakuValue();
            }

            var sequenceCount = 0;
            var twoSided = false;

            foreach (var meld in decomposes) {
                if (meld.Type != MeldType.Pair && meld.Type != MeldType.Sequence) {
                    return new YakuValue();
                }

                if (meld.Type == MeldType.Sequence) {
                    sequenceCount++;
                    twoSided = twoSided || meld.IsTwoSidedIgnoreColor(winningTile);
                }
            }

            if (sequenceCount == 4 && twoSided) {
                return new YakuValue { Name = "Pinfu", Value = 1 };
            }

            return new YakuValue();
        }

        public static YakuValue SeatWind(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var wind = roundStatus.SeatWind;

            foreach (var meld in decomposes) {
                if (meld.IdenticalTo(MeldType.Triplet, wind)) {
                    return new YakuValue { Name = $"SeatWind: {wind}", Value = 1 };
                }
            }

            return new YakuValue();
        }

        public static YakuValue FieldWind(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var wind = roundStatus.RoundWind;

            foreach (var meld in decomposes) {
                if (meld.IdenticalTo(MeldType.Triplet, wind)) {
                    return new YakuValue { Name = $"FieldWind: {wind}", Value = 1 };
                }
            }

            return new YakuValue();
        }

        public static YakuValue DragonWhite(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            foreach (var meld in decomposes) {
                if (meld.IdenticalTo(MeldType.Triplet, new Tile(Suit.Z, 5))) {
                    return new YakuValue { Name = "Dragon: White", Value = 1 };
                }
            }

            return new YakuValue();
        }

        public static YakuValue DragonGreen(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            foreach (var meld in decomposes) {
                if (meld.IdenticalTo(MeldType.Triplet, new Tile(Suit.Z, 6))) {
                    return new YakuValue { Name = "Dragon: Green", Value = 1 };
                }
            }

            return new YakuValue();
        }

        public static YakuValue DragonRed(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            foreach (var meld in decomposes) {
                if (meld.IdenticalTo(MeldType.Triplet, new Tile(Suit.Z, 7))) {
                    return new YakuValue { Name = "Dragon: Red", Value = 1 };
                }
            }

            return new YakuValue();
        }

        /// <summary>
        /// All simples.
        /// </summary>
        public static YakuValue Tanyao(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            foreach (var meld in decomposes) {
                if (meld.HasYaochuu) {
                    return new YakuValue();
                }
            }

            return new YakuValue { Name = "Tanyao", Value = 1 };
        }

        /// <summary>
        /// After a kong.
        /// </summary>
        public static YakuValue Rinshan(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            return handStatus.Rinshan
                ? new YakuValue { Name = "Rinshan", Value = 1 }
                : new YakuValue();
        }

        /// <summary>
        /// Haitei or Hotei.
        /// </summary>
        public static YakuValue HaiteiOrHotei(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.HaiteiOrHotei) {
                return new YakuValue();
            }

            return handStatus.Tsumo
                ? new YakuValue { Name = "Haitei", Value = 1 }
                : new YakuValue { Name = "Hotei", Value = 1 };
        }

        /// <summary>
        /// Robbing a kong.
        /// </summary>
        public static YakuValue Chankan(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            return handStatus.Chankan
                ? new YakuValue { Name = "Chankan", Value = 1 }
                : new YakuValue();
        }

        /// <summary>
        /// 7 pairs.
        /// </summary>
        public static YakuValue Chitoitsu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.Menzenchin) {
                return new YakuValue();
            }
            if (decomposes.Count != 7) {
                return new YakuValue();
            }

            foreach (var meld in decomposes) {
                if (meld.Type != MeldType.Pair) {
                    return new YakuValue();
                }
            }

            return new YakuValue { Name = "Chitoitsu", Value = 2 };
        }

        /// <summary>
        /// Pure Straight, e.g. 123456789m
        /// </summary>
        public static YakuValue Ittsu(IList<Meld> decompose, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            const int Flag = 0b1001001;
            var handFlag = 0;

            foreach (var meld in decompose) {
                if (meld.Type == MeldType.Sequence) {
                    handFlag |= 1 << Tile.GetIndex(meld.First);
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            while (handFlag > 0) {
                if ((handFlag & Flag) == Flag) {
                    return new YakuValue { Name = "Ittsu", Value = handStatus.Menzenchin ? 2 : 1 };
                }
                handFlag >>= 9;
            }

            return new YakuValue();
        }

        /// <summary>
        /// Mixed Triple Sequence, e.g. 123m123p123s
        /// </summary>
        public static YakuValue SanshokuDojun(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            const int Flag = 0b1000000001000000001;
            var handFlag = 0;

            foreach (var meld in decomposes) {
                if (meld.Type == MeldType.Sequence) {
                    handFlag |= 1 << Tile.GetIndex(meld.First);
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            for (var i = 0; i < 9; i++) {
                if ((handFlag & Flag) == Flag) {
                    return new YakuValue { Name = "SanshokuDojun", Value = handStatus.Menzenchin ? 2 : 1 };
                }
                handFlag >>= 1;
            }

            return new YakuValue();
        }

        /// <summary>
        /// Triple Triplets.
        /// </summary>
        public static YakuValue SanshokuDoko(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            const int Flag = 0b1000000001000000001;
            var handFlag = 0;

            foreach (var meld in decomposes) {
                if (meld.Type == MeldType.Triplet && meld.Suit != Suit.Z) {
                    handFlag |= 1 << Tile.GetIndex(meld.First);
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            for (var i = 0; i < 9; i++) {
                if ((handFlag & Flag) == Flag) {
                    return new YakuValue { Name = "SanshokuDoko", Value = 2 };
                }
                handFlag >>= 1;
            }

            return new YakuValue();
        }

        /// <summary>
        /// Chanta or Junchan.
        /// </summary>
        public static YakuValue ChantaOrJunchan(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var hasJihai = false;

            foreach (var meld in decomposes) {
                if (!meld.HasYaochuu) {
                    return new YakuValue();
                }

                if (meld.IsJihai) {
                    hasJihai = true;
                }
            }

            return hasJihai
                ? new YakuValue { Name = "Chanta", Value = handStatus.Menzenchin ? 2 : 1 }
                : new YakuValue { Name = "Junchan", Value = handStatus.Menzenchin ? 3 : 2 };
        }

        /// <summary>
        /// Honroto or Chinroto.
        /// </summary>
        public static YakuValue HonrotoOrChinroto(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var hasJihai = false;

            foreach (var meld in decomposes) {
                if (!meld.IsYaochuu) {
                    return new YakuValue();
                }

                if (meld.IsJihai) {
                    hasJihai = true;
                }
            }

            return hasJihai
                ? new YakuValue { Name = "Honroto", Value = 2 }
                : new YakuValue { Name = "Chinroto", Value = 1, Type = YakuType.Yakuman };
        }

        /// <summary>
        /// All Wind & Dragon tiles.
        /// </summary>
        public static YakuValue Tsuiso(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (decomposes.All(meld => meld.IsJihai)) {
                return new YakuValue { Name = "Tsuiso", Value = 1, Type = YakuType.Yakuman };
            }

            return new YakuValue();
        }

        /// <summary>
        /// Iipeko or Ryampeko.
        /// </summary>
        public static YakuValue IipekoOrRyampeko(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.Menzenchin) {
                return new YakuValue();
            }

            var handFlag = 0;
            var count = 0;

            foreach (var meld in decomposes) {
                if (meld.Type == MeldType.Sequence) {
                    var tileFlag = 1 << Tile.GetIndex(meld.First);
                    if ((handFlag & tileFlag) != 0) {
                        count++;

                        // Toggle that bit, thus making it 0 again.
                        handFlag ^= tileFlag;
                    }
                    else {
                        handFlag |= tileFlag;
                    }
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            if (count == 2) {
                return new YakuValue { Name = "Ryampeko", Value = 3 };
            }
            if (count == 1) {
                return new YakuValue { Name = "Iipeko", Value = 1 };
            }

            return new YakuValue();
        }

        /// <summary>
        /// All Triplets.
        /// </summary>
        public static YakuValue Toitoi(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var countPairs = decomposes.Count(meld => meld.Type == MeldType.Pair);
            if (countPairs != 1) {
                return new YakuValue();
            }

            if (decomposes.All(meld => meld.Type == MeldType.Pair || meld.Type == MeldType.Triplet)) {
                return new YakuValue { Name = "Toitoi", Value = 2 };
            }

            return new YakuValue();
        }

        /// <summary>
        /// Sananko, Suanko, or SuankoTanki.
        /// </summary>
        public static YakuValue SanankoOrSuanko(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var count = decomposes.Count(meld => meld.Type == MeldType.Triplet && !meld.IsOpen);
            if (count < 3) {
                return new YakuValue();
            }

            Debug.Assert(count <= 4, "There could not be more than 4 triplets in a complete hand.");

            var winningTileInOther = decomposes.Any(meld => !meld.IsOpen &&
                (meld.Type == MeldType.Pair || meld.Type == MeldType.Sequence) &&
                meld.ContainsIgnoreColor(winningTile));

            if (handStatus.Tsumo) {
                if (count == 3) {
                    return new YakuValue { Name = "Sananko", Value = 2 };
                }

                // count == 4
                return winningTileInOther
                    ? new YakuValue { Name = "SuankoTanki", Value = 2, Type = YakuType.Yakuman }
                    : new YakuValue { Name = "Suanko", Value = 1, Type = YakuType.Yakuman };
            }

            // Rong
            if (count == 3 && !winningTileInOther) {
                return new YakuValue();
            }
            if (count == 3 && winningTileInOther) {
                return new YakuValue { Name = "Sananko", Value = 2 };
            }

            // count == 4
            return winningTileInOther
                ? new YakuValue { Name = "SuankoTanki", Value = 2, Type = YakuType.Yakuman }
                : new YakuValue { Name = "Sananko", Value = 2 };
        }

        /// <summary>
        /// Honitsu or Chinitsu.
        /// </summary>
        public static YakuValue HonitsuOrChinitsu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var allM = decomposes.All(meld => meld.Suit == Suit.M || meld.IsJihai);
            var allS = decomposes.All(meld => meld.Suit == Suit.S || meld.IsJihai);
            var allP = decomposes.All(meld => meld.Suit == Suit.P || meld.IsJihai);

            var single = allM || allS || allP;
            if (!single) {
                return new YakuValue();
            }

            var anyZ = decomposes.Any(meld => meld.IsJihai);
            return anyZ
                ? new YakuValue { Name = "Honitsu", Value = handStatus.Menzenchin ? 3 : 2 }
                : new YakuValue { Name = "Chinitsu", Value = handStatus.Menzenchin ? 6 : 5 };
        }

        /// <summary>
        /// Sankantsu or Sukantsu.
        /// </summary>
        public static YakuValue SankantsuOrSukantsu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var count = decomposes.Count(meld => meld.IsKong);
            if (count == 3) {
                return new YakuValue { Name = "Sankantsu", Value = 2 };
            }
            if (count == 4) {
                return new YakuValue { Name = "Sukantsu", Value = 1, Type = YakuType.Yakuman };
            }

            return new YakuValue();
        }

        /// <summary>
        /// Shosangen or Daisangen.
        /// </summary>
        public static YakuValue ShosangenOrDaisangen(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            const int Flag = 0b111;
            var tripletFlag = 0;
            var pairFlag = 0;

            foreach (var meld in decomposes) {
                if (meld.IsJihai && meld.First.Rank >= 5) {
                    if (meld.Type == MeldType.Triplet) {
                        tripletFlag |= 1 << (meld.First.Rank - 5);
                    }
                    if (meld.Type == MeldType.Pair) {
                        pairFlag |= 1 << (meld.First.Rank - 5);
                    }
                }
            }

            if (tripletFlag == Flag) {
                return new YakuValue { Name = "Daisangen", Value = 1, Type = YakuType.Yakuman };
            }
            if ((tripletFlag | pairFlag) == Flag && pairFlag != Flag) {
                return new YakuValue { Name = "Shosangen", Value = 2 };
            }

            return new YakuValue();
        }

        public static YakuValue TenhoOrChiho(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.Tsumo || !handStatus.Menzenchin || !handStatus.TenhoOrChiho) {
                return new YakuValue();
            }

            return roundStatus.IsDealer
                ? new YakuValue { Name = "Tenho", Value = 1, Type = YakuType.Yakuman }
                : new YakuValue { Name = "Chiho", Value = 1, Type = YakuType.Yakuman };
        }

        public static YakuValue Kokushi(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (decomposes.Count != 13) {
                return new YakuValue();
            }

            var pair = decomposes.First(meld => meld.Type == MeldType.Pair);
            return pair.ContainsIgnoreColor(winningTile)
                ? new YakuValue { Name = "Kokushi13Wait", Value = 2, Type = YakuType.Yakuman }
                : new YakuValue { Name = "Kokushi", Value = 1, Type = YakuType.Yakuman };
        }

        /// <summary>
        /// Churen or True Churen.
        /// </summary>
        public static YakuValue Churen(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            if (!handStatus.Menzenchin) {
                return new YakuValue();
            }

            var first = decomposes[0];
            var all = decomposes.All(meld => meld.Suit == first.Suit);
            if (!all) {
                return new YakuValue();
            }

            var counts = new int[9];
            foreach (var meld in decomposes) {
                foreach (var tile in meld.Tiles) {
                    counts[tile.Rank - 1]++;
                }
            }

            if (counts[0] < 3 || counts[8] < 3) {
                return new YakuValue();
            }

            for (var i = 1; i < 8; i++) {
                if (counts[i] < 1) {
                    return new YakuValue();
                }
            }

            var isPure = counts[winningTile.Rank - 1] == 2 || counts[winningTile.Rank - 1] == 4;
            return isPure
                ? new YakuValue { Name = "TrueChuren", Value = 2, Type = YakuType.Yakuman }
                : new YakuValue { Name = "Churen", Value = 1, Type = YakuType.Yakuman };
        }

        public static YakuValue ShosushiOrDaisushi(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            const int Flag = 15;
            var tripletFlag = 0;
            var pairFlag = 0;

            foreach (var meld in decomposes) {
                if (meld.IsJihai && meld.First.Rank <= 4) {
                    if (meld.Type == MeldType.Triplet) {
                        tripletFlag |= 1 << (meld.First.Rank - 1);
                    }
                    if (meld.Type == MeldType.Pair) {
                        pairFlag |= 1 << (meld.First.Rank - 1);
                    }
                }
            }

            if (tripletFlag == Flag) {
                return new YakuValue { Name = "Daisushi", Value = 2, Type = YakuType.Yakuman };
            }
            if ((tripletFlag | pairFlag) == Flag && pairFlag != Flag) {
                return new YakuValue { Name = "Shosushi", Value = 1, Type = YakuType.Yakuman };
            }

            return new YakuValue();
        }

        /// <summary>
        /// All Green.
        /// </summary>
        public static YakuValue Ryuiso(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var counts = TileMaker.CountMeldTiles(decomposes);
            var green = TileMaker.GetGreenTiles();

            for (var i = 0; i < counts.Length; i++) {
                if (!green.Contains(i) && counts[i] > 0) {
                    return new YakuValue();
                }
            }

            return new YakuValue { Name = "Ryuiso", Value = 1, Type = YakuType.Yakuman };
        }
    }
}
