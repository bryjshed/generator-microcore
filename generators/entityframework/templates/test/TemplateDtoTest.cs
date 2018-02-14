using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class <%= modelName %>DtoTest
    {
        [Fact]
        public void Valid()
        {
            // Arrange
            var dto = BogusData.Get<%= modelName %>Dto();

            var context = new ValidationContext(dto, null, null);
            var result = new List<ValidationResult>();

            // Act
            var valid = Validator.TryValidateObject(dto, context, result, true);

            // Assert
            Assert.True(valid);
        }
    }
}
