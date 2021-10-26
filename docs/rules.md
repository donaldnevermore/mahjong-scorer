# Rules

## How & When to Calculate Fu

When the Han is less than 5, we need to consider Fu.

### Special Patterns

If your hand is one of these special patterns, the Fu is:

- 7 Pairs: 25
- Pinfu Tsumo: 20
- Pinfu Ron with an Open Hand (Tsumo is also OK): 30

If not, your Fu starts with a base Fu of 20, and increases according to meld pattern and waiting pattern.

### Meld Patterns

- A Yakuhai pair (Seat Wind, Round Wind, or Dragons): +2 Fu, +4 if it is Seat Wind and also Round Wind
- Open Triplet: +2 Fu, +4 if it's but a Closed Triplet, +8 if they're Yaochuu tiles
- Open Quad: +8 Fu, +16 if it's but a Closed Triplet, +32 if they're Yaochuu tiles

### Waiting Patterns

- Single Wait, End Wait, or Middle Wait: +2 Fu
- Tsumo: +2 Fu
- Menzenchin Ron: +10 Fu

And finally, round up the number you get to 10 unless it is a special pattern. For example, if the number is 32, you end
up with 40 Fu.

Let f be Fu, h be Han, and b be base points, we'll get:

b = f * 2^(h+2), when h < 5.

If b >= 2000, b will be 2000 and we'll get a Mangan.

## Glossary

Han 「飜」 is the main portion of scoring, as each yaku is assigned a value in terms of han.

Fu 「符」 takes the hand composition into consideration in terms of tile melds, wait patterns and/or win method.

Chii takes a discarded tile from the player on your left to form a sequence (open meld).

Pon takes a discarded tile to form a open triplet.

Kan takes a discarded tile to form a open quad.

Tempai is a status where you only need 1 tile to win.

Dora can add 1 Han after you win.

Roto is a type of tile whose rank is 1 or 9, excluding Honor tiles, e.g. 19m19p19s.

Yaochuu is Roto plus Honor tiles, e.g. 19m19p19s1234567z.

Check out this [website](https://repo.riichi.moe/guides/mahjong-terms.html) for more terms explained.
