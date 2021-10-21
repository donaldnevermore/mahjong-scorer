// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MahjongSharp.Domain;
using MahjongSharp.Util;

namespace MahjongSharp.Score {
    public class YakuCalculator {
        private readonly IList<Meld> decompose;
        private readonly Tile winningTile;
        private readonly HandConfig hand;
        private readonly RoundConfig round;
        private readonly RuleConfig rule;
        private readonly List<YakuValue> result;

        public YakuCalculator(IList<Meld> decompose, Tile winningTile, HandConfig hand, RoundConfig round,
            RuleConfig rule) {
            this.decompose = decompose;
            this.winningTile = winningTile;
            this.hand = hand;
            this.round = round;
            this.rule = rule;
            result = new List<YakuValue>();
        }

        public IList<YakuValue> GetYakuList() {
            if (decompose.Count == 0) {
                return result;
            }
            if (result.Count != 0) {
                return result;
            }

            CountYaku();

            var hasYakuman = result.Any(yakuValue => yakuValue.IsYakuman);
            return hasYakuman
                ? result.Where(yakuValue => yakuValue.IsYakuman).ToList()
                : result;
        }

        private void CountYaku() {
            Riichi();
            Tanyao();
            MenzenchinTsumo();
            Yakuhai();
            Pinfu();
            PureDoubleSequence();
            RobbingAKan();
            AfterAKan();
            Under();
            Ippatsu();
            Triple();
            Quads();
            AllTriplets();
            SevenPairs();
            PureStraight();
            OutsideHand();
            AllTerminals();
            ConcealedTriplets();
            FlushOrAllHonors();
            ThreeDragons();
            Blessing();
            ThirteenOrphans();
            NineGates();
            FourWinds();
            AllGreen();
        }

        /// <summary>
        /// Riichi or DoubleRiichi.
        /// </summary>
        private void Riichi() {
            if (!hand.Menzenchin || hand.Riichi == RiichiStatus.None) {
                return;
            }

            if (hand.Riichi == RiichiStatus.Riichi) {
                result.Add(new YakuValue(YakuType.Riichi, 1));
            }
            else if (hand.Riichi == RiichiStatus.DoubleRiichi) {
                result.Add(new YakuValue(YakuType.DoubleRiichi, 2));
            }
        }

        /// <summary>
        /// One Shot.
        /// </summary>
        private void Ippatsu() {
            var menzenchinRiichi = hand.Menzenchin && hand.Riichi != RiichiStatus.None;
            if (menzenchinRiichi && hand.Ippatsu) {
                result.Add(new YakuValue(YakuType.Ippatsu, 1));
            }
        }

        /// <summary>
        /// Menzenchin Tsumo
        /// </summary>
        private void MenzenchinTsumo() {
            if (hand.Menzenchin && hand.Tsumo) {
                result.Add(new YakuValue(YakuType.MenzenchinTsumo, 1));
            }
        }

        /// <summary>
        /// 4 Sequences + 1 non-Yakuhai pair, and the winning tile must be double-sided.
        /// </summary>
        private void Pinfu() {
            if (!hand.Menzenchin) {
                return;
            }

            if (IsPinfuType(decompose, winningTile)) {
                result.Add(new YakuValue(YakuType.Pinfu, 1));
            }
        }

        public static bool IsPinfuType(IList<Meld> decompose, Tile winningTile) {
            var sequenceCount = 0;
            var doubleSided = false;

            foreach (var meld in decompose) {
                if (meld.Type != MeldType.Pair || meld.Type != MeldType.Sequence) {
                    return false;
                }

                if (meld.Type == MeldType.Sequence) {
                    sequenceCount++;
                    doubleSided = doubleSided || meld.IsDoubleSidedIgnoreColor(winningTile);
                }
            }

            return sequenceCount == 4 && doubleSided;
        }

        /// <summary>
        /// Seat Wind, Round Wind, and Dragon tiles.
        /// </summary>
        private void Yakuhai() {
            var seatWind = round.SeatWindTile;
            var roundWind = round.RoundWindTile;

            foreach (var meld in decompose) {
                if (meld.Type != MeldType.Triplet || meld.Type != MeldType.Quad) {
                    continue;
                }

                if (meld.Tiles[0].EqualsIgnoreColor(seatWind)) {
                    result.Add(new YakuValue(YakuType.SeatWind, 1));
                }
                if (meld.Tiles[0].EqualsIgnoreColor(roundWind)) {
                    result.Add(new YakuValue(YakuType.RoundWind, 1));
                }
                if (meld.Tiles[0].EqualsIgnoreColor(new Tile(Suit.Z, 5))) {
                    result.Add(new YakuValue(YakuType.DragonWhite, 1));
                }
                if (meld.Tiles[0].EqualsIgnoreColor(new Tile(Suit.Z, 6))) {
                    result.Add(new YakuValue(YakuType.DragonGreen, 1));
                }
                if (meld.Tiles[0].EqualsIgnoreColor(new Tile(Suit.Z, 7))) {
                    result.Add(new YakuValue(YakuType.DragonRed, 1));
                }
            }
        }

