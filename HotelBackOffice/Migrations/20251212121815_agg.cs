using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBackOffice.Migrations
{
    /// <inheritdoc />
    public partial class agg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazione_AspNetUsers_CreataDaUserId",
                table: "Prenotazione");

            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazione_Camera_CameraId",
                table: "Prenotazione");

            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazione_Cliente_ClienteId",
                table: "Prenotazione");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prenotazione",
                table: "Prenotazione");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Camera",
                table: "Camera");

            migrationBuilder.RenameTable(
                name: "Prenotazione",
                newName: "Prenotazioni");

            migrationBuilder.RenameTable(
                name: "Cliente",
                newName: "Clienti");

            migrationBuilder.RenameTable(
                name: "Camera",
                newName: "Camere");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazione_CreataDaUserId",
                table: "Prenotazioni",
                newName: "IX_Prenotazioni_CreataDaUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazione_ClienteId",
                table: "Prenotazioni",
                newName: "IX_Prenotazioni_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazione_CameraId",
                table: "Prenotazioni",
                newName: "IX_Prenotazioni_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prenotazioni",
                table: "Prenotazioni",
                column: "PrenotazioneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clienti",
                table: "Clienti",
                column: "ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Camere",
                table: "Camere",
                column: "CameraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazioni_AspNetUsers_CreataDaUserId",
                table: "Prenotazioni",
                column: "CreataDaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazioni_Camere_CameraId",
                table: "Prenotazioni",
                column: "CameraId",
                principalTable: "Camere",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazioni_Clienti_ClienteId",
                table: "Prenotazioni",
                column: "ClienteId",
                principalTable: "Clienti",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazioni_AspNetUsers_CreataDaUserId",
                table: "Prenotazioni");

            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazioni_Camere_CameraId",
                table: "Prenotazioni");

            migrationBuilder.DropForeignKey(
                name: "FK_Prenotazioni_Clienti_ClienteId",
                table: "Prenotazioni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prenotazioni",
                table: "Prenotazioni");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clienti",
                table: "Clienti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Camere",
                table: "Camere");

            migrationBuilder.RenameTable(
                name: "Prenotazioni",
                newName: "Prenotazione");

            migrationBuilder.RenameTable(
                name: "Clienti",
                newName: "Cliente");

            migrationBuilder.RenameTable(
                name: "Camere",
                newName: "Camera");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazioni_CreataDaUserId",
                table: "Prenotazione",
                newName: "IX_Prenotazione_CreataDaUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazioni_ClienteId",
                table: "Prenotazione",
                newName: "IX_Prenotazione_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Prenotazioni_CameraId",
                table: "Prenotazione",
                newName: "IX_Prenotazione_CameraId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prenotazione",
                table: "Prenotazione",
                column: "PrenotazioneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente",
                column: "ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Camera",
                table: "Camera",
                column: "CameraId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazione_AspNetUsers_CreataDaUserId",
                table: "Prenotazione",
                column: "CreataDaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazione_Camera_CameraId",
                table: "Prenotazione",
                column: "CameraId",
                principalTable: "Camera",
                principalColumn: "CameraId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prenotazione_Cliente_ClienteId",
                table: "Prenotazione",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "ClienteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
