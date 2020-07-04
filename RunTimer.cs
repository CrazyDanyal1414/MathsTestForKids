using System;
using System.Threading;
using System.Threading.Tasks;
using static MathsTest.CanUseManyTimes;

namespace MathsTest
{
    public class RunTimer
    {
		public bool IsTimeLeft { get; private set; } = true;
		static bool timerRunning = true;
		public static void Timer(int numberOfSeconds, CancellationToken cancellationToken)
		{
			var whenToStop = DateTime.Now.AddSeconds(numberOfSeconds);
			while (DateTime.Now < whenToStop)
			{
                while (timerRunning)
                {
					string timeLeft = (whenToStop - DateTime.Now).ToString(@"hh\:mm\:ss");
					WriteToScreen($"Time Remaining: {timeLeft}", true);
					Thread.Sleep(1000);
				}
			}
		}

		public Task timerTask;
		CancellationTokenSource cancellationToken = new CancellationTokenSource();
		public RunTimer(int numberOfSeconds)
		{
			timerTask = Task.Run(() =>
			{
				Timer(numberOfSeconds, cancellationToken.Token);
				timerTask = null;
				IsTimeLeft = false;
			}, cancellationToken.Token);
		}
		public void StopTimer(int numberOfQuestionsLeft)
		{
			if(numberOfQuestionsLeft == 0)
            {
				cancellationToken.Cancel();
			}
		}
	}
}
