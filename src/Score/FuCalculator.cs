// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Util;

namespace MahjongScorer.Score {
    public class FuCalculator {
        /// <summary>
        /// Count Fu by taking the hand composition into consideration in terms of tile melds,
        /// wait patterns and/or win method.
        /// </summary>
        public static IList<FuValue> GetFuList(IList<Meld> decompose, Tile winningTile, IList<YakuValue> yakuList,
            HandConfig hand, RoundConfig round, RuleConfig rule) {
            var result = new List<FuValue>();

            if (yakuList.Count == 0) {
                return result;
            }

            // 7 Pairs
            if (decompose.Count == 7) {
                result.Add(new FuValue(FuType.SevenPairs, 25));
                return result;
            }

            // 13 Orphans
            if (decompose.Count == 13) {
                result.Add(new FuValue(FuType.DoesNotMatter, 0));
                return result;
            }

            // Pinfu Tsumo
            if (hand.Tsumo && yakuList.Any(yaku => yaku.Name == YakuType.Pinfu)) {
                result.Add(new FuValue(FuType.PinfuTsumo, 20));
                return result;
            }

            // Pinfu Ron with an Open Hand
            if (!hand.Menzenchin && !hand.Tsumo && YakuCalculator.IsPinfuType(decompose, winningTile)) {
                result.Add(new FuValue(FuType.PinfuRonWithAnOpenHand, 30));
                return result;
            }

            // Base Fu
            result.Add(new FuValue(FuType.BaseFu, 20));

            CountWaitingPattern(decompose, winningTile, hand, result);
            CountMeldPattern(decompose, winningTile, hand, round, rule, result);

            return result;
        }

        public static int CountFu(IList<FuValue> fuList) {
            if (fuList.Count == 1) {
                return fuList[0].Value;
            }

            var sum = fuList.Sum(item => item.Value);
            return NumberUtil.RoundUpToNextUnit(sum, 10);
        }

        private static void CountMeldPattern(IList<Meld> decompose, Tile winningTile,
            HandConfig hand, RoundConfig round, RuleConfig rule, IList<FuValue> result) {
            foreach (var meld in decompose) {
                switch (meld.Type) {
                case MeldType.Pair:
                    CountPair(meld, round, rule, result);
                    break;
                case MeldType.Triplet: {
                    var isOpen = meld.IsOpen || (!hand.Tsumo && meld.ContainsIgnoreColor(winningTile));
                    CountTriplet(meld, isOpen, result);
                    break;
                }
                case MeldType.Quad: {
                    var isOpen = meld.IsOpen || (!hand.Tsumo && meld.ContainsIgnoreColor(winningTile));
                    CountQuad(meld, isOpen, result);
                    break;
                }
                }
            }
        }

        private static void CountPair(Meld pair, RoundConfig round, RuleConfig rule, IList<FuValue> result) {
            if (!pair.IsHonor) {
                return;
            }

            // Dragon tiles
            if (pair.Tiles[0].Rank >= 5 && pair.Tiles[0].Rank <= 7) {
                result.Add(new FuValue(FuType.Dragon, 2));
            }
            // Double Wind
            else if (pair.Tiles[0].EqualsIgnoreColor(round.SeatWindTile) &&
                pair.Tiles[0].EqualsIgnoreColor(round.RoundWindTile)) {
                result.Add(new FuValue(FuType.DoubleWind, rule.DoubleWindFu));
            }
            else if (pair.Tiles[0].EqualsIgnoreColor(round.SeatWindTile)) {
                result.Add(new FuValue(FuType.SeatWind, 2));
            }
            else if (pair.Tiles[0].EqualsIgnoreColor(round.RoundWindTile)) {
                result.Add(new FuValue(FuType.RoundWind, 2));
            }
        }

        private static void CountTriplet(Meld meld, bool isOpen, IList<FuValue> result) {
            var fu = 2;

            if (isOpen) {
                if (meld.IsYaochuu) {
                    fu *= 2;
                    result.Add(new FuValue(FuType.OpenTripletYaochuu, fu));
                }
                else {
                    result.Add(new FuValue(FuType.OpenTriplet, fu));
                }
            }
            else {
                fu *= 2;

                if (meld.IsYaochuu) {
                    fu *= 2;
                    result.Add(new FuValue(FuType.ClosedTripletYaochuu, fu));
                }
                else {
                    result.Add(new FuValue(FuType.ClosedTriplet, fu));
                }
            }
        }

        private static void CountQuad(Meld meld, bool isOpen, IList<FuValue> result) {
            var fu = 8;

            if (isOpen) {
                if (meld.IsYaochuu) {
                    fu *= 2;
                    result.Add(new FuValue(FuType.OpenQuadYaochuu, fu));
                }
                else {
                    result.Add(new FuValue(FuType.OpenQuad, fu));
                }
            }
            else {
                fu *= 2;

                if (meld.IsYaochuu) {
                    fu *= 2;
                    result.Add(new FuValue(FuType.ClosedQuadYaochuu, fu));
                }
                else {
                    result.Add(new FuValue(FuType.ClosedQuad, fu));
                }
            }
        }

        private static void CountWaitingPattern(IList<Meld> decompose, Tile winningTile,
            HandConfig hand, IList<FuValue> result) {
            // Tsumo
            if (hand.Tsumo) {
                result.Add(new FuValue(FuType.Tsumo, 2));
            }
            else if (hand.Menzenchin) {
                // Menzenchin Ron
                result.Add(new FuValue(FuType.MenzenchinRon, 10));
            }

            // Waiting pattern
            foreach (var meld in decompose) {
                if (meld.IsOpen || !meld.Tiles.Contains(winningTile)) {
                    continue;
                }

                switch (meld.Type) {
                case MeldType.Pair:
                    result.Add(new FuValue(FuType.SingleWait, 2));
                    break;
                case MeldType.Sequence: {
                    if (winningTile.EqualsIgnoreColor(meld.Tiles[1])) {
                        result.Add(new FuValue(FuType.MiddleWait, 2));
                    }
                    else if ((winningTile.Rank == 3 && meld.Tiles[0].Rank == 1) ||
                        (winningTile.Rank == 7 && meld.Tiles[^1].Rank == 9)) {
                        result.Add(new FuValue(FuType.EndWait, 2));
                    }
                    break;
                }
                }
            }
        }
    }
}
