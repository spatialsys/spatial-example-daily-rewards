using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class RewardButtonView : MonoBehaviour
    {
        private static readonly Color COLOR_ACHIEVED = new Color(1, 1, 1, 1);
        private static readonly Color COLOR_AVAILABLE = new Color(0.8f, 0.8f, 0.8f, 1f);
        private static readonly Color COLOR_CLAIMED = new Color(0.5f, 0.5f, 0.5f, 1f);

        public RewardDataObject rewardData;
        public Button button => _button;

        [SerializeField] private Button _button;
        [SerializeField] private Image _imageBG;
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textValue;
        [SerializeField] private TextMeshProUGUI _textStatus;
        [SerializeField] private Image _imageClaimed;

        public void Setup(RewardDataObject rewardData, Action<RewardButtonView> buttonAction)
        {
            this.rewardData = rewardData;

            _textTitle.text = rewardData.title;
            _textValue.text = rewardData.rewardAmount.ToString("N0");

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => buttonAction(this));

            UpdateStatus();
        }

        public void UpdateStatus()
        {
            RewardDataObject.Status status = rewardData.status;
            _button.enabled = status == RewardDataObject.Status.Achieved;
            _textStatus.text = status == RewardDataObject.Status.Achieved ? "Claim" : string.Empty;
            _imageClaimed.gameObject.SetActive(status == RewardDataObject.Status.Claimed);

            switch (status)
            {
                case RewardDataObject.Status.None:
                case RewardDataObject.Status.Available:
                    _imageBG.color = COLOR_AVAILABLE;
                    break;
                case RewardDataObject.Status.Achieved:
                    _imageBG.color = COLOR_ACHIEVED;
                    break;
                case RewardDataObject.Status.Claimed:
                    _imageBG.color = COLOR_CLAIMED;
                    break;
                default:
                    break;
            }
        }
    }
}
