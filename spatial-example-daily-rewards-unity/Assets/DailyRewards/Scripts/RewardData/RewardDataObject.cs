using UnityEngine;

namespace DailyRewards
{
    [CreateAssetMenu(fileName = "RewardData_", menuName = "DailyRewards/RewardData", order = 1)]
    public class RewardDataObject : ScriptableObject
    {
        public enum Status
        {
            None,
            Available,
            Achieved,
            Claimed,
        }
        public string id;
        public string title;
        public RequirementType requirementType;
        public int requirement;
        public int rewardAmount;
        public Status status;
    }
}