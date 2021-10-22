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
        public static void GetScore(string handTiles, string winningTile, string[] openMelds, HandConfig handConfig,
            RoundConfig round, RuleConfig rule) {
            var handInfo = HandMaker.GetHandInfo(handTiles, winningTile, openMelds);
            GetScore(handInfo, handConfig, round, rule);
        }

        public static void GetScore(HandInfo handInfo, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            var r = new PointCalculator(handInfo, handConfig, round, rule);
            var s = r.GetPoints();
            if (s.DealerTsumo is not null) {
                Console.WriteLine(s.DealerTsumo);
            }
            else if (s.NonDealerTsumo is not null) {
                Console.WriteLine(s.NonDealerTsumo);
            }
            else if (s.Ron is not null) {
                Console.WriteLine(s.Ron);
            }
        }
    }
}
