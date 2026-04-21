using Alumni_Portal.Models;
using Microsoft.EntityFrameworkCore;

namespace Alumni_Portal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Graduate> Graduates { get; set; } = null!;
        public DbSet<UserAccount> UserAccounts { get; set; } = null!;
        public DbSet<GraduateCareer> GraduateCareers { get; set; } = null!;
        public DbSet<Announcement> Announcements { get; set; } = null!;
        public DbSet<UserAuth> UserAuths { get; set; } = null!;
        public DbSet<UserSchema> UserSchemas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Graduate>(entity =>
            {
                entity.ToTable("graduate", "belek_graduate");
                entity.HasKey(e => e.GraduateId);
                entity.Property(e => e.GraduateId).HasColumnName("graduateid").ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasColumnName("firstname").HasMaxLength(50);
                entity.Property(e => e.LastName).HasColumnName("lastname").HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20);
                entity.Property(e => e.Faculty).HasColumnName("faculty").HasMaxLength(100);
                entity.Property(e => e.Department).HasColumnName("department").HasMaxLength(100);
                entity.Property(e => e.GraduationDate).HasColumnName("graduationdate");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date");
                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.BirthDate).HasColumnName("birthdate");
                entity.Property(e => e.OperationUserId).HasColumnName("operation_user_id");
                entity.Property(e => e.ArchiveAction).HasColumnName("archive_action");
                entity.Property(e => e.ArchiveDate).HasColumnName("archive_date");
            });
            modelBuilder.Entity<UserSchema>(entity =>
{
    entity.ToTable("user_schemas", "public");
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Id).HasColumnName("id");
    entity.Property(e => e.UserId).HasColumnName("user_id");
    entity.Property(e => e.SchemaName).HasColumnName("schema_name");
});

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("users", "public");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.FirstName).HasColumnName("first_name");
                entity.Property(e => e.LastName).HasColumnName("last_name");
                entity.Property(e => e.UserType).HasColumnName("user_type").HasMaxLength(20);
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.UpdateDate).HasColumnName("update_date");
                entity.Property(e => e.ProfileImageUrl).HasColumnName("profile_image_url");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.IsEmailVerified).HasColumnName("is_email_verified");
                entity.Property(e => e.EmailVerifiedAt).HasColumnName("email_verified_at");
                entity.Property(e => e.LanguageCode).HasColumnName("language_code");
            });

            modelBuilder.Entity<GraduateCareer>(entity =>
            {
                entity.ToTable("graduatecareer", "belek_graduate");
                entity.HasKey(e => e.CareerId);
                entity.Property(e => e.CareerId).HasColumnName("careerid").ValueGeneratedOnAdd();
                entity.Property(e => e.GraduateId).HasColumnName("graduateid");
                entity.Property(e => e.EmploymentStatus).HasColumnName("employmentstatus").HasMaxLength(20);
                entity.Property(e => e.CompanyName).HasColumnName("companyname").HasMaxLength(100);
                entity.Property(e => e.Position).HasColumnName("position").HasMaxLength(100);
                entity.Property(e => e.Sector).HasColumnName("sector").HasMaxLength(100);
                entity.Property(e => e.StartYear).HasColumnName("startyear");
                entity.Property(e => e.CreatedDate).HasColumnName("created_date");
                entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.OperationUserId).HasColumnName("operation_user_id");
                entity.Property(e => e.ArchiveAction).HasColumnName("archive_action");
                entity.Property(e => e.ArchiveDate).HasColumnName("archive_date");

                entity.HasOne(e => e.Graduate)
                      .WithOne(g => g.GraduateCareer)
                      .HasForeignKey<GraduateCareer>(e => e.GraduateId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.ToTable("announcement", "belek_graduate");
                entity.HasKey(e => e.AnnouncementId);
                entity.Property(e => e.AnnouncementId).HasColumnName("announcementid").ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(200);
                entity.Property(e => e.Content).HasColumnName("content");
                entity.Property(e => e.CreatedBy).HasColumnName("createdby");
                entity.Property(e => e.CreateDate).HasColumnName("create_date");
                entity.Property(e => e.IsActive).HasColumnName("isactive");
                entity.Property(e => e.UpdateDate).HasColumnName("update_date");
                entity.Property(e => e.OperationUserId).HasColumnName("operation_user_id");
                entity.Property(e => e.ArchiveAction).HasColumnName("archive_action");
                entity.Property(e => e.ArchiveDate).HasColumnName("archive_date");
            });

            modelBuilder.Entity<UserAuth>(entity =>
            {
                entity.ToTable("user_auth", "public");
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
                entity.Property(e => e.PasswordResetToken).HasColumnName("password_reset_token");
                entity.Property(e => e.PasswordResetExpires).HasColumnName("password_reset_expires");
                entity.Property(e => e.TempPassword).HasColumnName("temp_password");
                entity.Property(e => e.MustChangePassword).HasColumnName("must_change_password");
                entity.Property(e => e.PasswordChangedBy).HasColumnName("password_changed_by");
                entity.Property(e => e.PasswordChangedAt).HasColumnName("password_changed_at");
                entity.Property(e => e.LastLogin).HasColumnName("last_login");
                entity.Property(e => e.IsStaff).HasColumnName("is_staff");
                entity.Property(e => e.IsSuperuser).HasColumnName("is_superuser");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
                entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            });
        }
    }
}