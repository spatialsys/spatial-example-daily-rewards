using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class UIManager : MonoBehaviour
    {
        private static readonly int ANIM_TRIGGER_NORMAL = Animator.StringToHash("Pressed");

        [Header("Reward UI")]
        [SerializeField] private UIReward _reward;
        public UIReward reward => _reward;

        [Header("Score UI")]
        [SerializeField] private TextMeshProUGUI _textScore;
        [SerializeField] private Animator _animatorScore;

        [Header("Clicker Button")]
        [SerializeField] private Button _buttonClick;

        private void Awake()
        {
            _buttonClick.onClick.AddListener(OnClickerButtonClicked);
        }
        private void OnDestroy()
        {
            _buttonClick.onClick.RemoveAllListeners();
        }
        private void OnClickerButtonClicked()
        {
            GameManager.OnClickerButtonClicked();
        }

        public void UpdateScoreUI(int score)
        {
            _textScore.text = score.ToString("N0");
            _animatorScore.SetTrigger(ANIM_TRIGGER_NORMAL);
        }
    }
}