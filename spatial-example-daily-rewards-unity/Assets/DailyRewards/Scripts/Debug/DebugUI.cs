using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DailyRewards
{
    public class DebugUI : MonoBehaviour
    {
        [Header("Game Status")]
        [SerializeField] private TextMeshProUGUI _textResetTime;
        [SerializeField] private TextMeshProUGUI _textGameStatus;
        [SerializeField] private TextMeshProUGUI _textDebugResult;

        [Header("Debug Buttons (Now)")]
        [SerializeField] private Button _offsetDayNowButtonPlusSeven;
        [SerializeField] private Button _offsetDayNowButtonMinusSeven;
        [SerializeField] private Button _offsetDayNowButtonPlus;
        [SerializeField] private Button _offsetDayNowButtonMinus;
        [SerializeField] private Button _offsetHourNowButtonPlus;
        [SerializeField] private Button _offsetHourNowButtonMinus;
        [SerializeField] private Button _offsetMinuteNowButtonPlus;
        [SerializeField] private Button _offsetMinuteNowButtonMinus;

        [Header("Debug Buttons (LastLogin)")]
        [SerializeField] private Button _offsetDayLastLoginButtonPlusSeven;
        [SerializeField] private Button _offsetDayLastLoginButtonMinusSeven;
        [SerializeField] private Button _offsetDayLastLoginButtonPlus;
        [SerializeField] private Button _offsetDayLastLoginButtonMinus;
        [SerializeField] private Button _offsetHourLastLoginButtonPlus;
        [SerializeField] private Button _offsetHourLastLoginButtonMinus;
        [SerializeField] private Button _offsetMinuteLastLoginButtonPlus;
        [SerializeField] private Button _offsetMinuteLastLoginButtonMinus;

        [Header("Debug Texts")]
        [SerializeField] private TextMeshProUGUI _textDateTimeNow;
        [SerializeField] private TextMeshProUGUI _textDateTimeLastLogin;
        [SerializeField] private TextMeshProUGUI _textOffsetDayNow;
        [SerializeField] private TextMeshProUGUI _textOffsetHourNow;
        [SerializeField] private TextMeshProUGUI _textOffsetMinuteNow;
        [SerializeField] private TextMeshProUGUI _textOffsetDayLastLogin;
        [SerializeField] private TextMeshProUGUI _textOffsetHourLastLogin;
        [SerializeField] private TextMeshProUGUI _textOffsetMinuteLastLogin;

        [Header("Debug Buttons (LastLogin)")]
        [SerializeField] private Button _buttonUpdateTimePassed;
        [SerializeField] private Button _buttonResetOffset;

        [Header("Debug Buttons (LastLogin)")]
        [SerializeField] private Button _buttonResetSaveData;

        private int _offsetDayNow = 0;
        private int _offsetHourNow = 0;
        private int _offsetMinuteNow = 0;
        private int _offsetDayLastLogin = 0;
        private int _offsetHourLastLogin = 0;
        private int _offsetMinuteLastLogin = 0;

        private void Awake()
        {
            GameManager.onDataStoreLoaded += OnAnyButtonClicked;

            _offsetDayNowButtonPlusSeven.onClick.AddListener(OnOffsetDayNowButtonPlusSevenClicked);
            _offsetDayNowButtonMinusSeven.onClick.AddListener(OnOffsetDayNowButtonMinusSevenClicked);
            _offsetDayNowButtonPlus.onClick.AddListener(OnOffsetDayNowButtonPlusClicked);
            _offsetDayNowButtonMinus.onClick.AddListener(OnOffsetDayNowButtonMinusClicked);
            _offsetHourNowButtonPlus.onClick.AddListener(OnOffsetHourNowButtonPlusClicked);
            _offsetHourNowButtonMinus.onClick.AddListener(OnOffsetHourNowButtonMinusClicked);
            _offsetMinuteNowButtonPlus.onClick.AddListener(OnOffsetMinuteNowButtonPlusClicked);
            _offsetMinuteNowButtonMinus.onClick.AddListener(OnOffsetMinuteNowButtonMinusClicked);

            _offsetDayLastLoginButtonPlusSeven.onClick.AddListener(OnOffsetDayLastLoginButtonPlusSevenClicked);
            _offsetDayLastLoginButtonMinusSeven.onClick.AddListener(OnOffsetDayLastLoginButtonMinusSevenClicked);
            _offsetDayLastLoginButtonPlus.onClick.AddListener(OnOffsetDayLastLoginButtonPlusClicked);
            _offsetDayLastLoginButtonMinus.onClick.AddListener(OnOffsetDayLastLoginButtonMinusClicked);
            _offsetHourLastLoginButtonPlus.onClick.AddListener(OnOffsetHourLastLoginButtonPlusClicked);
            _offsetHourLastLoginButtonMinus.onClick.AddListener(OnOffsetHourLastLoginButtonMinusClicked);
            _offsetMinuteLastLoginButtonPlus.onClick.AddListener(OnOffsetMinuteLastLoginButtonPlusClicked);
            _offsetMinuteLastLoginButtonMinus.onClick.AddListener(OnOffsetMinuteLastLoginButtonMinusClicked);

            _buttonUpdateTimePassed.onClick.AddListener(UpdateTimePassed);
            _buttonResetOffset.onClick.AddListener(OnResetOffsetClicked);

            _buttonResetSaveData.onClick.AddListener(ResetSaveDataDebug);
        }
        private void OnDestroy()
        {
            GameManager.onDataStoreLoaded -= OnAnyButtonClicked;

            _offsetDayNowButtonPlusSeven.onClick.RemoveAllListeners();
            _offsetDayNowButtonMinusSeven.onClick.RemoveAllListeners();
            _offsetDayNowButtonPlus.onClick.RemoveAllListeners();
            _offsetDayNowButtonMinus.onClick.RemoveAllListeners();
            _offsetHourNowButtonPlus.onClick.RemoveAllListeners();
            _offsetHourNowButtonMinus.onClick.RemoveAllListeners();
            _offsetMinuteNowButtonPlus.onClick.RemoveAllListeners();
            _offsetMinuteNowButtonMinus.onClick.RemoveAllListeners();

            _offsetDayLastLoginButtonPlusSeven.onClick.RemoveAllListeners();
            _offsetDayLastLoginButtonMinusSeven.onClick.RemoveAllListeners();
            _offsetDayLastLoginButtonPlus.onClick.RemoveAllListeners();
            _offsetDayLastLoginButtonMinus.onClick.RemoveAllListeners();
            _offsetHourLastLoginButtonPlus.onClick.RemoveAllListeners();
            _offsetHourLastLoginButtonMinus.onClick.RemoveAllListeners();
            _offsetMinuteLastLoginButtonPlus.onClick.RemoveAllListeners();
            _offsetMinuteLastLoginButtonMinus.onClick.RemoveAllListeners();

            _buttonUpdateTimePassed.onClick.RemoveAllListeners();
            _buttonResetOffset.onClick.RemoveAllListeners();

            _buttonResetSaveData.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            _textResetTime.text = GetResetTime();
            _textGameStatus.text = GetGameStatus();
        }

        private string GetResetTime()
        {
            string resetTime = "Reset time daily: ";
            int time = GameManager.DATE_TIME_OFFSET;
            if (time > 12)
            {
                resetTime += (time - 12) + " PM";
            }
            else
            {
                resetTime += time + " AM";
            }
            resetTime += "\n" + "Reset day weekly: " + GameManager.DATE_RESET_WEEKLY;
            return resetTime;
        }
        private string GetGameStatus()
        {
            string gameStatus = "";
            gameStatus += "Score: " + GameManager.score + "\n";
            gameStatus += "Day Streak: " + GameManager.dayStreak + "\n";
            gameStatus += "Total Clicks Day: " + GameManager.totalClicksDay + "\n";
            gameStatus += "Total Clicks Week: " + GameManager.totalClicksWeek + "\n";
            gameStatus += "Last Login: " + GameManager.dateTimeLastLogin + "\n";
            gameStatus += "Now: " + DateTime.Now;
            return gameStatus;
        }

        private void OnOffsetDayNowButtonClicked(int offset)
        {
            _offsetDayNow += offset;
            _textOffsetDayNow.text = "Day offset: " + _offsetDayNow.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetDayNowButtonPlusSevenClicked()
        {
            OnOffsetDayNowButtonClicked(7);
        }
        private void OnOffsetDayNowButtonMinusSevenClicked()
        {
            OnOffsetDayNowButtonClicked(-7);
        }
        private void OnOffsetDayNowButtonPlusClicked()
        {
            OnOffsetDayNowButtonClicked(1);
        }
        private void OnOffsetDayNowButtonMinusClicked()
        {
            OnOffsetDayNowButtonClicked(-1);
        }
        private void OnOffsetHourNowButtonClicked(int offset)
        {
            _offsetHourNow += offset;
            _textOffsetHourNow.text = "Hour offset: " + _offsetHourNow.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetHourNowButtonPlusClicked()
        {
            OnOffsetHourNowButtonClicked(1);
        }
        private void OnOffsetHourNowButtonMinusClicked()
        {
            OnOffsetHourNowButtonClicked(-1);
        }
        private void OnOffsetMinuteNowButtonClicked(int offset)
        {
            _offsetMinuteNow += offset;
            _textOffsetMinuteNow.text = "Min offset: " + _offsetMinuteNow.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetMinuteNowButtonPlusClicked()
        {
            OnOffsetMinuteNowButtonClicked(1);
        }
        private void OnOffsetMinuteNowButtonMinusClicked()
        {
            OnOffsetMinuteNowButtonClicked(-1);
        }

        private void OnOffsetDayLastLoginButtonClicked(int offset)
        {
            _offsetDayLastLogin += offset;
            _textOffsetDayLastLogin.text = "Day offset: " + _offsetDayLastLogin.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetDayLastLoginButtonPlusSevenClicked()
        {
            OnOffsetDayLastLoginButtonClicked(7);
        }
        private void OnOffsetDayLastLoginButtonMinusSevenClicked()
        {
            OnOffsetDayLastLoginButtonClicked(-7);
        }
        private void OnOffsetDayLastLoginButtonPlusClicked()
        {
            OnOffsetDayLastLoginButtonClicked(1);
        }
        private void OnOffsetDayLastLoginButtonMinusClicked()
        {
            OnOffsetDayLastLoginButtonClicked(-1);
        }
        private void OnOffsetHourLastLoginButtonClicked(int offset)
        {
            _offsetHourLastLogin += offset;
            _textOffsetHourLastLogin.text = "Hour offset: " + _offsetHourLastLogin.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetHourLastLoginButtonPlusClicked()
        {
            OnOffsetHourLastLoginButtonClicked(1);
        }
        private void OnOffsetHourLastLoginButtonMinusClicked()
        {
            OnOffsetHourLastLoginButtonClicked(-1);
        }
        private void OnOffsetMinuteLastLoginButtonClicked(int offset)
        {
            _offsetMinuteLastLogin += offset;
            _textOffsetMinuteLastLogin.text = "Min offset: " + _offsetMinuteLastLogin.ToString();
            OnAnyButtonClicked();
        }
        private void OnOffsetMinuteLastLoginButtonPlusClicked()
        {
            OnOffsetMinuteLastLoginButtonClicked(1);
        }
        private void OnOffsetMinuteLastLoginButtonMinusClicked()
        {
            OnOffsetMinuteLastLoginButtonClicked(-1);
        }

        private void OnResetOffsetClicked()
        {
            _offsetDayNow = 0;
            _offsetHourNow = 0;
            _offsetMinuteNow = 0;
            _offsetDayLastLogin = 0;
            _offsetHourLastLogin = 0;
            _offsetMinuteLastLogin = 0;

            _textOffsetDayNow.text = "Day offset: " + _offsetDayNow.ToString();
            _textOffsetHourNow.text = "Hour offset: " + _offsetHourNow.ToString();
            _textOffsetMinuteNow.text = "Min offset: " + _offsetMinuteNow.ToString();
            _textOffsetDayLastLogin.text = "Day offset: " + _offsetDayLastLogin.ToString();
            _textOffsetHourLastLogin.text = "Hour offset: " + _offsetHourLastLogin.ToString();
            _textOffsetMinuteLastLogin.text = "Min offset: " + _offsetMinuteLastLogin.ToString();

            OnAnyButtonClicked();
        }

        private void OnAnyButtonClicked()
        {
            DateTime now = DateTime.Now;
            DateTime lastLogin = GameManager.dateTimeLastLogin;

            now = now.AddDays(_offsetDayNow);
            now = now.AddHours(_offsetHourNow);
            now = now.AddMinutes(_offsetMinuteNow);

            lastLogin = lastLogin.AddDays(_offsetDayLastLogin);
            lastLogin = lastLogin.AddHours(_offsetHourLastLogin);
            lastLogin = lastLogin.AddMinutes(_offsetMinuteLastLogin);

            string nowString = "Now: " + now.ToString();
            nowString += "\n" + now.DayOfWeek;
            _textDateTimeNow.text = nowString;

            string lastLoginString = "LastLogin: " + lastLogin.ToString();
            lastLoginString += "\n" + lastLogin.DayOfWeek;
            _textDateTimeLastLogin.text = lastLoginString;
        }

        private void UpdateTimePassed()
        {
            DateTime now = DateTime.Now;
            DateTime lastLogin = GameManager.dateTimeLastLogin;

            now = now.AddDays(_offsetDayNow);
            now = now.AddHours(_offsetHourNow);
            now = now.AddMinutes(_offsetMinuteNow);

            lastLogin = lastLogin.AddDays(_offsetDayLastLogin);
            lastLogin = lastLogin.AddHours(_offsetHourLastLogin);
            lastLogin = lastLogin.AddMinutes(_offsetMinuteLastLogin);

            int dayPassed = DateTimeUtilities.GetDayPassed(now, lastLogin, -GameManager.DATE_TIME_OFFSET);
            bool isWeekChanged = DateTimeUtilities.IsWeekChanged(now, lastLogin, -GameManager.DATE_TIME_OFFSET, GameManager.DATE_RESET_WEEKLY);
            _textDebugResult.text = "Day Passed: " + dayPassed + "\n" + "Is Week Changed: " + isWeekChanged;

            GameManager.UpdateTimePassedDebug(now, lastLogin);
        }

        private void ResetSaveDataDebug()
        {
            GameManager.ResetSaveDataDebug();
        }
    }
}
