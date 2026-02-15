using System;

namespace CozyFarm.TimeSystem
{
    public class GameCalendar
    {
        public static int DAY_IN_MONTH = 31, SEASONS_IN_YEAR = 4;
        public int Season { get; private set; }
        public int Day { get; private set;}
        public int WeekDay { get; set; }
        public int Year { get; set; }

        public event Action<int> OnSeasonChanged;

        public GameCalendar()
        {
            Season = 0;
            Day = 1;
            WeekDay = GetWeekDay(Season, Day);
            Year = 0;
        }

        public GameCalendar(int year, int season, int day)
        {
            Season = season;
            Day = day;
            WeekDay = GetWeekDay(Season, Day);
            Year = year;
        }

        private int GetWeekDay(int season, int day)
        {
            int yearDay = (Year * SEASONS_IN_YEAR + season) * DAY_IN_MONTH + day - 1;
            return yearDay % 7;
        }

        public void ProgressTime()
        {
            Day++;
            if (Day > DAY_IN_MONTH)
            {
                Day = 1;
                Season++;

                if (Season >= SEASONS_IN_YEAR)
                {
                    Season = 0;
                    Year++;
                }
                OnSeasonChanged?.Invoke(Season);
            }
            WeekDay = GetWeekDay(Season, Day);
        }
    }
}
