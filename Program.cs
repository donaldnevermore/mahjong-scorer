// using System.Text.Json;
// using MahjongScorer.Config;
// using MahjongScorer.Domain;
// using MahjongScorer;

// var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true, Ippatsu = true };
// var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 2 };
// var rule = new RuleConfig();

// var hand = Scorer.GetHandInfo("23440556m23489s", "7s", "", "8p,7m");
// var pt = Scorer.GetScore(hand, handConfig, round, rule);
// var s = JsonSerializer.Serialize(pt);
// Console.WriteLine(pt);

using MahjongScorer.Util;

Wall.SampleMahjongShuffle();
