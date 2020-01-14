using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Model
{
    public partial class SanasorsaContext : DbContext
    {
        public SanasorsaContext()
        {
        }

        public SanasorsaContext(DbContextOptions<SanasorsaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Player> Player { get; set; }
        public virtual DbSet<Statistics> Statistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("SanasorsaDB"));
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.playerID).HasColumnName("PlayerID");

                entity.Property(e => e.Timeof)
                    .HasColumnName("Timeof")
                    .HasColumnType("datetime");

                entity.Property(e => e.nickname).HasMaxLength(50);

                entity.Property(e => e.scores)
                    .HasColumnName("Scores")
                    .HasColumnType("int");

            });
            modelBuilder.Entity<Statistics>(entity =>
            {
                entity.Property(e => e.statID).HasColumnName("StatID");

                entity.Property(e => e.Time)
                    .HasColumnName("Time")
                    .HasColumnType("datetime");
                entity.Property(e => e.json).HasColumnName("Json");
                OnModelCreatingPartial(modelBuilder);
            });
        }
       partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}


