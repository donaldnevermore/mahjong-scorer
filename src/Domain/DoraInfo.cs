﻿// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain {
    public class DoraInfo {
        public int Dora { get; set; } = 0;

        /// <summary>
        /// Dora turned only for Riichi.
        /// </summary>
        public int UraDora { get; set; } = 0;

        /// <summary>
        /// Red 5.
        /// </summary>
        public int RedDora { get; set; } = 0;

        public int TotalDora => Dora + UraDora + RedDora;

        public override string ToString() {
            return $"RedDora = {RedDora}, Dora = {Dora}, UraDora = {UraDora}";
        }
    }
}
