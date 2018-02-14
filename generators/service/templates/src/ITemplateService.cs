using System.Threading.Tasks;

namespace <%= namespace %>
{
    /// <summary>
    /// Interface for classes providing <%= serviceName %>Service functionality.
    /// </summary>
    public interface I<%= serviceName %>Service
    {
        /// <summary>
        /// Gets a <%= serviceName.toLowerCase() %> with the given id.
        /// </summary>
        /// <param name="id">The <%= serviceName.toLowerCase() %> id to search by.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        Task<ServiceResponse<<%= serviceDtoName %>Dto>> Get(long id, string idToken);

        /// <summary>
        /// Save a <%= serviceName.toLowerCase() %>.
        /// </summary>
        /// <param name="dto">The <%= serviceName.toLowerCase() %> to save.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        Task<ServiceResponse<<%= serviceDtoName %>Dto>> Save(<%= serviceDtoName %>Dto dto, string idToken);

        /// <summary>
        /// Update a <%= serviceName.toLowerCase() %>.
        /// </summary>
        /// <param name="id">The id to update.</param>
        /// <param name="dto">The <%= serviceName.toLowerCase() %> to update.</param>
        /// <param name="idToken">The bearer authentication token.</param>
        Task<ServiceResponse<<%= serviceDtoName %>Dto>> Update(long id, <%= serviceDtoName %>Dto dto, string idToken);
    }
}
