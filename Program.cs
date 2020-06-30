using System;
using System.Collections.Generic;
using System.IO;
using static MathsTest.Calculation;
using static MathsTest.SaveLastTestResults;
using static MathsTest.UserLoginSignUp;


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
				CanUseManyTimes.ScoreDisplay(numberOfQuestions, score, userDifficulty, userName);
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
				OperationQuestionScore playerOneScore = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{userName} got a score of {playerOneScore.PlayerOneScore} out of {numberOfQuestions}");
				CanUseManyTimes.ScoreDisplay(numberOfQuestions, playerOneScore, userDifficulty, userName);
				Console.WriteLine($"Now it is {playerTwoUserName}'s turn");
				string playerTwoReady;
				do
				{
					Console.WriteLine($"Are you ready {playerTwoUserName}?");
					playerTwoReady = Console.ReadLine().ToUpper();
				} while (playerTwoReady != "Y");
				OperationQuestionScore playerTwoScore = RunTest(numberOfQuestions, userDifficulty, numberOfSeconds);
				Console.WriteLine($"{playerTwoUserName} got a score of {playerTwoScore.PlayerTwoScore} out of {numberOfQuestions}");
				CanUseManyTimes.ScoreDisplay(numberOfQuestions, playerTwoScore, userDifficulty, playerTwoUserName);
				if (playerOneScore.PlayerOneScore > playerTwoScore.PlayerTwoScore)
                {
					Console.WriteLine($"{userName} won the challenge!🥳");
					score.PlayerOneTwoPlayerChallenge++;
                }
				else if (playerOneScore.PlayerOneScore < playerTwoScore.PlayerTwoScore)
				{
					Console.WriteLine($"{playerTwoUserName} won the challenge!🥳");
					score.PlayerTwoTwoPlayerChallenge++;
				}
                else
                {
					Console.WriteLine("This challenge ended in stalemate");
                }
				score.TotalScore = playerOneScore.PlayerOneScore;
			}

			string statisticsDisplay;
			do
			{
				Console.WriteLine("Would you like to see your all time statistics? Please type 'Y' or 'N'");
				statisticsDisplay = Console.ReadLine();
			} while (statisticsDisplay != "Y" && statisticsDisplay != "N");
			if (statisticsDisplay == "Y")
            {
				Console.WriteLine($"You have answered {score.TotalEasyQuestion} easy questions so far with an average score of {score.TotalEasyScore}%");
				Console.WriteLine($"You have answered {score.TotalNormalQuestion} normal questions so far with an average score of {score.TotalNormalScore}%");
				Console.WriteLine($"You have answered {score.TotalHardQuestion} hard questions so far with an average score of {score.TotalHardScore}%");
				Console.WriteLine($"You have won {score.PlayerOneTwoPlayerChallenge} twoPlayerChallenges");
			}
		    SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, userName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.PlayerOneTwoPlayerChallenge);
			if (testOrTwoPlayer == "2")
            {
				SaveToFile.SerializeLastTest(numberOfQuestions, score.TotalScore, userDifficulty, playerTwoUserName, score.TotalEasyQuestion, score.TotalEasyScore, score.TotalNormalQuestion, score.TotalNormalScore, score.TotalHardQuestion, score.TotalHardScore, score.EasyTests, score.NormalTests, score.HardTests, score.PlayerOneTwoPlayerChallenge);
			}
		}
	}
}