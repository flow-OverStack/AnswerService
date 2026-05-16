using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerService.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LastModifiedAtIsNotNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                                 UPDATE public."Answer"
                                 SET "LastModifiedAt" = "CreatedAt"
                                 WHERE "LastModifiedAt" IS NULL;
                                 """);
            
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Answer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Answer",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
