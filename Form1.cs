using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;
using System.Reflection;

namespace NorthwindApplication
{
    public partial class Form1 : Form
    {
        private ApplicationDbContext _context;
        public Form1(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            PopulateComboTables();
            comboBoxTable.SelectedIndexChanged += comboBoxTable_SelectedIndexChanged;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {

        }

        private void btnAddForm_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdateRecord_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteRecord_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void PopulateComboTables()
        {
            var dbSetProperties = typeof(ApplicationDbContext).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .ToList();
            var tableNames = dbSetProperties.Select(p => p.Name).ToList();
            comboBoxTable.DataSource = tableNames;
        }
        private void LoadSelectedTableData()
        {
            string selectedTable = comboBoxTable.SelectedItem.ToString();

            // Get the DbSet property corresponding to the selected table
            var dbSetProperty = _context.GetType().GetProperty(selectedTable);

            if (dbSetProperty != null)
            {
                // Get the DbSet instance
                var dbSet = dbSetProperty.GetValue(_context) as IQueryable;

                if (dbSet != null)
                {
                    // Retrieve data asynchronously to avoid blocking the UI
                    var data = dbSet.Cast<object>().ToList();

                    // Bind data to the DataGridView
                    dataGridViewTable.DataSource = data;
                }
            }
        }
        private void comboBoxTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedTableData();
        }
    }
}
