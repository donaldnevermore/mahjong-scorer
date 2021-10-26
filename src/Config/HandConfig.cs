// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahjongScorer.Domain;

namespace MahjongScorer.Config {
    public class HandConfig {
        public IList<Tile> DoraIndicators { get; set; } = new List<Tile>();
        public IList<Tile> UraDoraIndicators { get; set; } = new List<Tile>();

        private RiichiStatus riichi = RiichiStatus.None;

        public RiichiStatus Riichi {
            get => riichi;
            set {
                Menzenchin = true;
                riichi = value;
            }
        }

        public bool Menzenchin { get; set; } = true;
        public bool Ippatsu { get; set; } = false;

        /// <summary>
        /// Tsumo or Ron.
        /// </summary>
        public bool Tsumo { get; set; } = false;

        public bool AfterAKan { get; set; } = false;
        public bool RobbingAKan { get; set; } = false;

        /// <summary>
        /// Blessing of Heaven or Earth.
        /// </summary>
        public bool Blessing { get; set; } = false;

        /// <summary>
        /// Under the Sea or River.
        /// </summary>
        public bool Under { get; set; } = false;
    }
}
