using System;
using MahjongScorer.Config;
using MahjongScorer.Domain;
using static MahjongScorer.Scorer;

var hand = new HandConfig { Riichi = RiichiStatus.Riichi };
var round = new RoundConfig();
var rule = new RuleConfig();

ShowScore("33345m23455p678s", "5p", Array.Empty<string>(),
    hand, round, rule);
