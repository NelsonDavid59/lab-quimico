using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroLaboratorio.Migrations.Auth
{
    /// <inheritdoc />
    public partial class RemoveClave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "clave",
                table: "usuario");

            migrationBuilder.DropColumn(
                name: "cod_perfil",
                table: "usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clave",
                table: "usuario",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "cod_perfil",
                table: "usuario",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
