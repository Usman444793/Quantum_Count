using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantum_Count.Migrations
{
    /// <inheritdoc />
    public partial class MaterialCodeUniqueIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialCode",
                table: "Materials",
                column: "MaterialCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Materials_MaterialCode",
                table: "Materials");
        }
    }
}
