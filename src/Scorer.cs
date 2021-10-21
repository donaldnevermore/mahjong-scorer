// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongSharp.Domain;
using MahjongSharp.Score;
using MahjongSharp.Util;

namespace MahjongSharp {
    public class Scorer {
        private HandConfig handConfig;
        private RoundConfig round;
        private RuleConfig rule;

        public Scorer(HandConfig handConfig, RoundConfig round, RuleConfig rule) {
            this.handConfig = handConfig;
            this.round = round;
            this.rule = rule;
        }

        public void GetScore(string handTiles, string winningTile, string[] openMelds) {
            var h = HandMaker.GetHandInfo(handTiles, winningTile, openMelds);
            GetScore(h);
        }

        public void GetScore(HandInfo handInfo) {
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
