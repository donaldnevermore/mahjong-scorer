// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Point;
using MahjongScorer.Config;
using MahjongScorer.Util;

public static class Scorer {
    public static PointInfo GetHandScore(string handTiles, string winningTile, string melds,
        string doraIndicators, HandConfig handConfig, RoundConfig round, RuleConfig rule) {
        var hand = GetHandInfo(handTiles, winningTile, melds, doraIndicators);

        return GetScore(hand, handConfig, round, rule);
    }

    public static HandInfo GetHandInfo(string handTiles, string winningTile, string melds, string doraIndicators) {
        var hand = TileMaker.ConvertTiles(handTiles).ToArray();
        var winning = TileMaker.ConvertTile(winningTile);
        var meldList = TileMaker.ConvertMelds(melds);

        var dora = new List<Tile>();
        var uraDora = new List<Tile>();

        if (!string.IsNullOrEmpty(doraIndicators)) {
            var s = doraIndicators.Split(',');
            if (s.Length == 2) {
                dora = TileMaker.ConvertTiles(s[0]);
                uraDora = TileMaker.ConvertTiles(s[1]);
            } else {
                dora = TileMaker.ConvertTiles(s[0]);
            }
        }
        return new HandInfo(hand, winning, meldList, dora, uraDora);
    }

    public static PointInfo GetScore(HandInfo hand, HandConfig handConfig, RoundConfig round,
        RuleConfig rule) {
        if (hand.OpenMelds.Any(meld => meld.IsOpen)) {
            handConfig.Menzenchin = false;
        }

        var pc = new PointCalculator(hand, handConfig, round, rule);
        var pt = pc.GetTotalPoints();
        return pt;
    }

    public static double[] GetActualPoint(int[] scores, int[] points, int returnPoint) {
        var result = new double[4];

        result[1] = (scores[1] - returnPoint) / 1000.0 + points[1];
        result[2] = (scores[2] - returnPoint) / 1000.0 + points[2];
        result[3] = (scores[3] - returnPoint) / 1000.0 + points[3];

        result[0] = -(result[1] + result[2] + result[3]);

        return result;
    }
}
