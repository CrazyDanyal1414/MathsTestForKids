using MathsTest;
using NUnit.Framework;
using System.Threading;

namespace NUnitTesting
{
    public class NUnitTest
    {
        [Test]
        public void CheckIfGetPossibleOperationsByDifficultyWorks()
        {
            var (operationmin, operationmax) = Calculation.GetPossibleOperationsByDifficulty(UserDifficulty.Easy);
            Assert.AreEqual(1, operationmin);
            Assert.AreEqual(4, operationmax);
        }

        [Test]
        public void TimerCancellationRequested()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();
            Program.RunTest(0, UserDifficulty.Easy, 30);
            Assert.That(cancellationToken.IsCancellationRequested);
        }

        [Test]
        public void CheckingIfSuggestingCorrectDifficulty()
        {
            UserDifficulty userDifficulty = CanUseManyTimes.SuggestingDifficulty("danyal");
            Assert.AreEqual(UserDifficulty.Easy, userDifficulty);
        }
    }
}