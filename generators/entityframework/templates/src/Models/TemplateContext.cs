using Microsoft.EntityFrameworkCore;

namespace <%= namespace %>
{
    /// <summary>
    /// Represents the <see cref="<%= appname %>Context"/> database context.
    /// </summary>
    public class <%= appname %>Context : DbContext
    {
        <%_ if (createModel) { _%>
        /// <summary>
        /// Gets or sets the <%= modelName.toLowerCase() %> dbset.
        /// </summary>
        public DbSet<<%= modelName %>> <%= modelName %>s { get; set; }
        <%_ } _%>

        /// <summary>
        /// Initializes a new <see cref="<%= appname %>Context"/>.
        /// </summary>
        /// <param name="options"></param>
        public <%= appname %>Context(DbContextOptions<<%= appname %>Context> options) : base(options)
        {

        }

        /// <summary>
        /// On creation of a model, configure the entity.
        /// </summary>
        /// <param name="modelBuilder">The model configuration object.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
