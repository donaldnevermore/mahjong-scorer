namespace MahjongScorer.Util;

using System.Text;

public class Wall {
    public static void SampleMahjongShuffle() {
        var mahjong = new Cards();
        mahjong.Reset();
        mahjong.Shuffle();

        Console.WriteLine(mahjong.Encode());
    }

}

public class Cards {
    private readonly Random rand;
    private readonly int[] cards = new[] {
        0x01, 0x01,0x01,0x01,
        0x02, 0x02, 0x02, 0x02,
        0x03, 0x03, 0x03, 0x03,
        0x04, 0x04, 0x04, 0x04,
        0x05,0x05,0x05,0x105, // 0x105 0p
        0x06, 0x06, 0x06, 0x06,
        0x07, 0x07, 0x07, 0x07,
        0x08, 0x08, 0x08, 0x08,
        0x09, 0x09, 0x09, 0x09,

        0x11, 0x11,0x11,0x11,
        0x12,0x12,0x12,0x12,
        0x13, 0x13, 0x13, 0x13,
        0x14, 0x14, 0x14, 0x14,
        0x15, 0x15, 0x15, 0x115,
        0x16, 0x16, 0x16, 0x16,
        0x17, 0x17, 0x17, 0x17,
        0x18, 0x18, 0x18, 0x18,
        0x19,0x19,0x19,0x19,

        0x21, 0x21, 0x21, 0x21,
        0x22, 0x22, 0x22, 0x22,
        0x23, 0x23, 0x23, 0x23,
        0x24, 0x24, 0x24, 0x24,
        0x25, 0x25, 0x25, 0x125,
        0x26, 0x26, 0x26, 0x26,
        0x27, 0x27, 0x27, 0x27,
        0x28, 0x28, 0x28, 0x28,
        0x29, 0x29, 0x29, 0x29,

        0x31, 0x31, 0x31, 0x31,
        0x41, 0x41, 0x41, 0x41,
        0x51, 0x51, 0x51, 0x51,
        0x61, 0x61, 0x61, 0x61,
        0x71, 0x71, 0x71, 0x71,
        0x81, 0x81, 0x81, 0x81,
        0x91, 0x91, 0x91, 0x91,
    };
    private int[] wall = new int[136];

    public Cards() {
        var seed = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() * 1000000;
        rand = new Random((int)seed);

        Array.Copy(cards, wall, cards.Length);
    }

    public void Shuffle() {
        var cardTotal = wall.Length;
        for (int i = cardTotal - 1; i > 0; i--) {
            var index = rand.Next(i + 1);
            var temp = wall[i];
            wall[i] = wall[index];
            wall[index] = temp;
        }
    }

    public int Len() {
        return wall.Length;
    }

    public void Reset() {
        var cardTotal = cards.Length;
        Array.Copy(cards, wall, cardTotal);
    }

    public string Encode() {
        var encodes = new StringBuilder();
        for (var i = 0; i < Len(); i++) {
            encodes.Append(cardCode(wall[i]));
        }
        return encodes.ToString();
    }

    public char[] cardCode(int card) {
        var code = new char[2];
        if (card <= 0x09) {
            code[0] = (char)(card - 0x00 + '0');
            code[1] = 'p';
        } else if (card <= 0x19) {
            code[0] = (char)(card - 0x10 + '0');
            code[1] = 's';
        } else if (card <= 0x29) {
            code[0] = (char)(card - 0x20 + '0');
            code[1] = 'm';
        } else if (card <= 0x91) {
            code[0] = (char)(card / 16 - 2 + '0');
            code[1] = 'z';
        } else if (card <= 0x0105) {
            code[0] = '0';
            code[1] = 'p';
        } else if (card <= 0x0115) {
            code[0] = '0';
            code[1] = 's';
        } else if (card <= 0x0125) {
            code[0] = '0';
            code[1] = 'm';
        }
        return code;
    }
}
