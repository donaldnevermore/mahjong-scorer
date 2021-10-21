// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongSharp.Score {
    public record NonDealerTsumoPoint {
        public int DealerBasePay { get; } = 0;
        public int NonDealerBasePay { get; } = 0;
        public int BaseGain => DealerBasePay + NonDealerBasePay * 2;
        public int TotalDealerPay => DealerBasePay + honba * 100;
        public int TotalNonDealerPay => NonDealerBasePay + honba * 100;
        public int TotalGain => BaseGain + riichiBets * 1000 + honba * 300;

        private int honba = 0;
        private int riichiBets = 0;

        public NonDealerTsumoPoint(int dealerBasePay, int nonDealerBasePay, int honba, int riichiBets) {
            DealerBasePay = dealerBasePay;
            NonDealerBasePay = nonDealerBasePay;
            this.honba = honba;
            this.riichiBets = riichiBets;
        }

        public override string ToString() {
            return $"BaseGain = {BaseGain}, DealerBasePay = {DealerBasePay}, NonDealerBasePay = {NonDealerBasePay}" +
                $"TotalGain = {TotalGain}, TotalDealerPay = {TotalDealerPay}, TotalNonDealerPay = {TotalNonDealerPay}";
        }
    }
}
