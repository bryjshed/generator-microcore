using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace <%= namespace %>.Migrations
{
    /// <summary>
    ///
    /// </summary>
    [DbContext(typeof(<%= appname %>Context))]
    [Migration("<%= migrationDate %>_InitialMigration")]
    partial class InitialMigration
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
