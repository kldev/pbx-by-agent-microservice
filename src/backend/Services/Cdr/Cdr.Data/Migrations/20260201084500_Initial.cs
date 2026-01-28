using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace CdrService.Data.Migrations
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
                name: "call_directions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name_pl = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_call_directions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "call_statuses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    name_pl = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_call_statuses", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "termination_causes",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    q850_code = table.Column<int>(type: "int", nullable: true),
                    name_pl = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    name_en = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_termination_causes", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "call_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    call_uuid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true),
                    caller_id = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    called_number = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    answer_time = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    duration = table.Column<int>(type: "int", nullable: false),
                    billable_seconds = table.Column<int>(type: "int", nullable: false),
                    call_status_id = table.Column<long>(type: "bigint", nullable: false),
                    termination_cause_id = table.Column<long>(type: "bigint", nullable: false),
                    call_direction_id = table.Column<long>(type: "bigint", nullable: false),
                    source_gateway_gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    source_gateway_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    destination_gateway_gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    destination_gateway_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    tariff_gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    tariff_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    rate_per_minute = table.Column<decimal>(type: "decimal(10,6)", precision: 10, scale: 6, nullable: false),
                    connection_fee = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    billing_increment = table.Column<int>(type: "int", nullable: false),
                    currency_code = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    destination_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    matched_prefix = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true),
                    total_cost = table.Column<decimal>(type: "decimal(12,4)", precision: 12, scale: 4, nullable: false),
                    customer_gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    customer_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    sip_account_gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    sip_account_username = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    user_data = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    raw_cdr_json = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_call_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_call_records_call_directions_call_direction_id",
                        column: x => x.call_direction_id,
                        principalTable: "call_directions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_call_records_call_statuses_call_status_id",
                        column: x => x.call_status_id,
                        principalTable: "call_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_call_records_termination_causes_termination_cause_id",
                        column: x => x.termination_cause_id,
                        principalTable: "termination_causes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_call_directions_code",
                table: "call_directions",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_call_directions_gid",
                table: "call_directions",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_call_records_call_direction_id",
                table: "call_records",
                column: "call_direction_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_call_status_id",
                table: "call_records",
                column: "call_status_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_call_uuid",
                table: "call_records",
                column: "call_uuid");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_called_number",
                table: "call_records",
                column: "called_number");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_caller_id",
                table: "call_records",
                column: "caller_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_customer_gid",
                table: "call_records",
                column: "customer_gid");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_customer_gid_start_time",
                table: "call_records",
                columns: new[] { "customer_gid", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_call_records_gid",
                table: "call_records",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_call_records_sip_account_gid",
                table: "call_records",
                column: "sip_account_gid");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_start_time",
                table: "call_records",
                column: "start_time");

            migrationBuilder.CreateIndex(
                name: "IX_call_records_start_time_call_status_id",
                table: "call_records",
                columns: new[] { "start_time", "call_status_id" });

            migrationBuilder.CreateIndex(
                name: "IX_call_records_termination_cause_id",
                table: "call_records",
                column: "termination_cause_id");

            migrationBuilder.CreateIndex(
                name: "IX_call_statuses_code",
                table: "call_statuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_call_statuses_gid",
                table: "call_statuses",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_termination_causes_code",
                table: "termination_causes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_termination_causes_gid",
                table: "termination_causes",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_termination_causes_q850_code",
                table: "termination_causes",
                column: "q850_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "call_records");

            migrationBuilder.DropTable(
                name: "call_directions");

            migrationBuilder.DropTable(
                name: "call_statuses");

            migrationBuilder.DropTable(
                name: "termination_causes");
        }
    }
}
