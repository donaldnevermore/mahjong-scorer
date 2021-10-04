using System.Text;

namespace MahjongSharp {
    public struct RiverTile {
        public Tile Tile;
        public bool IsRiichi;
        public bool IsGone;

        public override string ToString() {
            var builder = new StringBuilder(Tile.ToString());
            if (IsRiichi) {
                builder.Append("r");
            }
            if (IsGone) {
                builder.Append("g");
            }
            return builder.ToString();
        }
    }
}
