using System;
using Core.Common;
using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface IAggregateRepository
    {
        Task<ExecutionResult<bool>> DownloadData();
        List<ElectricityInfo> GroupAndTurnIntoentity(List<ElectricityInfoModel> data);
        Task<ExecutionResult<List<ElectricityInfo>>> GetElectricityInfo();
        ElectricityInfoModel ProcessCSVLine(string line);
    }
}

