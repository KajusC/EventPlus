using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eventplus.models.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id_category = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("category_pkey", x => x.id_category);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id_equipment = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("equipment_pkey", x => x.id_equipment);
                });

            migrationBuilder.CreateTable(
                name: "feedback_type",
                columns: table => new
                {
                    id_feedback_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_type_pkey", x => x.id_feedback_type);
                });

            migrationBuilder.CreateTable(
                name: "loyalty",
                columns: table => new
                {
                    id_loyalty = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    points = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("loyalty_pkey", x => x.id_loyalty);
                });

            migrationBuilder.CreateTable(
                name: "partner",
                columns: table => new
                {
                    id_partner = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    website = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("partner_pkey", x => x.id_partner);
                });

            migrationBuilder.CreateTable(
                name: "performer",
                columns: table => new
                {
                    id_performer = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    profesija = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("performer_pkey", x => x.id_performer);
                });

            migrationBuilder.CreateTable(
                name: "ticket_status",
                columns: table => new
                {
                    id_ticket_status = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_status_pkey", x => x.id_ticket_status);
                });

            migrationBuilder.CreateTable(
                name: "ticket_type",
                columns: table => new
                {
                    id_ticket_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_type_pkey", x => x.id_ticket_type);
                });

            migrationBuilder.CreateTable(
                name: "user_request_information",
                columns: table => new
                {
                    id_user_request_information = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    klausimas = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    atsakas = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_request_information_pkey", x => x.id_user_request_information);
                });

            migrationBuilder.CreateTable(
                name: "user_type",
                columns: table => new
                {
                    id_user_type = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_type_pkey", x => x.id_user_type);
                });

            migrationBuilder.CreateTable(
                name: "event_location",
                columns: table => new
                {
                    id_event_location = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    city = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    country = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    capacity = table.Column<int>(type: "integer", nullable: true),
                    contacts = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    price = table.Column<double>(type: "double precision", nullable: true),
                    turima_ÄÆranga = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event_location_pkey", x => x.id_event_location);
                    table.ForeignKey(
                        name: "event_location_turima_ÄÆranga_fkey",
                        column: x => x.turima_ÄÆranga,
                        principalTable: "equipment",
                        principalColumn: "id_equipment");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    last_login = table.Column<DateOnly>(type: "date", nullable: true),
                    fk_loyaltyid_loyalty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("User_pkey", x => x.id_user);
                    table.ForeignKey(
                        name: "User_fk_loyaltyid_loyalty_fkey",
                        column: x => x.fk_loyaltyid_loyalty,
                        principalTable: "loyalty",
                        principalColumn: "id_loyalty");
                });

            migrationBuilder.CreateTable(
                name: "sector",
                columns: table => new
                {
                    id_sector = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_event_locationid_event_location = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sector_pkey", x => new { x.id_sector, x.fk_event_locationid_event_location });
                    table.ForeignKey(
                        name: "sector_fk_event_locationid_event_location_fkey",
                        column: x => x.fk_event_locationid_event_location,
                        principalTable: "event_location",
                        principalColumn: "id_event_location",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "administrator",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrator_pkey", x => x.id_user);
                    table.ForeignKey(
                        name: "administrator_id_user_fkey",
                        column: x => x.id_user,
                        principalTable: "User",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "organiser",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    follower_amount = table.Column<int>(type: "integer", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("organiser_pkey", x => x.id_user);
                    table.ForeignKey(
                        name: "organiser_id_user_fkey",
                        column: x => x.id_user,
                        principalTable: "User",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "userrequest",
                columns: table => new
                {
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("userrequest_pkey", x => x.fk_userid_user);
                    table.ForeignKey(
                        name: "userrequest_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalTable: "User",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "event",
                columns: table => new
                {
                    id_event = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    max_ticket_count = table.Column<int>(type: "integer", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: true),
                    fk_event_locationid_event_location = table.Column<int>(type: "integer", nullable: false),
                    fk_organiserid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event_pkey", x => x.id_event);
                    table.ForeignKey(
                        name: "event_category_fkey",
                        column: x => x.category,
                        principalTable: "category",
                        principalColumn: "id_category");
                    table.ForeignKey(
                        name: "event_fk_event_locationid_event_location_fkey",
                        column: x => x.fk_event_locationid_event_location,
                        principalTable: "event_location",
                        principalColumn: "id_event_location");
                    table.ForeignKey(
                        name: "event_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalTable: "organiser",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    id_feedback = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    vote = table.Column<double>(type: "double precision", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false),
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_pkey", x => x.id_feedback);
                    table.ForeignKey(
                        name: "feedback_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalTable: "event",
                        principalColumn: "id_event");
                    table.ForeignKey(
                        name: "feedback_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalTable: "User",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "feedback_type_fkey",
                        column: x => x.type,
                        principalTable: "feedback_type",
                        principalColumn: "id_feedback_type");
                });

            migrationBuilder.CreateTable(
                name: "renginioatlikÄ—jas",
                columns: table => new
                {
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("renginioatlikÄ—jas_pkey", x => x.fk_eventid_event);
                    table.ForeignKey(
                        name: "renginioatlikÄ—jas_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalTable: "event",
                        principalColumn: "id_event");
                });

            migrationBuilder.CreateTable(
                name: "renginiopartneris",
                columns: table => new
                {
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("renginiopartneris_pkey", x => x.fk_eventid_event);
                    table.ForeignKey(
                        name: "renginiopartneris_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalTable: "event",
                        principalColumn: "id_event");
                });

            migrationBuilder.CreateTable(
                name: "sector_price",
                columns: table => new
                {
                    id_sector_price = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    price = table.Column<double>(type: "double precision", nullable: true),
                    fk_sectorid_sector = table.Column<int>(type: "integer", nullable: false),
                    fk_sectorfk_event_locationid_event_location = table.Column<int>(type: "integer", nullable: false),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sector_price_pkey", x => x.id_sector_price);
                    table.ForeignKey(
                        name: "sector_price_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalTable: "event",
                        principalColumn: "id_event",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "sector_price_fk_sectorid_sector_fk_sectorfk_event_location_fkey",
                        columns: x => new { x.fk_sectorid_sector, x.fk_sectorfk_event_locationid_event_location },
                        principalTable: "sector",
                        principalColumns: new[] { "id_sector", "fk_event_locationid_event_location" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                columns: table => new
                {
                    id_ticket = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    price = table.Column<double>(type: "double precision", nullable: true),
                    generation_date = table.Column<DateOnly>(type: "date", nullable: true),
                    scanned_date = table.Column<DateOnly>(type: "date", nullable: true),
                    qr_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false),
                    TicketStatusesIdTicketStatus = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_pkey", x => x.id_ticket);
                    table.ForeignKey(
                        name: "FK_ticket_ticket_status_TicketStatusesIdTicketStatus",
                        column: x => x.TicketStatusesIdTicketStatus,
                        principalTable: "ticket_status",
                        principalColumn: "id_ticket_status");
                    table.ForeignKey(
                        name: "ticket_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalTable: "event",
                        principalColumn: "id_event");
                    table.ForeignKey(
                        name: "ticket_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalTable: "User",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "ticket_type_fkey",
                        column: x => x.type,
                        principalTable: "ticket_type",
                        principalColumn: "id_ticket_type");
                });

            migrationBuilder.CreateTable(
                name: "seating",
                columns: table => new
                {
                    id_seating = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_sectorid_sector = table.Column<int>(type: "integer", nullable: false),
                    fk_sectorfk_event_locationid_event_location = table.Column<int>(type: "integer", nullable: false),
                    row = table.Column<int>(type: "integer", nullable: true),
                    place = table.Column<int>(type: "integer", nullable: true),
                    fk_ticketid_ticket = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("seating_pkey", x => new { x.id_seating, x.fk_sectorid_sector, x.fk_sectorfk_event_locationid_event_location });
                    table.ForeignKey(
                        name: "seating_fk_sectorid_sector_fk_sectorfk_event_locationid_ev_fkey",
                        columns: x => new { x.fk_sectorid_sector, x.fk_sectorfk_event_locationid_event_location },
                        principalTable: "sector",
                        principalColumns: new[] { "id_sector", "fk_event_locationid_event_location" });
                    table.ForeignKey(
                        name: "seating_fk_ticketid_ticket_fkey",
                        column: x => x.fk_ticketid_ticket,
                        principalTable: "ticket",
                        principalColumn: "id_ticket");
                });

            migrationBuilder.InsertData(
                table: "category",
                columns: new[] { "id_category", "name" },
                values: new object[,]
                {
                    { 1, "Music" },
                    { 2, "Conference" },
                    { 3, "Sports" }
                });

            migrationBuilder.InsertData(
                table: "equipment",
                columns: new[] { "id_equipment", "name" },
                values: new object[,]
                {
                    { 1, "Music" },
                    { 2, "Projector" },
                    { 3, "Microphone" },
                    { 4, "TV" }
                });

            migrationBuilder.InsertData(
                table: "loyalty",
                columns: new[] { "id_loyalty", "points" },
                values: new object[] { 1, 0 });

            migrationBuilder.InsertData(
                table: "ticket_status",
                columns: new[] { "id_ticket_status", "name" },
                values: new object[,]
                {
                    { 1, "Valid" },
                    { 2, "Invalid" },
                    { 3, "Scanned" }
                });

            migrationBuilder.InsertData(
                table: "ticket_type",
                columns: new[] { "id_ticket_type", "name" },
                values: new object[,]
                {
                    { 1, "Standard" },
                    { 2, "VIP" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "id_user", "fk_loyaltyid_loyalty", "last_login", "name", "password", "surname", "username" },
                values: new object[] { 1, 1, new DateOnly(2025, 4, 15), "Event", "password123", "Organizer", "organizer" });

            migrationBuilder.InsertData(
                table: "event_location",
                columns: new[] { "id_event_location", "address", "capacity", "city", "contacts", "country", "turima_ÄÆranga", "name", "price" },
                values: new object[] { 1, "123 Main St", 500, "Boston", "contact@venue.com", "USA", 2, "Conference Center", 1000.0 });

            migrationBuilder.InsertData(
                table: "organiser",
                columns: new[] { "id_user", "follower_amount", "rating" },
                values: new object[] { 1, 0, 5.0 });

            migrationBuilder.InsertData(
                table: "sector",
                columns: new[] { "fk_event_locationid_event_location", "id_sector", "name" },
                values: new object[,]
                {
                    { 1, 1, "Main Hall" },
                    { 1, 2, "VIP Section" }
                });

            migrationBuilder.InsertData(
                table: "event",
                columns: new[] { "id_event", "category", "description", "end_date", "fk_event_locationid_event_location", "fk_organiserid_user", "max_ticket_count", "name", "start_date" },
                values: new object[] { 1, 2, "Annual technology conference", new DateTime(2025, 6, 17, 17, 0, 0, 0, DateTimeKind.Utc), 1, 1, 500, "Tech Conference 2025", new DateTime(2025, 6, 15, 9, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "sector_price",
                columns: new[] { "id_sector_price", "fk_eventid_event", "fk_sectorfk_event_locationid_event_location", "fk_sectorid_sector", "price" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 50.0 },
                    { 2, 1, 1, 2, 150.0 }
                });

            migrationBuilder.CreateIndex(
                name: "event_fk_event_locationid_event_location_key",
                table: "event",
                column: "fk_event_locationid_event_location",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_category",
                table: "event",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_event_fk_organiserid_user",
                table: "event",
                column: "fk_organiserid_user");

            migrationBuilder.CreateIndex(
                name: "IX_event_location_turima_ÄÆranga",
                table: "event_location",
                column: "turima_ÄÆranga");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_fk_eventid_event",
                table: "feedback",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_fk_userid_user",
                table: "feedback",
                column: "fk_userid_user");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_type",
                table: "feedback",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_seating_fk_sectorid_sector_fk_sectorfk_event_locationid_eve~",
                table: "seating",
                columns: new[] { "fk_sectorid_sector", "fk_sectorfk_event_locationid_event_location" });

            migrationBuilder.CreateIndex(
                name: "seating_fk_ticketid_ticket_key",
                table: "seating",
                column: "fk_ticketid_ticket",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sector_fk_event_locationid_event_location",
                table: "sector",
                column: "fk_event_locationid_event_location");

            migrationBuilder.CreateIndex(
                name: "IX_sector_price_fk_eventid_event",
                table: "sector_price",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_sector_price_fk_sectorid_sector_fk_sectorfk_event_locationi~",
                table: "sector_price",
                columns: new[] { "fk_sectorid_sector", "fk_sectorfk_event_locationid_event_location" });

            migrationBuilder.CreateIndex(
                name: "IX_ticket_fk_eventid_event",
                table: "ticket",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_fk_userid_user",
                table: "ticket",
                column: "fk_userid_user");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_TicketStatusesIdTicketStatus",
                table: "ticket",
                column: "TicketStatusesIdTicketStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_type",
                table: "ticket",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "User_fk_loyaltyid_loyalty_key",
                table: "User",
                column: "fk_loyaltyid_loyalty",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrator");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "partner");

            migrationBuilder.DropTable(
                name: "performer");

            migrationBuilder.DropTable(
                name: "renginioatlikÄ—jas");

            migrationBuilder.DropTable(
                name: "renginiopartneris");

            migrationBuilder.DropTable(
                name: "seating");

            migrationBuilder.DropTable(
                name: "sector_price");

            migrationBuilder.DropTable(
                name: "user_request_information");

            migrationBuilder.DropTable(
                name: "user_type");

            migrationBuilder.DropTable(
                name: "userrequest");

            migrationBuilder.DropTable(
                name: "feedback_type");

            migrationBuilder.DropTable(
                name: "ticket");

            migrationBuilder.DropTable(
                name: "sector");

            migrationBuilder.DropTable(
                name: "ticket_status");

            migrationBuilder.DropTable(
                name: "event");

            migrationBuilder.DropTable(
                name: "ticket_type");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "event_location");

            migrationBuilder.DropTable(
                name: "organiser");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "loyalty");
        }
    }
}
