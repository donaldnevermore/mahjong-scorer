// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MahjongScorer.Domain {
    public record HandInfo {
        public IList<Tile> HandTiles { get; init; }
        public Tile WinningTile { get; init; }
        public IList<Meld> OpenMelds { get; init; }

        public List<Tile> AllTiles() {
            var list = new List<Tile>();
            list.AddRange(HandTiles);
            list.Add(WinningTile);

            foreach (var meld in OpenMelds) {
                list.AddRange(meld.Tiles);
            }

            list.Sort();

            return list;
        }
    }
}
