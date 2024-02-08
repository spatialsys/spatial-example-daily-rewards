using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DailyRewards
{
    [CreateAssetMenu(fileName = "RewardDataGroup_", menuName = "DailyRewards/RewardDataGroup", order = 1)]
    public class RewardDataGroupObject : ScriptableObject
    {
        public RewardType rewardType;
        public RewardDataObject[] rewards;
        public Dictionary<string, bool> rewardsClaimed; // key: reward id, value: claimed
        public Dictionary<string, object> rewardsClaimedToSave = new Dictionary<string, object>(); // TODO: Temporal Fix. Spatial doesn't support Dictionary<string, bool> type

        public void InitializeAllData()
        {
            if (rewardsClaimed == null)
            {
                rewardsClaimed = new Dictionary<string, bool>();
            }
            else
            {
                rewardsClaimed.Clear();
            }
            for (int i = 0; i < rewards.Length; i++)
            {
                rewardsClaimed.Add(rewards[i].id, false);
                rewards[i].status = RewardDataObject.Status.None;
            }
        }

        public bool ContainsReward(RewardDataObject reward)
        {
            return Array.IndexOf(rewards, reward) != -1;
        }

        public Dictionary<string, bool> GetRewardClaimedSafe()
        {
            if (rewardsClaimed == null)
            {
                Debug.Log($"rewardsClaimed is null, creating a new one");
                rewardsClaimed = new Dictionary<string, bool>();
                for (int i = 0; i < rewards.Length; i++)
                {
                    rewardsClaimed.Add(rewards[i].id, false);
                }
            }
            return rewardsClaimed;
        }

        public void SetRewardClaimedSafe(RewardDataObject reward, bool claimed)
        {
            Dictionary<string, bool> rewardsClaimedSafe = GetRewardClaimedSafe();
            if (!rewardsClaimedSafe.ContainsKey(reward.id))
            {
                rewardsClaimedSafe.Add(reward.id, claimed);
            }
            else
            {
                rewardsClaimedSafe[reward.id] = claimed;
            }
        }

        public bool GetRewardClaimedSafe(RewardDataObject reward)
        {
            Dictionary<string, bool> rewardsClaimedSafe = GetRewardClaimedSafe();
            if (!rewardsClaimedSafe.ContainsKey(reward.id))
            {
                rewardsClaimedSafe.Add(reward.id, false);
            }
            return rewardsClaimedSafe[reward.id];
        }

        public Dictionary<string, object> GetConvertedRewardsClaimed()
        {
            Dictionary<string, bool> rewardsClaimedSafe = GetRewardClaimedSafe();
            foreach (var item in rewardsClaimedSafe)
            {
                if (!rewardsClaimedToSave.ContainsKey(item.Key))
                {
                    rewardsClaimedToSave.Add(item.Key, item.Value);
                }
                else
                {
                    rewardsClaimedToSave[item.Key] = item.Value;
                }
            }
            return rewardsClaimedToSave;
        }

        public int GetRewardUnclaimedCount()
        {
            int count = 0;
            foreach (RewardDataObject reward in rewards)
            {
                if (reward.status == RewardDataObject.Status.Achieved &&
                    GetRewardClaimedSafe(reward) == false)
                {
                    count++;
                }
            }
            return count;
        }
    }
}