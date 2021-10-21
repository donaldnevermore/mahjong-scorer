// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using MahjongSharp.Domain;

namespace MahjongSharp.Util {
    public static class HandMaker {
        public static HandInfo GetHandInfo(string handTiles, string winningTile, string[] openMelds) {
            var h = TileMaker.ConvertTiles(handTiles);
            var w = TileMaker.ConvertTiles(winningTile);
            var m = TileMaker.ConvertMelds(openMelds);
            return new HandInfo { HandTiles = h, WinningTile = w[0], OpenMelds = m };
        }
    }
}
