using Microsoft.EntityFrameworkCore;
using FintechCore.Domain.Entities;

namespace FintechCore.Infrastructure
{
    public class FintechDbContext : DbContext
    {
        public FintechDbContext(DbContextOptions<FintechDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Outbox> Outboxe { get; set; }
        public DbSet<SagaState> SagaStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка Account
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Balance).HasPrecision(19, 4);
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            // Настройка Transaction
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(19, 4);
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.SagaId).HasMaxLength(50);
            });

            // Настройка User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Настройка Outbox
            modelBuilder.Entity<Outbox>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EventType).HasMaxLength(100);
                entity.Property(e => e.Payload).HasColumnType("jsonb");
                entity.HasIndex(e => e.Processed);
            });

            // Настройка SagaState
            modelBuilder.Entity<SagaState>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SagaId).HasMaxLength(50);
                entity.HasIndex(e => e.SagaId).IsUnique();
                entity.Property(e => e.Status).HasMaxLength(20);
                entity.Property(e => e.StepData).HasColumnType("jsonb");
            });
        }
    }
}
