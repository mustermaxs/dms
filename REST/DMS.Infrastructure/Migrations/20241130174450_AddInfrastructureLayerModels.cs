using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInfrastructureLayerModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTags");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.CreateTable(
                name: "DocumentModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    UploadDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    StatusModel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TagModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTagModels",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTagModels", x => new { x.DocumentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_DocumentTagModels_DocumentModels_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "DocumentModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentTagModels_TagModels_TagId",
                        column: x => x.TagId,
                        principalTable: "TagModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTagModels_TagId",
                table: "DocumentTagModels",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagModels_Label",
                table: "TagModels",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TagModels_Value",
                table: "TagModels",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentTagModels");

            migrationBuilder.DropTable(
                name: "DocumentModels");

            migrationBuilder.DropTable(
                name: "TagModels");

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    ModificationDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Path = table.Column<string>(type: "text", nullable: true),
                    StatusModel = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UploadDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Color = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Label = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Value = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTags",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTags", x => new { x.DocumentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_DocumentTags_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTags_TagId",
                table: "DocumentTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Label",
                table: "Tags",
                column: "Label",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Value",
                table: "Tags",
                column: "Value",
                unique: true);
        }
    }
}
