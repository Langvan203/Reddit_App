using Microsoft.EntityFrameworkCore;
using Reddit_App.Models;
namespace Reddit_App.Database
{
    public class DatabaseContext :DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options) 
        {


        }

        #region List table
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<users> Userss { get; set; }
        #endregion

        public static void UPdateDatabase(DatabaseContext context)
        {
            context.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //var sqlConnection = "Data Source = SQL5113.site4now.net; Initial Catalog = db_aa7602_redditapp; User Id = db_aa7602_redditapp_admin; Password = van321pro@";
                var sqlConnection = "Data Source=LANGVAN;Initial Catalog=Reddit_App;Integrated Security=True;Trust Server Certificate=True";
                optionsBuilder.UseSqlServer(sqlConnection);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Follow>().HasIndex(e => e.IDFollow);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<users>(entity =>
            {
                entity.Property(e => e.Image).IsRequired(false);
            });
        }
    }
}
