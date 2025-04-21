using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoworkingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoworkingSpaceImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "coworking_space_images",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coworking_space_id = table.Column<int>(type: "int", nullable: false),
                    file_path = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coworking_space_images", x => x.id);
                    table.ForeignKey(
                        name: "FK_coworking_space_images_coworking_spaces_coworking_space_id",
                        column: x => x.coworking_space_id,
                        principalTable: "coworking_spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_coworking_space_images_coworking_space_id",
                table: "coworking_space_images",
                column: "coworking_space_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "coworking_space_images");

       
        }
    }
}
