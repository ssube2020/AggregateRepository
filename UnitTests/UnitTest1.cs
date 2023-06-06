using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using AggregateApp.Services;
using Core.Common;
using Core.Interfaces;
using Core.Entities;
using Moq.Protected;
using System.Net;

namespace Tests
{
    [TestFixture]
    public class AggregationTestClass
    {
        private Mock<IAggregateRepository> _mockRepository;
        private ElectricityService _electricityService;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IAggregateRepository>();
            _electricityService = new ElectricityService(_mockRepository.Object);
        }

        //[Test]
        //public async Task ProcessCsvFile_ShouldReadAndProcessCsvFile()
        //{
        //    // Arrange
        //    var url = "https://example.com/csvfile.csv";
        //    var csvContent = "A,Butas,Type,1,10.5,2023-06-01,5.2\nB,NotButas,Type,2,8.9,2023-06-02,3.1";

        //    var expectedData = new List<ElectricityInfoModel>
        //{
        //    new ElectricityInfoModel
        //    {
        //        Network = "A",
        //        Obt_Name = "Butas",
        //        Obj_Gv_Type = "Type",
        //        Obj_Number = 1,
        //        P_Plus = 10.5,
        //        Pl_T = new DateTime(2023, 06, 01),
        //        P_Minus = 5.2
        //    }
        //};

        //    var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        //    var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        //    var httpClient = new HttpClient(mockHttpMessageHandler.Object);

        //    mockHttpMessageHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Content = new StringContent(csvContent)
        //        });

        //    mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

        //    var instance = new AggregateRepository(mockHttpClientFactory.Object);

        //    // Act
        //    var result = await instance.ProcessCsvFile(url);

        //    // Assert
        //    Assert.AreEqual(expectedData, result);
        //}

        [Test]
        public void ProcessCsvLineData_ShouldProcessCsvLineData()
        {
            // Arrange
            var data = new List<ElectricityInfoModel>();
            var line = "A,Butas,Type,1,10.5,2023-06-01,5.2";

            var expectedData =
                new ElectricityInfoModel
                {
                    Network = "A",
                    Obt_Name = "Butas",
                    Obj_Gv_Type = "Type",
                    Obj_Number = 1,
                    P_Plus = 10.5,
                    Pl_T = new DateTime(2023, 06, 01),
                    P_Minus = 5.2
                };

            _mockRepository.Setup(r => r.ProcessCSVLine(line)).Returns(new ElectricityInfoModel()
            {
                Network = "A",
                Obt_Name = "Butas",
                Obj_Gv_Type = "Type",
                Obj_Number = 1,
                P_Plus = 10.5,
                Pl_T = new DateTime(2023, 06, 01),
                P_Minus = 5.2,
                Id = 0
            });
            var result = _mockRepository.Object.ProcessCSVLine(line);
            Assert.That(expectedData.Id == result.Id);
        }

        [Test]
        public void GroupAndTurnIntoEntity_ShouldGroupDataAndCreateEntities()
        {
            // Arrange
            var testData = new List<ElectricityInfoModel>
            {
                new ElectricityInfoModel { Network = "Network1", P_Plus = 10, P_Minus = 5 },
                new ElectricityInfoModel { Network = "Network2", P_Plus = 8, P_Minus = 3 },
                new ElectricityInfoModel { Network = "Network1", P_Plus = 15, P_Minus = 7 }
            };

            var expectedEntities = new List<ElectricityInfo>
            {
                new ElectricityInfo { Network = "Network1", P_Plus = 25, P_Minus = 12 },
                new ElectricityInfo { Network = "Network2", P_Plus = 8, P_Minus = 3 }
            };


            _mockRepository
                  .Setup(r => r.GroupAndTurnIntoentity(It.IsAny<List<ElectricityInfoModel>>()))
                  .Returns<List<ElectricityInfoModel>>(data =>
                  {
                      var groupedData = data.GroupBy(info => info.Network)
                          .Select(group => new ElectricityInfo
                          {
                              Network = group.Key,
                              P_Plus = group.Sum(info => info.P_Plus),
                              P_Minus = group.Sum(info => info.P_Minus)
                          })
                          .ToList();

                      return groupedData;
                  });


            var result = _mockRepository.Object.GroupAndTurnIntoentity(testData);

            // Assert
            Assert.AreEqual(expectedEntities.Count, result.Count);

            foreach (var expectedEntity in expectedEntities)
            {
                var matchingEntity = result.FirstOrDefault(e => e.Network == expectedEntity.Network);
                Assert.IsNotNull(matchingEntity);

                Assert.AreEqual(expectedEntity.P_Plus, matchingEntity.P_Plus);
                Assert.AreEqual(expectedEntity.P_Minus, matchingEntity.P_Minus);
            }
        }
    }
}
