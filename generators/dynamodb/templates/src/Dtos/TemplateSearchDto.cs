using System.ComponentModel.DataAnnotations;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= modelName.toLowerCase() %> search information.
    /// </summary>
    public class Search<%= modelName %>Dto
    {
        /// <summary>
        /// The start key to use in the search.
        /// </summary>
        [RegularExpression(@"^[{(]?[0-9a-f]{8}[-]?([0-9a-f]{4}[-]?){3}[0-9a-f]{12}[)}]?$", ErrorMessage = "The {0} field is not valid.")]
        public string StartKey { get; set; }
        /// <summary>
        /// The max number of results to return.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "{0} must be greater than 0.")]
        public int PageSize { get; set; } = 10;
    }
}
