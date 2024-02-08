using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class RewardUnclaimedView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TextMeshProUGUI _textCount;
        public void Setup(int count)
        {
            _textCount.text = count.ToString();
            _root.SetActive(count > 0);
        }
    }
}