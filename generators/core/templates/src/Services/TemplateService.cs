using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.CodeAnalysis.Semantics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= appname %>Service.
    /// </summary>
    public class <%= appname %>Service : I<%= appname %>Service
    {
        private readonly ILogger<<%= appname %>Service> _logger;

        private readonly IMapper _mapper;
        <%_ if(createModel) { _%>

        private readonly I<%= modelName %>Repository _<%= modelNameCamel %>Repository;
        <%_ } _%>
        <%_ if(kafka) { _%>
        
        private readonly IKafkaProducer _producer;
        <%_ } _%>

        /// <summary>
        /// Initializes a new instance of the <see cref="<%= appname %>Service"/> class.
        /// </summary>
        <%_ if(createModel) { _%>
        /// <param name="<%= modelNameCamel %>Repository">The <see cref="<%= appname %>"/> database repository.</param>
        <%_ } _%>
        /// <param name="logger">The log handler for the controller.</param>
        <%_ if(kafka) { _%>
        /// <param name="producer">The kafka producer.</param>
        <%_ } _%> 
        /// <param name="mapper">Automapper.</param>
        public <%= appname %>Service(
            <%_ if(createModel) { _%>
            I<%= modelName %>Repository <%= modelNameCamel %>Repository,
            <%_ } _%>
            ILogger<<%= appname %>Service> logger,
            <%_ if(kafka) { _%>
            IKafkaProducer producer,
            <%_ } _%>
            IMapper mapper
            )

        {
            <%_ if(createModel) { _%>
            _<%= modelNameCamel %>Repository = <%= modelNameCamel %>Repository;
            <%_ } _%>
            _logger = logger;
            _mapper = mapper;
            <%_ if(kafka) { _%>
            _producer = producer;
            <%_ } _%>
        }

        <%_ if(createModel) { _%>
        /// <summary>
        /// Save a new <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="dto">The <%= modelName.toLowerCase() %> dto to save.</param>
        /// <param name="user">The application user.</param>
        public async Task<<%= modelName %>Dto> Save<%= modelName %>Async(Create<%= modelName %>Dto dto, ApplicationUser user)
        {
            var model = _mapper.Map<<%= modelName %>>(dto);

            // Set user information.
            model.CreatedById = user.Email;
            model.CreatedByDisplayName = user.DisplayName;
            model.UpdatedById = user.Email;
            model.UpdatedByDisplayName = user.DisplayName;

            <%_ if(database === 'dynamodb') { _%>
            await _<%= modelNameCamel %>Repository.SaveAsync(model);

            var resultDto = _mapper.Map<<%= modelName %>Dto>(model);
            <%_ if(kafka) { _%>
            _logger.LogInformation("Sending kafka event for <%= modelName.toLowerCase() %> <%= modelName.toLowerCase() %>_created");
            _producer.Publish(new KafkaEvent<<%= modelName %>Dto>("<%= modelName.toLowerCase() %>_created", user.Email, resultDto));
            <%_ } _%>
            <%_ } else { _%>
            var result = await _<%= modelNameCamel %>Repository.SaveAsync(model);

            var resultDto = _mapper.Map<<%= modelName %>Dto>(model);
            <%_ if(kafka) { _%>
            if (result > 0)
            {
                _logger.LogInformation("Sending kafka event for <%= modelName.toLowerCase() %> <%= modelName.toLowerCase() %>_created");
                _producer.Publish(new KafkaEvent<<%= modelName %>Dto>("<%= modelName.toLowerCase() %>_created", user.Email, resultDto));
            }
            <%_ } _%>
            <%_ } _%>
            return resultDto;
        }

        /// <summary>
        /// Update the <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The <%= modelName.toLowerCase() %> id.</param>
        /// <param name="dto">The <%= modelName.toLowerCase() %> dto to update.</param>
        /// <param name="user">The application user.</param>
        public async Task<<%= modelName %>Dto> Update<%= modelName %>Async(<%= idType %> id, Update<%= modelName %>Dto dto, ApplicationUser user)
        {
            var model = await _<%= modelNameCamel %>Repository.GetByIdAsync(id);
            if (model == null)
            {
                _logger.LogWarning("<%= modelName %> was not found.");
                throw new NotFoundException();
            }

            _mapper.Map<Update<%= modelName %>Dto, <%= modelName %>>(dto, model);

            // Setup update fields
            model.LastUpdatedDate = DateTime.UtcNow;
            model.UpdatedById = user.Email;
            model.UpdatedByDisplayName = user.DisplayName;
            <%_ if(database === 'dynamodb') { _%>
            await _<%= modelNameCamel %>Repository.UpdateAsync(model);

            var resultDto = _mapper.Map<<%= modelName %>Dto>(model);
            <%_ if(kafka) { _%>
            _logger.LogInformation("Sending kafka event for <%= modelName.toLowerCase() %> <%= modelName.toLowerCase() %>_updated");
            _producer.Publish(new KafkaEvent<<%= modelName %>Dto>("<%= modelName.toLowerCase() %>_updated", user.Email, resultDto));
            <%_ } _%>
            <%_ } else { _%>
            var result = await _<%= modelNameCamel %>Repository.UpdateAsync(model);

            var resultDto = _mapper.Map<<%= modelName %>Dto>(model);
            <%_ if(kafka) { _%>
            if (result > 0)
            {
                _logger.LogInformation("Sending kafka event for <%= modelName.toLowerCase() %> <%= modelName.toLowerCase() %>_updated");
                _producer.Publish(new KafkaEvent<<%= modelName %>Dto>("<%= modelName.toLowerCase() %>_updated", user.Email, resultDto));
            }
            <%_ } _%>
            <%_ } _%>
            return resultDto;
        }

        /// <summary>
        /// Get a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The <%= modelName.toLowerCase() %> id.</param>
        /// <param name="user">The application user.</param>
        public async Task<<%= modelName %>Dto> Get<%= modelName %>Async(<%= idType %> id, ApplicationUser user)
        {
            var model = await _<%= modelNameCamel %>Repository.GetByIdAsync(id);
            if (model == null)
            {
                return null;
            }
            var dto = _mapper.Map<<%= modelName %>Dto>(model);
            return dto;
        }
        <%_ if(database === 'dynamodb') { _%>

        /// <summary>
        /// Search <%= modelName.toLowerCase() %>s.
        /// </summary>
        /// <param name="dto">The search dto.</param>
        /// <param name="user">The application user.</param>
        public async Task<List<<%= modelName %>Dto>> Find<%= modelName %>Async(Search<%= modelName %>Dto dto, ApplicationUser user)
        {
            var results = await _<%= modelNameCamel %>Repository.FindAsync(null, dto.StartKey, dto.PageSize);
            return _mapper.Map<List<<%= modelName %>Dto>>(results.ToList());
        }
        <%_ } else { _%>

        /// <summary>
        /// Search <%= modelName.toLowerCase() %>s.
        /// </summary>
        /// <param name="dto">The search dto.</param>
        /// <param name="user">The application user.</param>
        public async Task<PaginatedList<<%= modelName %>Dto>> Find<%= modelName %>Async(Search<%= modelName %>Dto dto, ApplicationUser user)
        {
            var results = await _<%= modelNameCamel %>Repository.FindAsync(
                    dto.Query,
                    dto.PageNumber,
                    dto.PageSize);
            return _mapper.Map<PaginatedList<<%= modelName %>Dto>>(results);
        }
        <%_ } _%>
        <%_ } _%>
    }
}