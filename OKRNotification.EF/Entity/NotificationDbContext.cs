using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace OKRNotification.EF
{
    [ExcludeFromCodeCoverage]
    public partial class NotificationDbContext : DataContext
    {
        public NotificationDbContext()
        {
        }

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationMaster> ApplicationMaster { get; set; }
        public virtual DbSet<Emails> Emails { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<Mail> Mail { get; set; }
        public virtual DbSet<MailSentLog> MailSentLog { get; set; }
        public virtual DbSet<MailSetupConfig> MailSetupConfig { get; set; }
        public virtual DbSet<MailerTemplate> MailerTemplate { get; set; }
        public virtual DbSet<MessageType> MessageType { get; set; }
        public virtual DbSet<NotificationType> NotificationType { get; set; }
        public virtual DbSet<NotificationsDetails> NotificationsDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Server=52.21.77.184;Database=Notification_DevV2;User Id=okr-admin;Password=abcd@1234;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<ApplicationMaster>(entity =>
            {
                entity.Property(e => e.AppName).HasMaxLength(250);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Emails>(entity =>
            {
                entity.Property(e => e.EmailAddress)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(100);
            });

            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.Property(e => e.ApplicationName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.ErrorDetail).IsRequired();

                entity.Property(e => e.FunctionName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PageName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Mail>(entity =>
            {
                entity.Property(e => e.Bcc)
                    .HasColumnName("BCC")
                    .HasMaxLength(100);

                entity.Property(e => e.Body).HasColumnType("text");

                entity.Property(e => e.Cc)
                    .HasColumnName("CC")
                    .HasMaxLength(500);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.MailFrom).HasMaxLength(100);

                entity.Property(e => e.MailTo).HasMaxLength(400);

                entity.Property(e => e.Subject).HasMaxLength(400);
            });

            modelBuilder.Entity<MailSentLog>(entity =>
            {
                entity.Property(e => e.Bcc)
                    .HasColumnName("BCC")
                    .HasMaxLength(250);

                entity.Property(e => e.Body).HasColumnType("text");

                entity.Property(e => e.Cc)
                    .HasColumnName("CC")
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.MailFrom).HasMaxLength(250);

                entity.Property(e => e.MailSentOn).HasColumnType("datetime");

                entity.Property(e => e.MailSubject).HasMaxLength(250);

                entity.Property(e => e.MailTo).HasMaxLength(250);
            });

            modelBuilder.Entity<MailSetupConfig>(entity =>
            {
                entity.Property(e => e.AccountName)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.AccountPassword).HasMaxLength(300);

                entity.Property(e => e.AwsemailId)
                    .IsRequired()
                    .HasColumnName("AWSEmailID")
                    .HasMaxLength(300);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Host).HasMaxLength(300);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsSslenable).HasColumnName("IsSSLEnable");

                entity.Property(e => e.ServerName).HasMaxLength(250);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<MailerTemplate>(entity =>
            {
                entity.HasIndex(e => e.TemplateCode)
                    .HasName("UniqueConstraint")
                    .IsUnique();

                entity.HasIndex(e => new { e.TemplateName, e.TemplateCode })
                    .HasName("mailUniqueConstraint")
                    .IsUnique();

                entity.Property(e => e.Body).HasColumnType("text");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Subject).HasMaxLength(300);

                entity.Property(e => e.TemplateCode).HasMaxLength(20);

                entity.Property(e => e.TemplateName).HasMaxLength(250);
            });

            modelBuilder.Entity<MessageType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Message).HasMaxLength(250);
            });

            modelBuilder.Entity<NotificationType>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Notification).HasMaxLength(250);

                entity.Property(e => e.NotificationCode)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<NotificationsDetails>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.NotificationsMessage).HasColumnType("text");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.Url).HasColumnType("text");

                entity.Property(e => e.NotificationOnTypeId);

                entity.Property(e => e.NotificationOnId);

                entity.HasOne(d => d.ApplicationMaster)
                    .WithMany(p => p.NotificationsDetails)
                    .HasForeignKey(d => d.ApplicationMasterId)
                    .HasConstraintName("FK__Notificat__Appli__38996AB5");

                entity.HasOne(d => d.MessageType)
                    .WithMany(p => p.NotificationsDetails)
                    .HasForeignKey(d => d.MessageTypeId)
                    .HasConstraintName("FK__Notificat__Messa__37A5467C");

                entity.HasOne(d => d.NotificationType)
                    .WithMany(p => p.NotificationsDetails)
                    .HasForeignKey(d => d.NotificationTypeId)
                    .HasConstraintName("FK__Notificat__Notif__36B12243");
            });
        }
    }
}
