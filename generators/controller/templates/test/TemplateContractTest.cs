using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;

namespace <%= namespace %>.Tests
{
    public class <%= controllerName %>ContractTest : IClassFixture<TestServerFixture>
    {
        private readonly TestServerFixture _fixture;

        private readonly ITestOutputHelper _output;
        <%_ if(authentication) { _%>

        private readonly AuthTokenHelper _tokenHelper;

        private readonly ApplicationUser _applicationUser;
        <%_ } _%>

        public <%= controllerName %>ContractTest(TestServerFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
            <%_ if(authentication) { _%>
            _tokenHelper = new AuthTokenHelper();
            _applicationUser = _tokenHelper.GetUserToken(null, new[] { GroupTypes.Admin });
            <%_ } _%>
        }

        [Fact]
        public async void Create<%= modelName %>_Valid<%= modelName %>_CreatedResult()
        {
            // Arrange
            <%_ if(authentication) { _%>
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _applicationUser.IdToken);
            <%_ } _%>
            var dto = BogusData.GetCreate<%= modelName %>Dto();

            // Act
            var postResponse = await _fixture.Client.PostAsync("/<%= controllerName.toLowerCase() %>", dto);
            var created = await postResponse.Content.ReadAsJsonAsync<<%= modelName %>Dto>();
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>/{created.Id}");
            var fetched = await getResponse.Content.ReadAsJsonAsync<<%= modelName %>Dto>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal(created.Id, fetched.Id);
            <%_ if(authentication) { _%>
            Assert.Equal(_applicationUser.DisplayName, fetched.CreatedByDisplayName);
            Assert.Equal(_applicationUser.Email, fetched.CreatedById);
            <%_ } _%>
        }
        <%_ if(authentication) { _%>
        
        [Fact]
        public async void Create<%= modelName %>_Valid<%= modelName %>_ForbiddenUser()
        {
            // Arrange
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenHelper.GetUserToken(null, new[] { "TEST" }).IdToken);
            var dto = BogusData.GetCreate<%= modelName %>Dto();

            // Act
            var postResponse = await _fixture.Client.PostAsync("/<%= controllerName.toLowerCase() %>", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, postResponse.StatusCode);
        }
        <%_ } _%>

        [Fact]
        public async void Update<%= modelName %>_Valid<%= modelName %>_OK()
        {
            // Arrange
            <%_ if(authentication) { _%>
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _applicationUser.IdToken);
            <%_ } _%>
            var dtoCreate = BogusData.GetCreate<%= modelName %>Dto();

            var dtoUpdate = BogusData.GetUpdate<%= modelName %>Dto();

            var postResponse = await _fixture.Client.PostAsync("/<%= controllerName.toLowerCase() %>", dtoCreate);
            var created = await postResponse.Content.ReadAsJsonAsync<<%= modelName %>Dto>();

            // Act
            var putResponse = await _fixture.Client.PutAsync($"/<%= controllerName.toLowerCase() %>/{created.Id}", dtoUpdate);
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>/{created.Id}");
            var fetched = await getResponse.Content.ReadAsJsonAsync<<%= modelName %>Dto>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.Equal(created.Id, fetched.Id);
            <%_ if(authentication) { _%>
            Assert.Equal(_applicationUser.DisplayName, fetched.UpdatedByDisplayName);
            Assert.Equal(_applicationUser.Email, fetched.UpdatedById);
            <%_ } _%>
        }
        <%_ if(authentication) { _%>
        
        [Fact]
        public async void Update<%= modelName %>_Valid<%= modelName %>_ForbiddenUser()
        {
            // Arrange
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenHelper.GetUserToken(null, new[] { "TEST" }).IdToken);
            var dto = BogusData.GetUpdate<%= modelName %>Dto();

            // Act
            var putResponse = await _fixture.Client.PutAsync($"/<%= controllerName.toLowerCase() %>/232322", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, putResponse.StatusCode);
        }
        <%_ } _%>

        [Fact]
        public async void Get_ValidList_OkResult()
        {
            <%_ if(authentication) { _%>
            // Arrange
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _applicationUser.IdToken);

            <%_ } _%>

            <%_ if(database === 'dynamodb') { _%>
            // Act
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>");
            var fetched = await getResponse.Content.ReadAsJsonAsync<List<<%= modelName %>Dto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotEmpty(fetched);
            <%_ } else { _%>
            // Act
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>");
            var fetched = await getResponse.Content.ReadAsJsonAsync<PaginatedList<<%= modelName %>Dto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Assert.NotEmpty(fetched.Results);
            <%_ } _%>
        }
        <%_ if(authentication) { _%>

        [Fact]
        public async void Get_ValidList_ForbiddenUser()
        {
            // Arrange
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenHelper.GetUserToken(null, new[] { "TEST" }).IdToken);

            // Act
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, getResponse.StatusCode);
        }
        <%_ } _%>

        [Theory]
        [InlineData(22222)]
        public async void Get_Invalid<%= modelName %>Id_NotFound(long id)
        {
            <%_ if(authentication) { _%>
            // Arrange
            _fixture.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _applicationUser.IdToken);

            <%_ } _%>
            // Act
            var getResponse = await _fixture.Client.GetAsync($"/<%= controllerName.toLowerCase() %>/{id}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

    }
}
