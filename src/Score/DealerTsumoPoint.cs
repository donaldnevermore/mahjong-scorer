// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Score {
    public record DealerTsumoPoint {
        public int BasePayOnAll { get; } = 0;
        public int BaseGain => BasePayOnAll * 3;
        public int TotalPayOnAll => BasePayOnAll + honba * 100;
        public int TotalGain => BaseGain + riichiBets * 1000 + honba * 300;

        private int honba = 0;
        private int riichiBets = 0;

        public DealerTsumoPoint(int n, int honba, int riichiBets) {
            BasePayOnAll = n;
            this.honba = honba;
            this.riichiBets = riichiBets;
        }

        public override string ToString() {
            return $"BaseGain = {BaseGain}, BasePayOnAll = {BasePayOnAll}, " +
                $"TotalGain = {TotalGain}, TotalPayOnAll = {TotalPayOnAll}";
        }
    }
}
