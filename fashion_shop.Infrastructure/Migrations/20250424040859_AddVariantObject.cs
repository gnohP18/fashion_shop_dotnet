using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fashion_shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVariantObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_details_product_item_product_item_id",
                table: "order_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_setting",
                table: "setting");

            migrationBuilder.RenameTable(
                name: "setting",
                newName: "settings");

            migrationBuilder.AddColumn<string>(
                name: "variant_objects",
                table: "product_items",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "pk_settings",
                table: "settings",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_details_product_items_product_item_id",
                table: "order_details",
                column: "product_item_id",
                principalTable: "product_items",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_order_details_product_items_product_item_id",
                table: "order_details");

            migrationBuilder.DropPrimaryKey(
                name: "pk_settings",
                table: "settings");

            migrationBuilder.DropColumn(
                name: "variant_objects",
                table: "product_items");

            migrationBuilder.RenameTable(
                name: "settings",
                newName: "setting");

            migrationBuilder.AddPrimaryKey(
                name: "pk_setting",
                table: "setting",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_order_details_product_item_product_item_id",
                table: "order_details",
                column: "product_item_id",
                principalTable: "product_items",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
