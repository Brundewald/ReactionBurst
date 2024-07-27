using System;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace MyProject.ReactionBurst.UI.NavigationPanel
{
    public class View : BaseView
    {
        public event Action PauseButtonPressed = delegate { };
        public event Action InfoButtonPressed = delegate { };

        [Header("UI Components")]
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _infoButton;
        [SerializeField] private GameObject _categoryInfoContainer;
        [SerializeField] private TextMeshProUGUI _categoryNameText;
        [SerializeField] private Image _categoryIconImage;

        public override void Prepare<T>(T data)
        {
            var model = data as Model;
            Assert.IsNotNull(model);
        }
        
        private void OnEnable()
        {
            _pauseButton.onClick.AddListener(OnPauseButtonPressed);
            _infoButton?.onClick.AddListener(OnInfoButtonPressed);
        }
        
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveAllListeners();
            _infoButton?.onClick.RemoveAllListeners();
        }


        public override void UpdateData(string dataName, object value)
        {
            if (dataName.Equals("NeedClose"))
            {
                OnCloseButtonPressed();
            }
            else if (dataName.Equals("CategoryName"))
            {
                LocalizedString categoryName  = value as LocalizedString;
                _categoryNameText.text = categoryName.GetLocalizedString();
            }
        }

        private void OnPauseButtonPressed() => PauseButtonPressed();
        
        private void OnInfoButtonPressed() => InfoButtonPressed();

        public void EnableButtons(bool isEnabled)
        {
            _pauseButton.gameObject.SetActive(isEnabled);
            _infoButton?.gameObject.SetActive(isEnabled);
        }
    }
}