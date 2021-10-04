using System;
using System.Collections.Generic;
using static MahjongSharp.Yaku;

namespace MahjongSharp {
    public class YakuMethod {
        public static readonly List<Func<IList<Meld>, Tile, HandStatus, RoundStatus, Ruleset, YakuValue>> Methods =
            new() {
                Riichi, Tanyao, MenzenTsumo, SeatWind, FieldWind,
                DragonWhite, DragonGreen, DragonRed, Pinfu, IipekoOrRyampeko,
                Chankan, Rinshan, HaiteiOrHotei, Ippatsu, SanshokuDoko,
                SankantsuOrSukantsu, Toitoi, SanankoOrSuanko, ShosangenOrDaisangen, HonrotoOrChinroto,
                Chitoitsu, ChantaOrJunchan, Ittsu, SanshokuDojun, HonitsuOrChinitsu,
                TenhoOrChiho, Ryuiso, Kokushi, ShosushiOrDaisushi, Tsuiso,
                Churen
            };
    }
}
