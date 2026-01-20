using Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repository.Configuration;

public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
{
    public void Configure(EntityTypeBuilder<Notifications> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnType("INT").UseIdentityColumn();
        builder.Property(x => x.UserId).HasColumnType("INT").IsRequired();
        builder.Property(x => x.Subject).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Message).HasColumnType("VARCHAR(500)").IsRequired();
        builder.Property(x => x.Type).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x => x.Status).HasColumnType("VARCHAR(100)").IsRequired();
        builder.Property(x=> x.CreatedAt).HasColumnType("DATETIME").IsRequired();
        builder.Property(x=> x.DeliveredAt).HasColumnType("DATETIME");
    }
}