using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace User.APIApp
{
    internal class SyncDBContext : DbContext
    {
        public SyncDBContext() : base("SyncConn")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // table names doesn't changes
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<tblTestSTC> tblTestSTCs { get; set; }
        public DbSet<tblTestTWS> tblTestTWSs { get; set; }

    }
}