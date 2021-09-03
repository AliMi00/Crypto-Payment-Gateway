using Microsoft.EntityFrameworkCore.Migrations;

namespace Crypto_Payment_Gateway.Data.Migrations
{
    public partial class x2AddColToWalletTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAddedToTransactions",
                table: "WalletTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAddedToTransactions",
                table: "WalletTransactions");
        }
    }
}
