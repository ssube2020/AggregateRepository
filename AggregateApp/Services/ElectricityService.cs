using System;
using System.Globalization;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic.FileIO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AggregateApp.Services
{
	public class ElectricityService
    {

        private readonly IAggregateRepository _repository;

        public ElectricityService(IAggregateRepository repository)
        {
            _repository = repository;
        }

        public async Task<Core.Common.ExecutionResult<bool>> DownloadData()
        {
            return await _repository.DownloadData();
        }

        public async Task<Core.Common.ExecutionResult<List<ElectricityInfo>>> GetElectroData()
        {
            return await _repository.GetElectricityInfo();
        }

    }
}

