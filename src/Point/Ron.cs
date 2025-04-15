// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Point;

public record Ron : PointInfo {
    public int BasePay { get; init; } = 0;
    public int BaseGain => BasePay;

    public int TotalPayOnOne => BaseGain + HonbaPay;
    public int TotalGain => BaseGain + ExtraGain;

    public override string ToString() {
        var extraDetail = ExtraGain > 0 ? $"(+{ExtraGain})" : "";
        var honbaDetail = HonbaPay > 0 ? $"(+{HonbaPay})" : "";

        return $"""
      {base.ToString()},
      Ron: {BaseGain}{extraDetail} - {BasePay}{honbaDetail}
      """;
    }
}
