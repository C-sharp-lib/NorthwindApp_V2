using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;
using System.Linq.Expressions;
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
            btnUpdateRecord.Click += btnUpdateRecord_Click;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {

        }

        private void btnAddForm_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdateRecord_Click(object sender, EventArgs e)
        {
            string selectedTable = comboBoxTable.SelectedItem.ToString();
            object selectedKey = comboBoxColumn.SelectedItem;
            if (string.IsNullOrEmpty(selectedTable) || selectedKey == null) 
            {
                MessageBox.Show("Please select a table and a primary key value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UpdateForm updateForm = new UpdateForm(_context, selectedTable, selectedKey);
            updateForm.Show();
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
        private void LoadSelectedTableData(string selectedTable)
        {
            selectedTable = comboBoxTable.SelectedItem.ToString();

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
            string selectedTable = comboBoxTable.SelectedItem.ToString();
            LoadPrimaryKeys(selectedTable);
            LoadSelectedTableData(selectedTable);
        }
        private void LoadPrimaryKeys(string tableName)
        {
            Type entityType = GetEntityType(tableName);
            if (entityType == null)
            {
                MessageBox.Show("Entity type not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string primaryKeyName = GetPrimaryKeyName(entityType);
            if (primaryKeyName == null)
            {
                MessageBox.Show("Primary key not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use reflection to get the generic method 'Set<TEntity>()'
            var setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
            var genericSetMethod = setMethod.MakeGenericMethod(entityType);

            // Invoke the method on your context to get the DbSet
            var dbSet = genericSetMethod.Invoke(_context, null);

            // Cast the result to IQueryable
            var queryable = dbSet as IQueryable;

            // Build the expression tree for 'entity => EF.Property<object>(entity, primaryKeyName)'
            var parameter = Expression.Parameter(entityType, "entity");
            var efPropertyMethod = typeof(EF).GetMethod("Property", BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(typeof(object));
            var propertyAccess = Expression.Call(efPropertyMethod, parameter, Expression.Constant(primaryKeyName));
            var lambda = Expression.Lambda(propertyAccess, parameter);
            var selectMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, typeof(object));
            var selectResult = selectMethod.Invoke(null, new object[] { queryable, lambda });
            var primaryKeyValues = ((IQueryable<object>)selectResult).ToList();
            comboBoxColumn.DataSource = primaryKeyValues;
        }

        private Type GetEntityType(string tableName)
        {
            var dbSetProperty = _context.GetType().GetProperty(tableName);
            if (dbSetProperty != null)
            {
                var entityType = dbSetProperty.PropertyType.GetGenericArguments().FirstOrDefault();
                return entityType;
            }
            return null;
        }

        private string GetPrimaryKeyName(Type entityType)
        {
            var key = _context.Model.FindEntityType(entityType)?.FindPrimaryKey();
            return key?.Properties.FirstOrDefault()?.Name;
        }
        private void comboBoxColumn_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
}
