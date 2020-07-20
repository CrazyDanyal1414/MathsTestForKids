using System;
using System.IO;
using static MathsTest.SaveLastTestResults;

namespace MathsTest
{
    public class CanUseManyTimes
    {
		public static string ReadInput()
		{
			string input = Console.ReadLine();
			Console.Write(new string(' ', 100));
			Console.CursorLeft = 0;
			return input;
		}

		static readonly object lockObject = new object();

		public static void WriteToScreen(string message, bool resetCursor)
		{
			lock (lockObject)
			{
				if (resetCursor)
				{
					int leftPos = Console.CursorLeft;
					Console.WriteLine();
					Console.Write(message.PadRight(50, ' '));
					Console.CursorTop--;
					Console.CursorLeft = leftPos;
				}
				else
				{
					Console.WriteLine(message);
					Console.Write(new string(' ', 100));
					Console.CursorLeft = 0;
				}
			}
		}

		public static UserDifficulty SuggestingDifficulty(string userName)
		{
			ToFile objnew = SaveToFile.DeserializeLastTest(userName);
			UserDifficulty userDifficulty = UserDifficulty.Easy;

			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine($"Last time you did the test on {objnew.UserDifficulty} level and got {objnew.TotalScore}/{objnew.NumberOfQuestions}");
			Console.ResetColor();
			double decimalScore = objnew.TotalScore / (double)objnew.NumberOfQuestions;

			Console.ForegroundColor = ConsoleColor.Blue;
			if (objnew.UserDifficulty == UserDifficulty.Easy)
			{
				if (decimalScore <= 0.7)
				{
					Console.WriteLine($"You should stay on Easy difficulty");
					userDifficulty = UserDifficulty.Easy;
				}
				else
				{
					Console.WriteLine($"Easy difficulty seems to easy for you💪! You should go up to Normal difficulty");
					userDifficulty = UserDifficulty.Normal;
				}
			}
			else if (objnew.UserDifficulty == UserDifficulty.Normal)
			{
				if (decimalScore <= 0.3)
				{
					Console.WriteLine($"Normal difficulty seems to be to hard for you☹️. You should go down to Easy difficulty");
					userDifficulty = UserDifficulty.Easy;
				}
				else if ((decimalScore > 0.3) && (decimalScore <= 0.7))
				{
					Console.WriteLine($"You should stay on Normal difficulty");
					userDifficulty = UserDifficulty.Normal;
				}
				else
				{
					Console.WriteLine($"Normal difficulty seems to easy for you💪! You should go up to Hard difficulty");
					userDifficulty = UserDifficulty.Hard;
				}
			}
			else if (objnew.UserDifficulty == UserDifficulty.Hard)
			{
				if (decimalScore <= 0.3)
				{
					Console.WriteLine($"Hard difficulty seems to hard for you☹️. You should go down to Normal difficulty");
					userDifficulty = UserDifficulty.Normal;
				}
				else if ((decimalScore > 0.3) && (decimalScore <= 0.8))
				{
					Console.WriteLine($"You should stay on Hard difficulty");
					userDifficulty = UserDifficulty.Hard;
				}
				else
				{
					Console.WriteLine($"You are a maths Genius🥳! Sadly this is the hardest level");
					userDifficulty = UserDifficulty.Hard;
				}
			}
			Console.ResetColor();
			return userDifficulty;
		}

