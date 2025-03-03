using Microsoft.EntityFrameworkCore;
using NobetciEczaneGorsel.Models;

namespace NobetciEczaneGorsel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<IlModel> Iller { get; set; }
        public DbSet<EczaneModel> Eczaneler { get; set; }
    }
}