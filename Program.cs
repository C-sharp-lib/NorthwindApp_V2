using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;
using System.Configuration;

namespace NorthwindApplication
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;

            // Build the DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Create the context with options
            var context = new ApplicationDbContext(optionsBuilder.Options);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(context));
        }
    }
}