        /// <summary>
        /// All simples.
        /// </summary>
        private void Tanyao() {
            foreach (var meld in decompose) {
                if (meld.HasYaochuu) {
                    return;
                }
            }

            result.Add(new YakuValue(YakuType.Tanyao, 1));
        }

        /// <summary>
        /// After a Kan.
        /// </summary>
        private void AfterAKan() {
            if (hand.AfterAKan) {
                result.Add(new YakuValue(YakuType.AfterAKan, 1));
            }
        }

        /// <summary>
        /// Under the Sea or River.
        /// </summary>
        private void Under() {
            if (!hand.Under) {
                return;
            }

            if (hand.Tsumo) {
                result.Add(new YakuValue(YakuType.UnderTheSea, 1));
            }
            else {
                result.Add(new YakuValue(YakuType.UnderTheRiver, 1));
            }
        }

        /// <summary>
        /// Robbing a Kan.
        /// </summary>
        private void RobbingAKan() {
            if (hand.RobbingAKan) {
                result.Add(new YakuValue(YakuType.RobbingAKan, 1));
            }
        }

        /// <summary>
        /// 7 pairs.
        /// </summary>
        private void SevenPairs() {
            if (!hand.Menzenchin) {
                return;
            }
            if (decompose.Count != 7) {
                return;
            }

            foreach (var meld in decompose) {
                if (meld.Type != MeldType.Pair) {
                    return;
                }
            }

            result.Add(new YakuValue(YakuType.SevenPairs, 2));
        }

