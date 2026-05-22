using System;
using System.Collections.Generic;

[Serializable]
public class GameProgressData
{
    public List<DayID> completedDays = new List<DayID>();

    public bool IsDayCompleted(DayID dayID)
    {
        return completedDays.Contains(dayID);
    }

    public void MarkDayAsCompleted(DayID dayID)
    {
        if (!completedDays.Contains(dayID)) completedDays.Add(dayID);
    }
}