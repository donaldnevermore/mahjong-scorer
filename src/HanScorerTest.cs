using System.Linq;
using NUnit.Framework;

namespace MahjongSharp {
    [TestFixture]
    public class HanScorerTest {
        [Test]
        public void TestDealerRiichi() {
            var handTiles = TileMaker.ConvertTiles("33345m23455p678s").ToArray();
            var handStatus = new HandStatus { Riichi = true };
            var roundStatus = new RoundStatus { TotalPlayer = 4 };
            var ruleset = new Ruleset();
            var winningTile = new Tile(Suit.P, 5);

            var decomposes = HanScorer.Decompose(handTiles, new Meld[] { }, winningTile);
            Assert.AreNotEqual(0, decomposes.Count);

            var pt = HanScorer.GetPointInfoWithDora(
                decomposes,
                winningTile,
                handStatus,
                roundStatus,
                ruleset,
                0, 0, 0
            );
            Assert.AreEqual(1, pt.Han);
            Assert.AreEqual(20, pt.Fu);

            var pointPay = pt.GetTotalPoint();
            Assert.AreEqual(1500, pointPay.BaseGain);
        }
    }
}