        /// <summary>
        /// Pure Straight, e.g. 123456789m
        /// </summary>
        private void PureStraight() {
            const int Flag = 0b1001001;
            var handFlag = 0;

            foreach (var meld in decompose) {
                if (meld.Type == MeldType.Sequence) {
                    handFlag |= 1 << Tile.GetIndex(meld.Tiles[0]);
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            while (handFlag > 0) {
                if ((handFlag & Flag) == Flag) {
                    result.Add(new YakuValue(YakuType.PureStraight, hand.Menzenchin ? 2 : 1));
                    return;
                }
                handFlag >>= 9;
            }
        }

        /// <summary>
        /// Triple Triplets, e.g. 111m111p111s,
        /// and Mixed Triple Sequence, e.g. 123m123p123s.
        /// </summary>
        private void Triple() {
            const int Flag = 0b1000000001000000001;
            var tripletFlag = 0;
            var sequenceFlag = 0;

            foreach (var meld in decompose) {
                if (meld.Type == MeldType.Sequence) {
                    sequenceFlag |= 1 << Tile.GetIndex(meld.Tiles[0]);
                }
                if ((meld.Type == MeldType.Triplet || meld.Type == MeldType.Quad) && !meld.IsHonor) {
                    tripletFlag |= 1 << Tile.GetIndex(meld.Tiles[0]);
                }
            }

            Debug.Assert(tripletFlag >= 0 && sequenceFlag >= 0,
                "Only 27 flag bits, this number should not be less than 0.");

            for (var i = 0; i < 9; i++) {
                if ((tripletFlag & Flag) == Flag) {
                    result.Add(new YakuValue(YakuType.TripleTriplets, 2));
                    return;
                }
                tripletFlag >>= 1;

                if ((sequenceFlag & Flag) == Flag) {
                    result.Add(new YakuValue(YakuType.MixedTripleSequence, hand.Menzenchin ? 2 : 1));
                    return;
                }
                sequenceFlag >>= 1;
            }
        }

        /// <summary>
        /// Half Outside Hand or Fully Outside Hand.
        /// </summary>
        private void OutsideHand() {
            var hasHonor = false;

            foreach (var meld in decompose) {
                if (!meld.HasYaochuu) {
                    return;
                }

                if (meld.IsHonor) {
                    hasHonor = true;
                }
            }

            if (hasHonor) {
                result.Add(new YakuValue(YakuType.HalfOutsideHand, hand.Menzenchin ? 2 : 1));
            }
            else {
                result.Add(new YakuValue(YakuType.FullyOutsideHand, hand.Menzenchin ? 3 : 2));
            }
        }

        /// <summary>
        /// All Terminals & Honors or All Terminals.
        /// </summary>
        private void AllTerminals() {
            var hasHonor = false;

            foreach (var meld in decompose) {
                if (!meld.IsYaochuu) {
                    return;
                }

                if (meld.IsHonor) {
                    hasHonor = true;
                }
            }

            if (hasHonor) {
                result.Add(new YakuValue(YakuType.AllTerminalsAndHonors, 2));
            }
            else {
                result.Add(new YakuValue(YakuType.AllTerminals, 1, true));
            }
        }

        /// <summary>
        /// Pure Double Sequence or Twice Pure Double Sequence.
        /// </summary>
        private void PureDoubleSequence() {
            if (!hand.Menzenchin) {
                return;
            }

            var handFlag = 0;
            var count = 0;

            foreach (var meld in decompose) {
                if (meld.Type == MeldType.Sequence) {
                    var tileFlag = 1 << Tile.GetIndex(meld.Tiles[0]);
                    if ((handFlag & tileFlag) != 0) {
                        count++;

                        // Toggle that bit, thus making it 0 again.
                        handFlag ^= tileFlag;
                    }
                    else {
                        handFlag |= tileFlag;
                    }
                }
            }

            Debug.Assert(handFlag >= 0, "Only 27 flag bits, this number should not be less than 0.");

            if (count == 2) {
                result.Add(new YakuValue(YakuType.TwicePureDoubleSequence, 3));
            }
            else if (count == 1) {
                result.Add(new YakuValue(YakuType.PureDoubleSequence, 1));
            }
        }

        /// <summary>
        /// All Triplets.
        /// </summary>
        private void AllTriplets() {
            var countPairs = decompose.Count(meld => meld.Type == MeldType.Pair);
            if (countPairs != 1) {
                return;
            }

            if (decompose.All(meld => meld.Type == MeldType.Pair ||
                meld.Type == MeldType.Triplet ||
                meld.Type == MeldType.Quad)) {
                result.Add(new YakuValue(YakuType.AllTriplets, 2));
            }
        }

        /// <summary>
        /// 4 Concealed Triplets, 3 Concealed Triplets, or Single-wait 4 Concealed Triplets.
        /// </summary>
        private void ConcealedTriplets() {
            var count = decompose.Count(meld =>
                !meld.IsOpen && (meld.Type == MeldType.Triplet || meld.Type == MeldType.Quad));

            Debug.Assert(count <= 4, "There could not be more than 4 triplets in a complete hand.");

            // Winning tile in Triplet or Quad.
            var containsWinningTile = decompose.Any(meld =>
                !meld.IsOpen &&
                (meld.Type == MeldType.Triplet || meld.Type == MeldType.Quad) &&
                meld.ContainsIgnoreColor(winningTile));
            var hasOneOpen = !hand.Tsumo && containsWinningTile;

            if (count == 4) {
                if (hasOneOpen) {
                    result.Add(new YakuValue(YakuType.ThreeConcealedTriplets, 2));
                }
                else if (containsWinningTile) {
                    result.Add(new YakuValue(YakuType.FourConcealedTriplets, 1, true));
                }
                else {
                    result.Add(new YakuValue(YakuType.SingleWaitFourConcealedTriplets,
                        rule.DoubleYakumanValue, true));
                }
            }
            else if (count == 3 && !hasOneOpen) {
                result.Add(new YakuValue(YakuType.ThreeConcealedTriplets, 2));
            }
        }

        /// <summary>
        /// Half Flush, Full Flush, or All Honors.
        /// </summary>
        private void FlushOrAllHonors() {
            var arr = new bool[4];

            foreach (var meld in decompose) {
                switch (meld.Suit) {
                case Suit.M:
                    arr[(int)Suit.M] = true;
                    break;
                case Suit.P:
                    arr[(int)Suit.P] = true;
                    break;
                case Suit.S:
                    arr[(int)Suit.S] = true;
                    break;
                case Suit.Z:
                    arr[(int)Suit.Z] = true;
                    break;
                }
            }

            var count = arr.Count(b => b);
            if (count == 1) {
                if (arr[(int)Suit.Z]) {
                    result.Add(new YakuValue(YakuType.AllHonors, 1, true));
                }
                else {
                    result.Add(new YakuValue(YakuType.FullFlush, hand.Menzenchin ? 6 : 5));
                }
            }
            else if (count == 2 && arr[(int)Suit.Z]) {
                result.Add(new YakuValue(YakuType.HalfFlush, hand.Menzenchin ? 3 : 2));
            }
        }

        /// <summary>
        /// 3 Quads or 4 Quads.
        /// </summary>
        private void Quads() {
            var count = decompose.Count(meld => meld.Type == MeldType.Quad);
            if (count == 4) {
                result.Add(new YakuValue(YakuType.FourQuads, 1, true));
            }
            else if (count == 3) {
                result.Add(new YakuValue(YakuType.ThreeQuads, 2));
            }
        }

        /// <summary>
        /// Little 3 Dragons or Big 3 Dragons.
        /// </summary>
        private void ThreeDragons() {
            const int Flag = 0b111;
            var tripletFlag = 0;
            var pairFlag = 0;

            foreach (var meld in decompose) {
                if (!meld.IsHonor || meld.Tiles[0].Rank < 5) {
                    continue;
                }

                switch (meld.Type) {
                case MeldType.Triplet:
                    tripletFlag |= 1 << (meld.Tiles[0].Rank - 5);
                    break;
                case MeldType.Pair:
                    pairFlag |= 1 << (meld.Tiles[0].Rank - 5);
                    break;
                }
            }

            if (tripletFlag == Flag) {
                result.Add(new YakuValue(YakuType.BigThreeDragons, 1, true));
            }
            else if ((tripletFlag | pairFlag) == Flag && pairFlag != Flag) {
                result.Add(new YakuValue(YakuType.LittleThreeDragons, 2));
            }
        }

        /// <summary>
        /// Blessing of Heaven or Earth.
        /// </summary>
        private void Blessing() {
            if (!hand.Tsumo || !hand.Menzenchin || !hand.Blessing) {
                return;
            }

            if (round.IsDealer) {
                result.Add(new YakuValue(YakuType.BlessingOfHeaven, 1, true));
            }
            else {
                result.Add(new YakuValue(YakuType.BlessingOfEarth, 1, true));
            }
        }

        /// <summary>
        /// 13 Orphans or 13-wait 13 Orphans
        /// </summary>
        private void ThirteenOrphans() {
            if (decompose.Count != 13) {
                return;
            }

            var pair = decompose.First(meld => meld.Type == MeldType.Pair);
            if (pair.ContainsIgnoreColor(winningTile)) {
                result.Add(new YakuValue(YakuType.ThirteenWaitThirteenOrphans,
                    rule.DoubleYakumanValue, true));
            }
            else {
                result.Add(new YakuValue(YakuType.ThirteenOrphans, 1, true));
            }
        }

        /// <summary>
        /// 9 Gates or True 9 Gates.
        /// </summary>
        private void NineGates() {
            if (!hand.Menzenchin) {
                return;
            }

            var counts = new int[9];
            var first = decompose[0];

            foreach (var meld in decompose) {
                if (meld.IsHonor || meld.Suit != first.Suit) {
                    return;
                }

                foreach (var tile in meld.Tiles) {
                    counts[tile.Rank - 1]++;
                }
            }

            if (counts[0] < 3 || counts[8] < 3) {
                return;
            }

            for (var i = 1; i < 8; i++) {
                if (counts[i] < 1) {
                    return;
                }
            }

            var isTrueNineGates = counts[winningTile.Rank - 1] == 2 || counts[winningTile.Rank - 1] == 4;
            if (isTrueNineGates) {
                result.Add(new YakuValue(YakuType.TrueNineGates,
                    rule.DoubleYakumanValue, true));
            }
            else {
                result.Add(new YakuValue(YakuType.NineGates, 1, true));
            }
        }

        /// <summary>
        /// 4 Little Winds or 4 Big Winds.
        /// </summary>
        private void FourWinds() {
            const int Flag = 15;
            var tripletFlag = 0;
            var pairFlag = 0;

            foreach (var meld in decompose) {
                if (!meld.IsHonor || meld.Tiles[0].Rank > 4) {
                    continue;
                }

                switch (meld.Type) {
                case MeldType.Triplet:
                    tripletFlag |= 1 << (meld.Tiles[0].Rank - 1);
                    break;
                case MeldType.Pair:
                    pairFlag |= 1 << (meld.Tiles[0].Rank - 1);
                    break;
                }
            }

            if (tripletFlag == Flag) {
                result.Add(new YakuValue(YakuType.FourBigWinds,
                    rule.DoubleYakumanValue, true));
            }
            else if ((tripletFlag | pairFlag) == Flag && pairFlag != Flag) {
                result.Add(new YakuValue(YakuType.FourLittleWinds, 1, true));
            }
        }

        /// <summary>
        /// All Green.
        /// </summary>
        private void AllGreen() {
            var counts = TileMaker.CountMeldTiles(decompose);
            var green = TileMaker.GetGreenTiles();

            for (var i = 0; i < counts.Length; i++) {
                if (!green.Contains(i) && counts[i] > 0) {
                    return;
                }
            }

            result.Add(new YakuValue(YakuType.AllGreen, 1, true));
        }
    }
}
