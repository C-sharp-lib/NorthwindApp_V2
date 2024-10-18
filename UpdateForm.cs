using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NorthwindApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindApplication
{
    public partial class UpdateForm : Form
    {
        //private List<Control> inputControls = new List<Control>();
        private readonly string _tableName;
        private readonly object _primaryKeyValue;
        private readonly ApplicationDbContext _context;
        private readonly Type _entityType;
        private readonly string _primaryKeyName;
        private object _entity; // The entity to update
        private Dictionary<string, byte[]> _byteProperties = new Dictionary<string, byte[]>();
        public UpdateForm(ApplicationDbContext context, string tableName, object primaryKeyValue)
        {
            InitializeComponent();
            _context = context;
            _tableName = tableName;
            _primaryKeyValue = primaryKeyValue;

            // Initialize entity type and primary key name
            _entityType = GetEntityType(_tableName);
            _primaryKeyName = GetPrimaryKeyName(_entityType);

            if (_entityType == null || string.IsNullOrEmpty(_primaryKeyName))
            {
                MessageBox.Show("Invalid table or primary key.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
           
            LoadEntityData();
        }
        private Type GetEntityType(string tableName)
        {
            var dbSetProperty = typeof(ApplicationDbContext).GetProperty(tableName);
            return dbSetProperty?.PropertyType.GetGenericArguments().FirstOrDefault();
        }

        private string GetPrimaryKeyName(Type entityType)
        {
            if (entityType == null)
            {
                MessageBox.Show("Entity type is null.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var entityTypeMetadata = _context.Model.FindEntityType(entityType);
            if (entityTypeMetadata == null)
            {
                MessageBox.Show($"Entity type '{entityType.Name}' not found in the EF Core model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            var primaryKey = entityTypeMetadata.FindPrimaryKey();
            if (primaryKey == null)
            {
                MessageBox.Show($"Primary key not defined for entity '{entityType.Name}'.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            // Check if there's exactly one primary key property
            if (primaryKey.Properties.Count > 1)
            {
                MessageBox.Show($"Entity '{entityType.Name}' has a composite primary key. This implementation supports only single primary keys.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return primaryKey.Properties.FirstOrDefault()?.Name;
        }

        private void LoadEntityData()
        {
            try
            {
                // Get the generic method 'Set<TEntity>()' from DbContext
                MethodInfo setMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes);
                MethodInfo genericSetMethod = setMethod.MakeGenericMethod(_entityType);

                // Invoke 'Set<TEntity>()' to get the DbSet<TEntity>
                object dbSet = genericSetMethod.Invoke(_context, null);

                // Get the 'Find' method from DbSet<TEntity>
                MethodInfo findMethod = dbSet.GetType().GetMethod("Find", new Type[] { typeof(object[]) });

                // Invoke 'Find' with the primary key value
                _entity = findMethod.Invoke(dbSet, new object[] { new object[] { _primaryKeyValue } });

                if (_entity == null)
                {
                    MessageBox.Show("Record not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // Dynamically create controls based on entity properties
                CreateDynamicControls();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }



        private void CreateDynamicControls()
        {

            // Get all scalar properties excluding the primary key
            var properties = _context.Model.FindEntityType(_entityType).GetProperties()
                .Where(p => !p.IsPrimaryKey() && !p.IsShadowProperty())
                .ToList();

            int currentY = 10; // Starting Y position
            int spacing = 35; // Space between controls

            int index = 0;
            while (index < properties.Count)
            {
                var prop = properties[index];

                // Create label
                Label lbl = new Label
                {
                    Text = prop.Name,
                    AutoSize = true,
                    Location = new Point(50, currentY + 5),
                    Width = 80
                };

                // Create input control
                Control inputControl = GetControlForProperty(prop);
                inputControl.Location = new Point(200, currentY);
                inputControl.Height = 25;

                // Add controls to the panel
                this.Controls.Add(lbl);
                this.Controls.Add(inputControl);

                // Increment Y position for the next set of controls
                currentY += spacing;

                index++;
            }
            Button updateButton = new Button();
            updateButton.Location = new Point(100, 600);
            updateButton.Text = "Update Record";
            updateButton.Width = 161;
            updateButton.Height = 51;
            updateButton.BackColor = System.Drawing.Color.DodgerBlue;
            updateButton.ForeColor = System.Drawing.Color.White;
            updateButton.FlatStyle = FlatStyle.Popup;
            updateButton.Click += btnSave_Click;
            this.Controls.Add(updateButton);
        }


        private Control GetControlForProperty(IProperty prop)
        {
            var propertyType = Nullable.GetUnderlyingType(prop.ClrType) ?? prop.ClrType;
            object value = _entity.GetType().GetProperty(prop.Name).GetValue(_entity);

            switch (Type.GetTypeCode(propertyType))
            {
                case TypeCode.String:
                    return new TextBox
                    {
                        Width = 200,
                        Tag = prop.Name,
                        Text = value?.ToString(),
                        Margin = new Padding(3, 3, 3, 3),
                    };
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return new TextBox
                    {
                        Width = 200,
                        Tag = prop.Name,
                        Text = value?.ToString(),
                        Margin = new Padding(3, 3, 3, 3)
                    };
                case TypeCode.Boolean:
                    return new CheckBox
                    {
                        Tag = prop.Name,
                        Checked = value != null ? (bool)value : false,
                        Margin = new Padding(3, 3, 3, 3)
                    };
                case TypeCode.DateTime:
                    return new DateTimePicker
                    {
                        Width = 200,
                        Tag = prop.Name,
                        Value = value != null ? (DateTime)value : DateTime.Now,
                        Margin = new Padding(3, 3, 3, 3)
                    };
                default:
                    if (propertyType.IsEnum)
                    {
                        return new ComboBox
                        {
                            Width = 200,
                            Tag = prop.Name,
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            DataSource = Enum.GetValues(propertyType),
                            SelectedItem = value,
                            Margin = new Padding(3, 3, 3, 3)
                        };
                    }
                    else if (propertyType == typeof(Guid))
                    {
                        return new TextBox
                        {
                            Width = 200,
                            Tag = prop.Name,
                            Text = value?.ToString(),
                            Margin = new Padding(3, 3, 3, 3)
                        };
                    }
                    else
                    {
                        // For other types, use TextBox as default
                        return new TextBox
                        {
                            Width = 200,
                            Tag = prop.Name,
                            Text = value?.ToString(),
                            Margin = new Padding(3, 3, 3, 3)
                        };
                    }
            }
        }

        private void SaveChanges() 
        {
            
        }
            private void UpdateEntity<T>(object primaryKey, Action<T> updateAction) where T : class
            {
                var entity = _context.Set<T>().Find(primaryKey);
                if (entity == null)
                {
                    MessageBox.Show($"{typeof(T).Name} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                updateAction(entity);

                try
                {
                    _context.SaveChanges();
                    MessageBox.Show($"{typeof(T).Name} updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the record: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            private bool ValidateInputs()
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag == null) continue;

                string propName = control.Tag.ToString();
                var property = _entityType.GetProperty(propName);
                if (property == null) continue;

                // Retrieve the property metadata from EF Core
                var efProperty = _context.Model.FindEntityType(_entityType)?.FindProperty(propName);
                bool isRequired = efProperty?.IsNullable == false;

                // Check if the property is required and if the corresponding control is empty
                if (isRequired && (control is TextBox txtBox && string.IsNullOrWhiteSpace(txtBox.Text)))
                {
                    MessageBox.Show($"{propName} is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                // Add more validation rules as needed
            }

            return true;
        }


        private void BtnBrowse_Click(object sender, EventArgs e, TextBox txtBox, string propName)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = $"Select a file for {propName}";
                openFileDialog.Filter = "All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    txtBox.Text = filePath;

                    // Optionally, display the image if the property represents an image
                    PictureBox pb = this.Controls.Find($"pictureBox{propName}", true).FirstOrDefault() as PictureBox;
                    if (pb != null && IsImageFile(filePath))
                    {
                        pb.Image = Image.FromFile(filePath);
                    }
                }
            }
        }

        private bool IsImageFile(string path)
        {
            string extension = Path.GetExtension(path).ToLower();
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return imageExtensions.Contains(extension);
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_tableName.Equals("Employees", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Employees>(_primaryKeyValue, employee =>
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null) continue;
                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Employees).GetProperty(propName);
                            if (prop == null) continue;
                            if (prop.PropertyType == typeof(byte[]))
                            {
                                Panel panel = control as Panel;
                                if (panel != null)
                                {
                                    TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                    DateTimePicker dtPicker = panel.Controls.OfType<DateTimePicker>().FirstOrDefault();
                                    NumericUpDown numeric = panel.Controls.OfType<NumericUpDown>().FirstOrDefault();
                                    if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                    {
                                        employee.Photo = File.ReadAllBytes(txtBox.Text);
                                        employee.FirstName = txtBox.Text;
                                        employee.LastName = txtBox.Text;
                                        employee.Address = txtBox.Text;
                                        employee.City = txtBox.Text;
                                        employee.Region = txtBox.Text;
                                        employee.PostalCode = txtBox.Text;
                                        employee.Country = txtBox.Text;
                                        employee.BirthDate = dtPicker.Value;
                                        employee.HireDate = dtPicker.Value;
                                        employee.Title = txtBox.Text;
                                        employee.PhotoPath = txtBox.Text;
                                        employee.Extension = txtBox.Text;
                                        employee.HomePhone = txtBox.Text;
                                        employee.ReportsTo = (int?)numeric.Value;
                                        employee.Notes = txtBox.Text;
                                        employee.TitleOfCourtesy = txtBox.Text;
                                        PictureBox pb = this.Controls.Find($"pictureBox{propName}", true).FirstOrDefault() as PictureBox;
                                        if (pb != null)
                                        {
                                            pb.Image = Image.FromFile(txtBox.Text);
                                        }
                                    }
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(employee, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                CheckBox chkBox = control as CheckBox;
                                if (chkBox != null)
                                {
                                    prop.SetValue(employee, chkBox.Checked);
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTimePicker dtPicker = control as DateTimePicker;
                                if (dtPicker != null)
                                {
                                    prop.SetValue(employee, dtPicker.Value);
                                }
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                NumericUpDown numeric = control as NumericUpDown;
                                if (numeric != null)
                                {
                                    prop.SetValue(employee, numeric.Value);
                                }
                            }
                        }

                        _context.Employees.Update(employee);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Customers", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Customers>(_primaryKeyValue, customer =>
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null) continue;
                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Customers).GetProperty(propName);
                            if (prop == null) continue;
                            // Handle byte[] properties (e.g., ProfileImage)
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    customer.CompanyName = txtBox.Text;
                                    customer.ContactName = txtBox.Text;
                                    customer.ContactTitle = txtBox.Text;
                                    customer.Address = txtBox.Text;
                                    customer.City = txtBox.Text;
                                    customer.Region = txtBox.Text;
                                    customer.PostalCode = txtBox.Text;
                                    customer.Country = txtBox.Text;
                                    customer.Phone = txtBox.Text;
                                    customer.Fax = txtBox.Text;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(customer, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                CheckBox chkBox = control as CheckBox;
                                if (chkBox != null)
                                {
                                    prop.SetValue(customer, chkBox.Checked);
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTimePicker dtPicker = control as DateTimePicker;
                                if (dtPicker != null)
                                {
                                    prop.SetValue(customer, dtPicker.Value);
                                }
                            }
                        }
                        _context.Customers.Update(customer);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Orders", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Orders>(_primaryKeyValue, orders =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Orders).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                DateTimePicker dtPick = panel.Controls.OfType<DateTimePicker>().FirstOrDefault();
                                NumericUpDown numeric = panel.Controls.OfType<NumericUpDown>().FirstOrDefault();
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    orders.OrderDate = dtPick.Value;
                                    orders.RequiredDate = dtPick.Value;
                                    orders.ShippedDate = dtPick.Value;
                                    orders.ShipVia = (int?)numeric.Value;
                                    orders.Freight = (decimal?)numeric.Value;
                                    orders.ShipName = txtBox.Text;
                                    orders.ShipAddress = txtBox.Text;
                                    orders.ShipCity = txtBox.Text;
                                    orders.ShipRegion = txtBox.Text;
                                    orders.ShipPostalCode = txtBox.Text;
                                    orders.ShipCountry = txtBox.Text;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(orders, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                CheckBox chkBox = control as CheckBox;
                                if (chkBox != null)
                                {
                                    prop.SetValue(orders, chkBox.Checked);
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTimePicker dtPicker = control as DateTimePicker;
                                if (dtPicker != null)
                                {
                                    prop.SetValue(orders, dtPicker.Value);
                                }
                            }
                    }
                    _context.Orders.Update(orders);
                    _context.SaveChanges();
                });
                }
                else if (_tableName.Equals("Products", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Products>(_primaryKeyValue, product =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Products).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                DateTimePicker dtPick = panel.Controls.OfType<DateTimePicker>().FirstOrDefault();
                                NumericUpDown numeric = panel.Controls.OfType<NumericUpDown>().FirstOrDefault();
                                CheckBox check = panel.Controls.OfType<CheckBox>().FirstOrDefault();
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    product.ProductName = txtBox.Text;
                                    product.QuantityPerUnit = txtBox.Text;
                                    product.UnitPrice = (decimal?)numeric.Value;
                                    product.UnitsInStock = (short?)numeric.Value;
                                    product.UnitsOnOrder = (short?)numeric.Value;
                                    product.ReorderLevel = (short?)numeric.Value;
                                    product.Discontinued = check.Checked;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(product, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                CheckBox chkBox = control as CheckBox;
                                if (chkBox != null)
                                {
                                    prop.SetValue(product, chkBox.Checked);
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTimePicker dtPicker = control as DateTimePicker;
                                if (dtPicker != null)
                                {
                                    prop.SetValue(product, dtPicker.Value);
                                }
                            }
                        }
                        _context.Products.Update(product);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Categories", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Categories>(_primaryKeyValue, category =>
                    {
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null) continue;
                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Categories).GetProperty(propName);
                            if (prop == null) continue;
                            if (prop.PropertyType == typeof(byte[]))
                            {
                                Panel panel = control as Panel;
                                if (panel != null)
                                {
                                    TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                    DateTimePicker dtPicker = panel.Controls.OfType<DateTimePicker>().FirstOrDefault();
                                    NumericUpDown numeric = panel.Controls.OfType<NumericUpDown>().FirstOrDefault();
                                    if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                    {
                                        category.Picture = File.ReadAllBytes(txtBox.Text);
                                        category.CategoryName = txtBox.Text;
                                        category.Description = txtBox.Text;

                                        PictureBox pb = this.Controls.Find($"pictureBox{propName}", true).FirstOrDefault() as PictureBox;
                                        if (pb != null)
                                        {
                                            pb.Image = Image.FromFile(txtBox.Text);
                                        }
                                    }
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(category, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(bool))
                            {
                                CheckBox chkBox = control as CheckBox;
                                if (chkBox != null)
                                {
                                    prop.SetValue(category, chkBox.Checked);
                                }
                            }
                            else if (prop.PropertyType == typeof(DateTime))
                            {
                                DateTimePicker dtPicker = control as DateTimePicker;
                                if (dtPicker != null)
                                {
                                    prop.SetValue(category, dtPicker.Value);
                                }
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                NumericUpDown numeric = control as NumericUpDown;
                                if (numeric != null)
                                {
                                    prop.SetValue(category, numeric.Value);
                                }
                            }
                        }

                        _context.Categories.Update(category);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Region", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Models.Region>(_primaryKeyValue, region =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Models.Region).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    region.RegionDescription = txtBox.Text;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(region, txtBox.Text.Trim());
                                }
                            }
                        }
                        _context.Region.Update(region);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Shippers", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Shippers>(_primaryKeyValue, shippers =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Shippers).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                               
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    shippers.CompanyName = txtBox.Text;
                                    shippers.Phone = txtBox.Text;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(shippers, txtBox.Text.Trim());
                                }
                            }
                        }
                        _context.Shippers.Update(shippers);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Suppliers", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Suppliers>(_primaryKeyValue, supplier =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Suppliers).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    supplier.CompanyName = txtBox.Text;
                                    supplier.ContactName = txtBox.Text;
                                    supplier.ContactTitle = txtBox.Text;
                                    supplier.Address = txtBox.Text;
                                    supplier.City = txtBox.Text;
                                    supplier.Region = txtBox.Text;
                                    supplier.PostalCode = txtBox.Text;
                                    supplier.Country = txtBox.Text;
                                    supplier.Phone = txtBox.Text;
                                    supplier.Fax = txtBox.Text;
                                    supplier.HomePage = txtBox.Text;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(supplier, txtBox.Text.Trim());
                                }
                            }
                        }
                        _context.Suppliers.Update(supplier);
                        _context.SaveChanges();
                    });
                }
                else if (_tableName.Equals("Territories", StringComparison.OrdinalIgnoreCase))
                {
                    UpdateEntity<Territories>(_primaryKeyValue, territory =>
                    {
                        // Iterate through all controls in panelFields
                        foreach (Control control in this.Controls)
                        {
                            if (control.Tag == null)
                                continue; // Skip controls without a Tag

                            string propName = control.Tag.ToString();
                            PropertyInfo prop = typeof(Territories).GetProperty(propName);
                            if (prop == null)
                                continue; // Skip if the property does not exist
                            Panel panel = control as Panel;
                            if (panel != null)
                            {
                                TextBox txtBox = panel.Controls.OfType<TextBox>().FirstOrDefault();
                                NumericUpDown numeric = panel.Controls.OfType<NumericUpDown>().FirstOrDefault();
                                if (txtBox != null && !string.IsNullOrEmpty(txtBox.Text))
                                {
                                    territory.TerritoryDescription = txtBox.Text;
                                    territory.RegionID = (int)numeric.Value;
                                }
                            }
                            else if (prop.PropertyType == typeof(string))
                            {
                                TextBox txtBox = control as TextBox;
                                if (txtBox != null)
                                {
                                    prop.SetValue(territory, txtBox.Text.Trim());
                                }
                            }
                            else if (prop.PropertyType == typeof(int))
                            {
                                NumericUpDown numeric = control as NumericUpDown;
                                if (numeric != null)
                                {
                                    prop.SetValue(territory, numeric.Value);
                                }
                            }
                        }
                        _context.Territories.Update(territory);
                        _context.SaveChanges();
                    });
                }
                else
                {
                    MessageBox.Show("Unsupported table selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
