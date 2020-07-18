using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AddCinemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Poster",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Movies",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Actors",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cinemas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cinemas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MoviesCinemas",
                columns: table => new
                {
                    MovieId = table.Column<int>(nullable: false),
                    CinemaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesCinemas", x => new { x.CinemaId, x.MovieId });
                    table.ForeignKey(
                        name: "FK_MoviesCinemas_Cinemas_CinemaId",
                        column: x => x.CinemaId,
                        principalTable: "Cinemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesCinemas_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MoviesCinemas_MovieId",
                table: "MoviesCinemas",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MoviesCinemas");

            migrationBuilder.DropTable(
                name: "Cinemas");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Actors");

            migrationBuilder.AddColumn<string>(
                name: "Poster",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Actors",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
