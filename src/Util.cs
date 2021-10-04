namespace MahjongSharp {
    public static class Util {
        /// <summary>
        /// Round up to next unit.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static int RoundUpToNextUnit(int value, int unit) {
            if (value % unit == 0) {
                return value;
            }
            return (value / unit + 1) * unit;
        }
    }
}
