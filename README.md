# MahjongScorer

MahjongScorer is a C# library to help you calculate the Han and Fu in Riichi Mahjong.

## Getting Started

1. make sure you have .NET 7.0 or above installed.
2. run `dotnet run` in the terminal.

## Examples

```csharp
var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true, Ippatsu = true };
var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 2 };
var rule = new RuleConfig();

var hand = Scorer.GetHandInfo("23440556m23489s", "7s", "", "8p,7m");
var pt = Scorer.GetScore(hand, handConfig, round, rule);
Console.WriteLine(pt);

Assert.AreEqual(4, pt.Han);
Assert.AreEqual(30, pt.Fu);
```

And you should see

```
Han = 4, Fu = 30, BasePoints = 1920,
Dora = 0, RedDora = 1, UraDora = 0
Yaku = [Riichi: 1, MenzenchinTsumo: 1, Ippatsu: 1]
Fu = [BaseFu: 20, Tsumo: 2, EndWait: 2]
NonDealerTsumo: 7900(+2000) - 2000, 3900
```

## Rules

see [docs/rules.md](docs/rules.md).

## Special Thanks

- [ArcturusZhang/Mahjong](https://github.com/ArcturusZhang/Mahjong)
- [livewing/mahjong-calc](https://github.com/livewing/mahjong-calc)
