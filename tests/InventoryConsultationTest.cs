// using AutoMapper;
// using Domain.Adapters;
// using Domain.Entities.Inventories;
// using Domain.Enums;
// using Domain.Services;
// using Infra.ServiceLayer.Interfaces;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using Moq;

// namespace tests
// {
//     public class InventoryConsultationTest
//     {
//         private List<Inventory> ListInventoryMock()
//         {
//             return new List<Inventory>()
//             {
//                 new Inventory(new List<InventoryValue> { new InventoryValue("", "01-01-01-01-01", "123", "Nome", 1.0, RtrictType.None) }, "")
//             };
//         }

//         [Fact]
//         public void Should_Get_Data_StockBySku_When_Pass_Sku()
//         {
//             // Arrange and Given
//             var inventories = ListInventoryMock();

//             // Act

//             // Assert
//         }

//         [Fact]
//         public async Task Should_Return_BadRequest_When_Pass_Empty_Sku()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsByItemCodeAsync("");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_NotFound_When_Pass_Unknown_Sku()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsByItemCodeAsync("456");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<NotFoundResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_Ok_When_Pass_Valid_Sku()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             serviceLayerAdapter
//                 .Setup(r => r.InventoryBalanceBinLocationsByItemCodeAsync(inventories.First().Value.First().ItemCode, 0).Result)
//                 .Returns(new Inventory(new List<InventoryValue> { new InventoryValue("", "01-01-01-01-01", "123", "Nome", 1.0, RtrictType.None) }, ""));

//             var mapperConfig = new MapperConfiguration(cfg => 
//             {
//                 cfg.AddProfile(new AutoMapperProfile());
//             });

//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsByItemCodeAsync(inventories.First().Value.First().ItemCode);
//             var actualResult = (OkObjectResult) (response.Result ?? throw new Exception());
//             var bodyResult = (List<InventoryViewModel>) (actualResult.Value ?? throw new Exception());

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<OkObjectResult>(response.Result);
//             Assert.Equal("Nome", bodyResult.First().ItemName);
//         }

//         [Fact]
//         public async Task Should_Return_BadRequest_When_Pass_Empty_Serie()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsBySerieAsync("");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_NotFound_When_Pass_Unknown_Serie()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsBySerieAsync("serie0000");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<NotFoundResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_Ok_When_Pass_Valid_Serie()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBalanceBinLocationsBySerieAsync("serieId");
//             var actualResult = (OkObjectResult) (response.Result ?? throw new Exception());
//             var bodyResult = (List<Inventory>) (actualResult.Value ?? throw new Exception());

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<OkObjectResult>(response.Result);
//             Assert.Equal("Nome", bodyResult.First().Value.First().ItemName);
//         }

//         [Fact]
//         public async Task Should_Return_BadRequest_When_Pass_Empty_BinCode()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByBinCodeAsync("");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_NotFound_When_Pass_Unknown_BinCode()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByBinCodeAsync("02-02-02-02-02");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<NotFoundResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_Ok_When_Pass_Valid_BinCode()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByBinCodeAsync("01-01-01-01-01");
//             var actualResult = (OkObjectResult) (response.Result ?? throw new Exception());
//             var bodyResult = (List<Inventory>) (actualResult.Value ?? throw new Exception());

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<OkObjectResult>(response.Result);
//             Assert.Equal("Nome", bodyResult.First().Value.First().ItemName);
//         }

//         [Fact]
//         public async Task Should_Return_BadRequest_When_Pass_Empty_Gtin()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByGtinAsync("");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<BadRequestObjectResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_NotFound_When_Pass_Unknown_Gtin()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByGtinAsync("0010123495768");

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<NotFoundResult>(response.Result);
//         }

//         [Fact]
//         public async Task Should_Return_Ok_When_Pass_Valid_Gtin()
//         {
//             // Given
//             var logger = new Mock<ILogger<InventoryController>>();
//             var inventories = ListInventoryMock();

//             var serviceLayerAdapter = new Mock<IInventorySLService>();
//             var transferService = new Mock<ITransferService>();
//             var mapper = new Mock<IMapper>();

//             // Arrange
//             var controller = new InventoryController(logger.Object, serviceLayerAdapter.Object, transferService.Object, mapper.Object);

//             // Act
//             var response = await controller.GetInventoryBinLocationsByGtinAsync("7890123495768");
//             var actualResult = (OkObjectResult) (response.Result ?? throw new Exception());
//             var bodyResult = (List<Inventory>) (actualResult.Value ?? throw new Exception());

//             // Assert
//             Assert.NotNull(response);
//             Assert.IsAssignableFrom<OkObjectResult>(response.Result);
//             Assert.Equal("Nome", bodyResult.First().Value.First().ItemName);
//         }
//     }
// }