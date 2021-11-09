// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Config;

public class RuleConfig {
    public bool DoubleWindFu { get; set; } = true;
    public bool AccumulatedYakuman { get; set; } = true;
    public bool MultipleYakuman { get; set; } = true;
    public bool AllowDoubleYakuman { get; set; } = true;
    public bool RoundUpMangan { get; set; } = false;
}
