using NUnit.Framework;

namespace MahjongSharp {
    [TestFixture]
    public class UtilTest {
        [Test]
        public void Test() {
            var n = Util.RoundUpToNextUnit(1920, 100);
            Assert.AreEqual(2000, n);

            var n2 = Util.RoundUpToNextUnit(1985, 10);
            Assert.AreEqual(1990, n2);
        }
    }
}
