using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace DailyRewards
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public const int DATE_TIME_OFFSET = 19; // Change day at 7 PM
        public const DayOfWeek DATE_RESET_WEEKLY = DayOfWeek.Friday; // Reset weekly at Friday
        private const int SECOND_TO_NEXT_DAY = 86400; // 24 * 60 * 60
        private const int SECOND_TO_NEXT_WEEK = 604800; // 7 * 24 * 60 * 60

        [Header("Game Data")]
        public static int score = 0;
        public static int dayStreak = 1;
        public static int totalClicksDay = 0;
        public static int totalClicksWeek = 0;
        public static DateTime dateTimeLastLogin;

        public static Action onDataStoreLoaded;

        [Header("References")]
        [SerializeField] private DataStoreManager _dataStoreManager;
        [SerializeField] private RewardDataManager _rewardDataManager;
        [SerializeField] private UIManager _uiManager;

        // [Header("Test")]
        // [SerializeField] private bool _isTest = false;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Start()
        {
            // Initialize all editor changes
            // Warning: This should happen before loading the data store. Otherwise, user data will be overwritten
            _rewardDataManager.InitializeData();

            // Load data store
            // if (_isTest)
            // {
            //     score = 0;
            //     totalClicksDay = 0;
            //     totalClicksWeek = 0;
            //     dateTimeLastLogin = DateTime.Now;
            //     dayStreak = 1;
            //     OnDataStoreLoaded();
            // }
            // else
            {
                _dataStoreManager.LoadData(OnDataStoreLoaded);
            }
        }

        // After data loading finished
        private void OnDataStoreLoaded()
        {
            UpdateTimePassed();

            // Reset daily and weekly data while playing
            int secondsToNextDay = DateTimeUtilities.SecondsToNextDay(dateTimeLastLogin, -DATE_TIME_OFFSET);
            StartCoroutine(ResetDailyCoroutine(secondsToNextDay));

            onDataStoreLoaded?.Invoke();
        }

        // Compare last login time with current time
        // And update day streak and daily/weekly missions
        private void UpdateTimePassed()
        {
            _instance.UpdateTimePassed(DateTime.Now, dateTimeLastLogin);
        }
        public static void UpdateTimePassedDebug(DateTime now, DateTime lastLogin)
        {
            _instance.UpdateTimePassed(now, lastLogin);
        }
        public void UpdateTimePassed(DateTime now, DateTime lastLogin)
        {
            int dayPassed = DateTimeUtilities.GetDayPassed(now, lastLogin, -DATE_TIME_OFFSET);
            if (dayPassed >= 1)
            {
                bool isWeekChanged = DateTimeUtilities.IsWeekChanged(now, lastLogin, -DATE_TIME_OFFSET, DATE_RESET_WEEKLY);
                if (isWeekChanged)
                {
                    ResetWeeklyData();
                    SaveWeeklyData();
                }
                ResetDailyData(dayPassed, isWeekChanged);
                SaveDailyData();
            }
            else
            {
                // If not ResetDailyData(), then update data status here
                _rewardDataManager.UpdateRewardDataStatus(RewardType.DayStreak);
                _rewardDataManager.UpdateRewardDataStatus(RewardType.DailyMissions);
                _rewardDataManager.UpdateRewardDataStatus(RewardType.WeeklyMissions);
                _uiManager.reward.RefreshRewardButtons();
            }

            _uiManager.UpdateScoreUI(score);
            _uiManager.reward.OnDataStoreLoaded();
        }

        // Reset daily and weekly data while playing
        private IEnumerator ResetDailyCoroutine(int secondsToNextDay)
        {
            yield return new WaitForSeconds(secondsToNextDay);
            UpdateTimePassed();

            StartCoroutine(ResetDailyCoroutine(SECOND_TO_NEXT_DAY));
        }

        // Reset daily data: day streak, daily missions
        private void ResetDailyData(int dayPassed, bool isWeekChanged)
        {
            if (dayPassed > 1 || isWeekChanged) // Reset day streak
            {
                dayStreak = 1;
                _rewardDataManager.InitializeData(RewardType.DayStreak);
            }
            else if (dayPassed == 1)
            {
                dayStreak++;
            }
            _rewardDataManager.UpdateRewardDataStatus(RewardType.DayStreak);

            totalClicksDay = 0;
            _rewardDataManager.InitializeData(RewardType.DailyMissions);

            _uiManager.reward.RefreshRewardButtons();
        }

        private void SaveDailyData()
        {
            dateTimeLastLogin = DateTime.Now;
            _dataStoreManager.SaveData(DataStoreManager.DataName.DateTimeLastLogin, dateTimeLastLogin);
            _dataStoreManager.SaveData(DataStoreManager.DataName.DayStreak, dayStreak);
            _dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksDay, totalClicksDay);
        }

        // Reset weekly data: weekly missions
        private void ResetWeeklyData()
        {
            totalClicksWeek = 0;
            _rewardDataManager.InitializeData(RewardType.WeeklyMissions);
            _uiManager.reward.RefreshRewardButtons();
        }

        private void SaveWeeklyData()
        {
            _dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksWeek, totalClicksWeek);
        }

        // Clicker button clicked from UI
        // Add score and update daily/weekly missions
        public static void OnClickerButtonClicked(int amount = 1)
        {
            _instance.OnClickerButtonClickedInternal(amount);
        }
        public void OnClickerButtonClickedInternal(int amount)
        {
            totalClicksDay += amount;
            totalClicksWeek += amount;
            _dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksDay, totalClicksDay);
            _dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksWeek, totalClicksDay);

            _rewardDataManager.UpdateRewardDataStatus(RewardType.DailyMissions);
            _rewardDataManager.UpdateRewardDataStatus(RewardType.WeeklyMissions);

            AddScoreInternal(1);
        }

        // Reward claimed from UI-rewardButton
        // Add score, update reward status, save data and update UI
        public static void ClaimReward(RewardType rewardType, RewardDataObject reward)
        {
            _instance.ClaimRewardInternal(rewardType, reward);
        }
        private void ClaimRewardInternal(RewardType rewardType, RewardDataObject reward)
        {
            _rewardDataManager.SetRewardDataClaimed(rewardType, reward);
            _rewardDataManager.UpdateRewardDataStatus(rewardType);

            RewardDataGroupObject rewardDataGroupObject = RewardDataManager.GetRewardDataGroupByType(rewardType);
            switch (rewardType)
            {
                case RewardType.DayStreak:
                    // _dataStoreManager.SaveData(DataStoreManager.DataName.DayStreakClaimed, rewardDataGroupObject.rewardsClaimed);
                    _dataStoreManager.SaveData(DataStoreManager.DataName.DayStreakClaimed, rewardDataGroupObject.GetConvertedRewardsClaimed());
                    break;
                case RewardType.DailyMissions:
                    // _dataStoreManager.SaveData(DataStoreManager.DataName.DailyMissionClaimed, rewardDataGroupObject.rewardsClaimed);
                    _dataStoreManager.SaveData(DataStoreManager.DataName.DailyMissionClaimed, rewardDataGroupObject.GetConvertedRewardsClaimed());
                    break;
                case RewardType.WeeklyMissions:
                    // _dataStoreManager.SaveData(DataStoreManager.DataName.WeeklyMissionClaimed, rewardDataGroupObject.rewardsClaimed);
                    _dataStoreManager.SaveData(DataStoreManager.DataName.WeeklyMissionClaimed, rewardDataGroupObject.GetConvertedRewardsClaimed());
                    break;
            }

            AddScoreInternal(reward.rewardAmount);
        }

        // Update score and update UI
        public static void AddScore(int amount)
        {
            _instance.AddScoreInternal(amount);
        }
        public void AddScoreInternal(int amount)
        {
            score += amount;
            _dataStoreManager.SaveData(DataStoreManager.DataName.Score, score);

            _uiManager.UpdateScoreUI(score);
            _uiManager.reward.RefreshRewardButtons();
        }

        public static void ResetSaveDataDebug()
        {
            score = 0;
            totalClicksDay = 0;
            totalClicksWeek = 0;
            dateTimeLastLogin = DateTime.Now;
            dayStreak = 1;

            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.Score, score);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksDay, totalClicksDay);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.TotalClicksWeek, totalClicksWeek);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.DateTimeLastLogin, dateTimeLastLogin);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.DayStreak, dayStreak);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.DayStreakClaimed, null);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.DailyMissionClaimed, null);
            _instance._dataStoreManager.SaveData(DataStoreManager.DataName.WeeklyMissionClaimed, null);

            _instance._rewardDataManager.InitializeData();
            _instance._rewardDataManager.UpdateRewardDataStatus(RewardType.DayStreak);
            _instance._rewardDataManager.UpdateRewardDataStatus(RewardType.DailyMissions);
            _instance._rewardDataManager.UpdateRewardDataStatus(RewardType.WeeklyMissions);

            _instance._uiManager.reward.RefreshRewardButtons();
        }
    }
}
