// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using MahjongScorer.Domain;

namespace MahjongScorer.Config {
    public class HandConfig {
        public List<Tile> DoraIndicators { get; set; } = new();
        public List<Tile> UraDoraIndicators { get; set; } = new();

        private RiichiStatus riichi = RiichiStatus.None;
        private bool blessing = false;

        public RiichiStatus Riichi {
            get => riichi;
            set {
                riichi = value;
                Menzenchin = true;
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
        public bool Blessing {
            get => blessing;
            set {
                blessing = value;
                Tsumo = true;
            }
        }

        /// <summary>
        /// Under the Sea or River.
        /// </summary>
        public bool Under { get; set; } = false;
    }
}
