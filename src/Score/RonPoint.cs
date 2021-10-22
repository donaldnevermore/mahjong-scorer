// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Score {
    public record RonPoint {
        public int BasePayOnOne { get; } = 0;
        public int BaseGain => BasePayOnOne;
        public int TotalPayOnOne => BasePayOnOne + honba * 300;
        public int TotalGain => BaseGain + riichiBets * 1000 + honba * 300;

        private int honba = 0;
        private int riichiBets = 0;

        public RonPoint(int n, int honba, int riichiBets) {
            BasePayOnOne = n;
            this.honba = honba;
            this.riichiBets = riichiBets;
        }

        public override string ToString() {
            return $"BaseGain = {BaseGain}, BasePayOnOne = {BasePayOnOne}, " +
                $"TotalGain = {TotalGain}, TotalPayOnOne = {TotalPayOnOne}";
        }
    }
}
