using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class email_is_optional_in_external_user_profile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "hrim_analytics",
                table: "external_user_profiles",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "hrim_analytics",
                table: "external_user_profiles",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
