using DoanApp.Areas.Administration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class GetDateWeek
    {
        public static List<Date> CaculatorDate(int week,int date,int Month)
        {
            var Date31 = new int[] { 1, 3, 5, 7, 8, 10, 12 };
            var Date30 = new int[] {4,6,9,11 };

            var list = new List<Date>();
            var tam = date;
            for (int i = week; i>=2; i--)
            {
                var item = new Date();
                if (tam < 0||tam==0)
                    tam = CheckDate(Month-1,Date31,Date30);
                item.Day = tam;
                list.Add(item);
                tam -= 1;
            }
             tam = date;
            list.Reverse();
            for (int i = week+1; i <=8; i++)
            {
                var item = new Date();
                tam += 1;
                if (tam > CheckDate(Month, Date31, Date30))
                {
                    Month += 1;
                    tam = 1;
                }
                   
                item.Day = tam;
                list.Add(item);
            }
            return list;
        }
        private static int CheckDate(int month,int[] date31,int[] date30)
        {
            for (int i = 0; i < date31.Length; i++)
            {
                if (date31[i] == month) return 31;
                if (date30.Length> i)
                {
                    if (date30[i] == month) return 30;
                }
            }
            return 28;
        }
    }
}
