// Copyright (c) 2021 donaldnevermore
// All rights reserved.
// Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for more information.

using MahjongSharp.Domain;

namespace MahjongSharp.Score {
    public class HandConfig {
        public DoraInfo DoraInfo { get; set; } = new();

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
