using System.Collections.Generic;
using AutoMapper;
<%_ if(authentication) { _%>
using Microsoft.AspNetCore.Authorization;
<%_ } _%>
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace <%= namespace %>
{
    /// <summary>
    ///  Controlls <%= controllerName.toLowerCase() %> functionality.
    /// </summary>
    [EnableCors("AllowAny")]
    [Route("[controller]")]
    [Produces("application/json")]
    public class <%= controllerName %>Controller : Controller
    {

        private readonly IMapper _mapper;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="<%= controllerName %>Controller"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="mapper">The mapper for the objects.</param>
        public <%= controllerName %>Controller(
            ILogger<<%= controllerName %>Controller> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public string Get(int id)
        {
            return "value";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "Create")]
        <%_ } _%>
        public void Post([FromBody]string value)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [HttpPut("{id}")]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public void Put(int id, [FromBody]string value)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "Delete")]
        <%_ } _%>
        public void Delete(int id)
        {
        }
    }
}
