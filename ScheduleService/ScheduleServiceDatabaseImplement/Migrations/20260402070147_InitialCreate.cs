using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ScheduleServiceDatabaseImplement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "duty_persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty_persons", x => x.Id);
                },
                comment: "Дежурные");

            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreSystemId = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_groups", x => x.Id);
                },
                comment: "Учебные группы");

            migrationBuilder.CreateTable(
                name: "lesson_times",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PairNumber = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson_times", x => x.Id);
                },
                comment: "Время учебных пар");

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoreSystemId = table.Column<int>(type: "integer", nullable: false),
                    TeacherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.Id);
                },
                comment: "Преподаватели");

            migrationBuilder.CreateTable(
                name: "duty_schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LessonTimeId = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Place = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DutyPersonId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duty_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_duty_schedules_duty_persons_DutyPersonId",
                        column: x => x.DutyPersonId,
                        principalTable: "duty_persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_duty_schedules_lesson_times_LessonTimeId",
                        column: x => x.LessonTimeId,
                        principalTable: "lesson_times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "График дежурств");

            migrationBuilder.CreateTable(
                name: "schedule_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LessonTimeId = table.Column<int>(type: "integer", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    ClassroomId = table.Column<int>(type: "integer", nullable: true),
                    ClassroomNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GroupId = table.Column<int>(type: "integer", nullable: true),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TeacherId = table.Column<int>(type: "integer", nullable: true),
                    TeacherName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_schedule_items_groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_schedule_items_lesson_times_LessonTimeId",
                        column: x => x.LessonTimeId,
                        principalTable: "lesson_times",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_schedule_items_teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "Элементы расписания");

            migrationBuilder.CreateIndex(
                name: "IX_duty_schedules_DutyPersonId",
                table: "duty_schedules",
                column: "DutyPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_duty_schedules_LessonTimeId",
                table: "duty_schedules",
                column: "LessonTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_groups_CoreSystemId",
                table: "groups",
                column: "CoreSystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_groups_GroupName",
                table: "groups",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_times_PairNumber",
                table: "lesson_times",
                column: "PairNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_schedule_items_Date",
                table: "schedule_items",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_items_GroupId",
                table: "schedule_items",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_items_LessonTimeId",
                table: "schedule_items",
                column: "LessonTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_items_TeacherId",
                table: "schedule_items",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_CoreSystemId",
                table: "teachers",
                column: "CoreSystemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_teachers_TeacherName",
                table: "teachers",
                column: "TeacherName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "duty_schedules");

            migrationBuilder.DropTable(
                name: "schedule_items");

            migrationBuilder.DropTable(
                name: "duty_persons");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "lesson_times");

            migrationBuilder.DropTable(
                name: "teachers");
        }
    }
}
