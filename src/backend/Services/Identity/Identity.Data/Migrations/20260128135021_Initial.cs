using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Identity.Data.Migrations
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
                name: "app_permissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    is_special = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_permissions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_roles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "structure_dict",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    region = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_structure_dict", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    structure_id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    gid = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_teams_structure_dict_structure_id",
                        column: x => x.structure_id,
                        principalTable: "structure_dict",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    department = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    structure_id = table.Column<int>(type: "int", nullable: false),
                    team_id = table.Column<long>(type: "bigint", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    last_login_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    supervisor_id = table.Column<long>(type: "bigint", nullable: true),
                    gid = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    created_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    modified_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    modified_by_user_id = table.Column<long>(type: "bigint", nullable: true),
                    is_deleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_structure_dict_structure_id",
                        column: x => x.structure_id,
                        principalTable: "structure_dict",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_users_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_users_users_supervisor_id",
                        column: x => x.supervisor_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_permission_assignments",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<int>(type: "int", nullable: false),
                    granted_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    granted_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_permission_assignments", x => new { x.user_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_app_user_permission_assignments_app_permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "app_permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_app_user_permission_assignments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "app_user_role_assignments",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false),
                    assigned_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    assigned_by = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_app_user_role_assignments", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_app_user_role_assignments_app_user_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "app_user_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_app_user_role_assignments_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "login_audit_logs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    app_user_id = table.Column<long>(type: "bigint", nullable: false),
                    login_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ip_address = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    success = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FailureReason = table.Column<string>(type: "longtext", nullable: true),
                    user_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    user_fullname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_login_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_login_audit_logs_users_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    app_user_id = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_used = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_password_reset_tokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_password_reset_tokens_users_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_app_permissions_category",
                table: "app_permissions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_app_permissions_name",
                table: "app_permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_app_user_permission_assignments_permission_id",
                table: "app_user_permission_assignments",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_permission_assignments_user_id",
                table: "app_user_permission_assignments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_role_assignments_role_id",
                table: "app_user_role_assignments",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_role_assignments_user_id",
                table: "app_user_role_assignments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_app_user_roles_name",
                table: "app_user_roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_login_audit_logs_app_user_id",
                table: "login_audit_logs",
                column: "app_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_audit_logs_login_at",
                table: "login_audit_logs",
                column: "login_at");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_app_user_id",
                table: "password_reset_tokens",
                column: "app_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_expires_at",
                table: "password_reset_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_password_reset_tokens_token",
                table: "password_reset_tokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_structure_dict_code",
                table: "structure_dict",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_structure_dict_name",
                table: "structure_dict",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_code",
                table: "teams",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_gid",
                table: "teams",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teams_structure_id_name",
                table: "teams",
                columns: new[] { "structure_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_gid",
                table: "users",
                column: "gid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_structure_id",
                table: "users",
                column: "structure_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_supervisor_id",
                table: "users",
                column: "supervisor_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_team_id",
                table: "users",
                column: "team_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "app_user_permission_assignments");

            migrationBuilder.DropTable(
                name: "app_user_role_assignments");

            migrationBuilder.DropTable(
                name: "login_audit_logs");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "app_permissions");

            migrationBuilder.DropTable(
                name: "app_user_roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "structure_dict");
        }
    }
}
