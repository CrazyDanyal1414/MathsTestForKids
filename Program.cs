using System;
using System.Collections.Generic;
using System.IO;
using static MathsTest.Calculation;
using static MathsTest.SaveLastTestResults;
using static MathsTest.UserLoginSignUp;
using static MathsTest.CanUseManyTimes;


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
					CanUseManyTimes.WriteToScreen($"To the nearest integer, What is {message} =", false);
				}
				else
				{
					CanUseManyTimes.WriteToScreen($"What is {message} =", false);
				}

				double userAnswer = Convert.ToDouble(CanUseManyTimes.ReadInput());
				if (Math.Round(correctAnswer) == userAnswer)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					CanUseManyTimes.WriteToScreen("Well Done!", false);
					Console.ResetColor();
					score.Increment(mathOperation, true);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					CanUseManyTimes.WriteToScreen("Your answer is incorrect!", false);
					Console.ResetColor();
					score.Increment(mathOperation, false);
				}
				numberOfQuestionsLeft--;
				RunWithTimer.StopTimer(numberOfQuestionsLeft);
			}
			return score;
		}

		static (UserDifficulty, int, string, int, string) UserInputs()
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

			do
			{
				Console.WriteLine("Please type '2' for 2 player and 'T' for test");
				testOrTwoPlayer = Console.ReadLine().ToUpper();
			} while (testOrTwoPlayer != "2" && testOrTwoPlayer != "T");

			do
			{
				Console.WriteLine("Would you like to continue with the suggested difficulty? Please type 'Y' or 'N'");
				autoDifficultyInput = Console.ReadLine().Substring(0).ToUpper();
			} while (autoDifficultyInput != "Y" && autoDifficultyInput != "N");

			if (autoDifficultyInput == "N")
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
			} while (numberOfSeconds % 30 != 0);

			return (userDifficulty, numberOfQuestions, autoDifficultyInput, numberOfSeconds, testOrTwoPlayer);
		}

		public static void Main(string[] args)
	    {
			var filePath = Path.Combine(AppContext.BaseDirectory, "AccountDetail.gitignore");
			(string userName, int LogInOrSignUp) = UserManager.LogInProcess(filePath);

			OperationQuestionScore score = new OperationQuestionScore();
			OperationQuestionScore playerOneScore = new OperationQuestionScore();
			OperationQuestionScore playerTwoScore = new OperationQuestionScore();

			UserDifficulty userSuggestingDifficulty = UserDifficulty.Easy;
			if (File.Exists(FileUtils.GetUserFileName(userName)))
			{
				userSuggestingDifficulty = CanUseManyTimes.SuggestingDifficulty(userName);
			}
            var (userDifficulty, numberOfQuestions, autoDifficultyInput, numberOfSeconds, testOrTwoPlayer) = UserInputs();
			string playerTwoUserName = "";

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
				ScoreDisplay(numberOfQuestions, score, playerOneScore, playerTwoScore, userDifficulty, userName);
				StatsDisplay(score);
				SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, userName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.PlayerOneTwoPlayerChallenge, score.PlayerTwoTwoPlayerChallenge);
			}
			else if (testOrTwoPlayer == "2")
            {
				Console.WriteLine($"Player 1: {userName}");
				Console.WriteLine($"What is Player 2's name?");
				(playerTwoUserName, _) = UserManager.LogInProcess(filePath);
				if (File.Exists(FileUtils.GetUserFileName(playerTwoUserName)))
				{
					SaveToFile.DeserializeLastTest(playerTwoUserName);
				}
				Console.WriteLine($"{userName} will go first!");
				playerOneScore = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{userName} got a score of {playerOneScore.PlayerOneScore} out of {numberOfQuestions}", false);
				ScoreDisplay(numberOfQuestions, score, playerOneScore, playerTwoScore, userDifficulty, userName);
				Console.WriteLine($"Now it is {playerTwoUserName}'s turn");
				string playerTwoReady;
				do
				{
					Console.WriteLine($"Are you ready {playerTwoUserName}?");
					playerTwoReady = Console.ReadLine().ToUpper();
				} while (playerTwoReady != "Y");
				playerTwoScore = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{playerTwoUserName} got a score of {playerTwoScore.PlayerTwoScore} out of {numberOfQuestions}", false);
				ScoreDisplay(numberOfQuestions, score, playerOneScore, playerTwoScore, userDifficulty, playerTwoUserName);
				SaveToFile.SerializeLastTest(numberOfQuestions, playerTwoScore.TotalScore, userDifficulty, playerTwoUserName, playerTwoScore.TotalEasyQuestion, playerTwoScore.TotalEasyScore, playerTwoScore.TotalNormalQuestion, playerTwoScore.TotalNormalScore, playerTwoScore.TotalHardQuestion, playerTwoScore.TotalHardScore, playerTwoScore.EasyTests, playerTwoScore.NormalTests, playerTwoScore.HardTests, playerTwoScore.PlayerOneTwoPlayerChallenge, playerTwoScore.PlayerTwoTwoPlayerChallenge);
				if (playerOneScore.PlayerOneScore > playerTwoScore.PlayerTwoScore)
                {
					Console.WriteLine($"{userName} won the challenge!🥳");
					playerOneScore.PlayerOneTwoPlayerChallenge++;
                }
				else if (playerOneScore.PlayerOneScore < playerTwoScore.PlayerTwoScore)
				{
					Console.WriteLine($"{playerTwoUserName} won the challenge!🥳");
					playerOneScore.PlayerTwoTwoPlayerChallenge++;
				}
                else
                {
					Console.WriteLine("This challenge ended in stalemate");
                }
				SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, userName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.PlayerOneTwoPlayerChallenge, score.PlayerTwoTwoPlayerChallenge);
				SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, playerTwoUserName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.PlayerOneTwoPlayerChallenge, score.PlayerTwoTwoPlayerChallenge);
				StatsDisplay(playerOneScore);
			}
		}
	}
}