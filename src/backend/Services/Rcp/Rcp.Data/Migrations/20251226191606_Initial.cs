using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Rcp.Data.Migrations
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
                name: "rcp_periods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rcp_periods", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rcp_monthly_entries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    RcpPeriodId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    UserGid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalMinutes = table.Column<int>(type: "int", nullable: false),
                    EmployeeFullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    StatusChangedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    StatusChangedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    StatusChangedByFullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ApprovedByFullName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rcp_monthly_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rcp_monthly_entries_rcp_periods_RcpPeriodId",
                        column: x => x.RcpPeriodId,
                        principalTable: "rcp_periods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rcp_day_entries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MonthlyEntryId = table.Column<long>(type: "bigint", nullable: false),
                    WorkDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    WorkMinutes = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rcp_day_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rcp_day_entries_rcp_monthly_entries_MonthlyEntryId",
                        column: x => x.MonthlyEntryId,
                        principalTable: "rcp_monthly_entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rcp_entry_comments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MonthlyEntryId = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    AuthorUserId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    AuthorRole = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Gid = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rcp_entry_comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_rcp_entry_comments_rcp_monthly_entries_MonthlyEntryId",
                        column: x => x.MonthlyEntryId,
                        principalTable: "rcp_monthly_entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_rcp_day_entries_Gid",
                table: "rcp_day_entries",
                column: "Gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rcp_day_entries_MonthlyEntryId_WorkDate",
                table: "rcp_day_entries",
                columns: new[] { "MonthlyEntryId", "WorkDate" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_rcp_entry_comments_Gid",
                table: "rcp_entry_comments",
                column: "Gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rcp_entry_comments_MonthlyEntryId",
                table: "rcp_entry_comments",
                column: "MonthlyEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_rcp_monthly_entries_Gid",
                table: "rcp_monthly_entries",
                column: "Gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rcp_monthly_entries_RcpPeriodId_UserId",
                table: "rcp_monthly_entries",
                columns: new[] { "RcpPeriodId", "UserId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_rcp_periods_Gid",
                table: "rcp_periods",
                column: "Gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rcp_periods_Year_Month",
                table: "rcp_periods",
                columns: new[] { "Year", "Month" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rcp_day_entries");

            migrationBuilder.DropTable(
                name: "rcp_entry_comments");

            migrationBuilder.DropTable(
                name: "rcp_monthly_entries");

            migrationBuilder.DropTable(
                name: "rcp_periods");
        }
    }
}
