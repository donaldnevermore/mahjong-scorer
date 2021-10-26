// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace MahjongScorer.Domain {
    public record HandInfo {
        public IList<Tile> HandTiles { get; }
        public Tile WinningTile { get; }
        public IList<Meld> OpenMelds { get; }
        public IList<Tile> AllTiles { get; }

        public HandInfo(IList<Tile> handTiles, Tile winningTile, IList<Meld> openMelds) {
            HandTiles = handTiles;
            WinningTile = winningTile;
            OpenMelds = openMelds;

            AllTiles = InitAllTiles();
        }

        public int RedDora => AllTiles.Count(t => t.IsRed);

        private List<Tile> InitAllTiles() {
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