		public static void ScoreDisplay(int numberOfQuestions, Calculation.OperationQuestionScore score, UserDifficulty userDifficulty, string userName)
        {
			if (File.Exists(FileUtils.GetUserFileName(userName)))
			{
				ToFile objnew = SaveToFile.DeserializeLastTest(userName);
				score.TotalEasyQuestion = objnew.TotalEasyQuestion;
				score.TotalEasyScore = objnew.TotalEasyScore;
				score.TotalNormalQuestion = objnew.TotalNormalQuestion;
				score.TotalNormalScore = objnew.TotalNormalScore;
				score.TotalHardQuestion = objnew.TotalHardQuestion;
				score.TotalHardScore = objnew.TotalHardScore;
				score.EasyTests = objnew.EasyTests;
				score.NormalTests = objnew.NormalTests;
				score.HardTests = objnew.HardTests;
				score.TwoPlayerChallengeScore = objnew.TwoPlayerChallengeScore;
				score.AllTimeCorrectAnswers = objnew.AllTimeCorrectAnswers;
			}

			if (userDifficulty == UserDifficulty.Easy)
			{
				Console.WriteLine($"Addition score: {score.AdditionScore} of {score.AdditionQuestion}");
				Console.WriteLine($"Subtraction score: {score.SubtractionScore} of {score.SubtractionQuestion}");
				Console.WriteLine($"Multiplication score: {score.MultiplicationScore} of {score.MultiplicationQuestion}");
				score.EasyTests++;
				score.TotalEasyQuestion += numberOfQuestions;
				score.TotalEasyScore = Math.Round((score.TotalEasyScore + (double)(score.TotalScore / (double)numberOfQuestions) * 100) / score.EasyTests, 2);
			}
			else if (userDifficulty == UserDifficulty.Normal)
			{
				Console.WriteLine($"Addition score: {score.AdditionScore} of {score.AdditionQuestion}");
				Console.WriteLine($"Subtraction score: {score.SubtractionScore} of {score.SubtractionQuestion}");
				Console.WriteLine($"Multiplication score: {score.MultiplicationScore} of {score.MultiplicationQuestion}");
				Console.WriteLine($"Division score: {score.DivisionScore} of {score.DivisionQuestion}");
				score.NormalTests++;
				score.TotalNormalQuestion += numberOfQuestions;
				score.TotalNormalScore = Math.Round((score.TotalNormalScore + (double)(score.TotalScore / (double)numberOfQuestions) * 100) / score.NormalTests, 2);
			}
			else if (userDifficulty == UserDifficulty.Hard)
			{
				Console.WriteLine($"Multipication score: {score.MultiplicationScore} of {score.MultiplicationQuestion}");
				Console.WriteLine($"Division score: {score.DivisionScore} of {score.DivisionQuestion}");
				Console.WriteLine($"Power score: {score.PowerScore} of {score.PowerQuestion}");
				Console.WriteLine($"Squareroot score: {score.SquareRootScore} of {score.SquareRootQuestion}");
				score.HardTests++;
				score.TotalHardQuestion += numberOfQuestions;
				score.TotalHardScore = Math.Round((score.TotalHardScore + (double)(score.TotalScore / (double)numberOfQuestions) * 100) / score.HardTests, 2);
			}
			score.AllTimeCorrectAnswers += score.TotalScore;
			Console.WriteLine("\n");
		}

		public static void StatsDisplay(Calculation.OperationQuestionScore score)
		{
			string statisticsDisplay;
			do
			{
				Console.WriteLine("Would you like to see your all time statistics? Please type 'Y' or 'N'");
				statisticsDisplay = Console.ReadLine().ToUpper();
			} while (statisticsDisplay != "Y" && statisticsDisplay != "N");
			if (statisticsDisplay == "Y")
			{
				Console.WriteLine($"You have answered {score.TotalEasyQuestion} easy questions so far with an average score of {score.TotalEasyScore}%");
				Console.WriteLine($"You have answered {score.TotalNormalQuestion} normal questions so far with an average score of {score.TotalNormalScore}%");
				Console.WriteLine($"You have answered {score.TotalHardQuestion} hard questions so far with an average score of {score.TotalHardScore}%");
				Console.WriteLine($"You have won {score.TwoPlayerChallengeScore} twoPlayerChallenges");
				Console.WriteLine($"You have gotten {score.AllTimeCorrectAnswers} answers correct so far!");
			}
		}
	}
}
