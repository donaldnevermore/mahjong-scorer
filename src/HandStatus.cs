using System;

namespace MahjongSharp {
    public class HandStatus {
        public bool Riichi { get; set; }
        public bool DoubleRiichi { get; set; }
        public bool Tsumo { get; set; }
        public bool Ippatsu { get; set; }
        public bool Menzenchin { get; set; }

        /// <summary>
        /// Blessing of Heaven or Earth.
        /// </summary>
        public bool TenhoOrChiho { get; set; }

        /// <summary>
        /// Rong immediately after a kong.
        /// </summary>
        public bool Rinshan { get; set; }

        /// <summary>
        /// Rong the last tile.
        /// </summary>
        public bool HaiteiOrHotei { get; set; }

        /// <summary>
        /// Robbing a kong.
        /// </summary>
        public bool Chankan { get; set; }
    }
}
