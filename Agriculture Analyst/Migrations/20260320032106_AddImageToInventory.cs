using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agriculture_Analyst.Migrations
{
    /// <inheritdoc />
    public partial class AddImageToInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CHỈ GIỮ ĐÚNG DÒNG NÀY: Thêm cột ImageUrl vào InventoryTransaction
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "InventoryTransaction", // Hoặc InventoryTransactions tùy cách bạn đặt tên bảng
                type: "nvarchar(max)",
                nullable: true);

            // XÓA HẾT MỌI THỨ DƯỚI NÀY (Đặc biệt là đoạn CreateTable("Posts",...))
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // CHỈ GIỮ DÒNG NÀY
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "InventoryTransaction");

            // XÓA HẾT CÁC LỆNH KHÁC
        }
    }
}
