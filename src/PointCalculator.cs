// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Point;
using MahjongScorer.Util;

namespace MahjongScorer {
    public class PointCalculator {
        private HandInfo handInfo;
        private HandConfig handConfig;
        private RoundConfig round;
        private RuleConfig rule;

        // Yaku base points.
        private const int Mangan = 2000;
        private const int Haneman = 3000;
        private const int Baiman = 4000;
        private const int Sanbaiman = 6000;
        private const int Yakuman = 8000;

        public PointCalculator(HandInfo handInfo, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            this.handInfo = handInfo;
            this.handConfig = handConfig;
            this.round = round;
            this.rule = rule;
        }

        public PointInfo GetTotalPoints() {
            var decomposes = Decomposer.Decompose(handInfo);
            if (decomposes.Count == 0) {
                return new PointInfo();
            }

            var pointInfoList = new List<PointInfo>();

            foreach (var decompose in decomposes) {
                var yakuList = GetYakuList(decompose);
                var fuList = GetFuList(decompose, yakuList);
                if (yakuList.Count != 0) {
                    var info = CountHanAndFu(yakuList, fuList);
                    pointInfoList.Add(info);
                }
            }

            if (pointInfoList.Count == 0) {
                return new PointInfo();
            }

            // Sort and get the highest point.
            pointInfoList.Sort();
            return pointInfoList[^1];
        }

        private void UpdateStatus() {
            if (handInfo.OpenMelds.Any(meld => meld.IsOpen)) {
                handConfig.Menzenchin = false;
            }
        }

        private DoraInfo GetDoraInfo() {
            return new() {
                Dora = handConfig.Dora,
                UraDora = handConfig.UraDora,
                RedDora = handInfo.RedDora
            };
        }

        private PointInfo CountHanAndFu(IList<YakuValue> yakuList, IList<FuValue> fuList) {
            if (yakuList.Count == 0) {
                return new PointInfo();
            }

            var han = 0;
            var yakumanCount = 0;

            foreach (var yaku in yakuList) {
                if (yaku.IsYakuman) {
                    yakumanCount += yaku.Value;
                }
                else {
                    han += yaku.Value;
                }
            }

            var fu = 0;
            var basePoints = 0;
            var dora = GetDoraInfo();

            if (yakumanCount > 0) {
                // Handle Yakuman.
                yakumanCount = rule.MultipleYakuman ? yakumanCount : 1;
                basePoints = Yakuman * yakumanCount;
            }
            else {
                han += dora.TotalDora;
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
                    // When han < 5, calculate Fu & base points.
                    fu = FuCalculator.CountFu(fuList);
                    var point = fu * (int)Math.Pow(2, han + 2);
                    if (rule.RoundUpMangan && point >= 1920) {
                        basePoints = Mangan;
                    }
                    else {
                        basePoints = Math.Min(Mangan, point);
                    }
                }
            }

            if (round.IsDealer) {
                if (handConfig.Tsumo) {
                    var basePayOnAll = NumberUtil.RoundUpToNextUnit(basePoints * 2, 100);
                    return new DealerTsumo {
                        BasePayOnAll = basePayOnAll,
                        YakuList = yakuList, FuList = fuList, YakumanCount = yakumanCount,
                        Han = han, Fu = fu, BasePoints = basePoints,
                        Dora = dora, Honba = round.Honba, RiichiBets = round.RiichiBets
                    };
                }

                var dealerRon = NumberUtil.RoundUpToNextUnit(basePoints * 6, 100);
                return new Ron {
                    BasePay = dealerRon,
                    YakuList = yakuList, FuList = fuList, YakumanCount = yakumanCount,
                    Han = han, Fu = fu, BasePoints = basePoints,
                    Dora = dora, Honba = round.Honba, RiichiBets = round.RiichiBets
                };
            }

            if (handConfig.Tsumo) {
                var nonDealerBasePay = NumberUtil.RoundUpToNextUnit(basePoints, 100);
                var dealerBasePay = NumberUtil.RoundUpToNextUnit(basePoints * 2, 100);
                return new NonDealerTsumo {
                    NonDealerBasePay = nonDealerBasePay, DealerBasePay = dealerBasePay,
                    YakuList = yakuList, FuList = fuList, YakumanCount = yakumanCount,
                    Han = han, Fu = fu, BasePoints = basePoints,
                    Dora = dora, Honba = round.Honba, RiichiBets = round.RiichiBets
                };
            }

            var nonDealerRon = NumberUtil.RoundUpToNextUnit(basePoints * 4, 100);
            return new Ron {
                BasePay = nonDealerRon,
                YakuList = yakuList, FuList = fuList, YakumanCount = yakumanCount,
                Han = han, Fu = fu, BasePoints = basePoints,
                Dora = dora, Honba = round.Honba, RiichiBets = round.RiichiBets
            };
        }

        private IList<FuValue> GetFuList(IList<Meld> decompose, IList<YakuValue> yakuList) {
            return FuCalculator.GetFuList(decompose, handInfo.WinningTile, yakuList, handConfig, round, rule);
        }

        /// <summary>
        /// Count Han, the main portion of scoring, as each yaku is assigned a value in terms of han.
        /// </summary>
        private IList<YakuValue> GetYakuList(IList<Meld> decompose) {
            var yc = new YakuCalculator(decompose, handInfo.WinningTile, handConfig, round, rule);
            return yc.GetYakuList();
        }

        public static bool HasWin(HandInfo handInfo) {
            return Decomposer.Decompose(handInfo).Count > 0;
        }

        public static IList<Tile> GetWinningTiles(HandInfo hand) {
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
}
