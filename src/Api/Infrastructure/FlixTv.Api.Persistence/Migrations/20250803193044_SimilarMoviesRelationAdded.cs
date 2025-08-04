using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlixTv.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimilarMoviesRelationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieSimilarities",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    SimilarMoviesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieSimilarities", x => new { x.MovieId, x.SimilarMoviesId });
                    table.ForeignKey(
                        name: "FK_MovieSimilarities_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieSimilarities_Movies_SimilarMoviesId",
                        column: x => x.SimilarMoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieSimilarities_SimilarMoviesId",
                table: "MovieSimilarities",
                column: "SimilarMoviesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieSimilarities");
        }
    }
}
