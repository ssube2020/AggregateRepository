using System;
using System.Globalization;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Repositories
{

	public class AggregateRepository : IAggregateRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AggregateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ExecutionResult<List<ElectricityInfo>>> GetElectricityInfo()
        {
            try
            {
                var data = await _dbContext.ElectricityInfos.ToListAsync();
                if(data != null)
                {
                    return new ExecutionResult<List<ElectricityInfo>>
                    {
                        Success = true,
                        Data = data
                    };
                } else
                {
                    return new ExecutionResult<List<ElectricityInfo>>
                    {
                        Success = false,
                        Data = null,
                        Message = "data not found"
                    };
                }
            }
            catch(Exception ex)
            {
                return new ExecutionResult<List<ElectricityInfo>>
                {
                    Success = false,
                    Data = null,
                    StatusCode = 500,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ExecutionResult<bool>> DownloadData()
        {
            try
            {
                List<ElectricityInfoModel> allData = new List<ElectricityInfoModel>();

                string url1 = "https://data.gov.lt/dataset/1975/download/10763/2022-02.csv";
                string url2 = "https://data.gov.lt/dataset/1975/download/10764/2022-03.csv";
                string url3 = "https://data.gov.lt/dataset/1975/download/10765/2022-04.csv";
                string url4 = "https://data.gov.lt/dataset/1975/download/10766/2022-05.csv";

                //Create 4 different task to execute them paralelly on 4 different thread to improve performance

                Task<List<ElectricityInfoModel>> downloadTask1 = ProcessCsvFile(url1);
                Task<List<ElectricityInfoModel>> downloadTask2 = ProcessCsvFile(url2);
                Task<List<ElectricityInfoModel>> downloadTask3 = ProcessCsvFile(url3);
                Task<List<ElectricityInfoModel>> downloadTask4 = ProcessCsvFile(url4);

                await Task.WhenAll(downloadTask1, downloadTask2, downloadTask3, downloadTask4);

                allData.AddRange(downloadTask1.Result);
                allData.AddRange(downloadTask2.Result);
                allData.AddRange(downloadTask3.Result);
                allData.AddRange(downloadTask4.Result);

                var groupedData = allData.GroupBy(info => info.Network)
                    .Select(group => new
                    {
                        Network = group.Key,
                        SumP_Plus = group.Sum(info => info.P_Plus),
                        SumP_Minus = group.Sum(info => info.P_Minus)
                    });

                List<ElectricityInfo> entitiesToAdd = new List<ElectricityInfo>();
                foreach (var item in groupedData)
                {
                    ElectricityInfo toAdd = new();
                    toAdd.Network = item.Network;
                    toAdd.P_Plus = item.SumP_Plus;
                    toAdd.P_Minus = item.SumP_Minus;

                    entitiesToAdd.Add(toAdd);
                }

                await _dbContext.AddRangeAsync(entitiesToAdd);
                await _dbContext.SaveChangesAsync();

                return new ExecutionResult<bool>
                {
                    Success = true,
                    Data = true,
                    Message ="data downloaded and saved succesfully"
                };
            }
            catch(Exception ex)
            {
                return new ExecutionResult<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "could not download data",
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }

        }

        public List<ElectricityInfo> GroupAndTurnIntoentity(List<ElectricityInfoModel> data) 
        {
            var groupedData = data.GroupBy(info => info.Network)
                    .Select(group => new
                    {
                        Network = group.Key,
                        SumP_Plus = group.Sum(info => info.P_Plus),
                        SumP_Minus = group.Sum(info => info.P_Minus)
                    });

            List<ElectricityInfo> entitiesToAdd = new();
            foreach (var item in groupedData)
            {
                ElectricityInfo toAdd = new();
                toAdd.Network = item.Network;
                toAdd.P_Plus = item.SumP_Plus;
                toAdd.P_Minus = item.SumP_Minus;

                entitiesToAdd.Add(toAdd);
            }

            return entitiesToAdd;
        }

        public async Task<List<ElectricityInfoModel>> ProcessCsvFile(string url)
        {
            List<ElectricityInfoModel> data = new List<ElectricityInfoModel>();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(10); // Set the timeout to 10 minutes
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (Stream stream = await response.Content.ReadAsStreamAsync())

                            using (StreamReader reader = new StreamReader(stream))
                            {
                                string line;
                                while ((line = await reader.ReadLineAsync()) != null)
                                {
                                 
                                        string[] fields = line.Split(',');
                                        if (fields[1] == "Butas")
                                        {

                                        int objNumber;
                                        double pPlus;
                                        double pMinus;
                                        // Create a new ElectricityInfoModel object and set its properties
                                        ElectricityInfoModel electricityInfo = new ElectricityInfoModel
                                        {
                                            Network = fields[0],
                                            Obt_Name = fields[1],
                                            Obj_Gv_Type = fields[2],
                                            Obj_Number = int.TryParse(fields[3], out objNumber) ? (int?)objNumber : null,
                                            P_Plus = double.TryParse(fields[4], NumberStyles.Any, CultureInfo.InvariantCulture, out pPlus) ? (double?)pPlus : null,
                                            Pl_T = DateTime.Parse(fields[5]),
                                            P_Minus = double.TryParse(fields[6], NumberStyles.Any, CultureInfo.InvariantCulture, out pMinus) ? (double?)pMinus : null
                                        };
                                        data.Add(electricityInfo);
                                        }
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine($"Failed to download CSV file. Status code: {response.StatusCode}");
                        }
                    }
                }
                catch(Exception ex)
                {
                    string mes = ex.Message;
                }
                
            }

            return data;
        }

        public ElectricityInfoModel ProcessCSVLine(string line)
        {
            string[] fields = line.Split(',');

                int objNumber;
                double pPlus;
                double pMinus;
                // Create a new ElectricityInfoModel object and set its properties
                ElectricityInfoModel electricityInfo = new ElectricityInfoModel
                {
                    Network = fields[0],
                    Obt_Name = fields[1],
                    Obj_Gv_Type = fields[2],
                    Obj_Number = int.TryParse(fields[3], out objNumber) ? (int?)objNumber : null,
                    P_Plus = double.TryParse(fields[4], NumberStyles.Any, CultureInfo.InvariantCulture, out pPlus) ? (double?)pPlus : null,
                    Pl_T = DateTime.Parse(fields[5]),
                    P_Minus = double.TryParse(fields[6], NumberStyles.Any, CultureInfo.InvariantCulture, out pMinus) ? (double?)pMinus : null
                };
            return electricityInfo;
            
        }
        
    }

}

