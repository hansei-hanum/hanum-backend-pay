using System;
using System.Collections.Generic;
using Hanum.Pay.Models;
using Microsoft.EntityFrameworkCore;

namespace Hanum.Pay.Contexts;

public partial class HanumContext : DbContext {
    public HanumContext(DbContextOptions<HanumContext> options)
        : base(options) {
    }

    public virtual DbSet<EoullimBalance> EoullimBalances { get; set; }

    public virtual DbSet<EoullimBooth> EoullimBooths { get; set; }

    public virtual DbSet<EoullimPayment> EoullimPayments { get; set; }

    public virtual DbSet<EoullimTransaction> EoullimTransactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VerificationKey> VerificationKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<EoullimBalance>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable(tb => tb.HasComment("한세어울림한마당 잔고"))
                .UseCollation("utf8mb4_bin");

            entity.HasIndex(e => e.BoothId, "booth_id").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("잔고 고유 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasComment("잔고 정산 후 총 잔액")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("amount");
            entity.Property(e => e.BoothId)
                .HasComment("부스 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("boothId");
            entity.Property(e => e.Comment)
                .HasMaxLength(24)
                .HasComment("잔고 메모")
                .HasColumnName("comment");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'personal'")
                .HasComment("잔고 분류")
                .HasColumnType("enum('personal','booth')")
                .HasColumnName("type");
            entity.Property(e => e.UserId)
                .HasComment("사용자 ID")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("userId");

            entity.HasOne(d => d.Booth).WithOne(p => p.EoullimBalance)
                .HasForeignKey<EoullimBalance>(d => d.BoothId)
                .HasConstraintName("BOOTH_ID_FK");

            entity.HasOne(d => d.User).WithOne(p => p.EoullimBalance)
                .HasForeignKey<EoullimBalance>(d => d.UserId)
                .HasConstraintName("USER_ID_FK");
        });

        modelBuilder.Entity<EoullimBooth>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable(tb => tb.HasComment("한세어울림한마당 부스"));

            entity.HasIndex(e => e.Token, "key").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("부스 고유번호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.Class)
                .HasMaxLength(24)
                .HasComment("부스 구분")
                .HasColumnName("class");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("부스 생성 날짜")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Location)
                .HasMaxLength(64)
                .HasDefaultValueSql("''")
                .HasComment("부스 위치")
                .HasColumnName("location");
            entity.Property(e => e.Name)
                .HasMaxLength(24)
                .HasComment("부스명")
                .HasColumnName("name");
            entity.Property(e => e.Token)
                .HasMaxLength(16)
                .HasComment("부스  토큰")
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasComment("부스 수정 날짜")
                .HasColumnType("datetime")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<EoullimPayment>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable(tb => tb.HasComment("한세어울림한마당 결제내역"));

            entity.HasIndex(e => e.BoothBalanceId, "EOULLIM_PAYMENTS_BOOTH_BALANCE_ID_FK");

            entity.HasIndex(e => e.BoothId, "EOULLIM_PAYMENTS_BOOTH_ID_FK");

            entity.HasIndex(e => e.UserBalanceId, "EOULLIM_PAYMENTS_USER_BALANCE_ID_FK");

            entity.HasIndex(e => e.UserId, "EOULLIM_PAYMENTS_USER_ID_FK");

