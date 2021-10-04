using System.Collections.Generic;

namespace MahjongSharp {
    public class MahjongConfig {
        public const int WallCount = 4;
        public const int WallTilesCount = 34;
        public const int TotalTilesCount = WallCount * WallTilesCount;

        public const int TileKinds = 34;
        public const int SuitCount = 4;
        public const int FullHandTilesCount = 14;

        public const int MaxKongs = 4;
        public const int InitialDrawRound = 3;
        public const int TilesEveryRound = 4;
        public const int TilesLastRound = 1;

        // Yaku base points.
        public const int Mangan = 2000;
        public const int Haneman = 3000;
        public const int Baiman = 4000;
        public const int Sanbaiman = 6000;
        public const int Yakuman = 8000;
        public const int AccumulatedYakuman = 8000;
        public const int YakumanBaseHan = 13;
    }
}
