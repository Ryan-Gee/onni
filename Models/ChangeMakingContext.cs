using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace onni.Models
{
    public partial class ChangeMakingContext : DbContext
    {
        public ChangeMakingContext()
        {
        }

        public ChangeMakingContext(DbContextOptions<ChangeMakingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<SavedProjects> SavedProjects { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Data Source=(local);Initial Catalog=ChangeMaking;Integrated Security=True");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.Property(e => e.CategoriesName).IsUnicode(false);
            });

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.Property(e => e.BodyContent).IsUnicode(false);

                entity.Property(e => e.UserName).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Projects_Comments");
            });

            modelBuilder.Entity<Projects>(entity =>
            {
                entity.Property(e => e.BodyContent).IsUnicode(false);

                entity.Property(e => e.Files).IsUnicode(false);

                entity.Property(e => e.Images).IsUnicode(false);

                entity.Property(e => e.ProjectName).IsUnicode(false);

                entity.Property(e => e.UserName).IsUnicode(false);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_Projects_Categories");

                entity.HasOne(d => d.ParentProject)
                    .WithMany(p => p.InverseParentProject)
                    .HasForeignKey(d => d.ParentProjectId)
                    .HasConstraintName("FK_Parents_Project");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Projects)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Projects_Status");
            });

            modelBuilder.Entity<SavedProjects>(entity =>
            {
                entity.Property(e => e.UserName).IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.SavedProjects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Projects_SavedProjects");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.StatusName).IsUnicode(false);
            });
        }
    }
}
