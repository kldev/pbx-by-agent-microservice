using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace RateService.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "destination_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name_pl = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name_en = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_destination_groups", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "tariffs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    currency_code = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    is_default = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    valid_from = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    valid_to = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    billing_increment = table.Column<int>(type: "int", nullable: false),
                    minimum_duration = table.Column<int>(type: "int", nullable: false),
                    connection_fee = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tariffs", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    prefix = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    destination_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    rate_per_minute = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    connection_fee = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: true),
                    billing_increment = table.Column<int>(type: "int", nullable: true),
                    minimum_duration = table.Column<int>(type: "int", nullable: true),
                    effective_from = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    effective_to = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    tariff_id = table.Column<long>(type: "bigint", nullable: false),
                    destination_group_id = table.Column<int>(type: "int", nullable: true),
                    gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rates", x => x.id);
                    table.ForeignKey(
                        name: "FK_rates_destination_groups_destination_group_id",
                        column: x => x.destination_group_id,
                        principalTable: "destination_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rates_tariffs_tariff_id",
                        column: x => x.tariff_id,
                        principalTable: "tariffs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_destination_groups_name",
                table: "destination_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rates_destination_group_id",
                table: "rates",
                column: "destination_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_rates_gid",
                table: "rates",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rates_prefix",
                table: "rates",
                column: "prefix");

            migrationBuilder.CreateIndex(
                name: "IX_rates_tariff_id_prefix",
                table: "rates",
                columns: new[] { "tariff_id", "prefix" });

            migrationBuilder.CreateIndex(
                name: "IX_tariffs_gid",
                table: "tariffs",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tariffs_is_default",
                table: "tariffs",
                column: "is_default");

            migrationBuilder.CreateIndex(
                name: "IX_tariffs_name",
                table: "tariffs",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rates");

            migrationBuilder.DropTable(
                name: "destination_groups");

            migrationBuilder.DropTable(
                name: "tariffs");
        }
    }
}
