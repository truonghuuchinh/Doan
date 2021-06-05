using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Commons
{
    public class ConvertViewCount
    {
        public static string ConvertView(int view)
        {
            string viewConvert = "";
                if (view > 1000000)
                {
                    view = view / 1000000;
                    viewConvert = view.ToString() + " Tr";
                }
                else
                {
                    if (view > 1000)
                    {
                        view = view / 1000;
                    viewConvert = view.ToString() + " N";
                    }
                    else viewConvert = view.ToString();
                }
            return viewConvert;
        }
    }
}
