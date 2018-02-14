using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace <%= namespace %>.Migrations
{
    public partial class InitialMigration : Migration
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "<%= modelName.toLowerCase() %>",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGeneratedOnAdd", true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedByDisplayName = table.Column<string>(maxLength: 100, nullable: false),
                    CreatedById = table.Column<string>(maxLength: 100, nullable: false),
                    UpdatedByDisplayName = table.Column<string>(maxLength: 100, nullable: true),
                    UpdatedById = table.Column<string>(maxLength: 100, nullable: true),
                    LastUpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_<%= modelName.toLowerCase() %>", x => x.Id);
                });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "<%= modelName.toLowerCase() %>");
        }
    }
}
