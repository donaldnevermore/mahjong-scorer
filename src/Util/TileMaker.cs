// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using MahjongScorer.Domain;

namespace MahjongScorer.Util {
    public class TileMaker {
        public static readonly int[] GreenTiles = GetGreenTiles();

        public static HandInfo GetHandInfo(string handTiles, string winningTile, string openMelds) {
            var h = ConvertTiles(handTiles).ToArray();
            var w = ConvertTile(winningTile);
            var m = ConvertOpenMelds(openMelds);
            return new HandInfo(h, w, m);
        }

        /// <summary>
        /// Convert tile string to list of tiles.
        /// </summary>
        public static List<Tile> ConvertTiles(string s) {
            if (string.IsNullOrEmpty(s)) {
                throw new ArgumentException(s);
            }

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

            result.Sort();
            return result;
        }

        public static Tile ConvertTile(string s) {
            var tiles = ConvertTiles(s);
            return tiles[0];
        }

        private static List<Tile> ConvertSuit(Suit s, List<int> ranks) {
            var list = new List<Tile>();

            foreach (var r in ranks) {
                var t = r == 0 ? new Tile(s, 5, true) : new Tile(s, r);
                list.Add(t);
            }

            list.Sort();
            return list;
        }

        /// <summary>
        /// Convert string array to open melds.
        /// String not ending with an 'o' will be treated as closed meld.
        /// Melds are separated by commas.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static List<Meld> ConvertOpenMelds(string s) {
            var list = new List<Meld>();

            if (string.IsNullOrEmpty(s)) {
                return list;
            }

            var melds = s.Split(',');

            foreach (var m in melds) {
                var isOpen = false;
                var meldStr = m;
                if (m[^1] == 'o') {
                    meldStr = m[..^1];
                    isOpen = true;
                }

                var tiles = ConvertTiles(meldStr);
                switch (tiles.Count) {
                case 3:
                    list.Add(new Meld(isOpen, tiles[0], tiles[1], tiles[2]));
                    break;
                case 4:
                    list.Add(new Meld(isOpen, tiles[0], tiles[1], tiles[2], tiles[3]));
                    break;
                default:
                    throw new ArgumentException("Meld must have 3 or 4 tiles.");
                }
            }

            return list;
        }

        public static Tile[] GetRedTiles() {
            return new Tile[] {
                new(Suit.M, 5, true),
                new(Suit.P, 5, true),
                new(Suit.S, 5, true)
            };
        }

        public static int[] CountMeldTiles(List<Meld> melds) {
            var arr = new int[34];
            foreach (var meld in melds) {
                foreach (var tile in meld.Tiles) {
                    arr[Tile.GetIndex(tile)]++;
                }
            }

            return arr;
        }

        /// <summary>
        /// Convert tiles into a array representing the number of each tile.
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public static int[] CountTiles(List<Tile> tiles) {
            var arr = new int[34];
            foreach (var tile in tiles) {
                arr[Tile.GetIndex(tile)]++;
            }

            return arr;
        }

        private static int[] GetGreenTiles() {
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
