// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongSharp.Util {
    public static class NumberUtil {
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
