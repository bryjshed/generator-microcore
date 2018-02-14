namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <%= serviceDtoName.toLowerCase() %> information.
    /// </summary>
    public class <%= serviceDtoName %>Dto
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public long? Id { get; set; }
    }
}
