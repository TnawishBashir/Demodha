using Demodha.Data.Identity;
using Demodha.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Demodha.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<NdcApplication> NdcApplications {get; set;}
        public DbSet<NdcParty> NdcParties {get; set;}
        public DbSet<NdcDocument> NdcDocuments { get; set; }

        public DbSet<Dealer> Dealers { get; set; }

        public DbSet<NdcTaskDefinition> NdcTaskDefinitions { get; set; }
        public DbSet<NdcTask> NdcTasks { get; set; }
        public DbSet<NdcClearance> NdcClearances { get; set; }
        public DbSet<NdcFinanceCase> NdcFinanceCases { get; set; }
        public DbSet<NdcAppointment> NdcAppointments { get; set; }
        public DbSet<NdcVerification> NdcVerifications { get; set; }
        public DbSet<NdcStatusHistory> NdcStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<NdcApplication>(e =>
            {
                e.HasIndex(x => new { x.PlotOrFileNo, x.Block, x.SectorOrPhase, x.SocietyOrScheme });

                e.Property(x => x.CurrentStage).HasConversion<byte>();
                e.Property(x => x.CurrentStatus).HasConversion<byte>();

                e.HasIndex(x => x.DealerUserId);

                e.HasOne(x => x.DealerUser)
                 .WithMany()
                 .HasForeignKey(x => x.DealerUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<NdcParty>(e =>
            {
                e.Property(x => x.PartyType).HasConversion<byte>();
                e.HasIndex(x => new { x.NdcApplicationId, x.PartyType }).IsUnique(); 
            });

            builder.Entity<NdcDocument>(e =>
            {
                e.Property(x => x.DocType).HasConversion<byte>();
                e.HasIndex(x => new { x.NdcApplicationId, x.DocType });
            });

            builder.Entity<NdcTaskDefinition>(e =>
            {
                e.Property(x => x.Stage).HasConversion<byte>();
                e.HasIndex(x => new { x.Stage, x.SortOrder });
            });

            builder.Entity<NdcTask>(e =>
            {
                e.HasIndex(x => new { x.NdcApplicationId, x.TaskDefinitionId }).IsUnique();
            });

            builder.Entity<NdcClearance>(e =>
            {
                e.Property(x => x.Department).HasConversion<byte>();
                e.Property(x => x.Status).HasConversion<byte>();
                e.HasIndex(x => new { x.NdcApplicationId, x.Department }).IsUnique();

                e.HasOne(x => x.CompletionCertificateDocument)
                    .WithMany()
                    .HasForeignKey(x => x.CompletionCertificateDocumentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<NdcFinanceCase>(e =>
            {
                e.Property(x => x.OutstandingAmount).HasColumnType("decimal(19,2)");

                e.HasOne(x => x.ChallanDocument)
                    .WithMany()
                    .HasForeignKey(x => x.ChallanDocumentId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasOne(x => x.PaymentReceiptDocument)
                    .WithMany()
                    .HasForeignKey(x => x.PaymentReceiptDocumentId)
                    .OnDelete(DeleteBehavior.NoAction);

                e.HasIndex(x => x.NdcApplicationId).IsUnique();
            });

            builder.Entity<NdcAppointment>(e =>
            {
                e.HasIndex(x => x.NdcApplicationId).IsUnique();
            });

            builder.Entity<NdcVerification>(e =>
            {
                e.HasIndex(x => x.NdcApplicationId).IsUnique();
            });

            builder.Entity<NdcStatusHistory>(e =>
            {
                e.Property(x => x.FromStage).HasConversion<byte?>();
                e.Property(x => x.ToStage).HasConversion<byte>();
                e.Property(x => x.FromStatus).HasConversion<byte?>();
                e.Property(x => x.ToStatus).HasConversion<byte>();
                e.HasIndex(x => new { x.NdcApplicationId, x.ActionOn });
            });

            builder.Entity<Dealer>(e =>
            {
                e.HasIndex(x => x.Name);
            });

            builder.Entity<NdcTaskDefinition>().HasData(NdcSeed.TaskDefinitions);


            builder.Entity<ApplicationUser>()
                .HasIndex(x => new { x.User_CNIC, x.File_Number })
                .IsUnique();
        }
    }
}
