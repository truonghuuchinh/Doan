using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class GetWeek
    {
        public int GetIso8601WeekOfYear(DateTime time)
        {
            var th= time.DayOfWeek.ToString();
            if (th == "Monday") return 2;
            if (th == "Tuesday") return 3;
            if (th == "Wednesday") return 4;
            if (th == "Thursday") return 5;
            if (th == "Friday") return 6;
            if (th == "Saturday") return 7;
            return 8;

        }
    }
}
