using Microsoft.EntityFrameworkCore;
using NorthwindApplication.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace NorthwindApplication
{
    public partial class Form1 : Form
    {
        private ApplicationDbContext _context;
        private readonly Dictionary<string, (Type EntityType, string PrimaryKeyPropName)> _tableMappings = new Dictionary<string, (Type, string)>(StringComparer.OrdinalIgnoreCase)
        {
            { "Employees", (typeof(Employees), "EmployeeID") },
            { "Customers", (typeof(Customers), "CustomerID") },
            { "Products", (typeof(Products), "ProductID") },
            { "Orders", (typeof(Orders), "OrderID") },
            { "Region", (typeof(Models.Region), "RegionID") },
            { "Shippers", (typeof(Shippers), "ShipperID") },
            { "Suppliers", (typeof(Suppliers), "SupplierID") },
            { "Territories", (typeof(Territories), "TerritoryID") },
        };
        public Form1(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            PopulateComboTables();
            GetTableRowCount();
            comboBoxTable.SelectedIndexChanged += comboBoxTable_SelectedIndexChanged;
            btnUpdateRecord.Click += btnUpdateRecord_Click;
            button1.Click += button1_Click;
            this.Bounds = Screen.FromControl(this).Bounds;

        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {

        }

        private void btnAddForm_Click(object sender, EventArgs e)
        {
            string selectedTable = comboBoxTable.SelectedItem.ToString();
            if (string.IsNullOrEmpty(selectedTable))
            {
                MessageBox.Show("Please select a table and a primary key value.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddForm addForm = new AddForm(_context, selectedTable);
            addForm.Show();
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
            try
            {
                string selectedTable = comboBoxTable.SelectedItem.ToString();
                if (comboBoxColumn.SelectedItem == null)
                {
                    MessageBox.Show("Please select a Record ID to delete.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Confirm deletion with the user
                DialogResult dialogResult = MessageBox.Show($"Are you sure you want to delete the selected record from {selectedTable}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult != DialogResult.Yes)
                {
                    return; // User canceled the deletion
                }

                // Get the primary key value
                object primaryKeyValue = comboBoxColumn.SelectedItem;

                // Perform deletion dynamically
                DeleteEntityDynamically(selectedTable, primaryKeyValue);

                // Refresh the Record ID ComboBox after deletion
                LoadPrimaryKeys(selectedTable);
                LoadSelectedTableData(selectedTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void DeleteEntityDynamically(string tableName, object primaryKeyValue)
        {
            if (!_tableMappings.TryGetValue(tableName, out var mapping))
            {
                MessageBox.Show("Unsupported table selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Type entityType = mapping.EntityType;
            string primaryKeyPropName = mapping.PrimaryKeyPropName;

            // Get the generic Set<T>() method
            MethodInfo setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
            MethodInfo genericSetMethod = setMethod.MakeGenericMethod(entityType);
            var dbSet = genericSetMethod.Invoke(_context, null);

            // Get the entity to delete
            MethodInfo findMethod = typeof(DbSet<>)
                                    .MakeGenericType(entityType)
                                    .GetMethod("Find", new Type[] { typeof(object[]) });

            var entity = findMethod.Invoke(dbSet, new object[] { new object[] { primaryKeyValue } });

            if (entity == null)
            {
                MessageBox.Show("Record not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Remove the entity
            MethodInfo removeMethod = typeof(DbSet<>)
                                       .MakeGenericType(entityType)
                                       .GetMethod("Remove", new Type[] { entityType });
            removeMethod.Invoke(dbSet, new object[] { entity });

            // Save changes
            _context.SaveChanges();

            MessageBox.Show($"{entityType.Name} record deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Helper method to get primary key property name based on table name
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
                    DataGridViewImageColumn imageColumn = dataGridViewTable.Columns["Image"] as DataGridViewImageColumn;
                    if (imageColumn != null)
                    {
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Normal;
                    }
                    dataGridViewTable.RowTemplate.Height = 100;
                    dataGridViewTable.RowTemplate.Resizable = DataGridViewTriState.True;

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
            GetTableRowCount();
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        public void GetTableRowCount() 
        {
            string selectedTable = comboBoxTable.Text;
            labelTableName.Text = selectedTable;
            if (selectedTable.Equals("Employees"))
            {
                labelTableNameRowCount.Text = _context.Employees.Count().ToString();
            }
            if (selectedTable.Equals("Products")) 
            {
                labelTableNameRowCount.Text = _context.Products.Count().ToString();
            }
            if (selectedTable.Equals("Customers"))
            {
                labelTableNameRowCount.Text = _context.Customers.Count().ToString();
            }
            if (selectedTable.Equals("Orders"))
            {
                labelTableNameRowCount.Text = _context.Orders.Count().ToString();
            }
            if (selectedTable.Equals("Region"))
            {
                labelTableNameRowCount.Text = _context.Region.Count().ToString();
            }
            if (selectedTable.Equals("Shippers"))
            {
                labelTableNameRowCount.Text = _context.Shippers.Count().ToString();
            }
            if (selectedTable.Equals("Territories"))
            {
                labelTableNameRowCount.Text = _context.Territories.Count().ToString();
            }
            if (selectedTable.Equals("Suppliers"))
            {
                labelTableNameRowCount.Text = _context.Suppliers.Count().ToString();
            }
            if (selectedTable.Equals("Categories"))
            {
                labelTableNameRowCount.Text = _context.Categories.Count().ToString();
            }
        }
    }
}
