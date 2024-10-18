using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic.Logging;

namespace NorthwindApplication
{
    internal static class Program
    {
        // Service provider to manage dependencies
        public static ServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            // Setup configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath("C:\\Users\\strad\\source\\repos\\NorthwindApplication\\NorthwindApplication")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            try
            {

                // Setup DI
                var services = new ServiceCollection();

                // Register DbContext with DI
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                // Register forms and other services
                services.AddTransient<Form1>();
                services.AddTransient<UpdateForm>();

                ServiceProvider = services.BuildServiceProvider();

                // Start the application
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Retrieve the main form from DI
                var form = ServiceProvider.GetService<Form1>();
                Application.Run(form);
            }
            catch (Exception ex)
            {
                MessageBox.Show("A critical error occurred. Please check the logs.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}