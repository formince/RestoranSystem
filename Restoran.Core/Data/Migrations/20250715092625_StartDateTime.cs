﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restoran.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class StartDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationDateTime",
                table: "Reservations",
                newName: "StartDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Reservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "Reservations",
                newName: "ReservationDateTime");
        }
    }
}
