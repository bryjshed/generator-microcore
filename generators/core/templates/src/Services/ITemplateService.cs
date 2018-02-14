using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= appname %>Service interface.
    /// </summary>
    public interface I<%= appname %>Service
    {
        <%_ if(createModel) { _%>
        /// <summary>
        /// Save a new <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="dto">The <%= modelName.toLowerCase() %> dto to save.</param>
        /// <param name="user">The application user.</param>
        Task<<%= modelName %>Dto> Save<%= modelName %>Async(Create<%= modelName %>Dto dto, ApplicationUser user);

        /// <summary>
        /// Update the <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The <%= modelName.toLowerCase() %> id.</param>
        /// <param name="dto">The <%= modelName.toLowerCase() %> dto to update.</param>
        /// <param name="user">The application user.</param>
        Task<<%= modelName %>Dto> Update<%= modelName %>Async(<%= idType %> id, Update<%= modelName %>Dto dto, ApplicationUser user);

        /// <summary>
        /// Get a <%= modelName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The <%= modelName.toLowerCase() %> id.</param>
        /// <param name="user">The application user.</param>
        Task<<%= modelName %>Dto> Get<%= modelName %>Async(<%= idType %> id, ApplicationUser user);

        /// <summary>
        /// Search <%= modelName.toLowerCase() %>s.
        /// </summary>
        /// <param name="dto">The search dto.</param>
        /// <param name="user">The application user.</param>
        <%_ if(database === 'dynamodb') { _%>
        Task<List<<%= modelName %>Dto>> Find<%= modelName %>Async(Search<%= modelName %>Dto dto, ApplicationUser user);
        <%_ } else { _%>
        Task<PaginatedList<<%= modelName %>Dto>> Find<%= modelName %>Async(Search<%= modelName %>Dto dto, ApplicationUser user);
        <%_ } _%>
        <%_ } _%>
    }
}