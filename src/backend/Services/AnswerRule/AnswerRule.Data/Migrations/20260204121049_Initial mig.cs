using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace AnswerRule.Data.Migrations
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
                name: "answering_rules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    sip_account_gid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    priority = table.Column<int>(type: "int", nullable: false),
                    is_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    action_type = table.Column<string>(type: "longtext", nullable: false),
                    action_target = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    voicemail_box_gid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    voice_message_gid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    send_email_notification = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    notification_email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    gid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answering_rules", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "answering_rule_time_slots",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    answering_rule_id = table.Column<long>(type: "bigint", nullable: false),
                    day_of_week = table.Column<string>(type: "longtext", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    is_all_day = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    gid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answering_rule_time_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_answering_rule_time_slots_answering_rules_answering_rule_id",
                        column: x => x.answering_rule_id,
                        principalTable: "answering_rules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_answering_rule_time_slots_answering_rule_id",
                table: "answering_rule_time_slots",
                column: "answering_rule_id");

            migrationBuilder.CreateIndex(
                name: "IX_answering_rule_time_slots_gid",
                table: "answering_rule_time_slots",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_answering_rules_gid",
                table: "answering_rules",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_answering_rules_is_enabled_priority",
                table: "answering_rules",
                columns: new[] { "is_enabled", "priority" });

            migrationBuilder.CreateIndex(
                name: "IX_answering_rules_sip_account_gid",
                table: "answering_rules",
                column: "sip_account_gid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answering_rule_time_slots");

            migrationBuilder.DropTable(
                name: "answering_rules");
        }
    }
}
