using System;

namespace MahjongSharp {
    public struct OpenMeld {
        public Meld Meld { get; init; }
        public Tile Tile { get; init; }
        public MeldSide Side { get; init; }
        public Tile Extra { get; init; }
        public bool IsAdded { get; init; }

        public MeldType Type => Meld.Type;
        public Tile First => Meld.First;
        public bool IsKong => Meld.IsKong;
        public bool IsOpen => Meld.IsOpen;
        public Tile[] Tiles => Meld.Tiles;

        public Tile[] GetForbiddenTiles(Tile tile) {
            return Meld.GetForbiddenTiles(tile);
        }

        public OpenMeld AddToKong(Tile extra) {
            return new OpenMeld {
                Meld = Meld.AddToKong(extra),
                Tile = Tile,
                Side = Side,
                Extra = extra,
                IsAdded = true
            };
        }

        public override string ToString() {
            return $"[{Meld}, {Tile}, {Side}]";
        }
    }
}
