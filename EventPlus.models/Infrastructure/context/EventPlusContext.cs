using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eventplus.models.Domain.Events;
using eventplus.models.Domain.Tickets;
using eventplus.models.Domain.Users;
using eventplus.models.Domain.Feedbacks;
using eventplus.models.Domain.UserLoyalties;
using eventplus.models.Domain.UserAnswers;
using eventplus.models.Domain.Sectors;

namespace eventplus.models.Infrastructure.context;

public partial class EventPlusContext : DbContext
{
    public EventPlusContext()
    {
    }

    public EventPlusContext(DbContextOptions<EventPlusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<AdministratorFeedback> AdministratorFeedbacks { get; set; }

    public virtual DbSet<AdministratorLoyalty> AdministratorLoyalties { get; set; }

    public virtual DbSet<AdministratorTicket> AdministratorTickets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventLocation> EventLocations { get; set; }

    public virtual DbSet<Eventpartner> Eventpartners { get; set; }

    public virtual DbSet<Eventperformer> Eventperformers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackType> FeedbackTypes { get; set; }

    public virtual DbSet<Loyalty> Loyalties { get; set; }

    public virtual DbSet<Organiser> Organisers { get; set; }

    public virtual DbSet<OrganiserFeedback> OrganiserFeedbacks { get; set; }

    public virtual DbSet<OrganiserLoyalty> OrganiserLoyalties { get; set; }

    public virtual DbSet<OrganiserTicket> OrganiserTickets { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Performer> Performers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Seating> Seatings { get; set; }

    public virtual DbSet<Sector> Sectors { get; set; }

    public virtual DbSet<SectorPrice> SectorPrices { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<Ticketstatus> Ticketstatuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }

    public virtual DbSet<UserLoyalty> UserLoyalties { get; set; }

    public virtual DbSet<UserRequestAnswer> UserRequestAnswers { get; set; }

    public virtual DbSet<UserRequestAnswerAdministrator> UserRequestAnswerAdministrators { get; set; }

    public virtual DbSet<UserRequestAnswerOrganiser> UserRequestAnswerOrganisers { get; set; }

    public virtual DbSet<UserRequestAnswerUser> UserRequestAnswerUsers { get; set; }

    public virtual DbSet<UserTicket> UserTickets { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("administrator_pkey");

            entity.ToTable("administrator", "models");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<AdministratorFeedback>(entity =>
        {
            entity.HasKey(e => new { e.FkAdministratoridUser, e.FkFeedbackidFeedback }).HasName("administrator_feedback_pkey");

            entity.ToTable("administrator_feedback", "models");

            entity.HasIndex(e => e.FkFeedbackidFeedback, "administrator_feedback_fk_feedbackid_feedback_key").IsUnique();

            entity.Property(e => e.FkAdministratoridUser).HasColumnName("fk_administratorid_user");
            entity.Property(e => e.FkFeedbackidFeedback).HasColumnName("fk_feedbackid_feedback");

            entity.HasOne(d => d.FkAdministratoridUserNavigation).WithMany(p => p.AdministratorFeedbacks)
                .HasForeignKey(d => d.FkAdministratoridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_feedback_fk_administratorid_user_fkey");

            entity.HasOne(d => d.FkFeedbackidFeedbackNavigation).WithOne(p => p.AdministratorFeedback)
                .HasForeignKey<AdministratorFeedback>(d => d.FkFeedbackidFeedback)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_feedback_fk_feedbackid_feedback_fkey");
        });

        modelBuilder.Entity<AdministratorLoyalty>(entity =>
        {
            entity.HasKey(e => new { e.FkAdministratoridUser, e.FkLoyaltyidLoyalty }).HasName("administrator_loyalty_pkey");

            entity.ToTable("administrator_loyalty", "models");

            entity.HasIndex(e => e.FkAdministratoridUser, "administrator_loyalty_fk_administratorid_user_key").IsUnique();

            entity.HasIndex(e => e.FkLoyaltyidLoyalty, "administrator_loyalty_fk_loyaltyid_loyalty_key").IsUnique();

            entity.Property(e => e.FkAdministratoridUser).HasColumnName("fk_administratorid_user");
            entity.Property(e => e.FkLoyaltyidLoyalty).HasColumnName("fk_loyaltyid_loyalty");

            entity.HasOne(d => d.FkAdministratoridUserNavigation).WithOne(p => p.AdministratorLoyalty)
                .HasForeignKey<AdministratorLoyalty>(d => d.FkAdministratoridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_loyalty_fk_administratorid_user_fkey");

            entity.HasOne(d => d.FkLoyaltyidLoyaltyNavigation).WithOne(p => p.AdministratorLoyalty)
                .HasForeignKey<AdministratorLoyalty>(d => d.FkLoyaltyidLoyalty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_loyalty_fk_loyaltyid_loyalty_fkey");
        });

        modelBuilder.Entity<AdministratorTicket>(entity =>
        {
            entity.HasKey(e => new { e.FkAdministratoridUser, e.FkTicketidTicket }).HasName("administrator_ticket_pkey");

            entity.ToTable("administrator_ticket", "models");

            entity.HasIndex(e => e.FkTicketidTicket, "administrator_ticket_fk_ticketid_ticket_key").IsUnique();

            entity.Property(e => e.FkAdministratoridUser).HasColumnName("fk_administratorid_user");
            entity.Property(e => e.FkTicketidTicket).HasColumnName("fk_ticketid_ticket");

            entity.HasOne(d => d.FkAdministratoridUserNavigation).WithMany(p => p.AdministratorTickets)
                .HasForeignKey(d => d.FkAdministratoridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_ticket_fk_administratorid_user_fkey");

            entity.HasOne(d => d.FkTicketidTicketNavigation).WithOne(p => p.AdministratorTicket)
                .HasForeignKey<AdministratorTicket>(d => d.FkTicketidTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_ticket_fk_ticketid_ticket_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("category_pkey");

            entity.ToTable("category", "models");

            entity.Property(e => e.IdCategory)
                .ValueGeneratedNever()
                .HasColumnName("id_category");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.IdEquipment).HasName("equipment_pkey");

            entity.ToTable("equipment", "models");

            entity.Property(e => e.IdEquipment)
                .ValueGeneratedNever()
                .HasColumnName("id_equipment");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("event_pkey");

            entity.ToTable("event", "models");

            entity.HasIndex(e => e.FkEventLocationidEventLocation, "event_fk_event_locationid_event_location_key").IsUnique();

            entity.Property(e => e.IdEvent)
                .ValueGeneratedNever()
                .HasColumnName("id_event");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.FkEventLocationidEventLocation).HasColumnName("fk_event_locationid_event_location");
            entity.Property(e => e.FkOrganiseridUser).HasColumnName("fk_organiserid_user");
            entity.Property(e => e.MaxTicketCount).HasColumnName("max_ticket_count");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.CategoryNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.Category)
                .HasConstraintName("event_category_fkey");

            entity.HasOne(d => d.FkEventLocationidEventLocationNavigation).WithOne(p => p.Event)
                .HasForeignKey<Event>(d => d.FkEventLocationidEventLocation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_fk_event_locationid_event_location_fkey");

            entity.HasOne(d => d.FkOrganiseridUserNavigation).WithMany(p => p.Events)
                .HasForeignKey(d => d.FkOrganiseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("event_fk_organiserid_user_fkey");
        });

        modelBuilder.Entity<EventLocation>(entity =>
        {
            entity.HasKey(e => e.IdEventLocation).HasName("event_location_pkey");

            entity.ToTable("event_location", "models");

            entity.Property(e => e.IdEventLocation)
                .ValueGeneratedNever()
                .HasColumnName("id_event_location");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Contacts)
                .HasMaxLength(255)
                .HasColumnName("contacts");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.HoldingEquipment).HasColumnName("holding_equipment");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.HoldingEquipmentNavigation).WithMany(p => p.EventLocations)
                .HasForeignKey(d => d.HoldingEquipment)
                .HasConstraintName("event_location_holding_equipment_fkey");
        });

        modelBuilder.Entity<Eventpartner>(entity =>
        {
            entity.HasKey(e => e.FkEventidEvent).HasName("eventpartner_pkey");

            entity.ToTable("eventpartner", "models");

            entity.Property(e => e.FkEventidEvent)
                .ValueGeneratedNever()
                .HasColumnName("fk_eventid_event");
        });

        modelBuilder.Entity<Eventperformer>(entity =>
        {
            entity.HasKey(e => e.FkEventidEvent).HasName("eventperformer_pkey");

            entity.ToTable("eventperformer", "models");

            entity.Property(e => e.FkEventidEvent)
                .ValueGeneratedNever()
                .HasColumnName("fk_eventid_event");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.IdFeedback).HasName("feedback_pkey");

            entity.ToTable("feedback", "models");

            entity.Property(e => e.IdFeedback)
                .ValueGeneratedNever()
                .HasColumnName("id_feedback");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_fk_eventid_event_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("feedback_type_fkey");
        });

        modelBuilder.Entity<FeedbackType>(entity =>
        {
            entity.HasKey(e => e.IdFeedbackType).HasName("feedback_type_pkey");

            entity.ToTable("feedback_type", "models");

            entity.Property(e => e.IdFeedbackType)
                .ValueGeneratedNever()
                .HasColumnName("id_feedback_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Loyalty>(entity =>
        {
            entity.HasKey(e => e.IdLoyalty).HasName("loyalty_pkey");

            entity.ToTable("loyalty", "models");

            entity.Property(e => e.IdLoyalty)
                .ValueGeneratedNever()
                .HasColumnName("id_loyalty");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Points).HasColumnName("points");
        });

        modelBuilder.Entity<Organiser>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("organiser_pkey");

            entity.ToTable("organiser", "models");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.FollowerCount).HasColumnName("follower_count");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<OrganiserFeedback>(entity =>
        {
            entity.HasKey(e => new { e.FkOrganiseridUser, e.FkFeedbackidFeedback }).HasName("organiser_feedback_pkey");

            entity.ToTable("organiser_feedback", "models");

            entity.HasIndex(e => e.FkFeedbackidFeedback, "organiser_feedback_fk_feedbackid_feedback_key").IsUnique();

            entity.Property(e => e.FkOrganiseridUser).HasColumnName("fk_organiserid_user");
            entity.Property(e => e.FkFeedbackidFeedback).HasColumnName("fk_feedbackid_feedback");

            entity.HasOne(d => d.FkFeedbackidFeedbackNavigation).WithOne(p => p.OrganiserFeedback)
                .HasForeignKey<OrganiserFeedback>(d => d.FkFeedbackidFeedback)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_feedback_fk_feedbackid_feedback_fkey");

            entity.HasOne(d => d.FkOrganiseridUserNavigation).WithMany(p => p.OrganiserFeedbacks)
                .HasForeignKey(d => d.FkOrganiseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_feedback_fk_organiserid_user_fkey");
        });

        modelBuilder.Entity<OrganiserLoyalty>(entity =>
        {
            entity.HasKey(e => new { e.FkOrganiseridUser, e.FkLoyaltyidLoyalty }).HasName("organiser_loyalty_pkey");

            entity.ToTable("organiser_loyalty", "models");

            entity.HasIndex(e => e.FkLoyaltyidLoyalty, "organiser_loyalty_fk_loyaltyid_loyalty_key").IsUnique();

            entity.HasIndex(e => e.FkOrganiseridUser, "organiser_loyalty_fk_organiserid_user_key").IsUnique();

            entity.Property(e => e.FkOrganiseridUser).HasColumnName("fk_organiserid_user");
            entity.Property(e => e.FkLoyaltyidLoyalty).HasColumnName("fk_loyaltyid_loyalty");

            entity.HasOne(d => d.FkLoyaltyidLoyaltyNavigation).WithOne(p => p.OrganiserLoyalty)
                .HasForeignKey<OrganiserLoyalty>(d => d.FkLoyaltyidLoyalty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_loyalty_fk_loyaltyid_loyalty_fkey");

            entity.HasOne(d => d.FkOrganiseridUserNavigation).WithOne(p => p.OrganiserLoyalty)
                .HasForeignKey<OrganiserLoyalty>(d => d.FkOrganiseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_loyalty_fk_organiserid_user_fkey");
        });

        modelBuilder.Entity<OrganiserTicket>(entity =>
        {
            entity.HasKey(e => new { e.FkOrganiseridUser, e.FkTicketidTicket }).HasName("organiser_ticket_pkey");

            entity.ToTable("organiser_ticket", "models");

            entity.HasIndex(e => e.FkTicketidTicket, "organiser_ticket_fk_ticketid_ticket_key").IsUnique();

            entity.Property(e => e.FkOrganiseridUser).HasColumnName("fk_organiserid_user");
            entity.Property(e => e.FkTicketidTicket).HasColumnName("fk_ticketid_ticket");

            entity.HasOne(d => d.FkOrganiseridUserNavigation).WithMany(p => p.OrganiserTickets)
                .HasForeignKey(d => d.FkOrganiseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_ticket_fk_organiserid_user_fkey");

            entity.HasOne(d => d.FkTicketidTicketNavigation).WithOne(p => p.OrganiserTicket)
                .HasForeignKey<OrganiserTicket>(d => d.FkTicketidTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_ticket_fk_ticketid_ticket_fkey");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.IdPartner).HasName("partner_pkey");

            entity.ToTable("partner", "models");

            entity.Property(e => e.IdPartner)
                .ValueGeneratedNever()
                .HasColumnName("id_partner");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<Performer>(entity =>
        {
            entity.HasKey(e => e.IdPerformer).HasName("performer_pkey");

            entity.ToTable("performer", "models");

            entity.Property(e => e.IdPerformer)
                .ValueGeneratedNever()
                .HasColumnName("id_performer");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Profession)
                .HasMaxLength(255)
                .HasColumnName("profession");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.IdQuestion).HasName("question_pkey");

            entity.ToTable("question", "models");

            entity.Property(e => e.IdQuestion)
                .ValueGeneratedNever()
                .HasColumnName("id_question");
            entity.Property(e => e.FkAdministratoridUser).HasColumnName("fk_administratorid_user");
            entity.Property(e => e.FormulatedQuestion)
                .HasMaxLength(255)
                .HasColumnName("formulated_question");

            entity.HasOne(d => d.FkAdministratoridUserNavigation).WithMany(p => p.Questions)
                .HasForeignKey(d => d.FkAdministratoridUser)
                .HasConstraintName("question_fk_administratorid_user_fkey");
        });

        modelBuilder.Entity<Seating>(entity =>
        {
            entity.HasKey(e => e.IdSeating).HasName("seating_pkey");

            entity.ToTable("seating", "models");

            entity.Property(e => e.IdSeating)
                .ValueGeneratedNever()
                .HasColumnName("id_seating");
            entity.Property(e => e.FkSectoridSector).HasColumnName("fk_sectorid_sector");
            entity.Property(e => e.Place).HasColumnName("place");
            entity.Property(e => e.Row).HasColumnName("row");

            entity.HasOne(d => d.FkSectoridSectorNavigation).WithMany(p => p.Seatings)
                .HasForeignKey(d => d.FkSectoridSector)
                .HasConstraintName("seating_fk_sectorid_sector_fkey");
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.HasKey(e => e.IdSector).HasName("sector_pkey");

            entity.ToTable("sector", "models");

            entity.Property(e => e.IdSector)
                .ValueGeneratedNever()
                .HasColumnName("id_sector");
            entity.Property(e => e.FkEventLocationidEventLocation).HasColumnName("fk_event_locationid_event_location");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.FkEventLocationidEventLocationNavigation).WithMany(p => p.Sectors)
                .HasForeignKey(d => d.FkEventLocationidEventLocation)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_fk_event_locationid_event_location_fkey");
        });

        modelBuilder.Entity<SectorPrice>(entity =>
        {
            entity.HasKey(e => e.IdSectorPrice).HasName("sector_price_pkey");

            entity.ToTable("sector_price", "models");

            entity.Property(e => e.IdSectorPrice)
                .ValueGeneratedNever()
                .HasColumnName("id_sector_price");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.FkSectoridSector).HasColumnName("fk_sectorid_sector");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.SectorPrices)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_price_fk_eventid_event_fkey");

            entity.HasOne(d => d.FkSectoridSectorNavigation).WithMany(p => p.SectorPrices)
                .HasForeignKey(d => d.FkSectoridSector)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_price_fk_sectorid_sector_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("ticket_pkey");

            entity.ToTable("ticket", "models");

            entity.HasIndex(e => e.FkSeatingidSeating, "ticket_fk_seatingid_seating_key").IsUnique();

            entity.Property(e => e.IdTicket)
                .ValueGeneratedNever()
                .HasColumnName("id_ticket");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.FkSeatingidSeating).HasColumnName("fk_seatingid_seating");
            entity.Property(e => e.FkTicketstatus).HasColumnName("fk_ticketstatus");
            entity.Property(e => e.GenerationDate).HasColumnName("generation_date");
            entity.Property(e => e.ScannedDate).HasColumnName("scanned_date");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.QrCode)
                .HasMaxLength(255)
                .HasColumnName("qr_code");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_fk_eventid_event_fkey");

            entity.HasOne(d => d.FkSeatingidSeatingNavigation).WithOne(p => p.Ticket)
                .HasForeignKey<Ticket>(d => d.FkSeatingidSeating)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_fk_seatingid_seating_fkey");

            entity.HasOne(d => d.FkTicketstatusNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FkTicketstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_fk_ticketstatus_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("ticket_type_fkey");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.IdTicketType).HasName("ticket_type_pkey");

            entity.ToTable("ticket_type", "models");

            entity.Property(e => e.IdTicketType)
                .ValueGeneratedNever()
                .HasColumnName("id_ticket_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ticketstatus>(entity =>
        {
            entity.HasKey(e => e.IdStatus).HasName("ticketstatus_pkey");

            entity.ToTable("ticketstatus", "models");

            entity.Property(e => e.IdStatus)
                .ValueGeneratedNever()
                .HasColumnName("id_status");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("user_pkey");

            entity.ToTable("user", "models");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserFeedback>(entity =>
        {
            entity.HasKey(e => new { e.FkUseridUser, e.FkFeedbackidFeedback }).HasName("user_feedback_pkey");

            entity.ToTable("user_feedback", "models");

            entity.HasIndex(e => e.FkFeedbackidFeedback, "user_feedback_fk_feedbackid_feedback_key").IsUnique();

            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.FkFeedbackidFeedback).HasColumnName("fk_feedbackid_feedback");

            entity.HasOne(d => d.FkFeedbackidFeedbackNavigation).WithOne(p => p.UserFeedback)
                .HasForeignKey<UserFeedback>(d => d.FkFeedbackidFeedback)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_feedback_fk_feedbackid_feedback_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.UserFeedbacks)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_feedback_fk_userid_user_fkey");
        });

        modelBuilder.Entity<UserLoyalty>(entity =>
        {
            entity.HasKey(e => new { e.FkUseridUser, e.FkLoyaltyidLoyalty }).HasName("user_loyalty_pkey");

            entity.ToTable("user_loyalty", "models");

            entity.HasIndex(e => e.FkLoyaltyidLoyalty, "user_loyalty_fk_loyaltyid_loyalty_key").IsUnique();

            entity.HasIndex(e => e.FkUseridUser, "user_loyalty_fk_userid_user_key").IsUnique();

            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.FkLoyaltyidLoyalty).HasColumnName("fk_loyaltyid_loyalty");

            entity.HasOne(d => d.FkLoyaltyidLoyaltyNavigation).WithOne(p => p.UserLoyalty)
                .HasForeignKey<UserLoyalty>(d => d.FkLoyaltyidLoyalty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_loyalty_fk_loyaltyid_loyalty_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithOne(p => p.UserLoyalty)
                .HasForeignKey<UserLoyalty>(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_loyalty_fk_userid_user_fkey");
        });

        modelBuilder.Entity<UserRequestAnswer>(entity =>
        {
            entity.HasKey(e => e.IdUserRequestAnswer).HasName("user_request_answer_pkey");

            entity.ToTable("user_request_answer", "models");

            entity.Property(e => e.IdUserRequestAnswer)
                .ValueGeneratedNever()
                .HasColumnName("id_user_request_answer");
            entity.Property(e => e.Answer)
                .HasMaxLength(255)
                .HasColumnName("answer");
            entity.Property(e => e.FkQuestionidQuestion).HasColumnName("fk_questionid_question");

            entity.HasOne(d => d.FkQuestionidQuestionNavigation).WithMany(p => p.UserRequestAnswers)
                .HasForeignKey(d => d.FkQuestionidQuestion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_fk_questionid_question_fkey");
        });

        modelBuilder.Entity<UserRequestAnswerAdministrator>(entity =>
        {
            entity.HasKey(e => new { e.FkUserRequestAnsweridUserRequestAnswer, e.FkAdministratoridUser }).HasName("user_request_answer_administrator_pkey");

            entity.ToTable("user_request_answer_administrator", "models");

            entity.HasIndex(e => e.FkUserRequestAnsweridUserRequestAnswer, "user_request_answer_administr_fk_user_request_answerid_user_key").IsUnique();

            entity.Property(e => e.FkUserRequestAnsweridUserRequestAnswer).HasColumnName("fk_user_request_answerid_user_request_answer");
            entity.Property(e => e.FkAdministratoridUser).HasColumnName("fk_administratorid_user");

            entity.HasOne(d => d.FkAdministratoridUserNavigation).WithMany(p => p.UserRequestAnswerAdministrators)
                .HasForeignKey(d => d.FkAdministratoridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_administrator_fk_administratorid_user_fkey");

            entity.HasOne(d => d.FkUserRequestAnsweridUserRequestAnswerNavigation).WithOne(p => p.UserRequestAnswerAdministrator)
                .HasForeignKey<UserRequestAnswerAdministrator>(d => d.FkUserRequestAnsweridUserRequestAnswer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_administr_fk_user_request_answerid_use_fkey");
        });

        modelBuilder.Entity<UserRequestAnswerOrganiser>(entity =>
        {
            entity.HasKey(e => new { e.FkUserRequestAnsweridUserRequestAnswer, e.FkOrganiseridUser }).HasName("user_request_answer_organiser_pkey");

            entity.ToTable("user_request_answer_organiser", "models");

            entity.HasIndex(e => e.FkUserRequestAnsweridUserRequestAnswer, "user_request_answer_organiser_fk_user_request_answerid_user_key").IsUnique();

            entity.Property(e => e.FkUserRequestAnsweridUserRequestAnswer).HasColumnName("fk_user_request_answerid_user_request_answer");
            entity.Property(e => e.FkOrganiseridUser).HasColumnName("fk_organiserid_user");

            entity.HasOne(d => d.FkOrganiseridUserNavigation).WithMany(p => p.UserRequestAnswerOrganisers)
                .HasForeignKey(d => d.FkOrganiseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_organiser_fk_organiserid_user_fkey");

            entity.HasOne(d => d.FkUserRequestAnsweridUserRequestAnswerNavigation).WithOne(p => p.UserRequestAnswerOrganiser)
                .HasForeignKey<UserRequestAnswerOrganiser>(d => d.FkUserRequestAnsweridUserRequestAnswer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_organiser_fk_user_request_answerid_use_fkey");
        });

        modelBuilder.Entity<UserRequestAnswerUser>(entity =>
        {
            entity.HasKey(e => new { e.FkUserRequestAnsweridUserRequestAnswer, e.FkUseridUser }).HasName("user_request_answer_user_pkey");

            entity.ToTable("user_request_answer_user", "models");

            entity.HasIndex(e => e.FkUserRequestAnsweridUserRequestAnswer, "user_request_answer_user_fk_user_request_answerid_user_requ_key").IsUnique();

            entity.Property(e => e.FkUserRequestAnsweridUserRequestAnswer).HasColumnName("fk_user_request_answerid_user_request_answer");
            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");

            entity.HasOne(d => d.FkUserRequestAnsweridUserRequestAnswerNavigation).WithOne(p => p.UserRequestAnswerUser)
                .HasForeignKey<UserRequestAnswerUser>(d => d.FkUserRequestAnsweridUserRequestAnswer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_user_fk_user_request_answerid_user_req_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.UserRequestAnswerUsers)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_request_answer_user_fk_userid_user_fkey");
        });

        modelBuilder.Entity<UserTicket>(entity =>
        {
            entity.HasKey(e => new { e.FkUseridUser, e.FkTicketidTicket }).HasName("user_ticket_pkey");

            entity.ToTable("user_ticket", "models");

            entity.HasIndex(e => e.FkTicketidTicket, "user_ticket_fk_ticketid_ticket_key").IsUnique();

            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.FkTicketidTicket).HasColumnName("fk_ticketid_ticket");

            entity.HasOne(d => d.FkTicketidTicketNavigation).WithOne(p => p.UserTicket)
                .HasForeignKey<UserTicket>(d => d.FkTicketidTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_ticket_fk_ticketid_ticket_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.UserTickets)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_ticket_fk_userid_user_fkey");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.IdUserType).HasName("user_type_pkey");

            entity.ToTable("user_type", "models");

            entity.Property(e => e.IdUserType)
                .ValueGeneratedNever()
                .HasColumnName("id_user_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();

            // Seed initial category data
            entity.HasData(
                new Category { IdCategory = 1, Name = "Music" },
                new Category { IdCategory = 2, Name = "Theatre" },
                new Category { IdCategory = 3, Name = "Opera" },
                new Category { IdCategory = 4, Name = "Exposition" },
                new Category { IdCategory = 5, Name = "Fashion show" }
            );
        });

        // Default settings for Equipment entity
        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();

            // Seed initial equipment data
            entity.HasData(
                new Equipment { IdEquipment = 1, Name = "Sound System" },
                new Equipment { IdEquipment = 2, Name = "TV" },
                new Equipment { IdEquipment = 3, Name = "Microphone" },
                new Equipment { IdEquipment = 4, Name = "Projector" }
            );
        });

        // Default settings for FeedbackType entity
        modelBuilder.Entity<FeedbackType>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();

            // Seed initial feedback type data
            entity.HasData(
                new FeedbackType { IdFeedbackType = 1, Name = "Positive" },
                new FeedbackType { IdFeedbackType = 2, Name = "Negative" }
            );
        });

        // Default settings for TicketType entity
        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();

            // Seed initial ticket type data
            entity.HasData(
                new TicketType { IdTicketType = 1, Name = "Standard" },
                new TicketType { IdTicketType = 2, Name = "VIP" },
                new TicketType { IdTicketType = 3, Name = "Super-VIP" }
            );
        });

        // Default settings for UserType entity
        modelBuilder.Entity<UserType>(entity =>
        {
            entity.Property(e => e.Name).IsRequired();

            // Seed initial user type data
            entity.HasData(
                new UserType { IdUserType = 1, Name = "Regular" },
                new UserType { IdUserType = 2, Name = "Organizer" },
                new UserType { IdUserType = 3, Name = "Admin" }
            );
        });

        // Default settings for Ticketstatus entity
        modelBuilder.Entity<Ticketstatus>(entity =>
        {
            // Seed initial ticket status data
            entity.HasData(
                new Ticketstatus { IdStatus = 1, Name = "Active" },
                new Ticketstatus { IdStatus = 2, Name = "Inactive" },
                new Ticketstatus { IdStatus = 3, Name = "Scanned" }
            );
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.GenerationDate).HasDefaultValueSql("CURRENT_DATE");
        });

        modelBuilder.Entity<Loyalty>(entity =>
        {
            entity.Property(e => e.Date).HasDefaultValueSql("CURRENT_DATE");
        });

        // Default settings for User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.LastLogin).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Default settings for Administrator entity
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.Property(e => e.LastLogin).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Default settings for Organiser entity
        modelBuilder.Entity<Organiser>(entity =>
        {
            entity.Property(e => e.LastLogin).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Loyalty>().HasData(
    new Loyalty { IdLoyalty = 1, Points = 0 }
);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                IdUser = 1,
                Name = "Event",
                Surname = "Organizer",
                Username = "organizer",
                Password = "password123",
            }
        );

        modelBuilder.Entity<Organiser>().HasData(
            new Organiser
            {
                IdUser = 1,
                FollowerCount = 0,
                Rating = 5.0
            }
        );

        modelBuilder.Entity<EventLocation>().HasData(
            new EventLocation
            {
                IdEventLocation = 1,
                Name = "Conference Center",
                Address = "123 Main St",
                City = "Boston",
                Country = "USA",
                Capacity = 500,
                Contacts = "contact@venue.com",
                Price = 1000.0,
                HoldingEquipment = 2 // Projector
            }
        );

        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                IdEvent = 1,
                Name = "Tech Conference 2025",
                Description = "Annual technology conference",
                StartDate = new DateOnly(2025, 6, 15),
                EndDate = new DateOnly(2025, 6, 17),
                MaxTicketCount = 500,
                Category = 2, // Conference
                FkEventLocationidEventLocation = 1,
                FkOrganiseridUser = 1
            }
        );


        modelBuilder.Entity<Sector>().HasData(
            new Sector
            {
                IdSector = 1,
                FkEventLocationidEventLocation = 1,
                Name = "Main Hall"
            },
            new Sector
            {
                IdSector = 2,
                FkEventLocationidEventLocation = 1,
                Name = "VIP Section"
            }
        );

        modelBuilder.Entity<SectorPrice>().HasData(
            new SectorPrice
            {
                IdSectorPrice = 1,
                Price = 50.0,
                FkSectoridSector = 1,
                FkEventidEvent = 1
            },
            new SectorPrice
            {
                IdSectorPrice = 2,
                Price = 150.0,
                FkSectoridSector = 2,
                FkEventidEvent = 1
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
