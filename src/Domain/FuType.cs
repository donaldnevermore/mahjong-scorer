// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

namespace MahjongScorer.Domain;

public enum FuType {
    BaseFu,
    SevenPairs,
    PinfuTsumo,
    PinfuRonWithAnOpenHand,
    MenzenchinRon,
    Tsumo,

    Dragon,
    DoubleWind,
    SeatWind,
    RoundWind,

    SingleWait,
    EndWait,
    MiddleWait,

    OpenTriplet,
    OpenTripletYaochuu,
    ClosedTriplet,
    ClosedTripletYaochuu,
    OpenQuad,
    OpenQuadYaochuu,
    ClosedQuad,
    ClosedQuadYaochuu
}
