﻿using System;
using System.Threading;
using System.Threading.Tasks;
using static MathsTest.CanUseManyTimes;

namespace MathsTest
{
    public class RunTimer
    {
		public bool IsTimeLeft { get; private set; } = true;
		public static void Timer(int numberOfSeconds)
		{
			var whenToStop = DateTime.Now.AddSeconds(numberOfSeconds);
			while (DateTime.Now < whenToStop)
			{
				string timeLeft = (whenToStop - DateTime.Now).ToString(@"hh\:mm\:ss");
				WriteToScreen($"Time Remaining: {timeLeft}", true);
				Thread.Sleep(1000);
			}
			if (DateTime.Now >= whenToStop)
            {
				cancellationTokenSource.Cancel();
            }
		}

		static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		readonly CancellationToken cancellationToken = cancellationTokenSource.Token;
		public Task timerTask;
		public RunTimer(int numberOfSeconds)
		{
			timerTask = Task.Run(() =>
			{
				while (!cancellationToken.IsCancellationRequested)
                {
					Timer(numberOfSeconds);
				}
				timerTask = null;
				IsTimeLeft = false;
				cancellationTokenSource.Dispose();
				cancellationTokenSource = new CancellationTokenSource();
			}, cancellationToken);
		}
		public void StopTimer(int numberOfQuestionsLeft)
		{
			if(numberOfQuestionsLeft == 0)
            {
				cancellationTokenSource.Cancel();
			}
		}
	}
}