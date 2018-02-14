using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace <%= namespace %>.Migrations
{
    [DbContext(typeof(<%= appname %>Context))]
    partial class <%= modelName %>ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("<%= namespace %>.<%= modelName %>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("CreatedByDisplayName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<DateTime>("LastUpdatedDate");

                    b.Property<string>("UpdatedByDisplayName")
                        .HasMaxLength(100);

                    b.Property<string>("UpdatedById")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("<%= modelName.toLowerCase() %>");
                });
        }
    }
}
