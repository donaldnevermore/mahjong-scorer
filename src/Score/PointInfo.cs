// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Util;

namespace MahjongScorer.Score {
    public class PointInfo : IComparable<PointInfo> {
        public DealerTsumoPoint? DealerTsumo { get; private set; } = null;
        public NonDealerTsumoPoint? NonDealerTsumo { get; private set; } = null;
        public RonPoint? Ron { get; private set; } = null;

        public int BasePoints { get; private set; } = 0;
        public int Han { get; private set; } = 0;
        public int Fu { get; private set; } = 0;
        public bool HasYakuman { get; private set; } = false;
        public int YakumanTimes { get; private set; } = 0;

        public IList<YakuValue> YakuList { get; }
        public IList<FuValue> FuList { get; }

        private HandConfig? hand = null;
        private RoundConfig? round = null;

        // Yaku base points.
        private const int Mangan = 2000;
        private const int Haneman = 3000;
        private const int Baiman = 4000;
        private const int Sanbaiman = 6000;
        private const int Yakuman = 8000;
        private const int AccumulatedYakuman = 8000;
        private const int YakumanBaseHan = 13;

        public PointInfo() {
            YakuList = new List<YakuValue>();
            FuList = new List<FuValue>();
        }

        public PointInfo(IList<YakuValue> yakuList, IList<FuValue> fuList, HandConfig hand, RoundConfig round) {
            YakuList = yakuList;
            FuList = fuList;
            this.hand = hand;
            this.round = round;
        }

        public void CountHanAndFu() {
            if (YakuList.Count == 0) {
                return;
            }
            if (hand is null || round is null) {
                return;
            }

            var hanWithoutDoraAndYakuman = 0;

            foreach (var yaku in YakuList) {
                if (yaku.IsYakuman) {
                    HasYakuman = true;
                    YakumanTimes += yaku.Value;
                }
                else {
                    hanWithoutDoraAndYakuman += yaku.Value;
                }
            }

            if (HasYakuman) {
                // 6 times Yakuman at most.
                YakumanTimes = Math.Min(6, YakumanTimes);
                BasePoints = Yakuman * YakumanTimes;
                Han = YakumanTimes * YakumanBaseHan;
            }
            else {
                Han = hanWithoutDoraAndYakuman + hand.DoraInfo.TotalDora;
                if (Han >= 13) {
                    // 13 Han at most.
                    Han = 13;
                    BasePoints = AccumulatedYakuman;
                }
                else if (Han >= 11) {
                    BasePoints = Sanbaiman;
                }
                else if (Han >= 8) {
                    BasePoints = Baiman;
                }
                else if (Han >= 6) {
                    BasePoints = Haneman;
                }
                else if (Han >= 5) {
                    BasePoints = Mangan;
                }
                else {
                    Fu = FuCalculator.CountFu(FuList);
                    // Calculate base points.
                    var point = Fu * (int)Math.Pow(2, Han + 2);
                    BasePoints = Math.Min(Mangan, point);
                }
            }

            if (round.IsDealer) {
                if (hand.Tsumo) {
                    var n = NumberUtil.RoundUpToNextUnit(BasePoints * 2, 100);
                    DealerTsumo = new(n, round.Honba, round.RiichiBets);
                }
                else {
                    var n = NumberUtil.RoundUpToNextUnit(BasePoints * 6, 100);
                    Ron = new(n, round.Honba, round.RiichiBets);
                }
            }
            else {
                if (hand.Tsumo) {
                    var a = NumberUtil.RoundUpToNextUnit(BasePoints * 2, 100);
                    var b = NumberUtil.RoundUpToNextUnit(BasePoints, 100);
                    NonDealerTsumo = new(a, b, round.Honba, round.RiichiBets);
                }
                else {
                    var n = NumberUtil.RoundUpToNextUnit(BasePoints * 4, 100);
                    Ron = new(n, round.Honba, round.RiichiBets);
                }
            }
        }

        public override string ToString() {
            var yakuDetail = YakuList.Count == 0
                ? ""
                : string.Join(", ", YakuList.Select(yaku => yaku.ToString()));

            var fuDetail = FuList.Count == 0
                ? ""
                : string.Join(", ", FuList.Select(fu => fu.ToString()));

            return $"Han = {Han}, Fu = {Fu}, BasePoints = {BasePoints},\n" +
                $"Dora = {hand?.DoraInfo}, \nYaku = [{yakuDetail}], \nFu = [{fuDetail}]";
        }

        public int CompareTo(PointInfo? other) {
            if (other is null) {
                throw new ArgumentException(nameof(other));
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
}
