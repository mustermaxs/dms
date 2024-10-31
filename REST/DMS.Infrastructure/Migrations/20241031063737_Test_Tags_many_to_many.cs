using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Test_Tags_many_to_many : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTags_Tags_TagId",
                table: "DocumentTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
