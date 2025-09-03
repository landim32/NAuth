using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DB.Infra.Context;

public partial class NAuthContext : DbContext
{
    public NAuthContext()
    {
    }

    public NAuthContext(DbContextOptions<NAuthContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }

    public virtual DbSet<UserDocument> UserDocuments { get; set; }

    public virtual DbSet<UserPhone> UserPhones { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("nextval('user_id_seq'::regclass)")
                .HasColumnName("user_id");
            entity.Property(e => e.BirthDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("birth_date");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(180)
                .HasColumnName("email");
            entity.Property(e => e.Hash)
                .HasMaxLength(128)
                .HasColumnName("hash");
            entity.Property(e => e.IdDocument)
                .HasMaxLength(30)
                .HasColumnName("id_document");
            entity.Property(e => e.Image)
                .HasMaxLength(150)
                .HasColumnName("image");
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false)
                .HasColumnName("is_admin");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .HasColumnName("password");
            entity.Property(e => e.PixKey)
                .HasMaxLength(180)
                .HasColumnName("pix_key");
            entity.Property(e => e.RecoveryHash)
                .HasMaxLength(128)
                .HasColumnName("recovery_hash");
            entity.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(140)
                .HasColumnName("slug");
            entity.Property(e => e.StripeId)
                .HasMaxLength(120)
                .HasColumnName("stripe_id");
            entity.Property(e => e.Token)
                .HasMaxLength(128)
                .HasColumnName("token");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("pk_user_addresses");

            entity.ToTable("user_addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Address)
                .HasMaxLength(150)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(120)
                .HasColumnName("city");
            entity.Property(e => e.Complement)
                .HasMaxLength(150)
                .HasColumnName("complement");
            entity.Property(e => e.Neighborhood)
                .HasMaxLength(120)
                .HasColumnName("neighborhood");
            entity.Property(e => e.State)
                .HasMaxLength(80)
                .HasColumnName("state");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(15)
                .HasColumnName("zip_code");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_address");
        });

        modelBuilder.Entity<UserDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("user_documents_pkey");

            entity.ToTable("user_documents");

            entity.Property(e => e.DocumentId).HasColumnName("document_id");
            entity.Property(e => e.Base64).HasColumnName("base64");
            entity.Property(e => e.DocumentType)
                .HasDefaultValue(0)
                .HasColumnName("document_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserDocuments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_user_document");
        });

        modelBuilder.Entity<UserPhone>(entity =>
        {
            entity.HasKey(e => e.PhoneId).HasName("user_phones_pkey");

            entity.ToTable("user_phones");

            entity.Property(e => e.PhoneId).HasColumnName("phone_id");
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(30)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserPhones)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_phone");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("user_tokens_pkey");

            entity.ToTable("user_tokens");

            entity.Property(e => e.TokenId).HasColumnName("token_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpireAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expire_at");
            entity.Property(e => e.Fingerprint)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnName("fingerprint");
            entity.Property(e => e.IpAddress)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastAccess)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_access");
            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("token");
            entity.Property(e => e.UserAgent)
                .IsRequired()
                .HasMaxLength(512)
                .HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_token_user");
        });
        modelBuilder.HasSequence("network_id_seq");
        modelBuilder.HasSequence("profile_id_seq");
        modelBuilder.HasSequence("user_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
