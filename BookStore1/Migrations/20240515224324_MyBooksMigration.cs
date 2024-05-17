using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore1.Migrations
{
    /// <inheritdoc />
    public partial class MyBooksMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MyBooksId",
                table: "Book",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MyBooks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyBooks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Book_MyBooksId",
                table: "Book",
                column: "MyBooksId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_MyBooks_MyBooksId",
                table: "Book",
                column: "MyBooksId",
                principalTable: "MyBooks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_MyBooks_MyBooksId",
                table: "Book");

            migrationBuilder.DropTable(
                name: "MyBooks");

            migrationBuilder.DropIndex(
                name: "IX_Book_MyBooksId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "MyBooksId",
                table: "Book");
        }
    }
}
