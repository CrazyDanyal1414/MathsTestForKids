using System;
using System.Threading;
using System.Threading.Tasks;
using static MathsTest.CanUseManyTimes;

namespace MathsTest
{
    public class RunTimer
    {
		public bool IsTimeLeft { get; private set; } = true;
		static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        readonly CancellationToken cancellationToken = cancellationTokenSource.Token;
		public static void Timer(int numberOfSeconds, CancellationToken cancellationToken)
		{
			var whenToStop = DateTime.Now.AddSeconds(numberOfSeconds);
            while (!cancellationToken.IsCancellationRequested)
            {
				while (DateTime.Now < whenToStop)
				{
					string timeLeft = (whenToStop - DateTime.Now).ToString(@"hh\:mm\:ss");
					WriteToScreen($"Time Remaining: {timeLeft}", true);
					Thread.Sleep(1000);
				}
			}
		}

		public Task timerTask;
		public RunTimer(int numberOfSeconds)
		{
			timerTask = Task.Run(() =>
			{
				Timer(numberOfSeconds, cancellationToken);
				timerTask = null;
				IsTimeLeft = false;
			}, cancellationToken);
		}
		public void StopTimer(int numberOfQuestionsLeft)
		{
			if(numberOfQuestionsLeft == 0)
            {
				cancellationToken.ThrowIfCancellationRequested();
			}
		}
	}
}
