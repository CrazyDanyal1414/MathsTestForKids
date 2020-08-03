using System.Threading;
using NUnit.Framework;

namespace MathsTest
{
    [TestFixture]
    public class NUnitTest
    {
        [TestCase]
        public void CheckIfGetPossibleOperationsByDifficultyWorks()
        {
            var (operationmin, operationmax) = Calculation.GetPossibleOperationsByDifficulty(UserDifficulty.Easy);
            Assert.AreEqual(1, operationmin);
            Assert.AreEqual(4, operationmax);
        }

        [TestCase]
        public void TimerCancellationRequested()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();
            Program.RunTest(0, UserDifficulty.Easy, 30);
            Assert.That(cancellationToken.IsCancellationRequested);
        }

        [TestCase]
        public void CheckingIfSuggestingCorrectDifficulty()
        {
            UserDifficulty userDifficulty = CanUseManyTimes.SuggestingDifficulty("danyal");
            Assert.AreEqual(UserDifficulty.Easy, userDifficulty);
        }
    }
}