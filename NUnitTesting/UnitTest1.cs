using MathsTest;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTesting
{
    public class NUnitTests
    {
        [Test]
        public void CheckIfGetPossibleOperationsByDifficultyWorks()
        {
            var (operationmin, operationmax) = Calculation.GetPossibleOperationsByDifficulty(UserDifficulty.Easy);
            Assert.AreEqual(1, operationmin);
            Assert.AreEqual(4, operationmax);
        }

        [Test]
        public void CheckIfThereIsTimeLeft()
        {
            RunTimer RunWithTimer = new RunTimer(0);
            Thread.Sleep(1);
            Assert.AreEqual(RunWithTimer.IsTimeLeft, false);
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