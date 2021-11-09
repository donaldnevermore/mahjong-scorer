// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Point;

using System;
using System.Collections.Generic;
using System.Linq;
using MahjongScorer.Domain;

public record PointInfo : IComparable<PointInfo> {
    public int BasePoints { get; init; } = 0;
    public int Han { get; init; } = 0;
    public int Fu { get; init; } = 0;
    public int YakumanCount { get; init; } = 0;

    public DoraInfo Dora { get; init; } = new();
    public int Honba { get; init; } = 0;
    public int RiichiBets { get; init; } = 0;

    public List<YakuValue> YakuList { get; init; } = new();
    public List<FuValue> FuList { get; init; } = new();

    public int ExtraGain => RiichiBets * 1000 + Honba * 300;
    public int HonbaPay => Honba * 300;
    public int HonbaPayOnAll => Honba * 100;

    public override string ToString() {
        var yakuDetail = YakuList.Count == 0
            ? ""
            : string.Join(", ", YakuList.Select(yaku => yaku.ToString()));

        var fuDetail = FuList.Count == 0
            ? ""
            : string.Join(", ", FuList.Select(fu => fu.ToString()));

        return $"Han = {Han}, Fu = {Fu}, BasePoints = {BasePoints},\n" +
            $"{Dora}\n" +
            $"Yaku = [{yakuDetail}]\nFu = [{fuDetail}]\n";
    }

    public int CompareTo(PointInfo? other) {
        if (other is null) {
            throw new ArgumentNullException(nameof(other));
        }

        var basePointsCompare = BasePoints.CompareTo(other.BasePoints);
        if (basePointsCompare != 0) {
            return basePointsCompare;
        }

        var hanCompare = Han.CompareTo(other.Han);
        if (hanCompare != 0) {
            return hanCompare;
        }

        return Fu.CompareTo(other.Fu);
    }
}
