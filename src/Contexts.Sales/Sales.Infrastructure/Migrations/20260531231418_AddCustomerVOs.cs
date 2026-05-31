using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerVOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_tickets_OrderItemId",
                table: "tickets",
                column: "OrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_tickets_order_items_OrderItemId",
                table: "tickets",
                column: "OrderItemId",
                principalTable: "order_items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tickets_order_items_OrderItemId",
                table: "tickets");

            migrationBuilder.DropIndex(
                name: "IX_tickets_OrderItemId",
                table: "tickets");
        }
    }
}
