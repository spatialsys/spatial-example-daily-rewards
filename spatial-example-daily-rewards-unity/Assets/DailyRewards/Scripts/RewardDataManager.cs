using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyRewards
{
    public class RewardDataManager : MonoBehaviour
    {
        private static RewardDataManager _instance;

        [SerializeField] private RewardDataGroupObject _rewardGroupDayStreak;
        [SerializeField] private RewardDataGroupObject _rewardGroupDaily;
        [SerializeField] private RewardDataGroupObject _rewardGroupWeekly;

        private static Dictionary<RewardType, RewardDataGroupObject> _rewardDataGroupByType;

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

            _rewardDataGroupByType = new Dictionary<RewardType, RewardDataGroupObject>
            {
                { RewardType.None, null},
                { RewardType.DayStreak, _rewardGroupDayStreak },
                { RewardType.DailyMissions, _rewardGroupDaily },
                { RewardType.WeeklyMissions, _rewardGroupWeekly },
            };
        }
        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public static RewardDataGroupObject GetRewardDataGroupByType(RewardType rewardType)
        {
            return _rewardDataGroupByType[rewardType];
        }

        public static int GetRewardUnclaimedCount(RewardType rewardType)
        {
            RewardDataGroupObject rewardDataGroupObject = GetRewardDataGroupByType(rewardType);
            return rewardDataGroupObject.GetRewardUnclaimedCount();
        }

        public void InitializeData()
        {
            foreach (RewardDataGroupObject data in _rewardDataGroupByType.Values)
            {
                if (data != null)
                {
                    data.InitializeAllData();
                }
            }
        }

        public void InitializeData(RewardType rewardType)
        {
            RewardDataGroupObject rewardDataGroupObject = GetRewardDataGroupByType(rewardType);
            rewardDataGroupObject.InitializeAllData();
        }

        public void SetRewardDataClaimed(RewardType rewardType, RewardDataObject reward)
        {
            RewardDataGroupObject rewardDataGroupObject = GetRewardDataGroupByType(rewardType);
            rewardDataGroupObject.SetRewardClaimedSafe(reward, true);
        }

        // Update each data status: Claimed / Achieved / Available
        // By checking the progress of each reward in IsRequirementMet()
        public void UpdateRewardDataStatus(RewardType rewardType)
        {
            RewardDataGroupObject rewardDataGroupObject = GetRewardDataGroupByType(rewardType);
            RewardDataObject[] rewards = rewardDataGroupObject.rewards;
            for (int i = 0; i < rewards.Length; i++)
            {
                RewardDataObject reward = rewards[i];
                if (rewardDataGroupObject.GetRewardClaimedSafe(reward))
                {
                    reward.status = RewardDataObject.Status.Claimed;
                }
                else
                {
                    if (IsRequirementMet(reward))
                    {
                        reward.status = RewardDataObject.Status.Achieved;
                    }
                    else
                    {
                        reward.status = RewardDataObject.Status.Available;
                    }
                }
            }
        }

        private bool IsRequirementMet(RewardDataObject reward)
        {
            switch (reward.requirementType)
            {
                case RequirementType.DayStreak:
                    return GameManager.dayStreak >= reward.requirement;
                case RequirementType.TotalClicksDay:
                    return GameManager.totalClicksDay >= reward.requirement;
                case RequirementType.TotalClicksWeek:
                    return GameManager.totalClicksWeek >= reward.requirement;
                case RequirementType.None:
                default:
                    return true;
            }
        }
    }
}
