using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class CaculatorHours
    {
        public static string Caculator(string hours)
        {
            DateTime dFrom;
            DateTime dTo;
            var result = "";
            int tam = 0;
            string sDateTo = DateTime.Now.ToString("MM-d-yyyy HH:mm:s");
            if (DateTime.TryParse(hours, out dFrom) && DateTime.TryParse(sDateTo, out dTo))
            {
                TimeSpan TS = dTo - dFrom;
                int hour = TS.Hours;
                int mins = TS.Minutes;
                int day = TS.Days;
                if (day != 0)
                {
                    if (day == 7)
                         return result = "1 Tuần trước";
                    if (day < 7) return result = day.ToString() + " Ngày trước";
                    if (day > 7)
                    {
                        tam = (int)Math.Floor((double)(day / 30));
                        if (tam == 1)
                        {
                            return  "1 Tháng trước";
                        }
                        if (tam == 0) return result = day.ToString() + " Tuần trước";
                        if (tam < 12) return tam.ToString() + " Tháng trước";
                        if (tam == 12) return "1 Năm trước";
                        if (tam > 12)
                        {
                            tam = (int)Math.Floor((double)(day / 365));
                            if (tam==0) return "1 Năm trước";
                            else
                            {
                                return tam.ToString() + " Năm trước";
                            }
                        }
                    }
                  
                }
                if (hour != 0) return hour.ToString()+ " Giờ trước";
                 if (mins != 0) return mins.ToString()+ " Phút trước";
            }
            return null;
        }
    }
}
