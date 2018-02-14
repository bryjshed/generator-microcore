using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
<%_ if(database === 'dynamodb') { _%>
using Amazon.DynamoDBv2.DataModel;
using System.Linq;
using ThirdParty.Json.LitJson;
<%_ } _%>

namespace <%= namespace %>.Tests
{
    public class <%= controllerName %>ControllerTest
    {

        [Fact]
        public async void Create<%= modelName %>_When<%= modelName %>Valid_CreatedAtRouteResult()
        {
            // Arrange
            var dto = new <%= modelName %>Dto();
            var model = new <%= modelName %>();

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.SaveAsync(It.IsAny<<%= modelName %>>()))
                .Returns(Task.FromResult<int>(1));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<<%= modelName %>Dto>(It.IsAny<<%= modelName %>>())).Returns(dto);
            mapperMock.Setup(x => x.Map<<%= modelName %>>(It.IsAny<<%= modelName %>Dto>())).Returns(model);

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var actionResult = await controller.Create<%= modelName %>(dto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult);
            Assert.Equal(201, createdAtRouteResult.StatusCode);
            Assert.Equal("Get<%= modelName %>", createdAtRouteResult.RouteName);
            Assert.IsType<<%= modelName %>Dto>(createdAtRouteResult.Value);
        }

        [Fact]
        public async void Get_Valid<%= modelName %>_OkResult()
        {
            // Arrange
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new <%= modelName %>Dto() { Id = id };
            var model = new <%= modelName %>() { Id = id };

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(model));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<<%= modelName %>Dto>(It.IsAny<<%= modelName %>>())).Returns(dto);

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var getResult = await controller.Get<%= modelName %>(id);

            // Assert
            var providerResult = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<<%= modelName %>Dto>(providerResult.Value);
            Assert.Equal(200, providerResult.StatusCode);
            Assert.Equal(id, resultDto.Id);
        }

        [Fact]
        public async void Get_Invalid<%= modelName %>_NotFoundResult()
        {
            // Arrange
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(null));

            var mapperMock = new Mock<IMapper>();

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var getResult = await controller.Get<%= modelName %>(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(getResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async void Update<%= modelName %>_When<%= modelName %>Valid_OkResult()
        {
            // Arrange
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new <%= modelName %>Dto();
            var model = new <%= modelName %>() { Id = id };

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(model));
            repoMock.Setup(repo => repo.UpdateAsync(It.IsAny<<%= modelName %>>()))
                .Returns(Task.FromResult<int>(1));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<<%= modelName %>Dto>(It.IsAny<<%= modelName %>>())).Returns(dto);
            mapperMock.Setup(x => x.Map<<%= modelName %>>(It.IsAny<<%= modelName %>Dto>())).Returns(model);

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var putResult = await controller.Update<%= modelName %>(id, dto);

            // Assert
            var providerResult = Assert.IsType<OkResult>(putResult);
            Assert.Equal(200, providerResult.StatusCode);
        }

        [Fact]
        public async void Update<%= modelName %>_Invalid<%= modelName %>Id_NotFoundResult()
        {
            // Arrange
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new <%= modelName %>Dto();

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(null));

            var mapperMock = new Mock<IMapper>();

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var putResult = await controller.Update<%= modelName %>(id, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(putResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        <%_ if(database === 'dynamodb') { _%>
        [Fact]
        public async void Get<%= modelName %>s_WhenSearchValid_OkResult()
        {
            // Arrange
            var searchDto = new <%= modelName %>SearchDto();
            var models = new List<<%= modelName %>>();
            var twoDtos = new List<<%= modelName %>Dto>()
            {
                new <%= modelName %>Dto(),
                new <%= modelName %>Dto()
            };


            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.FindAsync(
                    It.IsAny<IEnumerable<ScanCondition>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Returns(Task.FromResult(models.AsQueryable()));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x =>
                x.Map<List<<%= modelName %>Dto>>(It.IsAny<List<<%= modelName %>>>()))
                .Returns(twoDtos);

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var getResult = await controller.Get<%= modelName %>s(searchDto);

            // Assert
            var result = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<List<<%= modelName %>Dto>>(result.Value);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(twoDtos.Count, resultDto.Count);
        }

        [Fact]
        public async void Get<%= modelName %>s_InvalidStartKey_BadRequestResult()
        {
            // Arrange
            var searchDto = new <%= modelName %>SearchDto();

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.FindAsync(
                    It.IsAny<IEnumerable<ScanCondition>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Throws(new JsonException());

            var mapperMock = new Mock<IMapper>();

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var getResult = await controller.Get<%= modelName %>s(searchDto);

            // Assert
            var providerResult = Assert.IsType<BadRequestObjectResult>(getResult);
            Assert.Equal(400, providerResult.StatusCode);
        }

        [Fact]
        public async void Get<%= modelName %>s_InvalidOperation_ThrowsException()
        {
            // Arrange
            var searchDto = new <%= modelName %>SearchDto();

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.FindAsync(
                    It.IsAny<IEnumerable<ScanCondition>>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()))
                .Throws(new Exception("Invalid Opertion."));

            var mapperMock = new Mock<IMapper>();

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            // Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await controller.Get<%= modelName %>s(searchDto));
            Assert.Equal("Invalid Opertion.", exception.Message);
        }

        <%_ } else { _%>
        [Fact]
        public async void Gets_Valid_OkResult()
        {
            // Arrange
            var models = new List<<%= modelName %>>();
            var twoDtos = new List<<%= modelName %>Dto>()
            {
                new <%= modelName %>Dto(),
                new <%= modelName %>Dto()
            };

            var repoMock = new Mock<I<%= modelName %>Repository>();
            repoMock.Setup(repo => repo.ListAsync())
                .Returns(Task.FromResult<List<<%= modelName %>>>(models));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x =>
               x.Map<List<<%= modelName %>Dto>>(It.IsAny<List<<%= modelName %>>>()))
               .Returns(twoDtos);

            var controller = new <%= controllerName %>Controller(
                new Mock<ILogger<<%= controllerName %>Controller>>().Object,
                mapperMock.Object,
                repoMock.Object)
            {
                ControllerContext = new ControllerContext(AuthData.GetActionContext())
            };

            // Act
            var getResult = await controller.Get<%= modelName %>s();

            // Assert
            var result = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<List<<%= modelName %>Dto>>(result.Value);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(twoDtos.Count, resultDto.Count);
        }
        <%_ } _%>
    }
}
