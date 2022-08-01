using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lab8Handout.Models
{
    public partial class LibraryContext : DbContext
    {
        public LibraryContext()
        {
        }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CheckedOut> CheckedOuts { get; set; } = null!;
        public virtual DbSet<Inventory> Inventories { get; set; } = null!;
        public virtual DbSet<Patron> Patrons { get; set; } = null!;
        public virtual DbSet<Phone> Phones { get; set; } = null!;
        public virtual DbSet<Title> Titles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("name=Lab8:ConnectionString", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.1.48-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<CheckedOut>(entity =>
            {
                entity.HasKey(e => e.Serial)
                    .HasName("PRIMARY");

                entity.ToTable("CheckedOut");

                entity.HasIndex(e => e.CardNum, "CardNum");

                entity.Property(e => e.Serial)
                    .HasColumnType("int(10) unsigned")
                    .ValueGeneratedNever();

                entity.Property(e => e.CardNum).HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.CardNumNavigation)
                    .WithMany(p => p.CheckedOuts)
                    .HasForeignKey(d => d.CardNum)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CheckedOut_ibfk_1");

                entity.HasOne(d => d.SerialNavigation)
                    .WithOne(p => p.CheckedOut)
                    .HasForeignKey<CheckedOut>(d => d.Serial)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CheckedOut_ibfk_2");
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Serial)
                    .HasName("PRIMARY");

                entity.ToTable("Inventory");

                entity.HasIndex(e => e.Isbn, "ISBN");

                entity.Property(e => e.Serial).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(14)
                    .HasColumnName("ISBN")
                    .IsFixedLength();

                entity.HasOne(d => d.IsbnNavigation)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(d => d.Isbn)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Inventory_ibfk_1");
            });

            modelBuilder.Entity<Patron>(entity =>
            {
                entity.HasKey(e => e.CardNum)
                    .HasName("PRIMARY");

                entity.Property(e => e.CardNum).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Phone>(entity =>
            {
                entity.HasKey(e => new { e.CardNum, e.Phone1 })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.Property(e => e.CardNum).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Phone1)
                    .HasMaxLength(8)
                    .HasColumnName("Phone")
                    .IsFixedLength();

                entity.HasOne(d => d.CardNumNavigation)
                    .WithMany(p => p.Phones)
                    .HasForeignKey(d => d.CardNum)
                    .HasConstraintName("Phones_ibfk_1");
            });

            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => e.Isbn)
                    .HasName("PRIMARY");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(14)
                    .HasColumnName("ISBN")
                    .IsFixedLength();

                entity.Property(e => e.Author).HasMaxLength(100);

                entity.Property(e => e.Title1)
                    .HasMaxLength(100)
                    .HasColumnName("Title");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
