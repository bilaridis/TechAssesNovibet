using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TechAssesNovibet.Repository
{
    public partial class NovibetContext : DbContext
    {
        public NovibetContext()
        {
        }

        public NovibetContext(DbContextOptions<NovibetContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BatchAddress> BatchAddresses { get; set; }
        public virtual DbSet<BatchProcess> BatchProcesses { get; set; }
        public virtual DbSet<IpDetail> IpDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=Bilaridis2020\\SqlExpress; Database=Novibet; User Id=EronAdmin; Password=EronAdmin;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BatchAddress>(entity =>
            {
                entity.ToTable("batchAddresses");

                entity.Property(e => e.BatchAddressId)
                    .HasColumnName("batchAddressId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.FBatchId).HasColumnName("F_BatchId");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.FBatch)
                    .WithMany(p => p.BatchAddresses)
                    .HasForeignKey(d => d.FBatchId)
                    .HasConstraintName("FK_batchAddresses_batchProcesses");
            });

            modelBuilder.Entity<BatchProcess>(entity =>
            {
                entity.HasKey(e => e.BatchId);

                entity.ToTable("batchProcesses");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Finished).HasColumnType("datetime");
            });

            modelBuilder.Entity<IpDetail>(entity =>
            {
                entity.HasKey(e => e.IpAddress)
                    .HasName("PK_IpDetails_1");

                entity.Property(e => e.IpAddress)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Continent)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
