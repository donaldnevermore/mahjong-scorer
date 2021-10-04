using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongSharp {
    public struct PointInfo : IComparable<PointInfo> {
        public int BasePoint { get; }
        public bool HasYakuman { get; }
        public int YakumanTimes { get; }
        public int Han { get; }

        public int Fu { get; }
        public YakuValue[] Yakus { get; }
        public int Dora { get; }
        public int UraDora { get; }
        public int RedDora { get; }
        public bool IsTsumo { get; }
        public bool IsDealer { get; }
        public int Honba { get; }
        public int RiichiBets { get; }

        public int TotalDora => Dora + UraDora + RedDora;

        public PointInfo(int fu, IList<YakuValue> yakuValues, int dora, int uraDora, int redDora, HandStatus hand,
            RoundStatus round) : this() {
            Fu = fu;
            Dora = dora;
            UraDora = uraDora;
            RedDora = redDora;
            IsDealer = round.IsDealer;
            IsTsumo = hand.Tsumo;
            Honba = round.Honba;
            RiichiBets = round.RiichiBets;

            var yakuArray = yakuValues.ToArray();
            Array.Sort(yakuArray);
            Yakus = yakuArray;

            if (Yakus.Length == 0) {
                return;
            }

            var hanWithoutDoraAndYakuman = 0;
            foreach (var yaku in Yakus) {
                if (yaku.Type == YakuType.Yakuman) {
                    HasYakuman = true;
                    YakumanTimes += yaku.Value;
                }
                else {
                    hanWithoutDoraAndYakuman += yaku.Value;
                }
            }

            // 6 times Yakuman at most.
            if (YakumanTimes >= 6) {
                YakumanTimes = 6;
            }

            if (HasYakuman) {
                BasePoint = MahjongConfig.Yakuman * YakumanTimes;
                Han = YakumanTimes * MahjongConfig.YakumanBaseHan;
            }
            else {
                Han = hanWithoutDoraAndYakuman + TotalDora;

                if (Han >= 13) {
                    // 13 Han max.
                    Han = 13;
                    BasePoint = MahjongConfig.AccumulatedYakuman;
                }
                else if (Han >= 11) {
                    BasePoint = MahjongConfig.Sanbaiman;
                }
                else if (Han >= 8) {
                    BasePoint = MahjongConfig.Baiman;
                }
                else if (Han >= 6) {
                    BasePoint = MahjongConfig.Haneman;
                }
                else if (Han >= 5) {
                    BasePoint = MahjongConfig.Mangan;
                }
                else {
                    // Calculate Fu number.
                    var point = Fu * (int)Math.Pow(2, Han + 2);
                    BasePoint = Math.Min(MahjongConfig.Mangan, point);
                }
            }
        }

        public PointPay GetTotalPoint() {
            var totalRiichiBets = RiichiBets * 1000;
            var totalHonba = Honba * 300;
            var eachHonba = Honba * 100;

            if (IsDealer) {
                if (IsTsumo) {
                    var basePayOnAll = Util.RoundUpToNextUnit(BasePoint * 2, 100);
                    return new PointPay {
                        IsDealer = IsDealer,
                        IsTsumo = IsTsumo,

                        BasePayOnAll = basePayOnAll,
                        PayOnAll = basePayOnAll + eachHonba,
                        BaseGain = basePayOnAll * 3,
                        TotalGain = basePayOnAll * 3 + totalRiichiBets + totalHonba
                    };
                }
                else {
                    var basePayOnOne = Util.RoundUpToNextUnit(BasePoint * 6, 100);
                    return new PointPay {
                        IsDealer = IsDealer,
                        IsTsumo = IsTsumo,

                        BasePayOnOne = basePayOnOne,
                        PayOnOne = basePayOnOne + totalHonba,
                        BaseGain = basePayOnOne,
                        TotalGain = basePayOnOne + totalRiichiBets + totalHonba
                    };
                }
            }
            else {
                if (IsTsumo) {
                    var dealerBasePay = Util.RoundUpToNextUnit(BasePoint * 2, 100);
                    var nonDealerBasePay = Util.RoundUpToNextUnit(BasePoint, 100);
                    return new PointPay {
                        IsDealer = IsDealer,
                        IsTsumo = IsTsumo,

                        DealerBasePay = dealerBasePay,
                        DealerPay = dealerBasePay + eachHonba,
                        NonDealerBasePay = nonDealerBasePay,
                        NonDealerPay = nonDealerBasePay + eachHonba,
                        BaseGain = dealerBasePay + nonDealerBasePay * 2,
                        TotalGain = dealerBasePay + nonDealerBasePay * 2 + totalRiichiBets + totalHonba
                    };
                }
                else {
                    var basePayOnOne = Util.RoundUpToNextUnit(BasePoint * 4, 100);
                    return new PointPay {
                        IsDealer = IsDealer,
                        IsTsumo = IsTsumo,

                        BasePayOnOne = basePayOnOne,
                        PayOnOne = basePayOnOne + totalHonba,
                        BaseGain = basePayOnOne,
                        TotalGain = basePayOnOne + totalRiichiBets + totalHonba
                    };
                }
            }
        }

        public override string ToString() {
            var yakus = Yakus.Length == 0 ? "" : string.Join(", ", Yakus.Select(yaku => yaku.ToString()));

            return $"Fu = {Fu}, Han = {Han}, Dora = {Dora}, UraDora = {UraDora}, RedDora = {RedDora}, " +
                $"Yaku = [{yakus}], BasePoint = {BasePoint}";
        }

        public int CompareTo(PointInfo other) {
            var basePointComparison = BasePoint.CompareTo(other.BasePoint);
            if (basePointComparison != 0) {
                return basePointComparison;
            }

            var hanComparison = Han.CompareTo(other.Han);
            if (hanComparison != 0) {
                return hanComparison;
            }

            return Fu.CompareTo(other.Fu);
        }
    }
}
