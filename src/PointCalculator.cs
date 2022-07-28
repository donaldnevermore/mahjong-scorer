// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System;
using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Point;
using MahjongScorer.Util;

public class PointCalculator {
    private HandInfo hand;
    private HandConfig handConfig;
    private RoundConfig round;
    private RuleConfig rule;

    // Yaku base points.
    private const int Mangan = 2000;
    private const int Haneman = 3000;
    private const int Baiman = 4000;
    private const int Sanbaiman = 6000;
    private const int Yakuman = 8000;

    public PointCalculator(HandInfo hand, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
        this.hand = hand;
        this.handConfig = handConfig;
        this.round = round;
        this.rule = rule;
    }

    public PointInfo GetTotalPoints() {
        var decomposes = Decomposer.Decompose(hand);
        if (decomposes.Count == 0) {
            return new PointInfo();
        }

        var maxPoint = new PointInfo();

        foreach (var decompose in decomposes) {
            var point = CountHanAndFu(decompose);
            // Find the highest point.
            if (point.CompareTo(maxPoint) > 0) {
                maxPoint = point;
            }
        }

        return maxPoint;
    }

    private DoraInfo GetDoraInfo() {
        return DoraCalculator.GetAllDora(hand);
    }

    private PointInfo CountHanAndFu(List<Meld> decompose) {
        var yakuList = GetYakuList(decompose);

        if (yakuList.Count == 0) {
            return new PointInfo();
        }

        var hanWithoutYakumanAndDora = 0;
        var yakumanCount = 0;

        foreach (var yaku in yakuList) {
            if (yaku.IsYakuman) {
                yakumanCount += yaku.Value;
            }
            else {
                hanWithoutYakumanAndDora += yaku.Value;
            }
        }

        int basePoints;
        int han;
        List<FuValue> fuList;
        int fu;
        var dora = GetDoraInfo();

        if (yakumanCount > 0) {
            // Handle Yakuman.
            yakumanCount = rule.StackingYakuman ? yakumanCount : 1;
            basePoints = Yakuman * yakumanCount;
            yakuList = yakuList.Where(yakuValue => yakuValue.IsYakuman).ToList();
            han = 0;
            fuList = new List<FuValue>();
            fu = 0;
        }
        else {
            han = hanWithoutYakumanAndDora + dora.TotalDora;
            fuList = GetFuList(decompose);
            fu = FuCalculator.CountFu(fuList);

            if (rule.AccumulatedYakuman && han >= 13) {
                basePoints = Yakuman;
            }
            else if (han >= 11) {
                basePoints = Sanbaiman;
            }
            else if (han >= 8) {
                basePoints = Baiman;
            }
            else if (han >= 6) {
                basePoints = Haneman;
            }
            else if (han >= 5) {
                basePoints = Mangan;
            }
            else {
                // When han < 5, calculate BP (base points).
                var rawBP = fu * (int)Math.Pow(2, han + 2);
                if (rule.RoundUpMangan && rawBP >= 1920) {
                    basePoints = Mangan;
                }
                else {
                    basePoints = Math.Min(Mangan, rawBP);
                }
            }
        }

        if (round.IsDealer) {
            if (handConfig.Tsumo) {
                var basePayOnAll = NumberUtil.RoundUpToNextUnit(basePoints * 2, 100);
                return new DealerTsumo {
                    BasePayOnAll = basePayOnAll,
                    YakuList = yakuList,
                    FuList = fuList,
                    YakumanCount = yakumanCount,
                    Han = han,
                    Fu = fu,
                    BasePoints = basePoints,
                    Dora = dora,
                    Honba = round.Honba,
                    RiichiBets = round.RiichiBets
                };
            }

            var dealerRon = NumberUtil.RoundUpToNextUnit(basePoints * 6, 100);
            return new Ron {
                BasePay = dealerRon,
                YakuList = yakuList,
                FuList = fuList,
                YakumanCount = yakumanCount,
                Han = han,
                Fu = fu,
                BasePoints = basePoints,
                Dora = dora,
                Honba = round.Honba,
                RiichiBets = round.RiichiBets
            };
        }

        if (handConfig.Tsumo) {
            var nonDealerBasePay = NumberUtil.RoundUpToNextUnit(basePoints, 100);
            var dealerBasePay = NumberUtil.RoundUpToNextUnit(basePoints * 2, 100);
            return new NonDealerTsumo {
                NonDealerBasePay = nonDealerBasePay,
                DealerBasePay = dealerBasePay,
                YakuList = yakuList,
                FuList = fuList,
                YakumanCount = yakumanCount,
                Han = han,
                Fu = fu,
                BasePoints = basePoints,
                Dora = dora,
                Honba = round.Honba,
                RiichiBets = round.RiichiBets
            };
        }

        var nonDealerRon = NumberUtil.RoundUpToNextUnit(basePoints * 4, 100);
        return new Ron {
            BasePay = nonDealerRon,
            YakuList = yakuList,
            FuList = fuList,
            YakumanCount = yakumanCount,
            Han = han,
            Fu = fu,
            BasePoints = basePoints,
            Dora = dora,
            Honba = round.Honba,
            RiichiBets = round.RiichiBets
        };
    }

    private List<FuValue> GetFuList(List<Meld> decompose) {
        return FuCalculator.GetFuList(decompose, hand.WinningTile, handConfig, round, rule);
    }

    /// <summary>
    /// Count Han, the main portion of scoring, as each yaku is assigned a value in terms of han.
    /// </summary>
    private List<YakuValue> GetYakuList(List<Meld> decompose) {
        var yc = new YakuCalculator(decompose, hand, handConfig, round, rule);
        return yc.GetYakuList();
    }

    public static bool HasWin(HandInfo handInfo) {
        return Decomposer.Decompose(handInfo).Count > 0;
    }

    public static List<Tile> GetWinningTiles(HandInfo hand) {
        var list = new List<Tile>();
        for (var index = 0; index < 34; index++) {
            var tile = Tile.GetTile(index);
            if (HasWin(hand)) {
                list.Add(tile);
            }
        }

        return list;
    }
}
