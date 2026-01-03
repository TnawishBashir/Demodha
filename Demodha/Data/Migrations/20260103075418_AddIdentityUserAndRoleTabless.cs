using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demodha.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityUserAndRoleTabless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File_Number",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User_CNIC",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_User_CNIC_File_Number",
                table: "AspNetUsers",
                columns: new[] { "User_CNIC", "File_Number" },
                unique: true,
                filter: "[User_CNIC] IS NOT NULL AND [File_Number] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_User_CNIC_File_Number",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "File_Number",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "User_CNIC",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserType",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
