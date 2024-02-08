using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpatialSys.UnitySDK;
using System;

namespace DailyRewards
{
    // Save/Load data from Spatial UserWorldDataStoreService
    // https://docs.spatial.io/data-store#block-c5a2364413f24c0ea59e34aa78dd2488
    public class DataStoreManager : MonoBehaviour
    {
        private const string DATA_KEY_SCORE = "rwd/score"; // int
        private const string DATA_KEY_TOTAL_CLICKS_DAY = "rwd/total_clicks_day"; // int
        private const string DATA_KEY_TOTAL_CLICKS_WEEK = "rwd/total_clicks_week"; // int
        private const string DATA_KEY_DATE_TIME_LAST_LOGIN = "rwd/date_time_last_login"; // datetime
        private const string DATA_KEY_DAY_STREAK = "rwd/day_streak"; // int

        private const string DATA_KEY_DAY_STREAK_CLAIMED = "rwd/day_streak_claimed"; // List<bool>
        private const string DATA_KEY_DAILY_MISSION_CLAIMED = "rwd/daily_mission_claimed"; // List<bool>
        private const string DATA_KEY_WEEKLY_MISSION_CLAIMED = "rwd/weekly_mission_claimed"; // List<bool>

        public enum DataName
        {
            Score,
            TotalClicksDay,
            TotalClicksWeek,
            DateTimeLastLogin,
            DayStreak,
            DayStreakClaimed,
            DailyMissionClaimed,
            WeeklyMissionClaimed,
        }
        private Dictionary<DataName, string> _saveDataKeyByName = new Dictionary<DataName, string>
        {
            { DataName.Score, DATA_KEY_SCORE },
            { DataName.TotalClicksDay, DATA_KEY_TOTAL_CLICKS_DAY },
            { DataName.TotalClicksWeek, DATA_KEY_TOTAL_CLICKS_WEEK },
            { DataName.DateTimeLastLogin, DATA_KEY_DATE_TIME_LAST_LOGIN },
            { DataName.DayStreak, DATA_KEY_DAY_STREAK },
            { DataName.DayStreakClaimed, DATA_KEY_DAY_STREAK_CLAIMED },
            { DataName.DailyMissionClaimed, DATA_KEY_DAILY_MISSION_CLAIMED },
            { DataName.WeeklyMissionClaimed, DATA_KEY_WEEKLY_MISSION_CLAIMED },
        };

        private const float LOAD_DATA_INTERVAL = 2f;

        private int _tryLoadDataCount = 0; // if failed to load data, try again
        private bool _isDataLoaded = false;

        public void SaveData(DataName dataName, object value)
        {
            if (_isDataLoaded)
            {
                SpatialBridge.userWorldDataStoreService.SetVariable(_saveDataKeyByName[dataName], value);
            }
        }

        public void LoadData(Action onDataStoreLoaded)
        {
            StartCoroutine(LoadDataCoroutine(onDataStoreLoaded));
        }

        private IEnumerator LoadDataCoroutine(Action onDataStoreLoaded)
        {
            // DATA_KEY_SCORE
            IEnumerator<object> loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_SCORE, defaultValue: 0);
                yield return loadData;
            }
            GameManager.score += (int)loadData.Current; // Add to existing data, so you won't lose your progress before loading

            // DATA_KEY_TOTAL_CLICKS_DAY
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_TOTAL_CLICKS_DAY, defaultValue: 0);
                yield return loadData;
            }
            GameManager.totalClicksDay += (int)loadData.Current; // Add to existing data, so you won't lose your progress before loading

            // DATA_KEY_TOTAL_CLICKS_WEEK
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_TOTAL_CLICKS_WEEK, defaultValue: 0);
                yield return loadData;
            }
            GameManager.totalClicksWeek += (int)loadData.Current; // Add to existing data, so you won't lose your progress before loading

            // DATA_KEY_DATE_TIME_LAST_LOGIN
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_DATE_TIME_LAST_LOGIN, defaultValue: DateTime.Now);
                yield return loadData;
            }
            GameManager.dateTimeLastLogin = (DateTime)loadData.Current;

            // DATA_KEY_DAY_STREAK
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_DAY_STREAK, defaultValue: 1);
                yield return loadData;
            }
            GameManager.dayStreak = (int)loadData.Current;

            // DATA_KEY_DAY_STREAK_CLAIMED
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_DAY_STREAK_CLAIMED, defaultValue: new Dictionary<string, bool>());
                yield return loadData;
            }
            RewardDataGroupObject rewardDataGroupObject = RewardDataManager.GetRewardDataGroupByType(RewardType.DayStreak);
            if (loadData.Current == null)
            {
                rewardDataGroupObject.InitializeAllData();
            }
            else
            {
                // rewardDataGroupObject.rewardsClaimed = (Dictionary<string, bool>)loadData.Current;
                rewardDataGroupObject.rewardsClaimed = TypeCastUtilities.GetDictionary<bool>(loadData.Current);
            }

            // DATA_KEY_DAILY_MISSION_CLAIMED
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_DAILY_MISSION_CLAIMED, defaultValue: new Dictionary<string, bool>());
                yield return loadData;
            }
            rewardDataGroupObject = RewardDataManager.GetRewardDataGroupByType(RewardType.DailyMissions);
            if (loadData.Current == null)
            {
                rewardDataGroupObject.InitializeAllData();
            }
            else
            {
                // rewardDataGroupObject.rewardsClaimed = (Dictionary<string, bool>)loadData.Current;
                rewardDataGroupObject.rewardsClaimed = TypeCastUtilities.GetDictionary<bool>(loadData.Current);
            }

            // DATA_KEY_WEEKLY_MISSION_CLAIMED
            loadData = null;
            while (loadData == null)
            {
                loadData = TryLoadDataCoroutine(DATA_KEY_WEEKLY_MISSION_CLAIMED, defaultValue: new Dictionary<string, bool>());
                yield return loadData;
            }
            rewardDataGroupObject = RewardDataManager.GetRewardDataGroupByType(RewardType.WeeklyMissions);
            if (loadData.Current == null)
            {
                rewardDataGroupObject.InitializeAllData();
            }
            else
            {
                // rewardDataGroupObject.rewardsClaimed = (Dictionary<string, bool>)loadData.Current;
                rewardDataGroupObject.rewardsClaimed = TypeCastUtilities.GetDictionary<bool>(loadData.Current);
            }

            _isDataLoaded = true;
            onDataStoreLoaded?.Invoke();
        }

        // If there's no data, use default value
        // If there's data but failed to load, try again
        private IEnumerator<object> TryLoadDataCoroutine(string key, object defaultValue)
        {
            bool hasVariableButFailedToLoad = false;
            DataStoreResponseCode responseCode = DataStoreResponseCode.Ok;
            var hasVariable = SpatialBridge.userWorldDataStoreService.HasVariable(key);
            yield return hasVariable;
            if (hasVariable.hasVariable)
            {
                var getVariable = SpatialBridge.userWorldDataStoreService.GetVariable(key, defaultValue: null);
                yield return getVariable;
                if (getVariable.succeeded)
                {
                    yield return getVariable.value;
                }
                else
                {
                    hasVariableButFailedToLoad = true;
                    responseCode = getVariable.responseCode;
                }
            }
            else
            {
                yield return defaultValue;
            }

            if (hasVariableButFailedToLoad)
            {
                Debug.LogError($"Failed to load [{key}] ({responseCode.ToString()}).\nTry again {++_tryLoadDataCount}.");
                yield return new WaitForSeconds(LOAD_DATA_INTERVAL);
                yield return null;
            }
        }
    }
}