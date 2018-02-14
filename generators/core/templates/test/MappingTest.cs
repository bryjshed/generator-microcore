using System.Linq;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class MappingTest
    {
        private readonly IMapper _mapper;

        public MappingTest()
        {
            var servicesProvider = new ServiceCollection();
            servicesProvider.AddAutoMapper(cfg => 
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = servicesProvider.BuildServiceProvider().GetService<IMapper>();
        }

        [Fact]
        public void AutoMapper_Configuration_IsValid()
        {
            // Assert
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void AutoMapper_<%= modelName %>To<%= modelName %>Dto_IsValid()
        {
            // Arrange
            var model = BogusData.Get<%= modelName %>();

            // Act
            var dto = _mapper.Map<<%= modelName %>Dto>(model);

            // Assert
            CompareModelToDto(model, dto);
        }
        <%_ if(addDatabase && database !== 'dynamodb') { _%>

        [Fact]
        public void AutoMapper_PaginatedList<%= modelName %>ToPaginatedList<%= modelName %>Dto_IsValid()
        {
            // Arrange
            var count = 5;
            var items = BogusData.Get<%= modelName %>s(count);
            var list = new PaginatedList<<%= modelName %>>(items.ToList(), count, 1, count, items.Count());

            // Act
            var dto = _mapper.Map<PaginatedList<<%= modelName %>Dto>>(list);

            // Assert
            Assert.Equal(list.HasNextPage, dto.HasNextPage);
            Assert.Equal(list.HasPreviousPage, dto.HasPreviousPage);
            Assert.Equal(list.PageNumber, dto.PageNumber);
            Assert.Equal(list.TotalFilterdRecords, dto.TotalFilterdRecords);
            Assert.Equal(list.TotalPages, dto.TotalPages);
            Assert.Equal(list.TotalRecords, dto.TotalRecords);
        }

        <%_ } _%>
        [Fact]
        public void AutoMapper_Create<%= modelName %>DtoTo<%= modelName %>_IsValid()
        {
            // Arrange
            var dto = BogusData.GetCreate<%= modelName %>Dto();

            // Act
            var model = _mapper.Map<<%= modelName %>>(dto);

            // Assert
            
        }

        [Fact]
        public void AutoMapper_Update<%= modelName %>DtoTo<%= modelName %>_IsValid()
        {
            // Arrange
            var dto = BogusData.GetUpdate<%= modelName %>Dto();

            // Act
            var model = _mapper.Map<<%= modelName %>>(dto);

            // Assert

        }

        [Fact]
        public void AutoMapper_Update<%= modelName %>DtoTo<%= modelName %>Current_IsValid()
        {
            // Arrange
            var dto = BogusData.GetUpdate<%= modelName %>Dto();
            var model = BogusData.Get<%= modelName %>();

            // Act
            _mapper.Map<Update<%= modelName %>Dto, <%= modelName %>>(dto, model);

            // Assert
        }

        private void CompareModelToDto(<%= modelName %> model, <%= modelName %>Dto dto)
        {
            Assert.Equal(model.CreationDate.FormatDate(Constants.ISODateTimeFormat), dto.CreationDate);
            Assert.Equal(model.LastUpdatedDate.FormatDate(Constants.ISODateTimeFormat), dto.LastUpdatedDate);
            Assert.Equal(model.CreatedById, dto.CreatedById);
            Assert.Equal(model.CreatedByDisplayName, dto.CreatedByDisplayName);
            Assert.Equal(model.UpdatedById, dto.UpdatedById);
            Assert.Equal(model.UpdatedByDisplayName, dto.UpdatedByDisplayName);
        }
    }
}
