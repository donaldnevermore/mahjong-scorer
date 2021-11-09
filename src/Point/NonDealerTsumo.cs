// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Point;

public record NonDealerTsumo : PointInfo {
    public int NonDealerBasePay { get; init; } = 0;
    public int DealerBasePay { get; init; } = 0;
    public int BaseGain => DealerBasePay + NonDealerBasePay * 2;

    public int TotalDealerPay => DealerBasePay + HonbaPayOnAll;
    public int TotalNonDealerPay => NonDealerBasePay + HonbaPayOnAll;
    public int TotalGain => BaseGain + ExtraGain;

    public override string ToString() {
        var extraDetail = ExtraGain > 0 ? $"(+{ExtraGain})" : "";
        var honbaDetail = HonbaPay > 0 ? $"(+{HonbaPayOnAll})" : "";

        return base.ToString() +
            $"NonDealerTsumo: {BaseGain}{extraDetail} - {NonDealerBasePay}{honbaDetail}, " +
            $"{DealerBasePay}{honbaDetail}";
    }
}
