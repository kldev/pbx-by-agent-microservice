using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace FinCosts.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initialmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "cost_document_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    name_pl = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    name_en = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    category = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cost_document_type", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "currency_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    name_pl = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    name_en = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    category = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency_type", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "document_entry",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    currency_name_pl = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    currency_foreign_rate = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    was_paid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    kse_ref_number = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true),
                    kse_account_number = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: true),
                    document_type_name_pl = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    document_type_id = table.Column<int>(type: "int", nullable: false),
                    issued_by = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    issued_vat_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    issued_for = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    issued_for_vat_number = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    vat_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_document_entry", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vat_rate_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vat_rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    name_pl = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    name_en = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vat_rate_type", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cost_document_type_name_en",
                table: "cost_document_type",
                column: "name_en",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cost_document_type_name_pl",
                table: "cost_document_type",
                column: "name_pl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_currency_type_name_en",
                table: "currency_type",
                column: "name_en",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_currency_type_name_pl",
                table: "currency_type",
                column: "name_pl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_currency_name_pl",
                table: "document_entry",
                column: "currency_name_pl");

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_document_type_id",
                table: "document_entry",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_gid",
                table: "document_entry",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_is_deleted",
                table: "document_entry",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_issued_for_vat_number",
                table: "document_entry",
                column: "issued_for_vat_number");

            migrationBuilder.CreateIndex(
                name: "IX_document_entry_issued_vat_number",
                table: "document_entry",
                column: "issued_vat_number");

            migrationBuilder.CreateIndex(
                name: "IX_vat_rate_type_name_en",
                table: "vat_rate_type",
                column: "name_en",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vat_rate_type_name_pl",
                table: "vat_rate_type",
                column: "name_pl",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vat_rate_type_vat_rate",
                table: "vat_rate_type",
                column: "vat_rate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cost_document_type");

            migrationBuilder.DropTable(
                name: "currency_type");

            migrationBuilder.DropTable(
                name: "document_entry");

            migrationBuilder.DropTable(
                name: "vat_rate_type");
        }
    }
}
