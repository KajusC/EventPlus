using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eventplus.models.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "models");

            migrationBuilder.CreateTable(
                name: "administrator",
                schema: "models",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrator_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "category",
                schema: "models",
                columns: table => new
                {
                    id_category = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("category_pkey", x => x.id_category);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                schema: "models",
                columns: table => new
                {
                    id_equipment = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("equipment_pkey", x => x.id_equipment);
                });

            migrationBuilder.CreateTable(
                name: "eventpartner",
                schema: "models",
                columns: table => new
                {
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("eventpartner_pkey", x => x.fk_eventid_event);
                });

            migrationBuilder.CreateTable(
                name: "eventperformer",
                schema: "models",
                columns: table => new
                {
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("eventperformer_pkey", x => x.fk_eventid_event);
                });

            migrationBuilder.CreateTable(
                name: "feedback_type",
                schema: "models",
                columns: table => new
                {
                    id_feedback_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_type_pkey", x => x.id_feedback_type);
                });

            migrationBuilder.CreateTable(
                name: "loyalty",
                schema: "models",
                columns: table => new
                {
                    id_loyalty = table.Column<int>(type: "integer", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: true),
                    date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("loyalty_pkey", x => x.id_loyalty);
                });

            migrationBuilder.CreateTable(
                name: "organiser",
                schema: "models",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    follower_count = table.Column<int>(type: "integer", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("organiser_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "partner",
                schema: "models",
                columns: table => new
                {
                    id_partner = table.Column<int>(type: "integer", nullable: false),
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
                schema: "models",
                columns: table => new
                {
                    id_performer = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    profession = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("performer_pkey", x => x.id_performer);
                });

            migrationBuilder.CreateTable(
                name: "ticket_type",
                schema: "models",
                columns: table => new
                {
                    id_ticket_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_type_pkey", x => x.id_ticket_type);
                });

            migrationBuilder.CreateTable(
                name: "ticketstatus",
                schema: "models",
                columns: table => new
                {
                    id_status = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticketstatus_pkey", x => x.id_status);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "models",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    surname = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_pkey", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "user_type",
                schema: "models",
                columns: table => new
                {
                    id_user_type = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_type_pkey", x => x.id_user_type);
                });

            migrationBuilder.CreateTable(
                name: "question",
                schema: "models",
                columns: table => new
                {
                    id_question = table.Column<int>(type: "integer", nullable: false),
                    formulated_question = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fk_administratorid_user = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("question_pkey", x => x.id_question);
                    table.ForeignKey(
                        name: "question_fk_administratorid_user_fkey",
                        column: x => x.fk_administratorid_user,
                        principalSchema: "models",
                        principalTable: "administrator",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "event_location",
                schema: "models",
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
                    holding_equipment = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("event_location_pkey", x => x.id_event_location);
                    table.ForeignKey(
                        name: "event_location_holding_equipment_fkey",
                        column: x => x.holding_equipment,
                        principalSchema: "models",
                        principalTable: "equipment",
                        principalColumn: "id_equipment");
                });

            migrationBuilder.CreateTable(
                name: "administrator_loyalty",
                schema: "models",
                columns: table => new
                {
                    fk_administratorid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_loyaltyid_loyalty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrator_loyalty_pkey", x => new { x.fk_administratorid_user, x.fk_loyaltyid_loyalty });
                    table.ForeignKey(
                        name: "administrator_loyalty_fk_administratorid_user_fkey",
                        column: x => x.fk_administratorid_user,
                        principalSchema: "models",
                        principalTable: "administrator",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "administrator_loyalty_fk_loyaltyid_loyalty_fkey",
                        column: x => x.fk_loyaltyid_loyalty,
                        principalSchema: "models",
                        principalTable: "loyalty",
                        principalColumn: "id_loyalty");
                });

            migrationBuilder.CreateTable(
                name: "organiser_loyalty",
                schema: "models",
                columns: table => new
                {
                    fk_organiserid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_loyaltyid_loyalty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("organiser_loyalty_pkey", x => new { x.fk_organiserid_user, x.fk_loyaltyid_loyalty });
                    table.ForeignKey(
                        name: "organiser_loyalty_fk_loyaltyid_loyalty_fkey",
                        column: x => x.fk_loyaltyid_loyalty,
                        principalSchema: "models",
                        principalTable: "loyalty",
                        principalColumn: "id_loyalty");
                    table.ForeignKey(
                        name: "organiser_loyalty_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalSchema: "models",
                        principalTable: "organiser",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "user_loyalty",
                schema: "models",
                columns: table => new
                {
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_loyaltyid_loyalty = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_loyalty_pkey", x => new { x.fk_userid_user, x.fk_loyaltyid_loyalty });
                    table.ForeignKey(
                        name: "user_loyalty_fk_loyaltyid_loyalty_fkey",
                        column: x => x.fk_loyaltyid_loyalty,
                        principalSchema: "models",
                        principalTable: "loyalty",
                        principalColumn: "id_loyalty");
                    table.ForeignKey(
                        name: "user_loyalty_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalSchema: "models",
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "user_request_answer",
                schema: "models",
                columns: table => new
                {
                    id_user_request_answer = table.Column<int>(type: "integer", nullable: false),
                    answer = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fk_questionid_question = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_request_answer_pkey", x => x.id_user_request_answer);
                    table.ForeignKey(
                        name: "user_request_answer_fk_questionid_question_fkey",
                        column: x => x.fk_questionid_question,
                        principalSchema: "models",
                        principalTable: "question",
                        principalColumn: "id_question");
                });

            migrationBuilder.CreateTable(
                name: "event",
                schema: "models",
                columns: table => new
                {
                    id_event = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
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
                        principalSchema: "models",
                        principalTable: "category",
                        principalColumn: "id_category");
                    table.ForeignKey(
                        name: "event_fk_event_locationid_event_location_fkey",
                        column: x => x.fk_event_locationid_event_location,
                        principalSchema: "models",
                        principalTable: "event_location",
                        principalColumn: "id_event_location");
                    table.ForeignKey(
                        name: "event_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalSchema: "models",
                        principalTable: "organiser",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "sector",
                schema: "models",
                columns: table => new
                {
                    id_sector = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    fk_event_locationid_event_location = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sector_pkey", x => x.id_sector);
                    table.ForeignKey(
                        name: "sector_fk_event_locationid_event_location_fkey",
                        column: x => x.fk_event_locationid_event_location,
                        principalSchema: "models",
                        principalTable: "event_location",
                        principalColumn: "id_event_location");
                });

            migrationBuilder.CreateTable(
                name: "user_request_answer_administrator",
                schema: "models",
                columns: table => new
                {
                    fk_user_request_answerid_user_request_answer = table.Column<int>(type: "integer", nullable: false),
                    fk_administratorid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_request_answer_administrator_pkey", x => new { x.fk_user_request_answerid_user_request_answer, x.fk_administratorid_user });
                    table.ForeignKey(
                        name: "user_request_answer_administr_fk_user_request_answerid_use_fkey",
                        column: x => x.fk_user_request_answerid_user_request_answer,
                        principalSchema: "models",
                        principalTable: "user_request_answer",
                        principalColumn: "id_user_request_answer");
                    table.ForeignKey(
                        name: "user_request_answer_administrator_fk_administratorid_user_fkey",
                        column: x => x.fk_administratorid_user,
                        principalSchema: "models",
                        principalTable: "administrator",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "user_request_answer_organiser",
                schema: "models",
                columns: table => new
                {
                    fk_user_request_answerid_user_request_answer = table.Column<int>(type: "integer", nullable: false),
                    fk_organiserid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_request_answer_organiser_pkey", x => new { x.fk_user_request_answerid_user_request_answer, x.fk_organiserid_user });
                    table.ForeignKey(
                        name: "user_request_answer_organiser_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalSchema: "models",
                        principalTable: "organiser",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "user_request_answer_organiser_fk_user_request_answerid_use_fkey",
                        column: x => x.fk_user_request_answerid_user_request_answer,
                        principalSchema: "models",
                        principalTable: "user_request_answer",
                        principalColumn: "id_user_request_answer");
                });

            migrationBuilder.CreateTable(
                name: "user_request_answer_user",
                schema: "models",
                columns: table => new
                {
                    fk_user_request_answerid_user_request_answer = table.Column<int>(type: "integer", nullable: false),
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_request_answer_user_pkey", x => new { x.fk_user_request_answerid_user_request_answer, x.fk_userid_user });
                    table.ForeignKey(
                        name: "user_request_answer_user_fk_user_request_answerid_user_req_fkey",
                        column: x => x.fk_user_request_answerid_user_request_answer,
                        principalSchema: "models",
                        principalTable: "user_request_answer",
                        principalColumn: "id_user_request_answer");
                    table.ForeignKey(
                        name: "user_request_answer_user_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalSchema: "models",
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                schema: "models",
                columns: table => new
                {
                    id_feedback = table.Column<int>(type: "integer", nullable: false),
                    comment = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    score = table.Column<double>(type: "double precision", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("feedback_pkey", x => x.id_feedback);
                    table.ForeignKey(
                        name: "feedback_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalSchema: "models",
                        principalTable: "event",
                        principalColumn: "id_event");
                    table.ForeignKey(
                        name: "feedback_type_fkey",
                        column: x => x.type,
                        principalSchema: "models",
                        principalTable: "feedback_type",
                        principalColumn: "id_feedback_type");
                });

            migrationBuilder.CreateTable(
                name: "seating",
                schema: "models",
                columns: table => new
                {
                    id_seating = table.Column<int>(type: "integer", nullable: false),
                    row = table.Column<int>(type: "integer", nullable: true),
                    place = table.Column<int>(type: "integer", nullable: true),
                    fk_sectorid_sector = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("seating_pkey", x => x.id_seating);
                    table.ForeignKey(
                        name: "seating_fk_sectorid_sector_fkey",
                        column: x => x.fk_sectorid_sector,
                        principalSchema: "models",
                        principalTable: "sector",
                        principalColumn: "id_sector");
                });

            migrationBuilder.CreateTable(
                name: "sector_price",
                schema: "models",
                columns: table => new
                {
                    id_sector_price = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: true),
                    fk_sectorid_sector = table.Column<int>(type: "integer", nullable: false),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sector_price_pkey", x => x.id_sector_price);
                    table.ForeignKey(
                        name: "sector_price_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalSchema: "models",
                        principalTable: "event",
                        principalColumn: "id_event");
                    table.ForeignKey(
                        name: "sector_price_fk_sectorid_sector_fkey",
                        column: x => x.fk_sectorid_sector,
                        principalSchema: "models",
                        principalTable: "sector",
                        principalColumn: "id_sector");
                });

            migrationBuilder.CreateTable(
                name: "administrator_feedback",
                schema: "models",
                columns: table => new
                {
                    fk_administratorid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_feedbackid_feedback = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrator_feedback_pkey", x => new { x.fk_administratorid_user, x.fk_feedbackid_feedback });
                    table.ForeignKey(
                        name: "administrator_feedback_fk_administratorid_user_fkey",
                        column: x => x.fk_administratorid_user,
                        principalSchema: "models",
                        principalTable: "administrator",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "administrator_feedback_fk_feedbackid_feedback_fkey",
                        column: x => x.fk_feedbackid_feedback,
                        principalSchema: "models",
                        principalTable: "feedback",
                        principalColumn: "id_feedback");
                });

            migrationBuilder.CreateTable(
                name: "organiser_feedback",
                schema: "models",
                columns: table => new
                {
                    fk_organiserid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_feedbackid_feedback = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("organiser_feedback_pkey", x => new { x.fk_organiserid_user, x.fk_feedbackid_feedback });
                    table.ForeignKey(
                        name: "organiser_feedback_fk_feedbackid_feedback_fkey",
                        column: x => x.fk_feedbackid_feedback,
                        principalSchema: "models",
                        principalTable: "feedback",
                        principalColumn: "id_feedback");
                    table.ForeignKey(
                        name: "organiser_feedback_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalSchema: "models",
                        principalTable: "organiser",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "user_feedback",
                schema: "models",
                columns: table => new
                {
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_feedbackid_feedback = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_feedback_pkey", x => new { x.fk_userid_user, x.fk_feedbackid_feedback });
                    table.ForeignKey(
                        name: "user_feedback_fk_feedbackid_feedback_fkey",
                        column: x => x.fk_feedbackid_feedback,
                        principalSchema: "models",
                        principalTable: "feedback",
                        principalColumn: "id_feedback");
                    table.ForeignKey(
                        name: "user_feedback_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalSchema: "models",
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.CreateTable(
                name: "ticket",
                schema: "models",
                columns: table => new
                {
                    id_ticket = table.Column<int>(type: "integer", nullable: false),
                    price = table.Column<double>(type: "double precision", nullable: true),
                    generation_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    scanned_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    qr_code = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    type = table.Column<int>(type: "integer", nullable: true),
                    fk_eventid_event = table.Column<int>(type: "integer", nullable: false),
                    fk_seatingid_seating = table.Column<int>(type: "integer", nullable: false),
                    fk_ticketstatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ticket_pkey", x => x.id_ticket);
                    table.ForeignKey(
                        name: "ticket_fk_eventid_event_fkey",
                        column: x => x.fk_eventid_event,
                        principalSchema: "models",
                        principalTable: "event",
                        principalColumn: "id_event");
                    table.ForeignKey(
                        name: "ticket_fk_seatingid_seating_fkey",
                        column: x => x.fk_seatingid_seating,
                        principalSchema: "models",
                        principalTable: "seating",
                        principalColumn: "id_seating");
                    table.ForeignKey(
                        name: "ticket_fk_ticketstatus_fkey",
                        column: x => x.fk_ticketstatus,
                        principalSchema: "models",
                        principalTable: "ticketstatus",
                        principalColumn: "id_status");
                    table.ForeignKey(
                        name: "ticket_type_fkey",
                        column: x => x.type,
                        principalSchema: "models",
                        principalTable: "ticket_type",
                        principalColumn: "id_ticket_type");
                });

            migrationBuilder.CreateTable(
                name: "administrator_ticket",
                schema: "models",
                columns: table => new
                {
                    fk_administratorid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_ticketid_ticket = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("administrator_ticket_pkey", x => new { x.fk_administratorid_user, x.fk_ticketid_ticket });
                    table.ForeignKey(
                        name: "administrator_ticket_fk_administratorid_user_fkey",
                        column: x => x.fk_administratorid_user,
                        principalSchema: "models",
                        principalTable: "administrator",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "administrator_ticket_fk_ticketid_ticket_fkey",
                        column: x => x.fk_ticketid_ticket,
                        principalSchema: "models",
                        principalTable: "ticket",
                        principalColumn: "id_ticket");
                });

            migrationBuilder.CreateTable(
                name: "organiser_ticket",
                schema: "models",
                columns: table => new
                {
                    fk_organiserid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_ticketid_ticket = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("organiser_ticket_pkey", x => new { x.fk_organiserid_user, x.fk_ticketid_ticket });
                    table.ForeignKey(
                        name: "organiser_ticket_fk_organiserid_user_fkey",
                        column: x => x.fk_organiserid_user,
                        principalSchema: "models",
                        principalTable: "organiser",
                        principalColumn: "id_user");
                    table.ForeignKey(
                        name: "organiser_ticket_fk_ticketid_ticket_fkey",
                        column: x => x.fk_ticketid_ticket,
                        principalSchema: "models",
                        principalTable: "ticket",
                        principalColumn: "id_ticket");
                });

            migrationBuilder.CreateTable(
                name: "user_ticket",
                schema: "models",
                columns: table => new
                {
                    fk_userid_user = table.Column<int>(type: "integer", nullable: false),
                    fk_ticketid_ticket = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_ticket_pkey", x => new { x.fk_userid_user, x.fk_ticketid_ticket });
                    table.ForeignKey(
                        name: "user_ticket_fk_ticketid_ticket_fkey",
                        column: x => x.fk_ticketid_ticket,
                        principalSchema: "models",
                        principalTable: "ticket",
                        principalColumn: "id_ticket");
                    table.ForeignKey(
                        name: "user_ticket_fk_userid_user_fkey",
                        column: x => x.fk_userid_user,
                        principalSchema: "models",
                        principalTable: "user",
                        principalColumn: "id_user");
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "category",
                columns: new[] { "id_category", "name" },
                values: new object[,]
                {
                    { 1, "Music" },
                    { 2, "Theatre" },
                    { 3, "Opera" },
                    { 4, "Exposition" },
                    { 5, "Fashion show" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "equipment",
                columns: new[] { "id_equipment", "name" },
                values: new object[,]
                {
                    { 1, "Sound System" },
                    { 2, "TV" },
                    { 3, "Microphone" },
                    { 4, "Projector" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "feedback_type",
                columns: new[] { "id_feedback_type", "name" },
                values: new object[,]
                {
                    { 1, "Positive" },
                    { 2, "Negative" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "loyalty",
                columns: new[] { "id_loyalty", "points" },
                values: new object[] { 1, 0 });

            migrationBuilder.InsertData(
                schema: "models",
                table: "organiser",
                columns: new[] { "id_user", "follower_count", "name", "password", "rating", "surname", "username" },
                values: new object[] { 1, 0, null, null, 5.0, null, null });

            migrationBuilder.InsertData(
                schema: "models",
                table: "ticket_type",
                columns: new[] { "id_ticket_type", "name" },
                values: new object[,]
                {
                    { 1, "Standard" },
                    { 2, "VIP" },
                    { 3, "Super-VIP" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "ticketstatus",
                columns: new[] { "id_status", "name" },
                values: new object[,]
                {
                    { 1, "Active" },
                    { 2, "Inactive" },
                    { 3, "Scanned" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "user",
                columns: new[] { "id_user", "name", "password", "surname", "username" },
                values: new object[] { 1, "Event", "password123", "Organizer", "organizer" });

            migrationBuilder.InsertData(
                schema: "models",
                table: "user_type",
                columns: new[] { "id_user_type", "name" },
                values: new object[,]
                {
                    { 1, "Regular" },
                    { 2, "Organizer" },
                    { 3, "Admin" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "event_location",
                columns: new[] { "id_event_location", "address", "capacity", "city", "contacts", "country", "holding_equipment", "name", "price" },
                values: new object[] { 1, "123 Main St", 500, "Boston", "contact@venue.com", "USA", 2, "Conference Center", 1000.0 });

            migrationBuilder.InsertData(
                schema: "models",
                table: "event",
                columns: new[] { "id_event", "category", "description", "end_date", "fk_event_locationid_event_location", "fk_organiserid_user", "max_ticket_count", "name", "start_date" },
                values: new object[] { 1, 2, "Annual technology conference", new DateOnly(2025, 6, 17), 1, 1, 500, "Tech Conference 2025", new DateOnly(2025, 6, 15) });

            migrationBuilder.InsertData(
                schema: "models",
                table: "sector",
                columns: new[] { "id_sector", "fk_event_locationid_event_location", "name" },
                values: new object[,]
                {
                    { 1, 1, "Main Hall" },
                    { 2, 1, "VIP Section" }
                });

            migrationBuilder.InsertData(
                schema: "models",
                table: "sector_price",
                columns: new[] { "id_sector_price", "fk_eventid_event", "fk_sectorid_sector", "price" },
                values: new object[,]
                {
                    { 1, 1, 1, 50.0 },
                    { 2, 1, 2, 150.0 }
                });

            migrationBuilder.CreateIndex(
                name: "administrator_feedback_fk_feedbackid_feedback_key",
                schema: "models",
                table: "administrator_feedback",
                column: "fk_feedbackid_feedback",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "administrator_loyalty_fk_administratorid_user_key",
                schema: "models",
                table: "administrator_loyalty",
                column: "fk_administratorid_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "administrator_loyalty_fk_loyaltyid_loyalty_key",
                schema: "models",
                table: "administrator_loyalty",
                column: "fk_loyaltyid_loyalty",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "administrator_ticket_fk_ticketid_ticket_key",
                schema: "models",
                table: "administrator_ticket",
                column: "fk_ticketid_ticket",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "event_fk_event_locationid_event_location_key",
                schema: "models",
                table: "event",
                column: "fk_event_locationid_event_location",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_category",
                schema: "models",
                table: "event",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_event_fk_organiserid_user",
                schema: "models",
                table: "event",
                column: "fk_organiserid_user");

            migrationBuilder.CreateIndex(
                name: "IX_event_location_holding_equipment",
                schema: "models",
                table: "event_location",
                column: "holding_equipment");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_fk_eventid_event",
                schema: "models",
                table: "feedback",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_type",
                schema: "models",
                table: "feedback",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "organiser_feedback_fk_feedbackid_feedback_key",
                schema: "models",
                table: "organiser_feedback",
                column: "fk_feedbackid_feedback",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "organiser_loyalty_fk_loyaltyid_loyalty_key",
                schema: "models",
                table: "organiser_loyalty",
                column: "fk_loyaltyid_loyalty",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "organiser_loyalty_fk_organiserid_user_key",
                schema: "models",
                table: "organiser_loyalty",
                column: "fk_organiserid_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "organiser_ticket_fk_ticketid_ticket_key",
                schema: "models",
                table: "organiser_ticket",
                column: "fk_ticketid_ticket",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_question_fk_administratorid_user",
                schema: "models",
                table: "question",
                column: "fk_administratorid_user");

            migrationBuilder.CreateIndex(
                name: "IX_seating_fk_sectorid_sector",
                schema: "models",
                table: "seating",
                column: "fk_sectorid_sector");

            migrationBuilder.CreateIndex(
                name: "IX_sector_fk_event_locationid_event_location",
                schema: "models",
                table: "sector",
                column: "fk_event_locationid_event_location");

            migrationBuilder.CreateIndex(
                name: "IX_sector_price_fk_eventid_event",
                schema: "models",
                table: "sector_price",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_sector_price_fk_sectorid_sector",
                schema: "models",
                table: "sector_price",
                column: "fk_sectorid_sector");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_fk_eventid_event",
                schema: "models",
                table: "ticket",
                column: "fk_eventid_event");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_fk_ticketstatus",
                schema: "models",
                table: "ticket",
                column: "fk_ticketstatus");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_type",
                schema: "models",
                table: "ticket",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ticket_fk_seatingid_seating_key",
                schema: "models",
                table: "ticket",
                column: "fk_seatingid_seating",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_feedback_fk_feedbackid_feedback_key",
                schema: "models",
                table: "user_feedback",
                column: "fk_feedbackid_feedback",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_loyalty_fk_loyaltyid_loyalty_key",
                schema: "models",
                table: "user_loyalty",
                column: "fk_loyaltyid_loyalty",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_loyalty_fk_userid_user_key",
                schema: "models",
                table: "user_loyalty",
                column: "fk_userid_user",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_request_answer_fk_questionid_question",
                schema: "models",
                table: "user_request_answer",
                column: "fk_questionid_question");

            migrationBuilder.CreateIndex(
                name: "IX_user_request_answer_administrator_fk_administratorid_user",
                schema: "models",
                table: "user_request_answer_administrator",
                column: "fk_administratorid_user");

            migrationBuilder.CreateIndex(
                name: "user_request_answer_administr_fk_user_request_answerid_user_key",
                schema: "models",
                table: "user_request_answer_administrator",
                column: "fk_user_request_answerid_user_request_answer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_request_answer_organiser_fk_organiserid_user",
                schema: "models",
                table: "user_request_answer_organiser",
                column: "fk_organiserid_user");

            migrationBuilder.CreateIndex(
                name: "user_request_answer_organiser_fk_user_request_answerid_user_key",
                schema: "models",
                table: "user_request_answer_organiser",
                column: "fk_user_request_answerid_user_request_answer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_request_answer_user_fk_userid_user",
                schema: "models",
                table: "user_request_answer_user",
                column: "fk_userid_user");

            migrationBuilder.CreateIndex(
                name: "user_request_answer_user_fk_user_request_answerid_user_requ_key",
                schema: "models",
                table: "user_request_answer_user",
                column: "fk_user_request_answerid_user_request_answer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "user_ticket_fk_ticketid_ticket_key",
                schema: "models",
                table: "user_ticket",
                column: "fk_ticketid_ticket",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrator_feedback",
                schema: "models");

            migrationBuilder.DropTable(
                name: "administrator_loyalty",
                schema: "models");

            migrationBuilder.DropTable(
                name: "administrator_ticket",
                schema: "models");

            migrationBuilder.DropTable(
                name: "eventpartner",
                schema: "models");

            migrationBuilder.DropTable(
                name: "eventperformer",
                schema: "models");

            migrationBuilder.DropTable(
                name: "organiser_feedback",
                schema: "models");

            migrationBuilder.DropTable(
                name: "organiser_loyalty",
                schema: "models");

            migrationBuilder.DropTable(
                name: "organiser_ticket",
                schema: "models");

            migrationBuilder.DropTable(
                name: "partner",
                schema: "models");

            migrationBuilder.DropTable(
                name: "performer",
                schema: "models");

            migrationBuilder.DropTable(
                name: "sector_price",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_feedback",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_loyalty",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_request_answer_administrator",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_request_answer_organiser",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_request_answer_user",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_ticket",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_type",
                schema: "models");

            migrationBuilder.DropTable(
                name: "feedback",
                schema: "models");

            migrationBuilder.DropTable(
                name: "loyalty",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user_request_answer",
                schema: "models");

            migrationBuilder.DropTable(
                name: "ticket",
                schema: "models");

            migrationBuilder.DropTable(
                name: "user",
                schema: "models");

            migrationBuilder.DropTable(
                name: "feedback_type",
                schema: "models");

            migrationBuilder.DropTable(
                name: "question",
                schema: "models");

            migrationBuilder.DropTable(
                name: "event",
                schema: "models");

            migrationBuilder.DropTable(
                name: "seating",
                schema: "models");

            migrationBuilder.DropTable(
                name: "ticketstatus",
                schema: "models");

            migrationBuilder.DropTable(
                name: "ticket_type",
                schema: "models");

            migrationBuilder.DropTable(
                name: "administrator",
                schema: "models");

            migrationBuilder.DropTable(
                name: "category",
                schema: "models");

            migrationBuilder.DropTable(
                name: "organiser",
                schema: "models");

            migrationBuilder.DropTable(
                name: "sector",
                schema: "models");

            migrationBuilder.DropTable(
                name: "event_location",
                schema: "models");

            migrationBuilder.DropTable(
                name: "equipment",
                schema: "models");
        }
    }
}
