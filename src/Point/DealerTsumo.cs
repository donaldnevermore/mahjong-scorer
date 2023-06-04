// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Point;

public record DealerTsumo : PointInfo {
  public int BasePayOnAll { get; init; } = 0;
  public int BaseGain => BasePayOnAll * 3;

  public int TotalPay => BasePayOnAll + HonbaPayOnAll;
  public int TotalGain => BaseGain + ExtraGain;

  public override string ToString() {
    var extraDetail = ExtraGain > 0 ? $"(+{ExtraGain})" : "";
    var honbaDetail = HonbaPayOnAll > 0 ? $"(+{HonbaPayOnAll})" : "";

    return $"""
      {base.ToString()}
      DealerTsumo: {BaseGain}{extraDetail} - {BasePayOnAll}{honbaDetail} All
      """;
  }
}
