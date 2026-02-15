using CozyFarm.TimeSystem;
using System;
using TMPro;
using UnityEngine;

namespace CozyFarm.UI
{
    public class PlayerCalendarUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _seasonTxt, _dayTxt, _timeTxt;

        private TimeManager _timeManager;

        private void OnEnable()
        {
            _timeManager = FindObjectOfType<TimeManager>(true);
            Debug.Assert(_timeManager != null, "Can't find TimeManager", gameObject);

            _timeManager.OnClockProgress += UpdateAllUIElements;
        }

        private void UpdateAllUIElements(object sender, TimeEventArgs timeArgs)
        {
            UpdateTime(timeArgs.CurrentTime);
            UpdateDay(timeArgs.CurrentDay, timeArgs.WeekDay);
            UpdateSeason(timeArgs.CurrentSeason);
        }

        private void UpdateSeason(int currentSeason)
        {
            _seasonTxt.text = CalendarNameHelper.GetSeasonName(currentSeason);
        }

        private void UpdateDay(int currentDay, int weekDay)
        {
            _dayTxt.text = $"{currentDay} ({CalendarNameHelper.GetWeekDayName(weekDay)})";
        }

        private void UpdateTime(TimeSpan currentTime)
        {
            _timeTxt.text = currentTime.ToString(@"hh\:mm");
        }

        private void OnDisable()
        {
            _timeManager.OnClockProgress -= UpdateAllUIElements;
        }
    }
}
