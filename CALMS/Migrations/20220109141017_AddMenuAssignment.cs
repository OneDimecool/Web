using Microsoft.EntityFrameworkCore.Migrations;

namespace CALMS.Migrations
{
    public partial class AddMenuAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    PageName = table.Column<string>(nullable: true),
                    PageDesc = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    MenuHref = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuRoles",
                columns: table => new
                {
                    ApplicationMenuId = table.Column<string>(nullable: false),
                    IdentityRoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuRoles", x => new { x.ApplicationMenuId, x.IdentityRoleId });
                    table.ForeignKey(
                        name: "FK_MenuRoles_Menus_ApplicationMenuId",
                        column: x => x.ApplicationMenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuRoles_AspNetRoles_IdentityRoleId",
                        column: x => x.IdentityRoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuRoles_IdentityRoleId",
                table: "MenuRoles",
                column: "IdentityRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuRoles");

            migrationBuilder.DropTable(
                name: "Menus");
        }
    }
}
