using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongSharp {
    public class MahjongSet {
        private Ruleset ruleset;
        private List<Tile> allTiles;
        private int tilesDrawn = 0;
        private int rinshanDrawn = 0;
        private int doraTurned = 0;

        public MahjongSet(Ruleset ruleset, IEnumerable<Tile> tiles) {
            this.ruleset = ruleset;
            allTiles = new List<Tile>(tiles);
            Console.WriteLine($"In current settings, total count of all tiles is {allTiles.Count}");

            // Add Red Dora
            var redTiles = Ruleset.GetRedTiles();
            for (var i = 0; i < redTiles.Length; i++) {
                var red = redTiles[i];
                var index = allTiles.FindIndex(t => t.EqualsIgnoreColor(red) && !t.IsRed);
                if (index < 0) {
                    Console.WriteLine($"Not enough tile of {red}");
                    continue;
                }
                allTiles[index] = new Tile(red.Suit, red.Rank, true);
            }
        }

        /// <summary>
        /// Shuffle and reset the mahjong set to its original state, then turn dora tiles of the initial count and return an array containing the initial doras.
        /// </summary>
        /// <returns>Array of initial dora tiles</returns>
        public Tile[] Reset() {
            // Shuffle
            var rnd = new Random();
            allTiles = allTiles.OrderBy(_ => rnd.Next()).ToList();

            tilesDrawn = 0;
            rinshanDrawn = 0;
            doraTurned = 0;
            var doraList = new List<Tile>();
            for (int i = 0; i < ruleset.InitialDora; i++) {
                doraList.Add(TurnDora());
            }
            return doraList.ToArray();
        }

        /// <summary>
        /// Draw a new tile on the wall, throw a exception when there are none
        /// </summary>
        /// <returns>The next tile</returns>
        public Tile DrawTile() {
            if (TilesRemain <= 0) throw new NoTilesException("There are no more tiles to drawn!");
            return allTiles[tilesDrawn++];
        }

        /// <summary>
        /// Peek the next tile on the wall without drawing it, throw a exception when there are none
        /// </summary>
        /// <returns>The next tile</returns>
        public Tile PeekTile() {
            if (TilesRemain <= 0) throw new NoTilesException("There are no more tiles to drawn!");
            return allTiles[tilesDrawn];
        }

        /// <summary>
        /// Draw a lingshang tile from the other side of the wall, throw a exception when there are none
        /// </summary>
        /// <returns>The lingshang tile</returns>
        public Tile DrawLingShang() {
            if (RinshanDrawn >= ruleset.RinshanTileCount)
                throw new NoTilesException("There are no more LingShang tiles to drawn!");
            var tile = allTiles[allTiles.Count - rinshanDrawn - 1];
            rinshanDrawn++;
            return tile;
        }

        /// <summary>
        /// Turn a new dora indicator, throw a exception when there are none
        /// </summary>
        /// <returns>The new dora indicator</returns>
        public Tile TurnDora() {
            if (doraTurned >= ruleset.MaxDora)
                throw new NoTilesException("There are no more dora indicators to turn.");
            var firstDora = ruleset.RinshanTileCount; // index from tail of list
            var currentDora = firstDora + DoraTurned * 2;
            doraTurned++;
            return allTiles[allTiles.Count - 1 - currentDora];
        }

        /// <summary>
        /// Returns current turned dora indicators
        /// </summary>
        /// <value>Array of dora indicators</value>
        public Tile[] DoraIndicators {
            get {
                var firstDora = ruleset.RinshanTileCount;
                var doraTiles = new Tile[DoraTurned];
                for (int i = 0; i < doraTiles.Length; i++) {
                    var currentDora = firstDora + i * 2;
                    doraTiles[i] = allTiles[allTiles.Count - 1 - currentDora];
                }
                return doraTiles;
            }
        }

        /// <summary>
        /// Returns current turned uradora indicators.
        /// </summary>
        public Tile[] UraDoraIndicators {
            get {
                var firstUraDora = ruleset.RinshanTileCount + 1;
                var uraDoraTiles = new Tile[DoraTurned];
                for (int i = 0; i < uraDoraTiles.Length; i++) {
                    var currentUraDora = firstUraDora + i * 2;
                    uraDoraTiles[i] = allTiles[allTiles.Count - 1 - currentUraDora];
                }
                return uraDoraTiles;
            }
        }

        private void SetTiles(IList<Tile> tiles) {
            for (var i = 0; i < tiles.Count; i++) {
                allTiles[tilesDrawn + i] = tiles[i];
            }
        }

        private void SetTilesReverse(IList<Tile> tiles) {
            for (var i = 0; i < tiles.Count; i++) {
                allTiles[allTiles.Count - rinshanDrawn - 1 - i] = tiles[i];
            }
        }

        public IList<Tile> AllTiles => allTiles.AsReadOnly();
        public int TilesDrawn => tilesDrawn;
        public int DoraTurned => doraTurned;
        public int RinshanDrawn => rinshanDrawn;
        public int TilesRemain => allTiles.Count - tilesDrawn - rinshanDrawn;

        public MahjongSetData Data => new() {
            TilesDrawn = TilesDrawn,
            DoraTurned = DoraTurned,
            LingShangDrawn = RinshanDrawn,
            TilesRemain = TilesRemain,
            DoraIndicators = DoraIndicators,
            TotalTiles = allTiles.Count
        };
    }

    public struct MahjongSetData {
        public int TilesDrawn;
        public int DoraTurned;
        public int LingShangDrawn;
        public int TilesRemain;
        public Tile[] DoraIndicators;
        public int TotalTiles;

        public override string ToString() {
            var doraIndicators = DoraIndicators.Length == 0
                ? ""
                : string.Join(", ", DoraIndicators.Select(d => d.ToString()));

            return $"TilesDrawn: {TilesDrawn}, DoraTurned: {DoraTurned}, " +
                $"RinshanDrawn: {LingShangDrawn}, TilesRemain: {TilesRemain}, " +
                $"DoraIndicators: {doraIndicators}, TotalTiles: {TotalTiles}";
        }
    }
}
