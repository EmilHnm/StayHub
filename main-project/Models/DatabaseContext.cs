using Microsoft.EntityFrameworkCore;

namespace main_project.Models;

public partial class DatabaseContext : DbContext
{
    public DbSet<AccommodationType> accommodationTypes { get; set; }
    public DbSet<Booking> bookings { get; set; }
    public DbSet<Hostel> hostels { get; set; }
    public DbSet<HostelImages> hostelImages { get; set; }
    public DbSet<HostelUtility> hostelUtilities { get; set; }
    public DbSet<HostProfile> hostProfiles { get; set; }
    public DbSet<LeaseContract> leaseContracts { get; set; }
    public DbSet<RenterProfile> renterProfiles { get; set; }
    public DbSet<Utility> utilities { get; set; }
    public DbSet<User> users { get; set; }

    public DatabaseContext()
    {
    }
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            try
            {
                optionsBuilder.UseMySQL("server=127.0.0.1;database=stayhub;user=root;password=Hoangominh_01;persistsecurityinfo=True");
            }
            catch (Exception)
            {
                throw new Exception("Can not connect to database");
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccommodationType>(entity =>
        {
            entity.ToTable("accommodation-types");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasMany<Hostel>(c => c.Hotels)
                .WithOne(c => c.AccommodationType)
                .HasForeignKey(c => c.AccommodationTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("bookings");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("end_date");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("start_date");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.RenterId).HasColumnName("renter_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.LeaseContractId).HasColumnName("lease_contract_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<RenterProfile>(c => c.RenterProfile)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.RenterId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Hostel>(c => c.Hostel)
                .WithMany(c => c.Bookings)
                .HasForeignKey(c => c.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Hostel>(entity =>
        {
            entity.ToTable("hostels");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccommodationTypeId).HasColumnName("accommodation_type_id");
            entity.Property(e => e.HostId).HasColumnName("host_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Rooms).HasColumnName("rooms");
            entity.Property(e => e.Fee).HasColumnName("fee");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<AccommodationType>(c => c.AccommodationType)
                .WithMany(c => c.Hotels)
                .HasForeignKey(c => c.AccommodationTypeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<HostProfile>(c => c.HostProfile)
                .WithMany(c => c.Hostels)
                .HasForeignKey(c => c.HostId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<HostelImages>(c => c.HostelImages)
                .WithOne(c => c.Hostel)
                .HasForeignKey(c => c.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<HostelUtility>(c => c.HostelUtility)
                .WithOne(c => c.Hostels)
                .HasForeignKey(c => c.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<Utility>(c => c.Utilities)
                .WithMany(c => c.Hostels)
                .UsingEntity(
                    "HostelUtility",
                    l => l.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    r => r.HasOne(typeof(Utility)).WithMany().HasForeignKey("UtilityId").HasPrincipalKey(nameof(Utility.Id)),
                    j => j.HasKey("HostelId", "UtilityId")
                );
            entity.HasMany<Booking>(c => c.Bookings)
                .WithOne(c => c.Hostel)
                .HasForeignKey(c => c.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<RenterProfile>(c => c.BookedBy)
                .WithMany(c => c.BookingHostels)
                .UsingEntity(
                    "Booking",
                    l => l.HasOne(typeof(RenterProfile)).WithMany().HasForeignKey("RenterId").HasPrincipalKey(nameof(RenterProfile.Id)),
                    r => r.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    j => j.HasKey("HostelId", "RenterId")
                );
            entity.HasMany<RenterProfile>(c => c.RentedBy)
                .WithMany(c => c.RentedHostels)
                .UsingEntity(
                    "LeaseContract",
                    l => l.HasOne(typeof(RenterProfile)).WithMany().HasForeignKey("RenterId").HasPrincipalKey(nameof(RenterProfile.Id)),
                    r => r.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    j => j.HasKey("HostelId", "RenterId")
                );
        });

        modelBuilder.Entity<HostelImages>(entity =>
        {
            entity.ToTable("hostel_images");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<Hostel>(c => c.Hostel)
                            .WithMany(c => c.HostelImages)
                            .HasForeignKey(c => c.HostelId)
                            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HostelUtility>(entity =>
        {
            entity.ToTable("hostel-utility");
            entity.HasKey(e => e.HostelId);
            entity.HasKey(e => e.UtilityId);
            entity.Property(e => e.UtilityId).HasColumnName("utility_id");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<Hostel>(c => c.Hostels)
                .WithMany(c => c.HostelUtility)
                .HasForeignKey(c => c.HostelId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Utility>(c => c.Utility)
                .WithMany(c => c.HostelUtility)
                .HasForeignKey(c => c.UtilityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HostProfile>(entity =>
        {
            entity.ToTable("hosts-profiles");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("fullname");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<User>(c => c.User)
                .WithOne(c => c.HostProfile)
                .HasForeignKey<User>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<Hostel>(c => c.Hostels)
                .WithOne(c => c.HostProfile)
                .HasForeignKey(c => c.HostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<LeaseContract>(entity =>
        {
            entity.ToTable("lease-contract");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RenterId).HasColumnName("renter_id");
            entity.Property(e => e.HostelId).HasColumnName("hostel_id");
            entity.Property(e => e.StartDate).HasColumnType("date").HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnType("date").HasColumnName("end_date");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.MonthlyRent).HasColumnName("monthly_rent");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<Hostel>(c => c.Hostel)
                .WithMany(c => c.LeaseContracts)
                .HasForeignKey(c => c.HostelId);
            entity.HasOne<RenterProfile>(c => c.Renter)
                .WithMany(c => c.LeaseContracts)
                .HasForeignKey(c => c.RenterId);
        });

        modelBuilder.Entity<RenterProfile>(entity =>
        {
            entity.ToTable("renter-profile");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("fullname");
            entity.Property(e => e.Phone).HasColumnName("phone");
            entity.Property(e => e.IdentityCardNumber).HasColumnName("identity_card_number");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<User>(c => c.User)
                .WithOne(c => c.RenterProfile)
                .HasForeignKey<User>(c => c.Id)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany<Hostel>(c => c.BookingHostels)
                .WithMany(c => c.BookedBy)
                .UsingEntity(
                    "Booking",
                    l => l.HasOne(typeof(RenterProfile)).WithMany().HasForeignKey("RenterId").HasPrincipalKey(nameof(RenterProfile.Id)),
                    r => r.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    j => j.HasKey("HostelId", "RenterId")
                );
            entity.HasMany<Hostel>(c => c.RentedHostels)
                .WithMany(c => c.RentedBy)
                .UsingEntity(
                    "LeaseContract",
                    l => l.HasOne(typeof(RenterProfile)).WithMany().HasForeignKey("RenterId").HasPrincipalKey(nameof(RenterProfile.Id)),
                    r => r.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    j => j.HasKey("HostelId", "RenterId")
                );
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasOne<HostProfile>(c => c.HostProfile)
                .WithOne(c => c.User)
                .HasForeignKey<HostProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();
            entity.HasOne<RenterProfile>(c => c.RenterProfile)
                .WithOne(c => c.User)
                .HasForeignKey<RenterProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade).IsRequired();

        });

        modelBuilder.Entity<Utility>(entity =>
        {
            entity.ToTable("utilities");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.HasMany<Hostel>(c => c.Hostels)
                .WithMany(c => c.Utilities)
                .UsingEntity(
                    "HostelUtility",
                    l => l.HasOne(typeof(Hostel)).WithMany().HasForeignKey("HostelId").HasPrincipalKey(nameof(Hostel.Id)),
                    r => r.HasOne(typeof(Utility)).WithMany().HasForeignKey("UtilityId").HasPrincipalKey(nameof(Utility.Id)),
                    j => j.HasKey("HostelId", "UtilityId")
                    );
        });

    }
}