            entity.Property(e => e.Id)
                .HasComment("결제고유변호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("id");
            entity.Property(e => e.BoothBalanceId)
                .HasComment("결제대상잔고고유번호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("boothBalanceId");
            entity.Property(e => e.BoothId)
                .HasComment("결제대상")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("boothId");
            entity.Property(e => e.PaidAmount)
                .HasComment("결제금액")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("paidAmount");
            entity.Property(e => e.PaidTime)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("결제시간")
                .HasColumnType("datetime")
                .HasColumnName("paidTime");
            entity.Property(e => e.PaymentComment)
                .HasMaxLength(24)
                .HasComment("결제메시지")
                .HasColumnName("paymentComment")
                .UseCollation("utf8mb4_bin");
            entity.Property(e => e.PaymentTransactionId)
                .HasComment("결제트랜잭션고유번호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("paymentTransactionId");
            entity.Property(e => e.RefundComment)
                .HasMaxLength(24)
                .HasComment("환불메시지")
                .HasColumnName("refundComment")
                .UseCollation("utf8mb4_bin");
            entity.Property(e => e.RefundTransactionId)
                .HasComment("환불트랜잭션고유번호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("refundTransactionId");
            entity.Property(e => e.RefundedAmount)
                .HasComment("결제취소금액")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("refundedAmount");
            entity.Property(e => e.RefundedTime)
                .HasComment("결제취소시간")
                .HasColumnType("datetime")
                .HasColumnName("refundedTime");
            entity.Property(e => e.Status)
                .HasComment("결제상태")
                .HasColumnType("enum('paid','refunded')")
                .HasColumnName("status")
                .UseCollation("utf8mb4_bin");
            entity.Property(e => e.UserBalanceId)
                .HasComment("걸제자잔고고유번호")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("userBalanceId");
            entity.Property(e => e.UserId)
                .HasComment("결제자")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("userId");

            entity.HasOne(d => d.BoothBalance).WithMany(p => p.EoullimPaymentBoothBalances)
                .HasForeignKey(d => d.BoothBalanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EOULLIM_PAYMENTS_BOOTH_BALANCE_ID_FK");

            entity.HasOne(d => d.Booth).WithMany(p => p.EoullimPayments)
                .HasForeignKey(d => d.BoothId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EOULLIM_PAYMENTS_BOOTH_ID_FK");

            entity.HasOne(d => d.UserBalance).WithMany(p => p.EoullimPaymentUserBalances)
                .HasForeignKey(d => d.UserBalanceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EOULLIM_PAYMENTS_USER_BALANCE_ID_FK");

            entity.HasOne(d => d.User).WithMany(p => p.EoullimPayments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("EOULLIM_PAYMENTS_USER_ID_FK");
        });

        modelBuilder.Entity<EoullimTransaction>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable(tb => tb.HasComment("한세어울림한마당 이체 내역"));

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
                .HasColumnName("receiverId");
            entity.Property(e => e.SenderId)
                .HasComment("송금자 ID, 환전소의 경우 NULL로 설정")
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("senderId");
            entity.Property(e => e.Time)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("트랜잭션 시간")
                .HasColumnType("datetime")
                .HasColumnName("time");

            entity.HasOne(d => d.Receiver).WithMany(p => p.EoullimTransactionReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RECEIVER_BALANCE_FK");

            entity.HasOne(d => d.Sender).WithMany(p => p.EoullimTransactionSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("SENDER_BALANCE_FK");
        });

        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
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

        modelBuilder.Entity<VerificationKey>(entity => {
            entity.HasKey(e => e.Key).HasName("PRIMARY");

            entity
                .ToTable("verification.keys")
                .UseCollation("utf8mb4_bin");

            entity.HasIndex(e => e.Key, "key").IsUnique();

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Key)
                .HasMaxLength(6)
                .HasColumnName("key");
            entity.Property(e => e.Classroom)
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("classroom");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Department)
                .HasColumnType("enum('CLOUD_SECURITY','NETWORK_SECURITY','HACKING_SECURITY','METAVERSE_GAME','GAME')")
                .HasColumnName("department");
            entity.Property(e => e.Grade)
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("grade");
            entity.Property(e => e.Number)
                .HasColumnType("tinyint(3) unsigned")
                .HasColumnName("number");
            entity.Property(e => e.Type)
                .HasColumnType("enum('STUDENT','TEACHER')")
                .HasColumnName("type");
            entity.Property(e => e.UsedAt)
                .HasColumnType("datetime")
                .HasColumnName("used_at");
            entity.Property(e => e.UserId)
                .HasColumnType("bigint(20) unsigned")
                .HasColumnName("user_id");
            entity.Property(e => e.ValidUntil)
                .HasColumnType("datetime")
                .HasColumnName("valid_until");

            entity.HasOne(d => d.User).WithMany(p => p.VerificationKeys)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("verification.keys_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
