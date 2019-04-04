

using Microsoft.EntityFrameworkCore;

namespace Blog.Models
{
    public class BlogContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<PostTag>()
                .HasKey(bc => new { bc.PostId, bc.TagId });
            modelBuilder.Entity<PostTag>()
                .HasOne(bc => bc.Post)
                .WithMany(b => b.PostTags)
                .HasForeignKey(bc => bc.PostId);
            modelBuilder.Entity<PostTag>()
                .HasOne(bc => bc.Tag)
                .WithMany(c => c.PostTags)
                .HasForeignKey(bc => bc.TagId);
        }
    }
}