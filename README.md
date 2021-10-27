# MahjongScorer

MahjongScorer is a C# library to help you calculate Han and/or Fu when playing Riichi Mahjong.

## Getting Started

1. make sure you have .NET 5.0 or above installed.
2. run `dotnet test`.

## Examples

```csharp
 var handConfig = new HandConfig { Riichi = RiichiStatus.Riichi, Tsumo = true, Ippatsu = true };
 var round = new RoundConfig { SeatWind = Wind.North, RiichiBets = 2 };
 var rule = new RuleConfig
 var p = Scorer.GetScore("23440556m23489s", "7s", "",
     "8p,7m", handConfig, round, rule);
 Console.WriteLine(p);
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
