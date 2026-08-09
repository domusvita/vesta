using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vesta.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // IF EXISTS guards against re-running after a partial prior execution
            migrationBuilder.Sql("DROP INDEX IF EXISTS ix_user_roles_user_id_name;");

            migrationBuilder.DropColumn(
                name: "name",
                table: "user_roles");

            migrationBuilder.AddColumn<Guid>(
                name: "role_id",
                table: "user_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            // existing rows have role_id = 00000000... which can't satisfy the FK
            migrationBuilder.Sql("DELETE FROM user_roles;");

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "user_roles");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "user_roles",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_name",
                table: "user_roles",
                columns: new[] { "user_id", "name" },
                unique: true);
        }
    }
}
