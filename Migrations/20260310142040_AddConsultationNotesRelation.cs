using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreyaCare.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationNotesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppointmentId1",
                table: "ConsultationNotes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationNotes_AppointmentId1",
                table: "ConsultationNotes",
                column: "AppointmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultationNotes_Appointments_AppointmentId1",
                table: "ConsultationNotes",
                column: "AppointmentId1",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultationNotes_Appointments_AppointmentId1",
                table: "ConsultationNotes");

            migrationBuilder.DropIndex(
                name: "IX_ConsultationNotes_AppointmentId1",
                table: "ConsultationNotes");

            migrationBuilder.DropColumn(
                name: "AppointmentId1",
                table: "ConsultationNotes");
        }
    }
}
