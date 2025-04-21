using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoworkingInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdentityIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder) { 

            // 1. Якщо існує foreign key на bookings.user_id (int), спочатку дропаємо його
            //    Ви можете подивитися в SSMS або в старих міграціях, як називається цей ключ.
            //    Припустимо, його ім’я - FK__bookings__user_i__656C112C (чи інше).
            //    Якщо у вас немає foreign key, пропустіть цей крок.

            migrationBuilder.DropForeignKey(
                name: "FK__bookings__user_i__656C112C",
                table: "bookings");

            // 2. Змінюємо тип стовпця user_id із int на string (nvarchar(450))
            migrationBuilder.AlterColumn<string>(
                name: "user_id",
                table: "bookings",
                type: "nvarchar(450)",
                nullable: true, // залежить від вашої логіки - чи дозволяєте null
                oldClrType: typeof(int),
                oldType: "int");

            // 3. Знову додаємо foreign key (якщо треба),
            //    щоб bookings.user_id посилався на AspNetUsers.Id (string)
            //    Якщо у вас таблиця користувачів називається інакше (наприклад, users), 
            //    замініть "AspNetUsers" на вашу назву.

            migrationBuilder.AddForeignKey(
                name: "FK__bookings__user_id__656C112C",
                table: "bookings",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            // onDelete можна змінити на Cascade, SetNull тощо
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Дропаємо новий foreign key
            migrationBuilder.DropForeignKey(
                name: "FK__bookings__user_id__656C112C",
                table: "bookings");

            // 2. Повертаємо стовпець user_id до int
            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "bookings",
                type: "int",
                nullable: false, // або nullable, залежно від вашої попередньої схеми
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // 3. (За бажанням) повертаємо старий foreign key
            migrationBuilder.AddForeignKey(
                name: "FK__bookings__user_i__656C112C",
                table: "bookings",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
