using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlixTv.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedViewData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ViewDatas_UserId",
                table: "ViewDatas");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ViewDatas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastPositionSeconds",
                table: "ViewDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastWatchedAt",
                table: "ViewDatas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxPositionSeconds",
                table: "ViewDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WatchedSeconds",
                table: "ViewDatas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ViewDatas_UserId_MovieId",
                table: "ViewDatas",
                columns: new[] { "UserId", "MovieId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ViewDatas_UserId_MovieId",
                table: "ViewDatas");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ViewDatas");

            migrationBuilder.DropColumn(
                name: "LastPositionSeconds",
                table: "ViewDatas");

            migrationBuilder.DropColumn(
                name: "LastWatchedAt",
                table: "ViewDatas");

            migrationBuilder.DropColumn(
                name: "MaxPositionSeconds",
                table: "ViewDatas");

            migrationBuilder.DropColumn(
                name: "WatchedSeconds",
                table: "ViewDatas");

            migrationBuilder.CreateIndex(
                name: "IX_ViewDatas_UserId",
                table: "ViewDatas",
                column: "UserId");
        }
    }
}
