using System.Linq;

namespace MahjongSharp {
    public class Ruleset {
        // Basic settings
        public int PlayerCount;
        public int RoundCount;

        public int MinHanNeeded => 1;
        public bool NegativePointGameEnd { get; init; } = true;
        public bool GameEndsWhenAllLastTop;
        public int InitialPoints;
        public int FirstPlacePoints;
        public int RedDora = 3;

        // Advance settings
        public int InitialDora => 1;
        public bool AllowRiichiWhenPointsLow;
        public bool AllowRiichiWhenNotReady;
        public bool AllowDiscardSameAfterOpen;
        public int RiichiMortgagePoints;
        public int ExtraRoundBonusPerPlayer;
        public int NotReadyPunishPerPlayer;
        public int FalseRiichiPunishPerPlayer;
        public bool AllowMultipleRong;

        public bool Allow3RongDraw;
        public bool Allow4RiichiDraw;
        public bool Allow4KongDraw;
        public bool Allow4WindDraw;
        public bool Allow9OrphanDraw;

        // Time settings.
        public int BaseTurnTime = 5;
        public int BonusTurnTime = 20;

        // Hidden entries in setting panel.
        public bool AllowChow;
        public bool AllowPong;

        public int DiceMin = 2;
        public int DiceMax = 12;
        public int ReservedTiles = 14;

        public int MaxDora = 5;
        public int RinshanTileCount = 4;

        // This field doesn't have a setting entry.
        public bool DoubleWindFu { get; init; } = true;

        public bool AllowKokushiRobConcealedKong { get; init; } = true;

        public static Tile[] GetRedTiles() {
            return new Tile[] {
                new(Suit.M, 5, true),
                new(Suit.P, 5, true),
                new(Suit.S, 5, true)
            };
        }

        public int MaxPlayer => 4;

        public bool IsAllLast(int index, int field, int totalPlayers) {
            return (index == totalPlayers - 1 && field == MaxRound - 1) || field >= MaxRound;
        }

        public bool GameForceEnd(int index, int field, int totalPlayers) {
            return index == totalPlayers - 1 && field == MaxRound - 1;
        }

        /// <summary>
        /// EastSouth round.
        /// </summary>
        public int MaxRound => 1;
    }

    public enum PointsToGameEnd {
        Negative,
        Never
    }
}
