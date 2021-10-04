namespace MahjongSharp {
    public struct RiverData {
        public RiverTile[] River;

        public override string ToString() {
            return string.Join(", ", River);
        }
    }
}
