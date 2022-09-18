using CashFlowDataBaseLibrary.DML;
using Microsoft.EntityFrameworkCore;

namespace CashFlowDataBaseLibrary.DAL
{
    internal class CashFlowDBContext : DbContext
    {
        public DbSet<CashFlowDataSet> CashFlowDbSet { get; set; }

        private string _dbPathString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"DataSource={_dbPathString}");

        public CashFlowDBContext(string dataSourcePath)
        {
            _dbPathString = dataSourcePath;
        }



    }
}
