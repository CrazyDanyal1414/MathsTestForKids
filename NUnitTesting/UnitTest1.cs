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
            int numberOfSeconds = 30;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Program.RunTest(0, UserDifficulty.Easy, numberOfSeconds);
            Assert.That(numberOfSeconds <= 30 && numberOfSeconds >= 29);
        }

        [Test]
        public void CheckingIfSuggestingCorrectDifficulty()
        {
            UserDifficulty userDifficulty = CanUseManyTimes.SuggestingDifficulty("danyal");
            Assert.AreEqual(UserDifficulty.Easy, userDifficulty);
        }

        [Test]
        public void CheckIfNumber1AndNumber2AreInRange()
        {
            (string message, double correctAnswer) = Calculation.GetMathsEquation(MathOperation.Multiplication, UserDifficulty.Easy);
            Assert.That(correctAnswer < 144 && correctAnswer >= 0);
        }
    }
}