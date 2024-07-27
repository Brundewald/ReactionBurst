using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.Pause
{
    public class View : BaseView
    {
        public event Action ContinueButtonPressed = delegate { };
        public event Action RestartButtonPressed = delegate { };
        public event Action InstructionButtonPressed = delegate { };
        public event Action LeaderTableButtonPressed = delegate { };
        public event Action ExitButtonPressed = delegate { };
        public event Action<bool> SFXToggled = delegate { };
        public event Action<bool> MusicToggled = delegate { };

        [Header("UI Components")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _instructionButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _settingsBackButton;
        [SerializeField] private Toggle _sfxToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Button _leaderTableButton;
        [SerializeField] private Visualization.Pause.Visualization _pauseVisualization;

        public override void Prepare<T>(T data)
        {
            var model = data as Model;
            
            _sfxToggle.isOn = model.SFXEnabled;
            _musicToggle.isOn = model.MusicEnabled;
            
            Assert.IsNotNull(model);
        }
        
        private void OnEnable()
        {
            SubscribeToEvents();

            _pauseVisualization.PlayOpenInitializationAsync().Forget();
        }

        private void OnDisable()
        {
            _pauseVisualization.PlayCloseInitializationAsync().Forget();

            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            _continueButton.onClick.AddListener(OnContinueButtonPressed);
            _restartButton.onClick.AddListener(OnRestartButtonPressed);
            _exitButton?.onClick.AddListener(OnExitButtonPressed);
            _instructionButton?.onClick.AddListener(OnInstructionButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
            _settingsBackButton.onClick.AddListener(OnSettingsBackButtonPressed);
            _sfxToggle.onValueChanged.AddListener(OnSFXValueChanged);
            _musicToggle.onValueChanged.AddListener(OnMusicValueChanged);
        }

        private void UnsubscribeFromEvents()
        {
            _continueButton.onClick.RemoveAllListeners();
            _restartButton.onClick.RemoveAllListeners();
            _exitButton?.onClick.RemoveAllListeners();
            _instructionButton?.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
            _settingsBackButton.onClick.RemoveAllListeners();
            _sfxToggle.onValueChanged.RemoveAllListeners();
            _musicToggle.onValueChanged.RemoveAllListeners();
        }

        public override void UpdateData(string dataName, object value)
        {
            if (dataName.Equals(Model.NeedCloseKey)) 
                OnCloseButtonPressed();

            if (dataName.Equals(Model.SFXEnabledKey)) 
                _sfxToggle.isOn = (bool) value;

            if (dataName.Equals(Model.MusicEnabledKey)) 
                _musicToggle.isOn = (bool) value;
        }

        private void OnSettingsButtonPressed() 
            => _pauseVisualization.SwitchToSettingsAsync().Forget();

        private void OnSettingsBackButtonPressed() 
            => _pauseVisualization.SwitchToPauseAsync().Forget();

        private void OnMusicValueChanged(bool state) => MusicToggled(state);

        private void OnSFXValueChanged(bool state) => SFXToggled(state);

        private void OnContinueButtonPressed() => ContinueButtonPressed();

        private void OnRestartButtonPressed() => RestartButtonPressed();

        private void OnInstructionButtonPressed() => InstructionButtonPressed();

        private void OnExitButtonPressed() => ExitButtonPressed();
    }
}