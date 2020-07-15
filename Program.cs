using System;
using System.IO;
using System.Collections.Generic;
using static MathsTest.Calculation;
using static MathsTest.UserLoginSignUp;
using static MathsTest.CanUseManyTimes;
using static MathsTest.SaveLastTestResults;

namespace MathsTest
{
    class Program
	{
		public static OperationQuestionScore RunTest(int numberOfQuestionsLeft, UserDifficulty userDifficulty, int numberOfSeconds)
		{
			Random random = new Random();
			var (operationMin, operationMax) = GetPossibleOperationsByDifficulty(userDifficulty);
			var score = new OperationQuestionScore();
			RunTimer RunWithTimer = new RunTimer(numberOfSeconds);

			while (numberOfQuestionsLeft > 0 && RunWithTimer.IsTimeLeft)
			{
				int mathRandomOperation = random.Next(operationMin, operationMax);
				MathOperation mathOperation = (MathOperation)mathRandomOperation;
				var (message, correctAnswer) = GetMathsEquation(mathOperation, userDifficulty);
				if (mathRandomOperation == 4 || mathRandomOperation == 6)
				{
					WriteToScreen($"To the nearest integer, What is {message} =", false);
				}
				else
				{
					WriteToScreen($"What is {message} =", false);
				}

				double userAnswer = Convert.ToDouble(ReadInput());
				if (Math.Round(correctAnswer) == userAnswer)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					WriteToScreen("Well Done!", false);
					Console.ResetColor();
					score.Increment(mathOperation, true);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					WriteToScreen("Your answer is incorrect!", false);
					Console.ResetColor();
					score.Increment(mathOperation, false);
				}
				numberOfQuestionsLeft--;
				RunWithTimer.StopTimer(numberOfQuestionsLeft);
			}
			return score;
		}

		static (UserDifficulty, int, string, int, string) UserInputs(string userName)
		{
			Dictionary<string, UserDifficulty> difficultyDictionary = new Dictionary<string, UserDifficulty>
            {
                { "E", UserDifficulty.Easy },
                { "N", UserDifficulty.Normal },
                { "H", UserDifficulty.Hard }
            };

            string userInputDifficulty = "E";
			int numberOfQuestions;
			string autoDifficultyInput = "";
			int numberOfSeconds;
			string testOrTwoPlayer;

			Console.ForegroundColor = ConsoleColor.Magenta;
			do
			{
				Console.WriteLine("Please type '2' for 2 player and 'T' for test");
				testOrTwoPlayer = Console.ReadLine().ToUpper();
			} while (testOrTwoPlayer != "2" && testOrTwoPlayer != "T");

			if (File.Exists(FileUtils.GetUserFileName(userName)))
            {
				do
				{
					Console.WriteLine("Would you like to continue with the suggested difficulty? Please type 'Y' or 'N'");
					autoDifficultyInput = Console.ReadLine().Substring(0).ToUpper();
				} while (autoDifficultyInput != "Y" && autoDifficultyInput != "N");
			}

			if (autoDifficultyInput != "Y")
			{
				do
				{
					Console.WriteLine("Which difficulty level would you like to do! Please type E for Easy, N for Normal and H for Hard");
					userInputDifficulty = Console.ReadLine().ToUpper();
				} while (userInputDifficulty != "E" && userInputDifficulty != "N" && userInputDifficulty != "H");
			}
			UserDifficulty userDifficulty = difficultyDictionary[userInputDifficulty];
			do
			{
				Console.WriteLine("How many questions would you like to answer? Please type a number divisible by 10!");
				int.TryParse(Console.ReadLine(), out numberOfQuestions);
			} while (numberOfQuestions % 10 != 0);

			do
			{
				Console.WriteLine("How many seconds would you like the test to be? Please type a number divisible by 30!");
				int.TryParse(Console.ReadLine(), out numberOfSeconds);
			} while (numberOfSeconds % 10 != 0);
			Console.ResetColor();

			return (userDifficulty, numberOfQuestions, autoDifficultyInput, numberOfSeconds, testOrTwoPlayer);
		}

