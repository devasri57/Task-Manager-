using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToToDo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ToDos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ToDos");
        }
    }
}
