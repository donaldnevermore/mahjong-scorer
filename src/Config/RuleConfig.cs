// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Config {
    public class RuleConfig {
        /// <summary>
        /// Must be 2 or 4.
        /// </summary>
        public int DoubleWindFu { get; set; } = 4;

        public bool AccumulatedYakuman { get; set; } = true;
        public bool MultipleYakuman { get; set; } = true;

        /// <summary>
        /// Must be 1 or 2.
        /// </summary>
        public int DoubleYakumanValue { get; set; } = 2;

        public bool RoundUpMangan { get; set; } = false;
    }
}
