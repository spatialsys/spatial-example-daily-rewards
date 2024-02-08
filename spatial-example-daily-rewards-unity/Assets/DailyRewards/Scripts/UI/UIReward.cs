using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class UIReward : MonoBehaviour
    {
        [Header("Root")]
        [SerializeField] private GameObject _sideButtonRoot;
        [SerializeField] private GameObject _loadingImage;

        [Header("Side Buttons")]
        [SerializeField] private Button _buttonDayStreak; // Day-Streak
        [SerializeField] private Button _buttonDaily; // Daily Reward
        [SerializeField] private Button _buttonWeekly; // Weekly Reward
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewDayStreak;
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewDaily;
        [SerializeField] private RewardUnclaimedView _rewardUnclaimedViewWeekly;

        [Header("Reward List Window")]
        [SerializeField] private GameObject _rootListWindow;
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _buttonViewReference;
        private List<RewardButtonView> _buttonViewsPool = new List<RewardButtonView>(); // object pooling
        private List<RewardButtonView> _currentButtonViews = new List<RewardButtonView>();

        private RewardType _currentRewardType = RewardType.None;

        private void Awake()
        {
            _buttonDayStreak.onClick.AddListener(() => OnSideButtonClicked(RewardType.DayStreak));
            _buttonDaily.onClick.AddListener(() => OnSideButtonClicked(RewardType.DailyMissions));
            _buttonWeekly.onClick.AddListener(() => OnSideButtonClicked(RewardType.WeeklyMissions));

            _currentRewardType = RewardType.None;
            _rootListWindow.gameObject.SetActive(false);
            _buttonViewReference.SetActive(false);

            _sideButtonRoot.SetActive(false);
            _loadingImage.SetActive(true);
        }
        private void OnDestroy()
        {
            _buttonDayStreak.onClick.RemoveAllListeners();
            _buttonDaily.onClick.RemoveAllListeners();
            _buttonWeekly.onClick.RemoveAllListeners();
        }

        // Remove loading image and show side buttons
        public void OnDataStoreLoaded()
        {
            _sideButtonRoot.SetActive(true);
            _loadingImage.SetActive(false);
        }

        public void RefreshRewardButtons()
        {
            _rewardUnclaimedViewDayStreak.Setup(RewardDataManager.GetRewardUnclaimedCount(RewardType.DayStreak));
            _rewardUnclaimedViewDaily.Setup(RewardDataManager.GetRewardUnclaimedCount(RewardType.DailyMissions));
            _rewardUnclaimedViewWeekly.Setup(RewardDataManager.GetRewardUnclaimedCount(RewardType.WeeklyMissions));

            if (_currentRewardType == RewardType.None)
            {
                return;
            }
            foreach (var buttonView in _currentButtonViews)
            {
                buttonView.UpdateStatus();
            }
        }

        // Show or hide reward list window
        // and update the list of reward buttons
        private void OnSideButtonClicked(RewardType rewardType)
        {
            if (_currentRewardType == rewardType)
            {
                _currentRewardType = RewardType.None;
                _rootListWindow.gameObject.SetActive(false);
                return;
            }

            _currentRewardType = rewardType;
            _rootListWindow.gameObject.SetActive(_currentRewardType != RewardType.None);

            if (_currentRewardType == RewardType.None)
            {
                return;
            }

            // Deactivate all button views
            foreach (var view in _buttonViewsPool)
            {
                view.gameObject.SetActive(false);
            }

            RewardDataObject[] rewards = RewardDataManager.GetRewardDataGroupByType(_currentRewardType).rewards;

            _currentButtonViews.Clear();
            for (int i = 0; i < rewards.Length; i++)
            {
                RewardButtonView view = GetRewardButtonViewFromPool();
                _currentButtonViews.Add(view);
                view.Setup(rewards[i], buttonAction: OnRewardButtonClicked);
                view.gameObject.SetActive(true);
            }
        }

        private RewardButtonView GetRewardButtonViewFromPool()
        {
            RewardButtonView view = _buttonViewsPool.Find(x => !x.gameObject.activeSelf);
            if (view == null)
            {
                view = Instantiate(_buttonViewReference, _content).GetComponent<RewardButtonView>();
                _buttonViewsPool.Add(view);
            }
            return view;
        }

        // ButtonClicked -> Claim reward if progress is achieved
        // GameManager will update the reward status, save the progress and buttons.
        private void OnRewardButtonClicked(RewardButtonView buttonView)
        {
            RewardDataObject rewardData = buttonView.rewardData;
            if (rewardData.status == RewardDataObject.Status.Achieved)
            {
                GameManager.ClaimReward(_currentRewardType, rewardData);
            }
        }
    }
}