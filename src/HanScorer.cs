using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongSharp {
    public class HanScorer {
        public static PointInfo GetPointInfo(Tile[] handTiles, Meld[] openMelds, Tile winningTile,
            HandStatus handStatus, RoundStatus roundStatus, Ruleset ruleset,
            Tile[]? doraTiles = null, Tile[]? uraDoraTiles = null) {
            var decomposes = Decompose(handTiles, openMelds, winningTile);
            if (decomposes.Count == 0) {
                return new PointInfo();
            }
            // count dora
            var dora = CountDora(handTiles, openMelds, winningTile, doraTiles);
            var uraDora = 0;
            if (handStatus.Riichi || handStatus.DoubleRiichi) {
                uraDora = CountDora(handTiles, openMelds, winningTile, uraDoraTiles);
            }
            var redDora = CountRedDora(handTiles, openMelds, winningTile);

            return GetPointInfoWithDora(decomposes, winningTile, handStatus, roundStatus, ruleset, dora, uraDora,
                redDora);
        }

        public static PointInfo GetPointInfoWithDora(ISet<List<Meld>> decomposes, Tile winningTile,
            HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset, int dora, int uraDora, int redDora) {
            var infos = new List<PointInfo>();
            foreach (var decompose in decomposes) {
                var yakus = CountHan(decompose, winningTile, handStatus, roundStatus, ruleset);
                var fu = FuScorer.CountFu(decompose, winningTile, handStatus, roundStatus, yakus, ruleset);
                if (yakus.Count == 0) {
                    continue;
                }
                var info = new PointInfo(fu, yakus, dora, uraDora, redDora, handStatus, roundStatus);
                infos.Add(info);
            }

            if (infos.Count == 0) {
                return new PointInfo();
            }
            infos.Sort();
            return infos[^1];
        }

        /// <summary>
        /// Count Han, the main portion of scoring, as each yaku is assigned a value in terms of han.
        /// </summary>
        public static IList<YakuValue> CountHan(IList<Meld> decompose, Tile winningTile, HandStatus handStatus,
            RoundStatus roundStatus, Ruleset ruleset) {
            var result = new List<YakuValue>();
            if (decompose.Count == 0) {
                return result;
            }

            foreach (var yaku in YakuMethod.Methods) {
                var value = yaku(decompose, winningTile, handStatus, roundStatus, ruleset);
                if (value.Value != 0) {
                    result.Add(value);
                }
            }

            var hasYakuman = result.Any(yakuValue => yakuValue.Type == YakuType.Yakuman);
            return hasYakuman ? result.Where(yakuValue => yakuValue.Type == YakuType.Yakuman).ToList() : result;
        }

        /// <summary>
        /// Divide hand tiles into 4 melds and 1 pair.
        /// </summary>
        public static ISet<List<Meld>> Decompose(IList<Tile> handTiles, IList<Meld> openMelds, Tile winningTile) {
            var decomposes = new HashSet<List<Meld>>(new MeldListEqualityComparer());

            // Check if the length of handTile is valid.
            if (handTiles.Count % 3 != 1) {
                return decomposes;
            }

            var allTiles = new List<Tile>(handTiles) { winningTile };
            var hand = TileMaker.CountTiles(allTiles);

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

        public static bool HasWin(IList<Tile> handTiles, IList<Meld> openMelds, Tile winningTile) {
            return Decompose(handTiles, openMelds, winningTile).Count > 0;
        }

        private static int CountDora(Tile[] handTiles, Meld[] openMelds, Tile winningTile, Tile[]? doraTiles) {
            if (doraTiles is null) {
                return 0;
            }
            int count = 0;
            foreach (var dora in doraTiles) {
                count += CountDora(handTiles, openMelds, winningTile, dora);
            }

            return count;
        }

        private static int CountDora(Tile[] handTiles, Meld[] openMelds, Tile winningTile, Tile dora) {
            var count = 0;
            foreach (var tile in handTiles) {
                if (tile.EqualsIgnoreColor(dora)) {
                    count++;
                }
            }
            foreach (var meld in openMelds) {
                foreach (var tile in meld.Tiles) {
                    if (tile.EqualsIgnoreColor(dora)) {
                        count++;
                    }
                }
            }

            if (winningTile.EqualsIgnoreColor(dora)) {
                count++;
            }

            return count;
        }

        private static int CountRedDora(Tile[] handTiles, Meld[] openMelds, Tile winningTile) {
            var count = 0;
            count += handTiles.Count(tile => tile.IsRed);
            count += openMelds.Sum(meld => meld.Tiles.Count(tile => tile.IsRed));
            count += winningTile.IsRed ? 1 : 0;
            return count;
        }

        public static IList<Tile> WinningTiles(IList<Tile> handTiles, IList<Meld> openMelds) {
            var list = new List<Tile>();
            for (var index = 0; index < MahjongConfig.TileKinds; index++) {
                var tile = Tile.GetTile(index);
                if (HasWin(handTiles, openMelds, tile))
                    list.Add(tile);
            }

            return list;
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
            var suitCount = new int[MahjongConfig.SuitCount];
            for (var i = 0; i < MahjongConfig.TileKinds; i++) {
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

                current.Add(new Meld(false, tile, tile.Next, tile.Next.Next));

                DecomposeCore(index, hand, current, result);

                // Backtrack
                current.RemoveAt(current.Count - 1);
                hand[index]++;
                hand[index + 1]++;
                hand[index + 2]++;
            }
        }

        private static void Analyze7Pairs(int[] hand, HashSet<List<Meld>> result) {
            if (hand.Sum() != MahjongConfig.FullHandTilesCount) {
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
            if (hand.Sum() != MahjongConfig.FullHandTilesCount) {
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
