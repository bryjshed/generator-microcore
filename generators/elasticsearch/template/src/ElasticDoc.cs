namespace <%= namespace %>
{
    /// <summary>
    /// Represents an ElasticSearch document.
    /// </summary>
    public class ElasticDoc
    {
        /// <summary>
        /// The unique identifier of the document.
        /// </summary>
        /// <returns>A string.</returns>
        public string Id { get; set; }
    }
}