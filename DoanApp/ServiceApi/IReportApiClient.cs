using DoanApp.Areas.Administration.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.ServiceApi
{
    public interface IReportApiClient
    {
        Task<List<ReportVideo_Vm>> GetAll(string token,string nameSearch=null);
        Task<int> Delete(string token, int id);
    }
}
