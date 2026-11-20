using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prova2.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicialProva2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Preco = table.Column<float>(type: "REAL", nullable: false),
                    Status = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdUsuarioCadastro = table.Column<int>(type: "INTEGER", nullable: false),
                    IdUsuarioUpdate = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Usuarios_IdUsuarioCadastro",
                        column: x => x.IdUsuarioCadastro,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Produtos_Usuarios_IdUsuarioUpdate",
                        column: x => x.IdUsuarioUpdate,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IdUsuarioCadastro",
                table: "Produtos",
                column: "IdUsuarioCadastro");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_IdUsuarioUpdate",
                table: "Produtos",
                column: "IdUsuarioUpdate");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Nome",
                table: "Usuarios",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
