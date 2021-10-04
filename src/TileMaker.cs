using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MahjongSharp {
    public class TileMaker {
        /// <summary>
        /// Convert tile string to list of tiles.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IList<Tile> ConvertTiles(string s) {
            var list = new List<Tile>();
            var suit = Suit.M;

            for (var i = 0; i < s.Length; i++) {
                switch (s[i]) {
                case 'm':
                    suit = Suit.P;
                    break;
                case 'p':
                    suit = Suit.S;
                    break;
                case 's':
                    suit = Suit.Z;
                    break;
                case 'z':
                    return list;
                default:
                    var rank = int.Parse(s[i].ToString());
                    list.Add(new Tile(suit, rank));
                    break;
                }
            }

            return list;
        }

        public static int[] CountMeldTiles(IList<Meld> melds) {
            var array = new int[MahjongConfig.TileKinds];
            foreach (var meld in melds) {
                foreach (var tile in meld.Tiles) {
                    array[Tile.GetIndex(tile)]++;
                }
            }

            return array;
        }

        /// <summary>
        /// Convert tiles into a array representing the number of each tile.
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public static int[] CountTiles(IList<Tile> tiles) {
            var arr = new int[MahjongConfig.TileKinds];
            foreach (var tile in tiles) {
                arr[Tile.GetIndex(tile)]++;
            }

            return arr;
        }

        public static Tile[] GetFullTiles() {
            var list = new List<Tile>();

            for (var i = 0; i < 4; i++) {
                for (var rank = 1; rank <= 9; rank++) {
                    list.Add(new Tile(Suit.M, rank));
                    list.Add(new Tile(Suit.P, rank));
                    list.Add(new Tile(Suit.S, rank));

                    // There are 7 kinds of Wind & Dragon tiles only.
                    if (rank <= 7) {
                        list.Add(new Tile(Suit.Z, rank));
                    }
                }
            }

            return list.ToArray();
        }

        public static int[] GetGreenTiles() {
            var tiles = new Tile[] {
                new(Suit.S, 2),
                new(Suit.S, 3),
                new(Suit.S, 4),
                new(Suit.S, 6),
                new(Suit.S, 8),
                new(Suit.Z, 6)
            };

            var arr = new int[6];
            for (var i = 0; i < arr.Length; i++) {
                arr[i] = Tile.GetIndex(tiles[i]);
            }

            return arr;
        }
    }
}
