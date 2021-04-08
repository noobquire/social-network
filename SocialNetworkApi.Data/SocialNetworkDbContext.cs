using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetworkApi.Data.Models;
using System.Globalization;

namespace SocialNetworkApi.Data
{
    public class SocialNetworkDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options) : base(options)
        {
        }
        
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserChat>()
                .HasKey(uc => new { uc.ChatId, uc.UserId });

            builder.Entity<UserChat>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.Chats)
                .HasForeignKey(uc => uc.UserId);
            builder.Entity<UserChat>()
                .HasOne(uc => uc.Chat)
                .WithMany(c => c.Participants)
                .HasForeignKey(uc => uc.ChatId);

            builder.Entity<Message>()
                .HasOne(m => m.Author)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.AuthorId);
            builder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId);

            builder.Entity<Post>()
                .HasOne(p => p.Profile)
                .WithMany(p => p.Posts)
                .HasForeignKey(p => p.ProfileId);
            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.AuthorId);
            builder.Entity<Post>()
                .HasOne(p => p.AttachedImage)
                .WithMany()
                .HasForeignKey(p => p.AttachedImageId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Profile>()
                .HasOne(p=> p.User)
                .WithOne(u => u.Profile)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Profile>()
                .HasOne(p => p.Avatar)
                .WithMany()
                .IsRequired(false);
            builder.Entity<Profile>()
                .HasOne(p => p.Avatar)
                .WithMany()
                .HasForeignKey(p => p.AvatarId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            var ti = CultureInfo.InvariantCulture.TextInfo;
            builder.Entity<Image>()
                .Property(i => i.Extension)
                .HasConversion(
                    e => e.ToString().ToLowerInvariant(),
                    e => (ImageExtensions) Enum.Parse(typeof(ImageExtensions), ti.ToTitleCase(e)));
            builder.Entity<Image>()
                .Property(i => i.OwnerId)
                .IsRequired(false);
            
            base.OnModelCreating(builder);
        }
    }
}
