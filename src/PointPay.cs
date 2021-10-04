using System.Collections.Generic;

namespace MahjongSharp {
    public struct PointPay {
        public bool IsDealer { get; init; }
        public bool IsTsumo { get; init; }

        /// <summary>
        /// Pay without riichi bets and honba.
        /// </summary>
        public int BaseGain { get; init; }

        public int TotalGain { get; init; }

        public int PayOnOne { get; init; }
        public int BasePayOnOne { get; init; }
        public int PayOnAll { get; init; }
        public int BasePayOnAll { get; init; }

        public int DealerPay { get; init; }
        public int DealerBasePay { get; init; }

        public int NonDealerPay { get; init; }
        public int NonDealerBasePay { get; init; }

        public override string ToString() {
            return $"BaseGain = {BaseGain}, BasePayOnOne = {BasePayOnOne}, BasePayOnAll = {BasePayOnAll}, " +
                $"DealerBasePay = {DealerBasePay}, NonDealerBasePay = {NonDealerBasePay}";
        }
    }
}
