// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Util;

public class FuCalculator {
    /// <summary>
    /// Count Fu by taking the hand composition into consideration in terms of tile melds,
    /// wait patterns and/or win method.
    /// </summary>
    public static List<FuValue> GetFuList(List<Meld> decompose, Tile winningTile,
        HandConfig hand, RoundConfig round, RuleConfig rule) {
        var result = new List<FuValue>();

        // 7 Pairs
        if (decompose.Count == 7) {
            result.Add(new FuValue(FuType.SevenPairs, 25));
            return result;
        }

        var isPinfuLike = YakuCalculator.IsPinfuLike(decompose, winningTile, round);

        // Pinfu Tsumo
        if (isPinfuLike && hand.Menzenchin && hand.Tsumo) {
            result.Add(new FuValue(FuType.PinfuTsumo, 20));
            return result;
        }

        // Pinfu Ron with an Open Hand (Tsumo is also OK)
        if (isPinfuLike && !hand.Menzenchin) {
            result.Add(new FuValue(FuType.PinfuRonWithAnOpenHand, 30));
            return result;
        }

        // Base Fu
        result.Add(new FuValue(FuType.BaseFu, 20));

        CountWaitingPattern(decompose, winningTile, isPinfuLike, hand, result);
        CountMeldPattern(decompose, winningTile, hand, round, rule, result);

        return result;
    }

    public static int CountFu(List<FuValue> fuList) {
        if (fuList.Count == 1) {
            return fuList[0].Value;
        }

        var sum = fuList.Sum(item => item.Value);
        return NumberUtil.RoundUpToNextUnit(sum, 10);
    }

    private static void CountMeldPattern(List<Meld> decompose, Tile winningTile,
        HandConfig hand, RoundConfig round, RuleConfig rule, List<FuValue> result) {
        foreach (var meld in decompose) {
            var isOpen = meld.IsOpen || (!hand.Tsumo && meld.ContainsIgnoreColor(winningTile));

            switch (meld.Type) {
            case MeldType.Pair:
                CountPair(meld, round, rule, result);
                break;
            case MeldType.Triplet:
                CountTriplet(meld, isOpen, result);
                break;
            case MeldType.Quad:
                CountQuad(meld, isOpen, result);
                break;
            }
        }
    }

    private static void CountPair(Meld pair, RoundConfig round, RuleConfig rule, List<FuValue> result) {
        if (!pair.IsHonor) {
            return;
        }

        var rank = pair.Tiles[0].Rank;
        var seatWind = (int)round.SeatWind;
        var roundWind = (int)round.RoundWind;

        if (rank >= 5 && rank <= 7) {
            // Dragon tiles
            result.Add(new FuValue(FuType.Dragon, 2));
        }
        else if (rank == seatWind && rank == roundWind) {
            // Double Wind
            result.Add(new FuValue(FuType.DoubleWind, rule.DoubleWindFu ? 4 : 2));
        }
        else if (rank == seatWind) {
            result.Add(new FuValue(FuType.SeatWind, 2));
        }
        else if (rank == roundWind) {
            result.Add(new FuValue(FuType.RoundWind, 2));
        }
    }

    private static void CountTriplet(Meld meld, bool isOpen, List<FuValue> result) {
        if (isOpen) {
            result.Add(meld.IsYaochuu
                ? new FuValue(FuType.OpenTripletYaochuu, 4)
                : new FuValue(FuType.OpenTriplet, 2));
        }
        else {
            result.Add(meld.IsYaochuu
                ? new FuValue(FuType.ClosedTripletYaochuu, 8)
                : new FuValue(FuType.ClosedTriplet, 4));
        }
    }

    private static void CountQuad(Meld meld, bool isOpen, List<FuValue> result) {
        if (isOpen) {
            result.Add(meld.IsYaochuu
                ? new FuValue(FuType.OpenQuadYaochuu, 16)
                : new FuValue(FuType.OpenQuad, 8));
        }
        else {
            result.Add(meld.IsYaochuu
                ? new FuValue(FuType.ClosedQuadYaochuu, 32)
                : new FuValue(FuType.ClosedQuad, 16));
        }
    }

    private static void CountWaitingPattern(List<Meld> decompose, Tile winningTile,
        bool isPinfuLike, HandConfig hand, List<FuValue> result) {
        if (hand.Tsumo) {
            // Tsumo
            result.Add(new FuValue(FuType.Tsumo, 2));
        }
        else if (hand.Menzenchin) {
            // Menzenchin Ron
            result.Add(new FuValue(FuType.MenzenchinRon, 10));
        }

        // Pinfu is double-sided waiting, so no Fu added.
        if (isPinfuLike) {
            return;
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
            case MeldType.Sequence:
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
