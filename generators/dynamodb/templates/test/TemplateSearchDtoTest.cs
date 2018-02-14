using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class Search<%= modelName %>DtoTest
    {
        [Fact]
        public void Valid()
        {
            // Arrange
            var dto = new Search<%= modelName %>Dto
            {
                StartKey = "6559bdfe-ca9d-42a2-8bf2-6500786fb8e8",
                PageSize = 1
            };
            var context = new ValidationContext(dto, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(dto, context, result, true);

            // Assert
            Assert.True(valid);
        }

        [Fact]
        public void PageSizeInvalid()
        {
            // Arrange
            var dto = new Search<%= modelName %>Dto
            {
                StartKey = "6559bdfe-ca9d-42a2-8bf2-6500786fb8e8",
                PageSize = 0
            };
            var context = new ValidationContext(dto, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(dto, context, result, true);

            // Assert
            Assert.False(valid);
            var failure = Assert.Single(result, x => x.ErrorMessage == $"PageSize must be greater than 0.");
            Assert.Single(failure.MemberNames, x => x.Equals("PageSize"));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("asdasdasdasdasdasd")]
        [InlineData("test")]
        [InlineData("6559bdfe-ca9d-42a2-8bf2-6500786fb8e8ss")]
        [InlineData("6559bdfe-ca9d-42a2-8bf2-6500786f")]
        public void StartKeyInvalid(string startKey)
        {
            // Arrange
            var dto = new Search<%= modelName %>Dto
            {
                StartKey = startKey,
                PageSize = 1
            };
            var context = new ValidationContext(dto, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(dto, context, result, true);

            // Assert
            Assert.False(valid);
            var failure = Assert.Single(result, x => x.ErrorMessage == $"The StartKey field is not valid.");
            Assert.Single(failure.MemberNames, x => x.Equals("StartKey"));
        }
    }
}
