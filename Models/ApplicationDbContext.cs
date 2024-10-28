using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Xml.Linq;

namespace NorthwindApplication.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employees> Employees { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Region> Region { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Shippers> Shippers { get; set; }
        public DbSet<Suppliers> Suppliers { get; set; }
        public DbSet<Territories> Territories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Employees>()
                .HasKey(u => new { u.EmployeeID });
            modelBuilder.Entity<Customers>()
                .HasKey(b => new { b.CustomerID });
            modelBuilder.Entity<Products>()
                .HasKey(g => new { g.ProductID });
            modelBuilder.Entity<Orders>()
                .HasKey(r => new { r.OrderID });
            modelBuilder.Entity<Region>()
                .HasKey(c => new { c.RegionID });
            modelBuilder.Entity<Categories>()
                .HasKey(gg => new { gg.CategoryID });
            modelBuilder.Entity<Shippers>()
                .HasKey(bi => new { bi.ShipperID });
            modelBuilder.Entity<Suppliers>()
                .HasKey(gg => new { gg.SupplierID });
            modelBuilder.Entity<Territories>()
                .HasKey(bi => new { bi.TerritoryID });
        }

        internal object Set(ApplicationDbContext context, string tableName, Type entityType)
        {
            throw new NotImplementedException();
        }
    }
}
