// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain;

using System.Collections.Generic;

public record HandInfo {
    public Tile[] HandTiles { get; }
    public Tile WinningTile { get; }
    public List<Meld> OpenMelds { get; }
    public Tile[] AllTiles { get; }

    public HandInfo(Tile[] handTiles, Tile winningTile, List<Meld> openMelds) {
        HandTiles = handTiles;
        WinningTile = winningTile;
        OpenMelds = openMelds;

        AllTiles = InitAllTiles();
    }

    private Tile[] InitAllTiles() {
        var list = new List<Tile>();
        list.AddRange(HandTiles);
        list.Add(WinningTile);

        foreach (var meld in OpenMelds) {
            list.AddRange(meld.Tiles);
        }

        list.Sort();
        return list.ToArray();
    }
}
