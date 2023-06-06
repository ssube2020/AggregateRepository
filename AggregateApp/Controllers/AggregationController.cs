using System;
using AggregateApp.Services;
using Core.Common;
using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AggregateApp.Controllers;

[ApiController]
[Route("[controller]/[Action]")] 
public class AggregationController : ControllerBase
{
    private readonly ILogger<AggregationController> _logger;
    private readonly ElectricityService _electricityService;

    public AggregationController(ILogger<AggregationController> logger, ElectricityService downloadereService)
    {
        _logger = logger;
        _electricityService = downloadereService;
    }

    [HttpGet(Name = "DownloadDataFromServer")]
    public async Task<ExecutionResult<List<ElectricityInfo>>> GetElectricityData()
    {
        return await _electricityService.GetElectroData();
    }

    [HttpGet(Name = "GetElectricityData")]
    public async Task<ExecutionResult<bool>> GetDataFromServer()
    {
        return await _electricityService.DownloadData();
    }
}

