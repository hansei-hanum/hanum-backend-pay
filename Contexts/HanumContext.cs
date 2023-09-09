using System;
using System.Collections.Generic;
using HanumPay.Models;
using Microsoft.EntityFrameworkCore;

namespace HanumPay.Contexts;

public partial class HanumContext : DbContext {
    public HanumContext() {
    }

    public HanumContext(DbContextOptions<HanumContext> options)
        : base(options) {
    }

    public virtual DbSet<Balance> Balances { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.6.14-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<Balance>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("balances", tb => tb.HasComment("계좌"))
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_bin");

            entity.HasIndex(e => e.UserId, "user_id").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("계좌 고유 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasComment("계좌 정산 후 총 잔액")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("amount");
            entity.Property(e => e.Comment)
                .HasMaxLength(24)
                .HasComment("계좌 메모")
                .HasColumnName("comment");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'personal'")
                .HasComment("계좌 분류")
                .HasColumnType("enum('personal','business')")
                .HasColumnName("type");
            entity.Property(e => e.UserId)
                .HasComment("사용자 ID, 비즈니스의 경우 NULL")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Balance)
                .HasForeignKey<Balance>(d => d.UserId)
                .HasConstraintName("USER_ID_FK");
        });

        modelBuilder.Entity<Transaction>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("transactions", tb => tb.HasComment("계좌 송금 내역"))
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_bin");

            entity.HasIndex(e => e.ReceiverId, "RECEIVER_BALANCE_FK");

            entity.HasIndex(e => e.SenderId, "SENDER_BALANCE_FK");

            entity.Property(e => e.Id)
                .HasComment("트랜잭션 고유 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasComment("송금액")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("amount");
            entity.Property(e => e.Comment)
                .HasMaxLength(24)
                .HasComment("송금 메모")
                .HasColumnName("comment");
            entity.Property(e => e.ReceiverId)
                .HasComment("수신자 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("receiver_id");
            entity.Property(e => e.SenderId)
                .HasComment("송금자 ID, 환전소의 경우 NULL로 설정")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("sender_id");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("트랜잭션 시간")
                .HasColumnType("datetime")
                .HasColumnName("time");

            entity.HasOne(d => d.Receiver).WithMany(p => p.TransactionReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RECEIVER_BALANCE_FK");

            entity.HasOne(d => d.Sender).WithMany(p => p.TransactionSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("SENDER_BALANCE_FK");
        });

        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_bin");

            entity.HasIndex(e => e.Phone, "phone").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(5)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(11)
                .HasColumnName("phone");
            entity.Property(e => e.Profile)
                .HasMaxLength(100)
                .HasColumnName("profile");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
