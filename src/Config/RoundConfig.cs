// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using MahjongScorer.Domain;

namespace MahjongScorer.Config {
    public class RoundConfig {
        public Wind RoundWind { get; set; } = Wind.East;
        public Wind SeatWind { get; set; } = Wind.East;
        public int Honba { get; set; } = 0;
        public int RiichiBets { get; set; } = 0;

        public Tile RoundWindTile => new(Suit.Z, (int)RoundWind);
        public Tile SeatWindTile => new(Suit.Z, (int)SeatWind);
        public bool IsDealer => SeatWind == Wind.East;
    }
}
