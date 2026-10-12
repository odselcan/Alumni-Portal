using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Alumni_Portal.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "belek_graduate");

            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "announcement",
                schema: "belek_graduate",
                columns: table => new
                {
                    announcementid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    createdby = table.Column<int>(type: "integer", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    isactive = table.Column<bool>(type: "boolean", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    operation_user_id = table.Column<int>(type: "integer", nullable: true),
                    archive_action = table.Column<string>(type: "text", nullable: true),
                    archive_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_announcement", x => x.announcementid);
                });

            migrationBuilder.CreateTable(
                name: "graduate",
                schema: "belek_graduate",
                columns: table => new
                {
                    graduateid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firstname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    lastname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    faculty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    graduationdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    operation_user_id = table.Column<int>(type: "integer", nullable: true),
                    archive_action = table.Column<string>(type: "text", nullable: true),
                    archive_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_graduate", x => x.graduateid);
                });

            migrationBuilder.CreateTable(
                name: "user_auth",
                schema: "public",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    password_reset_token = table.Column<string>(type: "text", nullable: true),
                    password_reset_expires = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    temp_password = table.Column<string>(type: "text", nullable: true),
                    must_change_password = table.Column<bool>(type: "boolean", nullable: false),
                    password_changed_by = table.Column<int>(type: "integer", nullable: true),
                    password_changed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_staff = table.Column<bool>(type: "boolean", nullable: false),
                    is_superuser = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_auth", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "user_schemas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: false),
                    schema_name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_schemas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    user_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: true),
                    updated_by = table.Column<int>(type: "integer", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    update_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    profile_image_url = table.Column<string>(type: "text", nullable: true),
                    phone = table.Column<string>(type: "text", nullable: true),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    language_code = table.Column<string>(type: "text", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "text", nullable: true),
                    PasswordResetExpires = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "graduatecareer",
                schema: "belek_graduate",
                columns: table => new
                {
                    careerid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    graduateid = table.Column<int>(type: "integer", nullable: false),
                    employmentstatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    companyname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    position = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    sector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    startyear = table.Column<int>(type: "integer", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    operation_user_id = table.Column<int>(type: "integer", nullable: true),
                    archive_action = table.Column<string>(type: "text", nullable: true),
                    archive_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_graduatecareer", x => x.careerid);
                    table.ForeignKey(
                        name: "FK_graduatecareer_graduate_graduateid",
                        column: x => x.graduateid,
                        principalSchema: "belek_graduate",
                        principalTable: "graduate",
                        principalColumn: "graduateid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_graduatecareer_graduateid",
                schema: "belek_graduate",
                table: "graduatecareer",
                column: "graduateid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "announcement",
                schema: "belek_graduate");

            migrationBuilder.DropTable(
                name: "graduatecareer",
                schema: "belek_graduate");

            migrationBuilder.DropTable(
                name: "user_auth",
                schema: "public");

            migrationBuilder.DropTable(
                name: "user_schemas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "graduate",
                schema: "belek_graduate");
        }
    }
}
