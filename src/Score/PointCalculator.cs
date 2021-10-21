// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahjongSharp.Domain;

namespace MahjongSharp.Score {
    public class PointCalculator {
        private HandInfo handInfo;
        private HandConfig handConfig;
        private RoundConfig round;
        private RuleConfig rule;

        public PointCalculator(HandInfo handInfo, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            this.handInfo = handInfo;
            this.handConfig = handConfig;
            this.round = round;
            this.rule = rule;
        }

        public PointInfo GetPoints() {
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

        private PointInfo CountHanAndFu(IList<YakuValue> yakuList, IList<FuValue> fuList) {
            var info = new PointInfo(yakuList, fuList, handConfig, round);
            info.CountHanAndFu();
            return info;
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
