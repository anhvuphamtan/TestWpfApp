using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BusinessObject
{
    public partial class JarvisContext : DbContext
    {
        public JarvisContext()
        {
        }

        public JarvisContext(DbContextOptions<JarvisContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }


        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
