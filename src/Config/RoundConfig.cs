// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using MahjongScorer.Domain;

namespace MahjongScorer.Config {
    public class RoundConfig {
        public Honor RoundWind { get; set; } = Honor.East;
        public Honor SeatWind { get; set; } = Honor.East;
        public int Honba { get; set; } = 0;
        public int RiichiBets { get; set; } = 0;
        public bool IsDealer => SeatWind == Honor.East;
    }
}
