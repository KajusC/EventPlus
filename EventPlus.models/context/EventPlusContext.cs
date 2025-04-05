using System;
using System.Collections.Generic;
using eventplus.models.Entities;
using Microsoft.EntityFrameworkCore;

namespace eventplus.models.context;

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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventLocation> EventLocations { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<FeedbackType> FeedbackTypes { get; set; }

    public virtual DbSet<Loyalty> Loyalties { get; set; }

    public virtual DbSet<Organiser> Organisers { get; set; }

    public virtual DbSet<Partner> Partners { get; set; }

    public virtual DbSet<Performer> Performers { get; set; }

    public virtual DbSet<EventPerformer> RenginioatlikÄJas { get; set; }

    public virtual DbSet<EventPartner> Renginiopartneris { get; set; }

    public virtual DbSet<Seating> Seatings { get; set; }

    public virtual DbSet<Sector> Sectors { get; set; }

    public virtual DbSet<SectorPrice> SectorPrices { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRequestInformation> UserRequestInformations { get; set; }

    public virtual DbSet<UserType> UserTypes { get; set; }

    public virtual DbSet<Userrequest> Userrequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("administrator_pkey");

            entity.ToTable("administrator");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Administrator)
                .HasForeignKey<Administrator>(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrator_id_user_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategory).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.IdEquipment).HasName("equipment_pkey");

            entity.ToTable("equipment");

            entity.Property(e => e.IdEquipment).HasColumnName("id_equipment");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("event_pkey");

            entity.ToTable("event");

            entity.HasIndex(e => e.FkEventLocationidEventLocation, "event_fk_event_locationid_event_location_key").IsUnique();

            entity.Property(e => e.IdEvent).HasColumnName("id_event");
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

            entity.ToTable("event_location");

            entity.Property(e => e.IdEventLocation).HasColumnName("id_event_location");
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
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Equipment).HasColumnName("turima_ÄÆranga");

            entity.HasOne(d => d.EquipmentNavigation).WithMany(p => p.EventLocations)
                .HasForeignKey(d => d.Equipment)
                .HasConstraintName("event_location_turima_ÄÆranga_fkey");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.IdFeedback).HasName("feedback_pkey");

            entity.ToTable("feedback");

            entity.Property(e => e.IdFeedback).HasColumnName("id_feedback");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.Vote).HasColumnName("vote");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_fk_eventid_event_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("feedback_fk_userid_user_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("feedback_type_fkey");
        });

        modelBuilder.Entity<FeedbackType>(entity =>
        {
            entity.HasKey(e => e.IdFeedbackType).HasName("feedback_type_pkey");

            entity.ToTable("feedback_type");

            entity.Property(e => e.IdFeedbackType).HasColumnName("id_feedback_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Loyalty>(entity =>
        {
            entity.HasKey(e => e.IdLoyalty).HasName("loyalty_pkey");

            entity.ToTable("loyalty");

            entity.Property(e => e.IdLoyalty).HasColumnName("id_loyalty");
            entity.Property(e => e.Points).HasColumnName("points");
        });

        modelBuilder.Entity<Organiser>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("organiser_pkey");

            entity.ToTable("organiser");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.FollowerAmount).HasColumnName("follower_amount");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Organiser)
                .HasForeignKey<Organiser>(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("organiser_id_user_fkey");
        });

        modelBuilder.Entity<Partner>(entity =>
        {
            entity.HasKey(e => e.IdPartner).HasName("partner_pkey");

            entity.ToTable("partner");

            entity.Property(e => e.IdPartner).HasColumnName("id_partner");
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

            entity.ToTable("performer");

            entity.Property(e => e.IdPerformer).HasColumnName("id_performer");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Profesija)
                .HasMaxLength(255)
                .HasColumnName("profesija");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<EventPerformer>(entity =>
        {
            entity.HasKey(e => e.FkEventidEvent).HasName("renginioatlikÄ—jas_pkey");

            entity.ToTable("renginioatlikÄ—jas");

            entity.Property(e => e.FkEventidEvent)
                .ValueGeneratedNever()
                .HasColumnName("fk_eventid_event");

            entity.HasOne(d => d.FkEventidEventNavigation).WithOne(p => p.EventPerformer)
                .HasForeignKey<EventPerformer>(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("renginioatlikÄ—jas_fk_eventid_event_fkey");
        });

        modelBuilder.Entity<EventPartner>(entity =>
        {
            entity.HasKey(e => e.FkEventidEvent).HasName("renginiopartneris_pkey");

            entity.ToTable("renginiopartneris");

            entity.Property(e => e.FkEventidEvent)
                .ValueGeneratedNever()
                .HasColumnName("fk_eventid_event");

            entity.HasOne(d => d.FkEventidEventNavigation).WithOne(p => p.EventPartner)
                .HasForeignKey<EventPartner>(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("renginiopartneris_fk_eventid_event_fkey");
        });

        modelBuilder.Entity<Seating>(entity =>
        {
            entity.HasKey(e => new { e.IdSeating, e.FkSectoridSector, e.FkSectorfkEventLocationidEventLocation }).HasName("seating_pkey");

            entity.ToTable("seating");

            entity.HasIndex(e => e.FkTicketidTicket, "seating_fk_ticketid_ticket_key").IsUnique();

            entity.Property(e => e.IdSeating)
                .ValueGeneratedOnAdd()
                .HasColumnName("id_seating");
            entity.Property(e => e.FkSectoridSector).HasColumnName("fk_sectorid_sector");
            entity.Property(e => e.FkSectorfkEventLocationidEventLocation).HasColumnName("fk_sectorfk_event_locationid_event_location");
            entity.Property(e => e.FkTicketidTicket).HasColumnName("fk_ticketid_ticket");
            entity.Property(e => e.Place).HasColumnName("place");
            entity.Property(e => e.Row).HasColumnName("row");

            entity.HasOne(d => d.FkTicketidTicketNavigation).WithOne(p => p.Seating)
                .HasForeignKey<Seating>(d => d.FkTicketidTicket)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seating_fk_ticketid_ticket_fkey");

            entity.HasOne(d => d.Sector).WithMany(p => p.Seatings)
                .HasForeignKey(d => new { d.FkSectoridSector, d.FkSectorfkEventLocationidEventLocation })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seating_fk_sectorid_sector_fk_sectorfk_event_locationid_ev_fkey");
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.HasKey(e => new { e.IdSector, e.FkEventLocationidEventLocation }).HasName("sector_pkey");

            entity.ToTable("sector");

            entity.Property(e => e.IdSector)
                .ValueGeneratedOnAdd()
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

            entity.ToTable("sector_price");

            entity.Property(e => e.IdSectorPrice).HasColumnName("id_sector_price");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.FkSectorfkEventLocationidEventLocation).HasColumnName("fk_sectorfk_event_locationid_event_location");
            entity.Property(e => e.FkSectoridSector).HasColumnName("fk_sectorid_sector");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.SectorPrices)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_price_fk_eventid_event_fkey");

            entity.HasOne(d => d.Sector).WithMany(p => p.SectorPrices)
                .HasForeignKey(d => new { d.FkSectoridSector, d.FkSectorfkEventLocationidEventLocation })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_price_fk_sectorid_sector_fk_sectorfk_event_location_fkey");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.IdTicket).HasName("ticket_pkey");

            entity.ToTable("ticket");

            entity.Property(e => e.IdTicket).HasColumnName("id_ticket");
            entity.Property(e => e.FkEventidEvent).HasColumnName("fk_eventid_event");
            entity.Property(e => e.FkUseridUser).HasColumnName("fk_userid_user");
            entity.Property(e => e.GenerationDate).HasColumnName("generation_date");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.QrCode)
                .HasMaxLength(255)
                .HasColumnName("qr_code");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.FkEventidEventNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FkEventidEvent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_fk_eventid_event_fkey");

            entity.HasOne(d => d.FkUseridUserNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ticket_fk_userid_user_fkey");

            entity.HasOne(d => d.TypeNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.Type)
                .HasConstraintName("ticket_type_fkey");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.IdTicketType).HasName("ticket_type_pkey");

            entity.ToTable("ticket_type");

            entity.Property(e => e.IdTicketType).HasColumnName("id_ticket_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.FkLoyaltyidLoyalty, "User_fk_loyaltyid_loyalty_key").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.FkLoyaltyidLoyalty).HasColumnName("fk_loyaltyid_loyalty");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
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

            entity.HasOne(d => d.FkLoyaltyidLoyaltyNavigation).WithOne(p => p.User)
                .HasForeignKey<User>(d => d.FkLoyaltyidLoyalty)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_fk_loyaltyid_loyalty_fkey");
        });

        modelBuilder.Entity<UserRequestInformation>(entity =>
        {
            entity.HasKey(e => e.IdUserRequestInformation).HasName("user_request_information_pkey");

            entity.ToTable("user_request_information");

            entity.Property(e => e.IdUserRequestInformation).HasColumnName("id_user_request_information");
            entity.Property(e => e.Response)
                .HasMaxLength(255)
                .HasColumnName("atsakas");
            entity.Property(e => e.Question)
                .HasMaxLength(255)
                .HasColumnName("klausimas");
        });

        modelBuilder.Entity<UserType>(entity =>
        {
            entity.HasKey(e => e.IdUserType).HasName("user_type_pkey");

            entity.ToTable("user_type");

            entity.Property(e => e.IdUserType).HasColumnName("id_user_type");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Userrequest>(entity =>
        {
            entity.HasKey(e => e.FkUseridUser).HasName("userrequest_pkey");

            entity.ToTable("userrequest");

            entity.Property(e => e.FkUseridUser)
                .ValueGeneratedNever()
                .HasColumnName("fk_userid_user");

            entity.HasOne(d => d.FkUseridUserNavigation).WithOne(p => p.Userrequest)
                .HasForeignKey<Userrequest>(d => d.FkUseridUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userrequest_fk_userid_user_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
