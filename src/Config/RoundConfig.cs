// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using MahjongScorer.Domain;

namespace MahjongScorer.Config {
    public class RoundConfig {
        private Wind roundWind = Wind.East;
        public Wind RoundWind {
            get => roundWind;
            set {
                if (value == Wind.North) {
                    throw new ArgumentException("The round wind cannot be north.");
                }
                roundWind = value;
            }
        }

        public Wind SeatWind { get; set; } = Wind.East;
        public int Honba { get; set; } = 0;
        public int RiichiBets { get; set; } = 0;
        public bool IsDealer => SeatWind == Wind.East;
    }
}
