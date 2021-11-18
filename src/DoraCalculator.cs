// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;

public class DoraCalculator {
    public static DoraInfo GetAllDora(HandInfo hand) {
        var doraCount = CountDora(hand.AllTiles, hand.DoraIndicators);
        var uraDoraCount = CountDora(hand.AllTiles, hand.UraDoraIndicators);
        var redDora = CountRedDora(hand.AllTiles);

        return new DoraInfo { RedDora = redDora, Dora = doraCount, UraDora = uraDoraCount };
    }

    public static int CountDora(Tile[] tiles, List<Tile> indicators) {
        if (indicators.Count == 0) {
            return 0;
        }

        var count = 0;

        foreach (var tile in tiles) {
            foreach (var t in indicators) {
                var dora = Tile.GetNextTile(t);
                if (tile.EqualsIgnoreColor(dora)) {
                    count++;
                }
            }
        }

        return count;
    }

    public static int CountRedDora(Tile[] tiles) {
        return tiles.Count(tile => tile.IsRed);
    }
}
