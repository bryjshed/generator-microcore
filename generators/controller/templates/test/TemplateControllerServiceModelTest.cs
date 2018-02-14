using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class <%= controllerName %>ControllerTest
    {

        private readonly Mock<I<%= appname %>Service> _service;

        private Mock<ILogger<<%= controllerName %>Controller>> _logger;

        public <%= controllerName %>ControllerTest()
        {
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new <%= modelName %>Dto();
            dto.Id = id;
            var model = new <%= modelName %>();
            model.Id = id;
            
            _logger = new Mock<ILogger<<%= controllerName %>Controller>>();

            _service = new Mock<I<%= appname %>Service>();

            <%_ if(database === 'dynamodb') { _%>
            var twoDtos = new List<<%= modelName %>Dto>()
            {
                new <%= modelName %>Dto(),
                new <%= modelName %>Dto()
            };
            _service.Setup(repo => repo.Find<%= modelName %>Async(It.IsAny<Search<%= modelName %>Dto>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<List<<%= modelName %>Dto>>(twoDtos));
            <%_ } else { _%>
            var twoDtos = new PaginatedList<<%= modelName %>Dto>()
            {
                Results = new List<<%= modelName %>Dto>()
                {
                    new <%= modelName %>Dto(),
                    new <%= modelName %>Dto()
                }
            };
            _service.Setup(repo => repo.Find<%= modelName %>Async(It.IsAny<Search<%= modelName %>Dto>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<PaginatedList<<%= modelName %>Dto>>(twoDtos));
            <%_ } _%>
            _service.Setup(repo => repo.Get<%= modelName %>Async(It.IsAny<<%= idType %>>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<<%= modelName %>Dto>(dto));
            _service.Setup(repo => repo.Save<%= modelName %>Async(It.IsAny<Create<%= modelName %>Dto>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<<%= modelName %>Dto>(dto));
            _service.Setup(repo => repo.Update<%= modelName %>Async(It.IsAny<<%= idType %>>(), It.IsAny<Update<%= modelName %>Dto>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<<%= modelName %>Dto>(dto));
            
        }

        private <%= controllerName %>Controller CreateController()
        {
            return new <%= controllerName %>Controller(
                _logger.Object,
                _service.Object)
            {
                ControllerContext = new ControllerContext(AuthTokenHelper.GetActionContext(new Claim(ClaimTypes.Groups, GroupTypes.Admin)))
            };
        }


        [Fact]
        public async void Create<%= modelName %>_When<%= modelName %>Valid_CreatedAtRouteResult()
        {
            // Arrange
            var dto = new Create<%= modelName %>Dto();

            var controller = CreateController();

            // Act
            var actionResult = await controller.Create<%= modelName %>(dto);

            // Assert
            var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(actionResult);
            Assert.Equal(201, createdAtRouteResult.StatusCode);
            Assert.Equal(createdAtRouteResult.RouteName, "Get<%= modelName %>");
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
            var controller = CreateController();

            // Act
            var getResult = await controller.Get<%= modelName %>(id);

            // Assert
            var providerResult = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<<%= modelName %>Dto>(providerResult.Value);
            Assert.Equal(200, providerResult.StatusCode);
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

            _service.Setup(repo => repo.Get<%= modelName %>Async(It.IsAny<<%= idType %>>(), It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult<<%= modelName %>Dto>(null));

            var controller = CreateController();

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
            var dto = new Update<%= modelName %>Dto();

            var controller = CreateController();

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
            var dto = new Update<%= modelName %>Dto();

            _service.Setup(repo => repo.Update<%= modelName %>Async(It.IsAny<<%= idType %>>(), It.IsAny<Update<%= modelName %>Dto>(), It.IsAny<ApplicationUser>()))
                .Throws(new NotFoundException());

            var controller = CreateController();

            // Act
            var putResult = await controller.Update<%= modelName %>(id, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(putResult);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        
        [Fact]
        public async void Gets_Valid_OkResult()
        {
            // Arrange
            var dto = new Search<%= modelName %>Dto();
            
            var controller = CreateController();

            // Act
            var getResult = await controller.Get<%= modelName %>s(dto);

            <%_ if(database === 'dynamodb') { _%>
            // Assert
            var result = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<List<<%= modelName %>Dto>>(result.Value);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, resultDto.Count);
            <%_ } else { _%>
            // Assert
            var result = Assert.IsType<OkObjectResult>(getResult);
            var resultDto = Assert.IsType<PaginatedList<<%= modelName %>Dto>>(result.Value);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, resultDto.Results.Count);
            <%_ } _%>
        }   
    }
}
