// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongScorer.Domain;
using MahjongScorer.Score;
using MahjongScorer.Config;
using MahjongScorer.Util;

namespace MahjongScorer {
    public static class Scorer {
        public static PointInfo GetScore(string handTiles, string winningTile, string[] openMelds,
            HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            var handInfo = HandMaker.GetHandInfo(handTiles, winningTile, openMelds);
            return GetHandScore(handInfo, handConfig, round, rule);
        }

        public static PointInfo GetHandScore(HandInfo handInfo, HandConfig handConfig,
            RoundConfig round, RuleConfig rule) {
            var pc = new PointCalculator(handInfo, handConfig, round, rule);
            var pt = pc.GetTotalPoints();
            return pt;
        }

        public static void ShowScore(string handTiles, string winningTile, string[] openMelds,
            HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            var pt = GetScore(handTiles, winningTile, openMelds, handConfig, round, rule);
            ShowPointInfo(pt);
        }

        public static void ShowHandScore(HandInfo handInfo, HandConfig handConfig,
            RoundConfig round, RuleConfig rule) {
            var pt = GetHandScore(handInfo, handConfig, round, rule);
            ShowPointInfo(pt);
        }

        public static void ShowPointInfo(PointInfo pt) {
            Console.WriteLine(pt);

            if (pt.DealerTsumo is not null) {
                Console.WriteLine(pt.DealerTsumo);
            }
            else if (pt.NonDealerTsumo is not null) {
                Console.WriteLine(pt.NonDealerTsumo);
            }
            else if (pt.Ron is not null) {
                Console.WriteLine(pt.Ron);
            }
        }
    }
}
