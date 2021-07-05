using DoanApp.Areas.Administration.Models;
using DoanApp.Models;
using DoanData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoanApp.Services
{
    public interface IReportVideoService
    {
        List<ReportVideo> GetAll();
        List<ReportVideo_Vm> GetList_Vm();
        Task<ReportVideo> FinByIdAsync(int id);
        Task<int> Create(ReportVideoRequest request);
        Task<int> Delete(int id);
        Task<int> Update(ReportVideoRequest request);
    }
}
