// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Point;
using MahjongScorer.Config;
using MahjongScorer.Util;

namespace MahjongScorer {
    public static class Scorer {
        public static PointInfo GetScore(string handTiles, string winningTile, string openMelds,
            string doraIndicators, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            var handInfo = TileMaker.GetHandInfo(handTiles, winningTile, openMelds);

            if (!string.IsNullOrEmpty(doraIndicators)) {
                var s = doraIndicators.Split(',');
                if (s.Length == 2) {
                    handConfig.DoraIndicators = TileMaker.ConvertTiles(s[0]);
                    handConfig.UraDoraIndicators = TileMaker.ConvertTiles(s[1]);
                }
                else {
                    handConfig.DoraIndicators = TileMaker.ConvertTiles(s[0]);
                }
            }

            return GetHandScore(handInfo, handConfig, round, rule);
        }

        public static PointInfo GetHandScore(HandInfo handInfo, HandConfig handConfig,
            RoundConfig round, RuleConfig rule) {
            if (handInfo.OpenMelds.Any(meld => meld.IsOpen)) {
                handConfig.Menzenchin = false;
            }

            var pc = new PointCalculator(handInfo, handConfig, round, rule);
            var pt = pc.GetTotalPoints();
            return pt;
        }

        public static void ShowScore(string handTiles, string winningTile, string openMelds,
            string doraIndicators, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            var pt = GetScore(handTiles, winningTile,
                openMelds, doraIndicators, handConfig, round, rule);
            Console.WriteLine(pt);
        }

        public static void ShowHandScore(HandInfo handInfo, HandConfig handConfig,
            RoundConfig round, RuleConfig rule) {
            var pt = GetHandScore(handInfo, handConfig, round, rule);
            Console.WriteLine(pt);
        }
    }
}
