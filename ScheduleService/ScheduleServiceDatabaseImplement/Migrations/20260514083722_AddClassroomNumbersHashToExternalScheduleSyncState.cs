using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleServiceDatabaseImplement.Migrations
{
    /// <inheritdoc />
    public partial class AddClassroomNumbersHashToExternalScheduleSyncState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassroomNumbersHash",
                table: "external_schedule_sync_states",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassroomNumbersHash",
                table: "external_schedule_sync_states");
        }
    }
}
