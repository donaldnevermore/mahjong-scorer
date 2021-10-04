using System;
using System.Diagnostics;

namespace MahjongSharp {
    public struct RoundStatus {
        public int PlayerIndex { get; set; }
        public int DealerIndex { get; set; }

        /// <summary>
        /// How many times a dealer becomes dealer again in the next round.
        /// </summary>
        public int Honba { get; set; }

        /// <summary>
        /// Riichi bets because their players have make a Riichi but haven't Rong'd.
        /// </summary>
        public int RiichiBets { get; set; }

        /// <summary>
        /// East/South field in a EastSouth game.
        /// </summary>
        public int RoundIndex { get; set; }

        public int TotalPlayer { get; init; }

        public Tile SeatWind {
            get {
                var offset = PlayerIndex - DealerIndex;
                if (offset < 0) {
                    offset += TotalPlayer;
                }

                Debug.Assert(offset >= 0 && offset <= 3, "Player Wind should be one of E, S, W, or N.");

                return new Tile(Suit.Z, offset + 1);
            }
        }

        public Tile RoundWind => new(Suit.Z, RoundIndex + 1);
        public bool IsDealer => PlayerIndex == DealerIndex;
    }
}
