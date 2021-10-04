using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MahjongSharp {
    public class Game {
        public static IDictionary<Tile, IList<Tile>> DiscardForReady(IList<Tile> handTiles, Tile? lastDraw) {
            var list = new List<Tile>(handTiles);
            if (lastDraw != null) list.Add((Tile)lastDraw);
            Dictionary<Tile, IList<Tile>> result = null;
            for (int i = 0; i < list.Count; i++) {
                var first = list[0];
                list.RemoveAt(0);
                var waitingList = HanScorer.WinningTiles(list, null);
                if (waitingList.Count > 0) {
                    if (result == null)
                        result = new Dictionary<Tile, IList<Tile>>(Tile.TileIgnoreColorEqualityComparer);
                    if (!result.ContainsKey(first)) result.Add(first, waitingList);
                }
                list.Add(first);
            }
            return result;
        }

        public static bool IsReady(IList<Tile> handTiles, IList<Meld> openMelds) {
            return HanScorer.WinningTiles(handTiles, openMelds).Count > 0;
        }

        public static bool Test9KindsOfOrphans(IList<Tile> handTiles, Tile lastDraw) {
            var set = new HashSet<Tile>(handTiles, Tile.TileIgnoreColorEqualityComparer);
            set.Add(lastDraw);
            int count = set.Count(tile => tile.IsYaochuu);
            return count >= 9;
        }

        public static bool IsMenzenchin(IList<Meld> openMelds) {
            return openMelds.Count == 0 || openMelds.All(m => m.IsKong && !m.IsOpen);
        }

        public static bool TestRichi(IList<Tile> handTiles, IList<Meld> openMelds, Tile lastDraw,
            bool allowNotReady, out IList<Tile> availableTiles) {
            if (!IsMenzenchin(openMelds)) {
                availableTiles = new List<Tile>();
                return false;
            }
            if (allowNotReady) {
                // return every hand tile as candidates
                availableTiles = new List<Tile>(handTiles);
                availableTiles.Add(lastDraw);
                return true;
            }
            var tiles = new List<Tile>(handTiles) { lastDraw };
            availableTiles = new List<Tile>();
            for (int i = 0; i < tiles.Count; i++) {
                var tile = tiles[0];
                tiles.RemoveAt(0);
                if (IsReady(tiles, openMelds)) availableTiles.Add(tile);
                tiles.Add(tile);
            }
            return availableTiles.Count > 0;
        }

        public static IEnumerable<OpenMeld> GetKongs(IList<Tile> handTiles, Tile discardTile, MeldSide side) {
            var result = new HashSet<Meld>(Meld.MeldConsiderColorEqualityComparer);
            var tileList = new List<Tile>(handTiles) { discardTile };
            var handCount = TileMaker.CountTiles(handTiles);
            int index = Tile.GetIndex(discardTile);
            if (handCount[index] == 3) {
                var tiles = tileList.FindAll(t => Tile.GetIndex(t) == index);
                result.Add(new Meld(true, tiles.ToArray()));
            }
            return result.Select(meld => new OpenMeld {
                Meld = meld,
                Tile = discardTile,
                Side = side
            });
        }

        public static IEnumerable<OpenMeld> GetSelfKongs(IList<Tile> handTiles, Tile lastDraw) {
            var result = new HashSet<Meld>(Meld.MeldConsiderColorEqualityComparer);
            var testTiles = new List<Tile>(handTiles) { lastDraw };
            var handCount = TileMaker.CountTiles(testTiles);
            for (int i = 0; i < handCount.Length; i++) {
                // Assert.IsTrue(handCount[i] <= 4);
                if (handCount[i] == 4) {
                    var tiles = testTiles.FindAll(tile => Tile.GetIndex(tile) == i);
                    result.Add(new Meld(false, tiles.ToArray()));
                }
            }
            return result.Select(meld => new OpenMeld {
                Meld = meld,
                Side = MeldSide.Self
            });
        }

        public static IEnumerable<OpenMeld> GetAddKongs(IList<Tile> handTiles, IList<OpenMeld> openMelds,
            Tile lastDraw) {
            var result = new List<OpenMeld>();
            var testTiles = new List<Tile>(handTiles) { lastDraw };
            var pongs = openMelds.Where(meld => meld.Type == MeldType.Triplet && !meld.IsKong);
            foreach (var pong in pongs) {
                var extraIndex = testTiles.FindIndex(t => t.EqualsIgnoreColor(pong.First));
                if (extraIndex < 0) continue;
                var extraTile = testTiles[extraIndex];
                result.Add(pong.AddToKong(extraTile));
            }
            return result;
        }

        public static IEnumerable<OpenMeld> GetRiichiKongs(IList<Tile> handTiles, Tile lastDraw) {
            var winningTiles = HanScorer.WinningTiles(handTiles, null);
            foreach (var winningTile in winningTiles) {
                var decomposes = HanScorer.Decompose(handTiles, null, winningTile);
                if (!decomposes.All(list =>
                    list.Exists(m => m.Type == MeldType.Triplet && m.First.EqualsIgnoreColor(lastDraw))))
                    return new List<OpenMeld>();
            }
            var tiles = new List<Tile>();
            for (int i = 0; i < handTiles.Count; i++) {
                if (handTiles[i].EqualsIgnoreColor(lastDraw)) tiles.Add(handTiles[i]);
            }
            tiles.Add(lastDraw);

            Debug.Assert(tiles.Count == 4);

            return new List<OpenMeld> {
                new() {
                    Meld = new Meld(false, tiles.ToArray()),
                    Side = MeldSide.Self
                }
            };
        }

        public static IEnumerable<OpenMeld> GetPongs(IList<Tile> handTiles, Tile discardTile, MeldSide side) {
            var result = new HashSet<Meld>(Meld.MeldConsiderColorEqualityComparer);
            var handTileList = new List<Tile>(handTiles);
            var particularTiles = handTileList.FindAll(tile => tile.EqualsIgnoreColor(discardTile));
            var combination = Combination(particularTiles, 2);
            foreach (var item in combination) {
                item.Add(discardTile);
                result.Add(new Meld(true, item.ToArray()));
            }
            return result.Select(meld => new OpenMeld {
                Meld = meld,
                Tile = discardTile,
                Side = side
            });
        }

        public static IList<List<T>> Combination<T>(IList<T> list, int count) {
            var result = new List<List<T>>();
            if (count <= 0 || count > list.Count) return result;
            CombinationBackTrack(list, count, 0, new List<T>(), result);
            return result;
        }

        private static void CombinationBackTrack<T>(IList<T> list, int count, int start, IList<T> current,
            IList<List<T>> result) {
            // exits
            if (current.Count == count) {
                result.Add(new List<T>(current));
                return;
            }
            for (int i = start; i < list.Count; i++) {
                current.Add(list[i]);
                CombinationBackTrack(list, count, i + 1, current, result);
                // back track
                current.RemoveAt(current.Count - 1);
            }
        }

        public static IEnumerable<OpenMeld> GetChows(IList<Tile> handTiles, Tile discardTile, MeldSide side) {
            var result = new HashSet<Meld>(Meld.MeldConsiderColorEqualityComparer);
            if (discardTile.Suit == Suit.Z) return new List<OpenMeld>();
            var handTileList = new List<Tile>(handTiles);
            GetChows1(handTileList, discardTile, result);
            GetChows2(handTileList, discardTile, result);
            GetChows3(handTileList, discardTile, result);
            return result.Select(meld => new OpenMeld {
                Meld = meld,
                Tile = discardTile,
                Side = side
            });
        }

        private static void GetChows1(List<Tile> handTiles, Tile discardTile, HashSet<Meld> result) {
            Tile first, second;
            if (Tile.TryTile(discardTile.Suit, discardTile.Rank - 2, out first) &&
                Tile.TryTile(discardTile.Suit, discardTile.Rank - 1, out second)) {
                var firstTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(first));
                if (firstTiles.Count == 0) return;
                var secondTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(second));
                if (secondTiles.Count == 0) return;
                foreach (var pair in CartesianJoin(firstTiles, secondTiles)) {
                    result.Add(new Meld(true, pair.Key, pair.Value, discardTile));
                }
            }
        }

        private static void GetChows2(List<Tile> handTiles, Tile discardTile, HashSet<Meld> result) {
            Tile first, second;
            if (Tile.TryTile(discardTile.Suit, discardTile.Rank - 1, out first) &&
                Tile.TryTile(discardTile.Suit, discardTile.Rank + 1, out second)) {
                var firstTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(first));
                if (firstTiles.Count == 0) return;
                var secondTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(second));
                if (secondTiles.Count == 0) return;
                foreach (var pair in CartesianJoin(firstTiles, secondTiles)) {
                    result.Add(new Meld(true, pair.Key, pair.Value, discardTile));
                }
            }
        }

        private static void GetChows3(List<Tile> handTiles, Tile discardTile, HashSet<Meld> result) {
            Tile? first, second;
            if (Tile.TryTile(discardTile.Suit, discardTile.Rank + 1, out first) &&
                Tile.TryTile(discardTile.Suit, discardTile.Rank + 2, out second)) {
                var firstTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(first));
                if (firstTiles.Count == 0) return;
                var secondTiles = handTiles.FindAll(tile => tile.EqualsIgnoreColor(second));
                if (secondTiles.Count == 0) return;
                foreach (var pair in CartesianJoin(firstTiles, secondTiles)) {
                    result.Add(new Meld(true, pair.Key, pair.Value, discardTile));
                }
            }
        }

        private static IEnumerable<KeyValuePair<T, T>> CartesianJoin<T>(IEnumerable<T> first, IEnumerable<T> second) {
            var result = first.SelectMany(x => second, (x, y) => new KeyValuePair<T, T>(x, y));
            return result;
        }

        private static string DecomposeToString(ISet<List<Meld>> decomposes) {
            var s = decomposes.Select(list => $"[{string.Join(", ", list)}");
            return string.Join("; ", s);
        }

        public static bool TestDiscardFuriten(IList<Tile> handTiles, List<RiverTile> riverTiles) {
            var winningTiles = HanScorer.WinningTiles(handTiles, new List<Meld>());
            foreach (var winningTile in winningTiles) {
                int index = riverTiles.FindIndex(riverTile => riverTile.Tile.EqualsIgnoreColor(winningTile));
                if (index >= 0) return true;
            }
            return false;
        }

        public static Tile GetDoraTile(Tile doraIndicator, IList<Tile> allTiles) {
            if (allTiles.Count == 0) {
                allTiles = TileMaker.GetFullTiles();
            }

            if (!allTiles.Contains(doraIndicator, Tile.TileIgnoreColorEqualityComparer)) {
                Console.WriteLine($"Full tile set does not contain tile {doraIndicator}, return itself.");
                return doraIndicator;
            }

            int repeat;
            if (doraIndicator.Suit == Suit.Z) {
                if (doraIndicator.Rank <= 4) repeat = 4;
                else repeat = 3;
            }
            else repeat = 9;
            int rank = doraIndicator.Rank;
            Tile dora;
            do {
                rank++;
                if (rank > repeat) rank -= repeat;
                dora = new Tile(doraIndicator.Suit, rank);
            } while (!allTiles.Contains(dora, Tile.TileIgnoreColorEqualityComparer));
            return dora;
        }

        public static IOrderedEnumerable<KeyValuePair<int, int>> SortPointsAndPlaces(IEnumerable<int> points) {
            return points.Select((p, i) => new KeyValuePair<int, int>(p, i))
                .OrderBy(key => key, new PointsComparer());
        }

        private struct PointsComparer : IComparer<KeyValuePair<int, int>> {
            public int Compare(KeyValuePair<int, int> point1, KeyValuePair<int, int> point2) {
                var pointsCmp = point1.Key.CompareTo(point2.Key);
                if (pointsCmp != 0) return -pointsCmp;
                return point1.Value.CompareTo(point2.Value);
            }
        }
    }
}
