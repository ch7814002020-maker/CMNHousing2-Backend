using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMNHousing.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRelationshipToEnquiries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Properties_PropertyId",
                table: "Enquiries");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_UserId",
                table: "Enquiries",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Properties_PropertyId",
                table: "Enquiries",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Users_UserId",
                table: "Enquiries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Properties_PropertyId",
                table: "Enquiries");

            migrationBuilder.DropForeignKey(
                name: "FK_Enquiries_Users_UserId",
                table: "Enquiries");

            migrationBuilder.DropIndex(
                name: "IX_Enquiries_UserId",
                table: "Enquiries");

            migrationBuilder.AddForeignKey(
                name: "FK_Enquiries_Properties_PropertyId",
                table: "Enquiries",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id");
        }
    }
}
