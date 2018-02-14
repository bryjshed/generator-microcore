using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace <%= namespace %>.Tests
{
    public class ValidateModelStateAttributeTest
    {
        [Fact]
        public void ValidationFilterAttributeTest_ModelStateErrors_ResultInBadRequestResult()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                filters: new List<IFilterMetadata>(), // for majority of scenarios you need not worry about populating this parameter
                actionArguments: new Dictionary<string, object>(), // if the filter uses this data, add some data to this dictionary
                controller: null); // since the filter being tested here does not use the data from this parameter, just provide null
            var validationFilter = new ValidateModelStateAttribute();

            // Act
            // Add an error into model state on purpose to make it invalid
            actionContext.ModelState.AddModelError("Error", "Some random error happened.");
            validationFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var jsonResult = Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
            Assert.Equal(400, jsonResult.StatusCode);
        }

        [Fact]
        public void ValidationFilterAttributeTest_Valid_ResultIsNull()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                filters: new List<IFilterMetadata>(), // for majority of scenarios you need not worry about populating this parameter
                actionArguments: new Dictionary<string, object>(), // if the filter uses this data, add some data to this dictionary
                controller: null); // since the filter being tested here does not use the data from this parameter, just provide null
            var validationFilter = new ValidateModelStateAttribute();

            // Act
            // Add an error into model state on purpose to make it invalid
            validationFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Null(actionExecutingContext.Result);
        }
    }
}