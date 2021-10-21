// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MahjongSharp.Domain;
using MahjongSharp.Util;

namespace MahjongSharp.Score {
    public class Decomposer {
        /// <summary>
        /// Divide hand tiles into 4 melds and 1 pair.
        /// </summary>
        public static ISet<List<Meld>> Decompose(HandInfo handInfo) {
            var handTiles = handInfo.HandTiles;
            var winningTile = handInfo.WinningTile;
            var openMelds = handInfo.OpenMelds;

            var decomposes = new HashSet<List<Meld>>(new MeldListEqualityComparer());

            // Check if the length of handTile is valid.
            if (handTiles.Count % 3 != 1) {
                return decomposes;
            }

            var handAndWinningTiles = new List<Tile>(handTiles) { winningTile };
            var hand = TileMaker.CountTiles(handAndWinningTiles);

            AnalyzeHand(hand, decomposes);

            if (decomposes.Count == 0) {
                return decomposes;
            }

            var result = new HashSet<List<Meld>>(new MeldListEqualityComparer());
            foreach (var meldList in decomposes) {
                if (openMelds.Count != 0) {
                    meldList.AddRange(openMelds);
                }
                meldList.Sort();
                result.Add(meldList);
            }

            return result;
        }

        /// <summary>

        /// </summary>
        /// <param name="decompose"></param>
        /// <param name="winningTile"></param>
        /// <returns></returns>
        public static bool WinningTileInOthers(IList<Meld> decompose, Tile winningTile) {
            return decompose.Any(meld =>
                !meld.IsOpen &&
                (meld.Type == MeldType.Pair || meld.Type == MeldType.Sequence) &&
                meld.ContainsIgnoreColor(winningTile));
        }

        private static void AnalyzeHand(int[] hand, HashSet<List<Meld>> decomposes) {
            AnalyzeNormal(hand, decomposes);
            Analyze7Pairs(hand, decomposes);
            Analyze13Orphans(hand, decomposes);
        }

        /// <summary>
        /// Analyze normal Hora.
        /// </summary>
        private static void AnalyzeNormal(int[] hand, HashSet<List<Meld>> result) {
            var suitCount = new int[4];
            for (var i = 0; i < 34; i++) {
                suitCount[i / 9] += hand[i];
            }

            // Remainder of each suit must be 0 or 2.
            var remainderOfZero = 0;
            var suitOfTwo = -1;
            for (var i = 0; i < suitCount.Length; i++) {
                if (suitCount[i] % 3 == 1) {
                    return;
                }
                if (suitCount[i] % 3 == 0) {
                    remainderOfZero++;
                }
                else if (suitCount[i] % 3 == 2) {
                    suitOfTwo = i;
                }
            }

            if (remainderOfZero != 3) {
                return;
            }

            FindCompleteForm(suitOfTwo, hand, result);
        }

        /// <summary>
        /// Find pairs, triplets, and sequences.
        /// </summary>
        private static void FindCompleteForm(int suitOfTwo, int[] hand, HashSet<List<Meld>> result) {
            var suit = (Suit)suitOfTwo;
            var start = suitOfTwo * 9;
            var end = suit == Suit.Z ? start + 7 : start + 9;

            for (var index = start; index < end; index++) {
                // This tile can form a pair.
                if (hand[index] >= 2) {
                    hand[index] -= 2;

                    var decomposes = new HashSet<List<Meld>>();

                    DecomposeCore(0, hand, new List<Meld>(), decomposes);

                    var pairTile = Tile.GetTile(index);
                    foreach (var meldList in decomposes) {
                        meldList.Add(new Meld(false, pairTile, pairTile));
                        meldList.Sort();
                        result.Add(meldList);
                    }

                    // Backtrack
                    hand[index] += 2;
                }
            }
        }

        private static void DecomposeCore(int index, int[] hand, IList<Meld> current, HashSet<List<Meld>> result) {
            // Outlet
            if (index == hand.Length) {
                var newList = new List<Meld>(current);
                newList.Sort();
                result.Add(newList);
                return;
            }

            // This tile doesn't exist in hand.
            if (hand[index] == 0) {
                DecomposeCore(index + 1, hand, current, result);
            }

            var tile = Tile.GetTile(index);

            // Find triplets.
            if (hand[index] >= 3) {
                hand[index] -= 3;
                current.Add(new Meld(false, tile, tile, tile));

                DecomposeCore(index, hand, current, result);

                // Backtrack
                current.RemoveAt(current.Count - 1);
                hand[index] += 3;
            }

            // Find sequences.
            if (tile.Suit != Suit.Z && tile.Rank <= 7 && hand[index + 1] > 0 && hand[index + 2] > 0) {
                hand[index]--;
                hand[index + 1]--;
                hand[index + 2]--;

                var nextTile = new Tile(tile.Suit, tile.Rank + 1);
                var next2Tile = new Tile(tile.Suit, tile.Rank + 2);
                current.Add(new Meld(false, tile, nextTile, next2Tile));

                DecomposeCore(index, hand, current, result);

                // Backtrack
                current.RemoveAt(current.Count - 1);
                hand[index]++;
                hand[index + 1]++;
                hand[index + 2]++;
            }
        }

        private static void Analyze7Pairs(int[] hand, HashSet<List<Meld>> result) {
            if (hand.Sum() != 14) {
                return;
            }

            var meldList = new List<Meld>();

            for (var index = 0; index < hand.Length; index++) {
                // Each tile number must be 0 or 2.
                if (hand[index] != 0 && hand[index] != 2) {
                    return;
                }

                if (hand[index] == 2) {
                    var tile = Tile.GetTile(index);
                    meldList.Add(new Meld(false, tile, tile));
                }
            }

            meldList.Sort();
            result.Add(meldList);
        }

        /// <summary>
        /// Kokushi.
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="result"></param>
        private static void Analyze13Orphans(int[] hand, HashSet<List<Meld>> result) {
            if (hand.Sum() != 14) {
                return;
            }

            var meldList = new List<Meld>();
            var yaochuuKinds = 0;

            for (var i = 0; i < hand.Length; i++) {
                var tile = Tile.GetTile(i);
                if (!tile.IsYaochuu && hand[i] != 0) {
                    return;
                }
                if (tile.IsYaochuu && (hand[i] == 0 || hand[i] > 2)) {
                    return;
                }

                if (tile.IsYaochuu) {
                    yaochuuKinds++;

                    // Each yaochuu tile number must be 1 or 2.
                    if (hand[i] == 1) {
                        meldList.Add(new Meld(false, tile));
                    }
                    if (hand[i] == 2) {
                        meldList.Add(new Meld(false, tile, tile));
                    }
                }
            }

            if (yaochuuKinds != 13) {
                return;
            }

            meldList.Sort();
            result.Add(meldList);
        }

        private struct MeldListEqualityComparer : IEqualityComparer<IList<Meld>> {
            public bool Equals(IList<Meld>? x, IList<Meld>? y) {
                if (x is null && y is null) {
                    return true;
                }
                if (x is null || y is null) {
                    return false;
                }

                if (x.Count != y.Count) {
                    return false;
                }

                for (var i = 0; i < x.Count; i++) {
                    if (!x[i].Equals(y[i])) {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(IList<Meld> obj) {
                var hash = 0;
                foreach (var meld in obj) {
                    hash = hash * 31 + meld.GetHashCode();
                }

                return hash;
            }
        }
    }
}
