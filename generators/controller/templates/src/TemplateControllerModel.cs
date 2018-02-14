using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
<%_ if(authentication) { _%>
using Microsoft.AspNetCore.Authorization;
<%_ } _%>
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
<%_ if(database === 'dynamodb') { _%>
using System;
using System.Linq;
using ThirdParty.Json.LitJson;
<%_ } _%>

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

        private readonly I<%= modelName %>Repository _<%= modelNameCamel %>Repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="<%= controllerName %>Controller"/> class.
        /// </summary>
        /// <param name="logger">The log handler for the controller.</param>
        /// <param name="mapper">The mapper for the <see cref="<%= modelName %>"/> and <see cref="<%= modelName %>Dto"/> objects.</param>
        /// <param name="<%= modelNameCamel %>Repository">The <see cref="<%= modelName %>"/> database repository.</param>
        public <%= controllerName %>Controller(
            ILogger<<%= controllerName %>Controller> logger,
            IMapper mapper,
            I<%= modelName %>Repository <%= modelNameCamel %>Repository)
        {
            _logger = logger;
            _mapper = mapper;
            _<%= modelNameCamel %>Repository = <%= modelNameCamel %>Repository;
        }

        /// <summary>
        /// Creates a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="dto"><%= modelName %> to save.</param>
        /// <response code="201">Created</response>
        /// <response code="400">Bad Request</response>
        /// <returns><%= modelName %>Dto</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(<%= modelName %>Dto), 201)]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "Create")]
        <%_ } _%>
        public async Task<IActionResult> Create<%= modelName %>([FromBody] Create<%= modelName %>Dto dto)
        {
            var model = _mapper.Map<<%= modelName %>>(dto);
            await _<%= modelNameCamel %>Repository.SaveAsync(model);
            var retDto = _mapper.Map<<%= modelName %>Dto>(model);

            return CreatedAtRoute("Get<%= modelName %>", new { controller = "<%= modelName %>", id = retDto.Id }, retDto);
        }


        /// <summary>
        /// Update a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">Id of <%= modelName.toLowerCase() %> to update.</param>
        /// <param name="dto"><%= modelName %> to update.</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="404">Not Found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(typeof(void), 404)]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "Update")]
        <%_ } _%>
        public async Task<IActionResult> Update<%= modelName %>(<%= idType %> id, [FromBody] Update<%= modelName %>Dto dto)
        {
            var model = await _<%= modelNameCamel %>Repository.GetByIdAsync(id);
            if (model == null)
            {
                _logger.LogWarning("Update <%= modelName %> Not Found: {Id}", id);
                return NotFound();
            }

            _mapper.Map<Update<%= modelName %>Dto, <%= modelName %>>(dto, model);

            await _<%= modelNameCamel %>Repository.UpdateAsync(model);

            return Ok();
        }

        /// <summary>
        /// Get a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">id of <%= modelName.toLowerCase() %>.</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <returns><%= modelName %>Dto</returns>
        [HttpGet("{id}", Name = "Get<%= modelName %>")]
        [ProducesResponseType(typeof(<%= modelName %>Dto), 200)]
        [ProducesResponseType(typeof(void), 404)]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public async Task<IActionResult> Get<%= modelName %>(<%= idType %> id)
        {
            var model = await _<%= modelNameCamel %>Repository.GetByIdAsync(id);
            if (model == null)
            {
              _logger.LogWarning("<%= modelName %> Not Found: {Id}", id);
              return NotFound();
            }

            return Ok(_mapper.Map<<%= modelName %>Dto>(model));
        }

        <%_ if(database === 'dynamodb') { _%>
        /// <summary>
        /// Get <%= modelName.toLowerCase() %>s.
        /// </summary>
        /// <param name="dto">The search dto for <%= modelName.toLowerCase() %>.</param>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        [HttpGet(Name = "Get<%= modelName %>s")]
        [ProducesResponseType(typeof(IList<<%= modelName %>Dto>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public async Task<IActionResult> Get<%= modelName %>s([FromQuery] Search<%= modelName %>Dto dto)
        {
            _logger.LogInformation("Search Requisition: {@search}");
            
            IQueryable<<%= modelName %>> queryable = null;
            try
            {
                queryable = await _<%= modelNameCamel %>Repository.FindAsync(null, dto.StartKey, dto.PageSize);
                return Ok(_mapper.Map<List<<%= modelName %>Dto>>(queryable.ToList()));
            }
            catch (Exception e)
            {
                // The Exception returned if the given startKey is invalid.
                if (e.GetType() == typeof(JsonException))
                {
                    _logger.LogWarning("Invaid StartKey");
                    ModelState.AddModelError("startKey", "Invalid startKey.");
                    return BadRequest(ModelState);
                }
                _logger.LogWarning("Unhandled <%= modelName %> Search Exception", e);
                throw e;
            }
        }
        <%_ } else { _%>
        /// <summary>
        /// Get <%= modelName.toLowerCase() %>s.
        /// </summary>
        /// <response code="200">OK</response>
        /// <returns>A list of <%= modelName %>Dtos.</returns>
        [HttpGet(Name = "Get<%= modelName %>s")]
        [ProducesResponseType(typeof(List<<%= modelName %>Dto>), 200)]
        <%_ if(authentication) { _%>
        [Authorize(Policy = "View")]
        <%_ } _%>
        public async Task<IActionResult> Get<%= modelName %>s()
        {
            var models = await _<%= modelNameCamel %>Repository.ListAsync();
            return Ok(_mapper.Map<List<<%= modelName %>Dto>>(models));
        }
        <%_ } _%>
    }
}
