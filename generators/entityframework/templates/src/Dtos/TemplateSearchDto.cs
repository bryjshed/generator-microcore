using System.ComponentModel.DataAnnotations;

namespace <%= namespace %>
{
    /// <summary>
    /// 
    /// </summary>
    public class Search<%= modelName %>Dto
    {
        /// <summary>
        /// The <%= modelName.toLowerCase() %> query.
        /// </summary>
        public string Query { set; get; }

        /// <summary>
        /// The number of the page to return.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be greater than or equal to {1}.")]
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The size of the page to return.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be greater than or equal to {1}.")]
        public int PageSize { get; set; } = 10;
    }
}