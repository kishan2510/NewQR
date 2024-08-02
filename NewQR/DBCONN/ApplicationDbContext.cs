using Microsoft.EntityFrameworkCore;
using NewQR.Models;

namespace NewQR.DBCONN
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
                
        }
       public DbSet<Auth>Auths { get; set; }
    }
}
