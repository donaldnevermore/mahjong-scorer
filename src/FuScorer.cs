using System.Collections.Generic;
using System.Linq;

namespace MahjongSharp {
    public class FuScorer {
        /// <summary>
        /// Count Fu by taking the hand composition into consideration in terms of tile melds,
        /// wait patterns and/or win method.
        /// </summary>
        public static int CountFu(IList<Meld> decomposes, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, IList<YakuValue> yakus, Ruleset ruleset) {
            // 7 Pairs.
            if (decomposes.Count == 7) {
                return 25;
            }

            // 13 Orphans.
            if (decomposes.Count == 13) {
                return 30;
            }

            // Pinfu tsumo
            if (handStatus.Tsumo && yakus.Any(yaku => yaku.Name == "Pinfu")) {
                return 20;
            }

            // Pinfu with open melds.
            if (yakus.Any(yaku => yaku.Name == "Pinfu") && decomposes.Any(meld => meld.IsOpen)) {
                return 30;
            }

            // Base Fu
            var fu = 20;

            // Menzenchin and Rong
            if (handStatus.Menzenchin && !handStatus.Tsumo) {
                fu += 10;
            }

            // Tsumo
            if (handStatus.Tsumo &&
                !yakus.Any(yaku => yaku.Name == "Pinfu" || yaku.Name == "Rinshan")) {
                fu += 2;
            }

            // Pairs
            var pair = decomposes.First(meld => meld.Type == MeldType.Pair);
            if (pair.Suit == Suit.Z) {
                // Dragon tiles
                if (pair.First.Rank >= 5 && pair.First.Rank <= 7) {
                    fu += 2;
                }

                var playerWind = roundStatus.SeatWind;
                var roundWind = roundStatus.RoundWind;
                if (pair.First.EqualsIgnoreColor(playerWind)) {
                    fu += 2;
                }

                if (pair.First.EqualsIgnoreColor(roundWind)) {
                    if (!roundWind.EqualsIgnoreColor(playerWind) || ruleset.DoubleWindFu) {
                        fu += 2;
                    }
                }
            }

            // Sequences
            var flag = 0;
            foreach (var meld in decomposes) {
                if (!meld.Tiles.Contains(winningTile)) {
                    continue;
                }

                if (meld.Type == MeldType.Pair) {
                    flag++;
                }
                if (meld.Type == MeldType.Sequence && !meld.IsOpen && meld.IsTwoSidedIgnoreColor(winningTile)) {
                    flag++;
                }
            }

            if (flag != 0) {
                fu += 2;
            }

            // Triplets
            var winningTileInOther = decomposes.Any(meld => !meld.IsOpen &&
                (meld.Type == MeldType.Pair || meld.Type == MeldType.Sequence) &&
                meld.ContainsIgnoreColor(winningTile));

            foreach (var meld in decomposes) {
                if (meld.Type != MeldType.Triplet) {
                    continue;
                }
                if (meld.IsOpen) {
                    fu += GetTripletFu(meld, true);
                }
                else if (handStatus.Tsumo) {
                    fu += GetTripletFu(meld, false);
                }
                else if (winningTileInOther) {
                    fu += GetTripletFu(meld, false);
                }
                else if (meld.ContainsIgnoreColor(winningTile)) {
                    fu += GetTripletFu(meld, true);
                }
                else {
                    fu += GetTripletFu(meld, false);
                }
            }

            return Util.RoundUpToNextUnit(fu, 10);
        }

        public static int GetTripletFu(Meld meld, bool isOpen) {
            var triplet = isOpen ? 2 : 4;
            if (meld.IsKong) {
                triplet *= 4;
            }
            if (meld.IsYaochuu) {
                triplet *= 2;
            }

            return triplet;
        }
    }
}
