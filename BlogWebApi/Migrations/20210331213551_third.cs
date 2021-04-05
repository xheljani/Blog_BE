using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogWebApi.Migrations
{
    public partial class third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryPostModel",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPostModel", x => new { x.CategoryId, x.PostId });
                    table.ForeignKey(
                        name: "FK_CategoryPostModel_CategoryModel_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CategoryModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryPostModel_PostModel_PostId",
                        column: x => x.PostId,
                        principalTable: "PostModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPostModel_PostId",
                table: "CategoryPostModel",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryPostModel");
        }
    }
}
