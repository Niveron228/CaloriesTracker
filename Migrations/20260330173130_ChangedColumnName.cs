using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaloriesTracker.Migrations
{
    /// <inheritdoc />
    public partial class ChangedColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "calorie",
                table: "Foods",
                newName: "calories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "calories",
                table: "Foods",
                newName: "calorie");
        }
    }
}
