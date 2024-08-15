using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMDT_MOHINHMACCA.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIES",
                columns: table => new
                {
                    CATE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CATE_NAME = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CATE_IMAGE = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CATEGORI__EB0A758F05F8D0A4", x => x.CATE_ID);
                });

            migrationBuilder.CreateTable(
                name: "CHOOSES",
                columns: table => new
                {
                    CHOOSE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CHOOSE_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CHOOSE_TIME = table.Column<int>(type: "int", nullable: true),
                    DESCRIPTION = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DISCOUNT = table.Column<int>(type: "int", nullable: true),
                    PRICE = table.Column<int>(type: "int", nullable: true),
                    RATE = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CHOOSES__80E419B8A7582BF5", x => x.CHOOSE_ID);
                });

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    ROLE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ROLE_NAME = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ROLES__5AC4D222B938A979", x => x.ROLE_ID);
                });

            migrationBuilder.CreateTable(
                name: "ACCOUNT",
                columns: table => new
                {
                    USERNAME = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PASSWORD = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    FULLNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GENDER = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    ADDRESS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BIRTHDAY = table.Column<DateOnly>(type: "date", nullable: true),
                    AVATAR = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    PHONE = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: true),
                    EMAIL = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    MONEY = table.Column<decimal>(type: "money", nullable: true),
                    ROLE_ID = table.Column<int>(type: "int", nullable: true),
                    SIGNUPDATE = table.Column<DateTime>(type: "datetime", nullable: true),
                    VALIDITY = table.Column<bool>(type: "bit", nullable: true),
                    RANDOMKEY = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    GOOGLE_ID = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ACCOUNT__B15BE12F3966D95C", x => x.USERNAME);
                    table.ForeignKey(
                        name: "FK__ACCOUNT__ROLE_ID__2A4B4B5E",
                        column: x => x.ROLE_ID,
                        principalTable: "ROLES",
                        principalColumn: "ROLE_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MESSAGE",
                columns: table => new
                {
                    MESSAGE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SENDER_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    RECEIVER_ID = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    STATUS = table.Column<int>(type: "int", nullable: true),
                    CONTENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SENTTIME = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MESSAGE__F610EE443884DFA2", x => x.MESSAGE_ID);
                    table.ForeignKey(
                        name: "FK__MESSAGE__RECEIVE__3D5E1FD2",
                        column: x => x.RECEIVER_ID,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME");
                    table.ForeignKey(
                        name: "FK__MESSAGE__SENDER___3C69FB99",
                        column: x => x.SENDER_ID,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME");
                });

            migrationBuilder.CreateTable(
                name: "POST",
                columns: table => new
                {
                    POST_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TITLE = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CONTENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COVER_IMAGE = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    CATE_ID = table.Column<int>(type: "int", nullable: true),
                    USERNAME = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TECHNOLOGY = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    POST_TIME = table.Column<DateTime>(type: "datetime", nullable: true),
                    PRICE_UP = table.Column<double>(type: "float", nullable: true),
                    PRICE_TO = table.Column<double>(type: "float", nullable: true),
                    CHOOSE_ID = table.Column<int>(type: "int", nullable: true),
                    STATUS = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    POST_APPROVEDTIME = table.Column<DateTime>(type: "datetime", nullable: true),
                    NOTE = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__POST__63FD1766EF102CCF", x => x.POST_ID);
                    table.ForeignKey(
                        name: "FK__POST__CATE_ID__2D27B809",
                        column: x => x.CATE_ID,
                        principalTable: "CATEGORIES",
                        principalColumn: "CATE_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__POST__CHOOSE_ID__2F10007B",
                        column: x => x.CHOOSE_ID,
                        principalTable: "CHOOSES",
                        principalColumn: "CHOOSE_ID");
                    table.ForeignKey(
                        name: "FK__POST__USERNAME__2E1BDC42",
                        column: x => x.USERNAME,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TRANSACTIONHISTORY",
                columns: table => new
                {
                    TRANSACTION_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERNAME = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    TRANSACTION_TYPE = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    AMOUNTMONEY = table.Column<decimal>(type: "money", nullable: true),
                    INITIALBALANCE = table.Column<decimal>(type: "money", nullable: true),
                    FINALBALANCE = table.Column<decimal>(type: "money", nullable: true),
                    CONTENT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TRANSACTION_DATE = table.Column<DateTime>(type: "datetime", nullable: true),
                    PAYMENTS = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TRANSACT__16998B616BBE39A3", x => x.TRANSACTION_ID);
                    table.ForeignKey(
                        name: "FK__TRANSACTI__USERN__5CD6CB2B",
                        column: x => x.USERNAME,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME");
                });

            migrationBuilder.CreateTable(
                name: "ORDERS",
                columns: table => new
                {
                    ORDER_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ORDER_TIME = table.Column<DateTime>(type: "datetime", nullable: true),
                    POST_ID = table.Column<int>(type: "int", nullable: true),
                    BUYER = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    PRICE = table.Column<double>(type: "float", nullable: true),
                    STATUS = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    REQUIREMENTS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NUMBERDAY = table.Column<int>(type: "int", nullable: true),
                    STAR = table.Column<double>(type: "float", nullable: true),
                    REVIEW = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LINK = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ORDERS__460A9464DC95DBCB", x => x.ORDER_ID);
                    table.ForeignKey(
                        name: "FK__ORDERS__BUYER__35BCFE0A",
                        column: x => x.BUYER,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME");
                    table.ForeignKey(
                        name: "FK__ORDERS__POST_ID__34C8D9D1",
                        column: x => x.POST_ID,
                        principalTable: "POST",
                        principalColumn: "POST_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POST_IMAGES",
                columns: table => new
                {
                    POST_ID = table.Column<int>(type: "int", nullable: false),
                    IMAGE_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IMAGE = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__POST_IMA__E4178F0E9C5E2952", x => new { x.POST_ID, x.IMAGE_ID });
                    table.ForeignKey(
                        name: "FK__POST_IMAG__POST___31EC6D26",
                        column: x => x.POST_ID,
                        principalTable: "POST",
                        principalColumn: "POST_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ORDER_DETAIL",
                columns: table => new
                {
                    DETAIL_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ORDER_ID = table.Column<int>(type: "int", nullable: true),
                    STAMPTIME = table.Column<DateTime>(type: "datetime", nullable: true),
                    PERSON = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    REQUI_TYPE = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ORDER_DE__76047C44793DC5B8", x => x.DETAIL_ID);
                    table.ForeignKey(
                        name: "FK__ORDER_DET__ORDER__38996AB5",
                        column: x => x.ORDER_ID,
                        principalTable: "ORDERS",
                        principalColumn: "ORDER_ID");
                    table.ForeignKey(
                        name: "FK__ORDER_DET__PERSO__398D8EEE",
                        column: x => x.PERSON,
                        principalTable: "ACCOUNT",
                        principalColumn: "USERNAME");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACCOUNT_ROLE_ID",
                table: "ACCOUNT",
                column: "ROLE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGE_RECEIVER_ID",
                table: "MESSAGE",
                column: "RECEIVER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGE_SENDER_ID",
                table: "MESSAGE",
                column: "SENDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DETAIL_ORDER_ID",
                table: "ORDER_DETAIL",
                column: "ORDER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ORDER_DETAIL_PERSON",
                table: "ORDER_DETAIL",
                column: "PERSON");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_BUYER",
                table: "ORDERS",
                column: "BUYER");

            migrationBuilder.CreateIndex(
                name: "IX_ORDERS_POST_ID",
                table: "ORDERS",
                column: "POST_ID");

            migrationBuilder.CreateIndex(
                name: "IX_POST_CATE_ID",
                table: "POST",
                column: "CATE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_POST_CHOOSE_ID",
                table: "POST",
                column: "CHOOSE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_POST_USERNAME",
                table: "POST",
                column: "USERNAME");

            migrationBuilder.CreateIndex(
                name: "IX_TRANSACTIONHISTORY_USERNAME",
                table: "TRANSACTIONHISTORY",
                column: "USERNAME");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MESSAGE");

            migrationBuilder.DropTable(
                name: "ORDER_DETAIL");

            migrationBuilder.DropTable(
                name: "POST_IMAGES");

            migrationBuilder.DropTable(
                name: "TRANSACTIONHISTORY");

            migrationBuilder.DropTable(
                name: "ORDERS");

            migrationBuilder.DropTable(
                name: "POST");

            migrationBuilder.DropTable(
                name: "CATEGORIES");

            migrationBuilder.DropTable(
                name: "CHOOSES");

            migrationBuilder.DropTable(
                name: "ACCOUNT");

            migrationBuilder.DropTable(
                name: "ROLES");
        }
    }
}
