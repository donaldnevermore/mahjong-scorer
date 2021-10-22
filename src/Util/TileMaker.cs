// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MahjongScorer.Domain;

namespace MahjongScorer.Util {
    public static class TileMaker {
        /// <summary>
        /// Convert tile string to list of tiles.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static IList<Tile> ConvertTiles(string s) {
            var result = new List<Tile>();
            var ranks = new List<int>();

            for (var i = 0; i < s.Length; i++) {
                switch (s[i]) {
                case 'm':
                    result.AddRange(ConvertSuit(Suit.M, ranks));
                    ranks.Clear();
                    break;
                case 'p':
                    result.AddRange(ConvertSuit(Suit.P, ranks));
                    ranks.Clear();
                    break;
                case 's':
                    result.AddRange(ConvertSuit(Suit.S, ranks));
                    ranks.Clear();
                    break;
                case 'z':
                    result.AddRange(ConvertSuit(Suit.Z, ranks));
                    ranks.Clear();
                    break;
                default:
                    var rank = s[i] - '0';
                    ranks.Add(rank);
                    break;
                }
            }

            return result;
        }

        public static Tile ConvertTile(string s) {
            var tiles = ConvertTiles(s);
            return tiles[0];
        }

        private static IList<Tile> ConvertSuit(Suit s, IList<int> ranks) {
            var list = new List<Tile>();

            foreach (var r in ranks) {
                list.Add(new Tile(s, r));
            }

            return list;
        }

        /// <summary>
        /// Convert string array to open melds.
        /// </summary>
        /// <param name="melds"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IList<Meld> ConvertMelds(string[] melds) {
            var meldList = new List<Meld>();

            foreach (var s in melds) {
                var tiles = ConvertTiles(s);
                switch (tiles.Count) {
                case 3:
                    meldList.Add(new Meld(true, tiles[0], tiles[1], tiles[2]));
                    break;
                case 4:
                    meldList.Add(new Meld(true, tiles[0], tiles[1], tiles[2], tiles[3]));
                    break;
                default:
                    throw new ArgumentException(nameof(melds));
                }
            }

            return meldList;
        }

        public static Tile[] GetRedTiles() {
            return new Tile[] {
                new(Suit.M, 5, true),
                new(Suit.P, 5, true),
                new(Suit.S, 5, true)
            };
        }

        public static int[] CountMeldTiles(IList<Meld> melds) {
            var array = new int[34];
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
            var arr = new int[34];
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
