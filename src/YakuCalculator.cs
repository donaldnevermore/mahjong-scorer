// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MahjongScorer.Domain;
using MahjongScorer.Config;
using MahjongScorer.Util;

public class YakuCalculator {
    private readonly List<Meld> decompose;
    private readonly HandInfo hand;
    private readonly HandConfig handConfig;
    private readonly RoundConfig round;
    private readonly RuleConfig rule;
    private readonly List<YakuValue> result = new();

    public YakuCalculator(List<Meld> decompose, HandInfo hand, HandConfig handConfig,
        RoundConfig round, RuleConfig rule) {
        this.decompose = decompose;
        this.hand = hand;
        this.handConfig = handConfig;
        this.round = round;
        this.rule = rule;
    }

    public List<YakuValue> GetYakuList() {
        if (decompose.Count == 0) {
            result.Clear();
            return result;
        }

        if (result.Count != 0) {
            return result;
        }

        CountYaku();

        return result;
    }

    private void CountYaku() {
        RiichiAndIppatsu();
        Tanyao();
        MenzenchinTsumo();
        Yakuhai();
        Pinfu();
        PureDoubleSequence();
        RobbingKan();
        AfterKan();
        Under();
        TripleTriplets();
        MixedTripleSequence();
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
    /// Riichi or Double Riichi.
    /// </summary>
    private void RiichiAndIppatsu() {
        if (!handConfig.Menzenchin) {
            return;
        }

        switch (handConfig.Riichi) {
        case RiichiStatus.Riichi:
            result.Add(new YakuValue(YakuType.Riichi, 1));
            Ippatsu();
            break;
        case RiichiStatus.DoubleRiichi:
            result.Add(new YakuValue(YakuType.DoubleRiichi, 2));
            Ippatsu();
            break;
        default:
            break;
        }
    }

    /// <summary>
    /// After Riichi, you Tsumo or Ron before next time you draw a tile without anyone making a call.
    /// </summary>
    private void Ippatsu() {
        if (handConfig.Ippatsu) {
            result.Add(new YakuValue(YakuType.Ippatsu, 1));
        }
    }

    /// <summary>
    /// Menzenchin Tsumo
    /// </summary>
    private void MenzenchinTsumo() {
        if (handConfig.Menzenchin && handConfig.Tsumo) {
            result.Add(new YakuValue(YakuType.MenzenchinTsumo, 1));
        }
    }

    /// <summary>
    /// 4 sequences + 1 non-Yakuhai pair, and the winning tile must be double-sided.
    /// </summary>
    private void Pinfu() {
        if (!handConfig.Menzenchin) {
            return;
        }

        if (IsPinfuLike(decompose, hand.WinningTile, round)) {
            result.Add(new YakuValue(YakuType.Pinfu, 1));
        }
    }

    public static bool IsPinfuLike(List<Meld> decompose, Tile winningTile, RoundConfig round) {
        var sequenceCount = 0;
        var doubleSided = false;

        foreach (var meld in decompose) {
            switch (meld.Type) {
            case MeldType.Pair:
                if (IsYakuhai(meld, round)) {
                    return false;
                }
                break;
            case MeldType.Sequence:
                sequenceCount++;
                if (meld.IsDoubleSidedIgnoreColor(winningTile)) {
                    doubleSided = true;
                }
                break;
            default:
                return false;
            }
        }

        return sequenceCount == 4 && doubleSided;
    }

    /// <summary>
    /// Seat Wind, Round Wind, and Dragon tiles.
    /// </summary>
    private void Yakuhai() {
        foreach (var meld in decompose) {
            if (!meld.IsHonor || !meld.IsTripletLike()) {
                continue;
            }

            var rank = meld.Tiles[0].Rank;

            if (rank == 5) {
                result.Add(new YakuValue(YakuType.DragonWhite, 1));
            }
            if (rank == 6) {
                result.Add(new YakuValue(YakuType.DragonGreen, 1));
            }
            if (rank == 7) {
                result.Add(new YakuValue(YakuType.DragonRed, 1));
            }
            if (rank == (int)round.SeatWind) {
                result.Add(new YakuValue(YakuType.SeatWind, 1));
            }
            if (rank == (int)round.RoundWind) {
                result.Add(new YakuValue(YakuType.RoundWind, 1));
            }
        }
    }

    public static bool IsYakuhai(Meld meld, RoundConfig round) {
        if (!meld.IsHonor) {
            return false;
        }

        var rank = meld.Tiles[0].Rank;

        if (rank >= 5 && rank <= 7) {
            return true;
        }
        if (rank == (int)round.SeatWind) {
            return true;
        }
        if (rank == (int)round.RoundWind) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// All simples, excluding Yaochuu tiles.
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
    private void AfterKan() {
        if (handConfig.AfterKan) {
            result.Add(new YakuValue(YakuType.AfterKan, 1));
        }
    }

    /// <summary>
    /// Under the Sea or River.
    /// </summary>
    private void Under() {
        if (!handConfig.Under) {
            return;
        }

        if (handConfig.Tsumo) {
            result.Add(new YakuValue(YakuType.UnderTheSea, 1));
        }
        else {
            result.Add(new YakuValue(YakuType.UnderTheRiver, 1));
        }
    }

    /// <summary>
    /// Robbing a Kan.
    /// </summary>
    private void RobbingKan() {
        if (handConfig.RobbingKan) {
            result.Add(new YakuValue(YakuType.RobbingKan, 1));
        }
    }

    /// <summary>
    /// 7 different pairs.
    /// </summary>
    private void SevenPairs() {
        if (!handConfig.Menzenchin) {
            return;
        }
        if (decompose.Count != 7) {
            return;
        }
        if (decompose.Any(meld => meld.Type != MeldType.Pair)) {
            return;
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
                result.Add(new YakuValue(YakuType.PureStraight, handConfig.Menzenchin ? 2 : 1));
                return;
            }
            handFlag >>= 9;
        }
    }

    /// <summary>
    /// Triple Triplets, e.g. 111m111p111s.
    /// </summary>
    private void TripleTriplets() {
        var list = new List<Tile>();

        foreach (var meld in decompose) {
            if (meld.IsHonor || !meld.IsTripletLike()) {
                continue;
            }

            var first = meld.Tiles[0];
            if (DifferentSuitSameRank(first, list)) {
                list.Add(first);
            }
        }

        if (list.Count == 3) {
            result.Add(new YakuValue(YakuType.TripleTriplets, 2));
        }
    }

    private static bool DifferentSuitSameRank(Tile tile, List<Tile> list) {
        foreach (var t in list) {
            if (tile.Suit == t.Suit || tile.Rank != t.Rank) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Mixed Triple Sequence, e.g. 123m123p123s.
    /// </summary>
    private void MixedTripleSequence() {
        var list = new List<Tile>();

        foreach (var meld in decompose) {
            if (meld.Type == MeldType.Sequence) {
                var first = meld.Tiles[0];
                if (DifferentSuitSameRank(first, list)) {
                    list.Add(first);
                }
            }
        }

        if (list.Count == 3) {
            result.Add(new YakuValue(YakuType.MixedTripleSequence, handConfig.Menzenchin ? 2 : 1));
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
            result.Add(new YakuValue(YakuType.HalfOutsideHand, handConfig.Menzenchin ? 2 : 1));
        }
        else {
            result.Add(new YakuValue(YakuType.FullyOutsideHand, handConfig.Menzenchin ? 3 : 2));
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
        if (!handConfig.Menzenchin) {
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
            meld.IsTripletLike())) {
            result.Add(new YakuValue(YakuType.AllTriplets, 2));
        }
    }

    /// <summary>
    /// 4 Concealed Triplets, 3 Concealed Triplets, or Single-wait 4 Concealed Triplets.
    /// </summary>
    private void ConcealedTriplets() {
        var nonOpenCount = 0;
        var containsWinningTile = false;

        foreach (var meld in decompose) {
            if (!meld.IsOpen && meld.IsTripletLike()) {
                nonOpenCount++;
                if (meld.ContainsIgnoreColor(hand.WinningTile)) {
                    containsWinningTile = true;
                }
            }
        }

        var ronTriplet = !handConfig.Tsumo && containsWinningTile;

        // If you have 4 non-open (including the one from Ron) triplets,
        // you must be in Menzenchin status.
        if (nonOpenCount == 4 && handConfig.Menzenchin) {
            if (ronTriplet) {
                result.Add(new YakuValue(YakuType.ThreeConcealedTriplets, 2));
            }
            else if (containsWinningTile) {
                result.Add(new YakuValue(YakuType.FourConcealedTriplets, 1, true));
            }
            else {
                result.Add(new YakuValue(YakuType.SingleWaitFourConcealedTriplets,
                    rule.DoubleYakuman ? 2 : 1, true));
            }
        }
        else if (nonOpenCount == 3 && !ronTriplet) {
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
                arr[0] = true;
                break;
            case Suit.P:
                arr[1] = true;
                break;
            case Suit.S:
                arr[2] = true;
                break;
            case Suit.Z:
                arr[3] = true;
                break;
            }
        }

        // How many suits it has.
        var count = arr.Count(b => b);
        if (count == 1) {
            if (arr[3]) {
                result.Add(new YakuValue(YakuType.AllHonors, 1, true));
            }
            else {
                result.Add(new YakuValue(YakuType.FullFlush, handConfig.Menzenchin ? 6 : 5));
            }
        }
        else if (count == 2 && arr[3]) {
            result.Add(new YakuValue(YakuType.HalfFlush, handConfig.Menzenchin ? 3 : 2));
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
            case MeldType.Quad:
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
        if (!handConfig.Tsumo || !handConfig.Menzenchin || !handConfig.Blessing) {
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
        if (!handConfig.Menzenchin) {
            return;
        }
        if (decompose.Count != 13) {
            return;
        }

        var pair = decompose.First(meld => meld.Type == MeldType.Pair);
        if (pair.ContainsIgnoreColor(hand.WinningTile)) {
            result.Add(new YakuValue(YakuType.ThirteenWaitThirteenOrphans,
                rule.DoubleYakuman ? 2 : 1, true));
        }
        else {
            result.Add(new YakuValue(YakuType.ThirteenOrphans, 1, true));
        }
    }

    /// <summary>
    /// 9 Gates or True 9 Gates.
    /// </summary>
    private void NineGates() {
        if (!handConfig.Menzenchin) {
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

        var isTrueNineGates = counts[hand.WinningTile.Rank - 1] == 2 || counts[hand.WinningTile.Rank - 1] == 4;
        if (isTrueNineGates) {
            result.Add(new YakuValue(YakuType.TrueNineGates,
                rule.DoubleYakuman ? 2 : 1, true));
        }
        else {
            result.Add(new YakuValue(YakuType.NineGates, 1, true));
        }
    }

    /// <summary>
    /// 4 Little Winds or 4 Big Winds.
    /// </summary>
    private void FourWinds() {
        const int Flag = 0b1111;
        var tripletFlag = 0;
        var pairFlag = 0;

        foreach (var meld in decompose) {
            if (!meld.IsHonor || meld.Tiles[0].Rank > 4) {
                continue;
            }

            switch (meld.Type) {
            case MeldType.Triplet:
            case MeldType.Quad:
                tripletFlag |= 1 << (meld.Tiles[0].Rank - 1);
                break;
            case MeldType.Pair:
                pairFlag |= 1 << (meld.Tiles[0].Rank - 1);
                break;
            }
        }

        if (tripletFlag == Flag) {
            result.Add(new YakuValue(YakuType.FourBigWinds,
                rule.DoubleYakuman ? 2 : 1, true));
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
        var greens = TileMaker.GreenTiles;

        for (var i = 0; i < counts.Length; i++) {
            if (!greens.Contains(i) && counts[i] > 0) {
                return;
            }
        }

        result.Add(new YakuValue(YakuType.AllGreen, 1, true));
    }
}
