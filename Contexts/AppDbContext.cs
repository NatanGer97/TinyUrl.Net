
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TinyUrl.Models;

namespace TinyUrl.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserClick> UserClicks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserClick>().HasKey(userclick => userclick.Id);
            modelBuilder.Entity<UserClick>().Property(userclick => userclick.Id)
                .ValueGeneratedOnAdd().HasColumnName("Id");
            modelBuilder.Entity<UserClick>().Property(userClick => userClick.TinyUrl).IsRequired().
                HasColumnName("tiny_url");
            modelBuilder.Entity<UserClick>().Property(userClick => userClick.OriginalUrl).IsRequired()
                .HasColumnName("original_url");
            modelBuilder.Entity<UserClick>().Property(userClick => userClick.Username).IsRequired()
                .HasColumnName("username");
            modelBuilder.Entity<UserClick>().Property(userClick => userClick.ClickedAt).IsRequired()
                .HasColumnName("clicked_at");
                
        }
    }
}