using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MyProject.ReactionBurst.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.EndGame
{
    public class View : BaseView
    {
        private readonly List<LeaderBoardDataView> _leaderBoardDataViews = new List<LeaderBoardDataView>();
        
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _leaderBoardButton;
        [SerializeField] private Button _resultButton;

        [SerializeField] private TextMeshProUGUI _highestScore;
        [SerializeField] private TextMeshProUGUI _playerScore;
        [SerializeField] private TextMeshProUGUI _playerLeaderboardScore;

        [SerializeField] private LeaderBoardDataView _leaderBoardPrefab;
        [SerializeField] private ScrollView _scrollView;
        [SerializeField] private Visualization _visualization;


        public event Action ExitButtonPressed = delegate { };
        public event Action RetryButtonPressed = delegate { };
        
        
        public override void Prepare<T>(T data)
        {
            base.Prepare(data);
            var model = data as Model;
        }

        public UniTask WarmUpScrollAsync(int scrollSize)
        {
            for (var i = 0; i < scrollSize; i++)
            {
                var instance = Instantiate(_leaderBoardPrefab);
                _leaderBoardDataViews.Add(instance);
            }
            
            _scrollView.FillScroll(_leaderBoardDataViews);
            return UniTask.CompletedTask;
        }

        public void OnEnable()
        {
            _exitButton.onClick.AddListener(OnExitButtonClick);
            _retryButton.onClick.AddListener(OnRetryButtonClick);
            _leaderBoardButton.onClick.AddListener(OnLeaderBoardButtonClick);
            _resultButton.onClick.AddListener(OnResultButtonClick);
        }
        
        public void OnDisable()
        { 
            _exitButton.onClick.RemoveListener(OnExitButtonClick);
            _retryButton.onClick.RemoveListener(OnRetryButtonClick);
            _leaderBoardButton.onClick.RemoveListener(OnLeaderBoardButtonClick);
            _resultButton.onClick.RemoveListener(OnResultButtonClick);
        }

        public async UniTask OpenAnimationAsync() => await _visualization.PlayOpenVisualizationAsync();

        public async UniTask CloseAnimationAsync() => await _visualization.PlayCloseVisualizationAsync();

        public override void UpdateData(string dataName, object value)
        {
            if (dataName == nameof(Model.LeaderboardData)) 
                UpdateLeaderboardData((LeaderBoardData[])value);

            if (dataName == nameof(Model.PlayerLeaderboardData)) 
                UpdatePlayerLeaderboardScore((LeaderBoardData)value);
        }


        private void UpdateLeaderboardData(LeaderBoardData[] boardDataArray)
        {
            for (var i = 0; i < boardDataArray.Length; i++)
            {
                _leaderBoardDataViews[i].SetLeaderBoardData(boardDataArray[i]);
            }
        }

        private void UpdatePlayerLeaderboardScore(LeaderBoardData leaderBoardScore)
        {
            _playerScore.text = leaderBoardScore.HighestScore.ToString();
            _playerLeaderboardScore.text = leaderBoardScore.LeaderboardNumber.ToString();
        }


        private void OnExitButtonClick() => ExitButtonPressed.Invoke();

        private void OnRetryButtonClick() => RetryButtonPressed.Invoke();

        private async void OnResultButtonClick()
        {
            await _visualization.PlaySwitchToResultAsync();
        }
        private async void OnLeaderBoardButtonClick()
        {
            await _visualization.PlaySwitchToLeaderBoardAsync();
            //_scrollView.MoveItemAsync();
        }
    }
}