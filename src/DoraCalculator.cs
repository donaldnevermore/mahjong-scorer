// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Linq;
using MahjongScorer.Domain;

namespace MahjongScorer {
    public class DoraCalculator {
        public static DoraInfo GetAllDora(Tile[] handTiles, Tile[] doraTiles, Tile[] uraDoraTiles) {
            var doraCount = CountDora(handTiles, doraTiles);
            var uraDoraCount = CountDora(handTiles, uraDoraTiles);
            var redDora = CountRedDora(handTiles);

            return new DoraInfo { Dora = doraCount, UraDora = uraDoraCount, RedDora = redDora };
        }

        public static int CountDora(Tile[] tiles, Tile[] doraTiles) {
            if (doraTiles.Length == 0) {
                return 0;
            }

            var count = 0;

            foreach (var tile in tiles) {
                foreach (var dora in doraTiles) {
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
}
