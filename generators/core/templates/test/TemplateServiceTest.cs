using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
<%_ if(database === 'dynamodb') { _%>
using Amazon.DynamoDBv2.DataModel;
<%_ } _%>
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class <%= appname %>ServiceTest
    {

        private readonly Mock<ILogger<<%= appname %>Service>> _logger;

        <%_ if(createModel) { _%>
        private readonly Mock<I<%= modelName %>Repository> _<%= modelNameCamel %>Repository;

        <%_ } _%>
        private readonly Mock<IMapper> _mapper;
        <%_ if(kafka) { _%>

        private readonly IKafkaProducer _producer;
        <%_ } _%>

        public <%= appname %>ServiceTest()
        {
            _logger = new Mock<ILogger<<%= appname %>Service>>();
            _mapper = new Mock<IMapper>();
            <%_ if(kafka) { _%>
            _producer = new KafkaProducerStub();
            <%_ } _%>
            <%_ if(createModel) { _%>
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new <%= modelName %>Dto();
            dto.Id = id;
            var model = new <%= modelName %>();
            model.Id = id;
            <%_ if(database === 'dynamodb') { _%>
            var twoModels = new List<<%= modelName %>>()
            {
                new <%= modelName %>(),
                new <%= modelName %>()
            };
            var twoDtos = new List<<%= modelName %>Dto>()
            {
                new <%= modelName %>Dto(),
                new <%= modelName %>Dto()
            };
            <%_ } else { _%>
            var twoModels = new PaginatedList<<%= modelName %>>()
            {
                Results = new List<<%= modelName %>>()
                {
                    new <%= modelName %>(),
                    new <%= modelName %>()
                }
            };
            var twoDtos = new PaginatedList<<%= modelName %>Dto>()
            {
                Results = new List<<%= modelName %>Dto>()
                {
                    new <%= modelName %>Dto(),
                    new <%= modelName %>Dto()
                }
            };
            <%_ } _%>
            
            _mapper.Setup(x => x.Map<<%= modelName %>Dto>(It.IsAny<<%= modelName %>>())).Returns(dto);
            _mapper.Setup(x => x.Map<<%= modelName %>>(It.IsAny<Create<%= modelName %>Dto>())).Returns(model);
            _mapper.Setup(x => x.Map<<%= modelName %>>(It.IsAny<Update<%= modelName %>Dto>())).Returns(model);
            <%_ if(database === 'dynamodb') { _%>
            _mapper.Setup(x => x.Map<List<MyTestDto>>(It.IsAny<List<MyTest>>())).Returns(twoDtos);
            <%_ } else { _%>
            _mapper.Setup(x => x.Map<PaginatedList<<%= modelName %>Dto>>(It.IsAny<PaginatedList<<%= modelName %>>>())).Returns(twoDtos);   
            <%_ } _%>

            _<%= modelNameCamel %>Repository = new Mock<I<%= modelName %>Repository>();
            _<%= modelNameCamel %>Repository.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(model));
            _<%= modelNameCamel %>Repository.Setup(repo => repo.SaveAsync(It.IsAny<<%= modelName %>>()))
                .Returns(Task.FromResult<int>(1));
            _<%= modelNameCamel %>Repository.Setup(repo => repo.UpdateAsync(It.IsAny<<%= modelName %>>()))
                .Returns(Task.FromResult<int>(1));
            <%_ if(database === 'dynamodb') { _%>
            _<%= modelNameCamel %>Repository.Setup(repo => repo.FindAsync(It.IsAny<IEnumerable<ScanCondition>>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IQueryable<<%= modelName %>>>(twoModels.AsQueryable()));
            <%_ } else { _%>
            _<%= modelNameCamel %>Repository.Setup(repo => repo.FindAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult<PaginatedList<<%= modelName %>>>(twoModels));
            <%_ } _%>
            <%_ } _%>
        }

        private <%= appname %>Service CreateService()
        {
            return new <%= appname %>Service(
                <%_ if(createModel) { _%>
                _<%= modelNameCamel %>Repository.Object,
                <%_ } _%>
                _logger.Object,
                <%_ if(kafka) { _%>
                _producer,
                <%_ } _%>
                _mapper.Object);
        }

        <%_ if(createModel) { _%>
        [Fact]
        public async void Create<%= modelName %>_ValidDto_Return<%= modelName %>Dto()
        {
            // Arrange
            var dto = new Create<%= modelName %>Dto();
            var user = new ApplicationUser();
            var service = CreateService();

            // Act
            var result = await service.Save<%= modelName %>Async(dto, user);

            // Assert
            var resultDto = Assert.IsType<<%= modelName %>Dto>(result);
            Assert.NotNull(resultDto);
        }

        [Fact]
        public async void Update<%= modelName %>_ValidDto_Return<%= modelName %>Dto()
        {
            // Arrange 
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new Update<%= modelName %>Dto();
            var user = new ApplicationUser();
            var service = CreateService();

            // Act
            var result = await service.Update<%= modelName %>Async(id, dto, user);

            // Assert
            var resultDto = Assert.IsType<<%= modelName %>Dto>(result);
            Assert.NotNull(resultDto);
        }

        [Fact]
        public async void Update<%= modelName %>_InvalidDto_NotFoundException()
        {
            // Arrange 
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var dto = new Update<%= modelName %>Dto();
            var user = new ApplicationUser();
            var service = CreateService();

            _<%= modelNameCamel %>Repository.Setup(repo => repo.GetByIdAsync(It.IsAny<<%= idType %>>()))
                .Returns(Task.FromResult<<%= modelName %>>(null));

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await service.Update<%= modelName %>Async(id, dto, user));
        }

        [Fact]
        public async void Get<%= modelName %>_ValidId_Return<%= modelName %>Dto()
        {
            // Arrange 
            <%_ if(database === 'dynamodb') { _%>
            string id = Guid.NewGuid().ToString();
            <%_ } else { _%>
            long id = 5;
            <%_ } _%>
            var user = new ApplicationUser();
            var service = CreateService();

            // Act
            var result = await service.Get<%= modelName %>Async(id, user);

            // Assert
            var resultDto = Assert.IsType<<%= modelName %>Dto>(result);
            Assert.NotNull(resultDto);
        }

        [Fact]
        public async void Find<%= modelName %>Async_ValidId_Return<%= modelName %>Dto()
        {
            // Arrange 
            var user = new ApplicationUser();
            var searchDto = new Search<%= modelName %>Dto();
            var service = CreateService();

            // Act
            var result = await service.Find<%= modelName %>Async(searchDto, user);

            // Assert
            <%_ if(database === 'dynamodb') { _%>
            var resultDto = Assert.IsType<List<<%= modelName %>Dto>>(result);
            Assert.NotNull(resultDto);
            <%_ } else { _%>
            var resultDto = Assert.IsType<PaginatedList<<%= modelName %>Dto>>(result);
            Assert.NotNull(resultDto.Results);
            <%_ } _%>
        }
        <%_ } _%>
    }
}
