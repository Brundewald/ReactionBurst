using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.Constants;
using MyProject.ReactionBurst.Core.SubSystems;
using MyProject.ReactionBurst.Data;
using MyProject.ReactionBurst.UI;
using UnityEngine.Assertions;
using Timer = MyProject.ReactionBurst.Core.SubSystems.Timer;

namespace MyProject.ReactionBurst.Core
{
    public sealed class GameplayState
    {
        private readonly IUIService _uiService;
        private readonly GameConfig _gameConfig;
        private readonly IAudioService _audioService;
        private readonly Timer _timer;
        private readonly GeneratorService _generatorService;
        private readonly CompareService _compareService;

        private UI.Gameplay.Presenter _gamePlayPresenter;
        private UI.TopBar.Presenter _topBarPresenter;

        private int _currentDifficult;
        private int _score;
        private bool _playerAction;
        private bool _gameSkipped;
        private int _modifier = 1;
        private int _totalAnswers;
        private int _totalCorrectAnswers;

        public GameplayState(IUIService uiService, GameConfig gameConfig, IAudioService audioService)
        {
            _uiService = uiService;
            _gameConfig = gameConfig;
            _audioService = audioService;
            
            _timer = new Timer();
            _generatorService = new GeneratorService(_gameConfig);
            _compareService = new CompareService();
        }

        public async UniTask<(bool, int)> StartAsync(CancellationToken token)
        {
            await PrepareFlowAsync();
            await GameLoopAsync(token).SuppressCancellationThrow();
            return (token.IsCancellationRequested, await ExitState());
        }
        
        public void Pause() => 
            _timer.Pause();

        public void Continue() => 
            _timer.Continue();

        public void Cancel()
        {
            HideUI();
            _timer.StopTimer();
        }
        
        private async UniTask PrepareFlowAsync()
        {
            _gamePlayPresenter = await _uiService.ConstructWindowAsync<UI.Gameplay.Presenter>(NameConstants.GameplayView);
            Assert.IsNotNull(_gamePlayPresenter, nameof(_gamePlayPresenter) + " is null");
            
            _topBarPresenter = await _uiService.ConstructWindowAsync<UI.TopBar.Presenter>(NameConstants.TopBarView);
            Assert.IsNotNull(_topBarPresenter, nameof(_topBarPresenter) + " is null");

            SetGameplayData();   
            SubscribeToEvents();
        }

        private void SetGameplayData()
        {
            _score = 0;
            _modifier = 1;
            _totalAnswers = 0;
            _totalCorrectAnswers = 0;
            //_uiDataProvider.SetNotation(_gameData.Notation);
            _topBarPresenter.SetAddScoreValue(_gameConfig.DefaultScore);
        }

        private async UniTask GameLoopAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(250, cancellationToken: cancellationToken);
            
            await SetupView();
            _topBarPresenter.UpdateTimer(_gameConfig.LevelDurationInSeconds);
            ShowUI();

            _timer.RunTimer(_gameConfig.LevelDurationInSeconds);
            await _timer.WaitForElapse(); 
        }

        private async UniTask<int> ExitState()
        {
            HideUI();

            _compareService.Clear();
            UnsubscribeFormEvents();

            return _score;
        }

        private async UniTask SetupView()
        {
            var initialFiguresPool = await _generatorService.GenerateFiguresPoolAsync();
            _gamePlayPresenter.SetInitialPool(initialFiguresPool);
            _compareService.SetComparable(initialFiguresPool.First());
        }

        private async UniTask SetupNextFigureAsync()
        {
            var figure = await _generatorService.GetComparableFigureAsync();
            _gamePlayPresenter.SetFigureAsync(figure);
        }

        private async void HandleAnswerButtonPressed(bool playerAnswer, ComparableFigure figure)
        {
            _totalAnswers++;
            //_audioService.PlaySound(SharedSFXNames.Click).Forget();

            var sameFigures = await _compareService.Compare(figure);

            var answerIsCorrect = (playerAnswer && sameFigures) || (!playerAnswer && !sameFigures);

            if (answerIsCorrect)
                _totalCorrectAnswers++;
            
            var updateModifier = _totalCorrectAnswers > 1 
                                 && _totalCorrectAnswers % _gameConfig.ScoreIncreaseSequenceLength == 0;
            
            var updatePoolRange = _totalAnswers> 1 
                                  && _totalAnswers % _gameConfig.ScoreIncreaseSequenceLength == 0;
            
            if (updateModifier && answerIsCorrect)
            {
                _modifier++;
                //_audioService.PlaySound(SharedSFXNames.CorrectCombo).Forget();
            }


            if(answerIsCorrect)
            {
                var addScore = _gameConfig.DefaultScore * _modifier;
                _score += addScore;
                
                _topBarPresenter.UpdateScore(_score);
                _topBarPresenter.SetAddScoreValue(addScore);
                if(updatePoolRange)
                    _generatorService.IncreaseGeneratorRange();
            }
            else
            {
                if (_modifier > 1)
                    _modifier--;
                
                _generatorService.DecreaseGeneratorRange();
            }
            
            
            _gamePlayPresenter.SetScoreModifier(_modifier);
            //_audioService.PlaySound(answerIsCorrect ? SharedSFXNames.CorrectAnswer : SharedSFXNames.WrongAnswer).Forget();
            
            await _gamePlayPresenter.PlayAnswerSequenceAsync(answerIsCorrect);        
            
            await SetupNextFigureAsync();
            await _gamePlayPresenter.UpdateScrollPositionAsync();
        }

        private void SubscribeToEvents()
        {
            _gamePlayPresenter.AnswerButtonClicked += HandleAnswerButtonPressed;
            _timer.TimeChanged += _topBarPresenter.UpdateTimer;
        }

        private void UnsubscribeFormEvents()
        {
            if(_gamePlayPresenter != null)
                _gamePlayPresenter.AnswerButtonClicked -= HandleAnswerButtonPressed;
            
            _timer.TimeChanged -= _topBarPresenter.UpdateTimer;
        }

        private void ShowUI()
        {
            _uiService.ShowWindow<UI.Gameplay.Presenter>();
            _uiService.ShowWindow<UI.TopBar.Presenter>();
        }

        private void HideUI()
        {
            _uiService.HideWindow<UI.Gameplay.Presenter>();
            _uiService.HideWindow<UI.TopBar.Presenter>();
        }
    }
}
