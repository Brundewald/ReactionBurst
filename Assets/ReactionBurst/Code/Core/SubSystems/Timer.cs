using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace MyProject.ReactionBurst.Core.SubSystems
{
    public sealed class Timer
    {
        private CancellationTokenSource _internalCancellation;
        private bool _timerElapsed;
        private int _timeLeft;
        private bool _isPaused;

        public event Action<int> TimeChanged = delegate {  }; 

        public async void RunTimer(int countdownTime)
        {
            _internalCancellation = new CancellationTokenSource();
            _timerElapsed = false;
            
            _timeLeft = countdownTime;

            if(_isPaused) return;

            for (int i = countdownTime; i > 0; i--)
            {
                TimeChanged.Invoke(_timeLeft);
                _timeLeft--;
                
                var isCanceled = await UniTask
                    .Delay(TimeSpan.FromSeconds(1), DelayType.DeltaTime, PlayerLoopTiming.Update, _internalCancellation.Token)
                    .SuppressCancellationThrow();
                
                if (isCanceled)
                    break;
            }

            _timerElapsed = _timeLeft <= 0;
        }

        public void Pause()
        {
            _isPaused = true;
            _internalCancellation?.Cancel();
        }

        public void Continue()
        {
            _isPaused = false;
            _internalCancellation = new CancellationTokenSource();
            RunTimer(_timeLeft);
        }
        
        
        public async UniTask WaitForElapse()
        {
            await UniTask.WaitUntil(() => _timerElapsed);
        }

        public void StopTimer()
        {
            _internalCancellation?.Cancel();
            _internalCancellation = null;
            _isPaused = false;
            _timerElapsed = true;
        }
    }
}