		public static void Main(string[] args)
	    {
	        var filePath = Path.Combine(AppContext.BaseDirectory, "AccountDetail.gitignore");
			(string userName, int LogInOrSignUp) = UserManager.LogInProcess(filePath);

			OperationQuestionScore score;
			OperationQuestionScore playerTwoScore;

			UserDifficulty userSuggestingDifficulty = UserDifficulty.Easy;
			if (File.Exists(FileUtils.GetUserFileName(userName)))
			{
				userSuggestingDifficulty = SuggestingDifficulty(userName);
			}
            var (userDifficulty, numberOfQuestions, autoDifficultyInput, numberOfSeconds, testOrTwoPlayer) = UserInputs(userName);
			string playerTwoUserName;
			int playerTwoLogInOrSignUp;

			if (LogInOrSignUp == 1)
			{
				if (autoDifficultyInput == "Y")
				{
					userDifficulty = userSuggestingDifficulty;
				}
			}

			if (testOrTwoPlayer == "T")
			{
				score = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);

				Console.WriteLine($"Total score: {score.TotalScore} of {numberOfQuestions}");
				ScoreDisplay(numberOfQuestions, score, userDifficulty, userName);
                if (LogInOrSignUp != 3)
                {
					StatsDisplay(score);
					SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, userName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.TwoPlayerChallengeScore);
				}
			}
			else if (testOrTwoPlayer == "2")
            {
				Console.WriteLine($"Player 1: {userName}");
				Console.WriteLine($"What is Player 2's name?");
				(playerTwoUserName, playerTwoLogInOrSignUp) = UserManager.LogInProcess(filePath);
				if (File.Exists(FileUtils.GetUserFileName(playerTwoUserName)))
				{
					SaveToFile.DeserializeLastTest(playerTwoUserName);
				}
				Console.WriteLine($"{userName} will go first!");
				score = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{userName} got a score of {score.PlayerOneScore} out of {numberOfQuestions}", false);
				ScoreDisplay(numberOfQuestions, score, userDifficulty, userName);
				Console.WriteLine($"Now it is {playerTwoUserName}'s turn");
				string playerTwoReady;
				do
				{
					Console.WriteLine($"Are you ready {playerTwoUserName}?");
					playerTwoReady = Console.ReadLine().ToUpper();
				} while (playerTwoReady != "Y");
				playerTwoScore = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{playerTwoUserName} got a score of {playerTwoScore.PlayerTwoScore} out of {numberOfQuestions}", false);
				ScoreDisplay(numberOfQuestions, playerTwoScore, userDifficulty, playerTwoUserName);
				if (score.TotalScore > playerTwoScore.TotalScore)
                {
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"{userName} won the challenge!🥳");
					Console.ResetColor();
					score.TwoPlayerChallengeScore++;
                }
				else if (score.TotalScore < playerTwoScore.TotalScore)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine($"{playerTwoUserName} won the challenge!🥳");
					Console.ResetColor();
					playerTwoScore.TwoPlayerChallengeScore++;
				}
                else
                {
					Console.WriteLine("This challenge ended in stalemate");
                }

				if (LogInOrSignUp != 3)
                {
					SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, userName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.TwoPlayerChallengeScore);
					StatsDisplay(score);
				}
				if (playerTwoLogInOrSignUp != 3)
                {
					SaveToFile.SerializeLastTest(numberOfQuestions, playerTwoScore.TotalScore, userDifficulty, playerTwoUserName, playerTwoScore.TotalEasyQuestion, playerTwoScore.TotalEasyScore, playerTwoScore.TotalNormalQuestion, playerTwoScore.TotalNormalScore, playerTwoScore.TotalHardQuestion, playerTwoScore.TotalHardScore, playerTwoScore.EasyTests, playerTwoScore.NormalTests, playerTwoScore.HardTests, playerTwoScore.TwoPlayerChallengeScore);
				}
			}
		}
	}
}