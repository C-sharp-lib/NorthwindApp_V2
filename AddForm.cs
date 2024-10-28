using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic.Logging;
using NorthwindApplication.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthwindApplication
{
    public partial class AddForm : Form
    {
        private readonly ApplicationDbContext _context;
        private readonly string _tableName;
        private readonly Type _entityType;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private readonly Dictionary<PropertyInfo, Control> _propertyControlMappings = new Dictionary<PropertyInfo, Control>();
        private readonly Dictionary<string, Type> _tableMappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "Employees", typeof(Employees) },
            { "Customers", typeof(Customers) },
            { "Products", typeof(Products)},
            { "Orders", typeof(Orders)},
            { "Region", typeof(Models.Region)},
            { "Shippers", typeof(Shippers)},
            { "Suppliers", typeof(Suppliers)},
            { "Territories", typeof(Territories)},
            // Add more mappings for other tables...
        };
        private ApplicationDbContext context;
        private string selectedTable;

        public AddForm(ApplicationDbContext context, string selectedTable)
        {
            InitializeComponent();
            _context = context;
            _tableName = selectedTable;
            _entityType = _context.Model.GetEntityTypes().FirstOrDefault(t => t.GetTableName() == _tableName)?.ClrType;
            if ( _entityType == null )
            {
                MessageBox.Show("Invalid Table Name");
                this.Close();
                return;
            }
            GenerateDynamicControls();
        }

        private void GenerateDynamicControls()
        {
            // Use reflection to get properties
            var properties = _entityType.GetProperties()
                .Where(p => !p.Name.Equals("EmployeeID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("OrderID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("CategoryID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("CustomerID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("ProductID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("RegionID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("ShipperID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("SupplierID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("TerritoryID", StringComparison.OrdinalIgnoreCase))
                .ToList();

            int yPosition = 20;

            foreach (var property in properties)
            {
                Label label = new Label
                {
                    Text = property.Name,
                    Location = new System.Drawing.Point(20, yPosition),
                    AutoSize = true
                };
                this.Controls.Add(label);
                Control inputControl = null;
                if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                {
                    inputControl = new DateTimePicker
                    {
                        Name = property.Name,
                        Location = new System.Drawing.Point(120, yPosition),
                        Width = 200
                    };
                    this.Controls.Add(inputControl);
                }
                else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?)) 
                {
                    inputControl = new NumericUpDown
                    {
                        Name = property.Name,
                        Location = new System.Drawing.Point(120, yPosition),
                        Width = 200
                    };
                    this.Controls.Add(inputControl);
                }
                else if (property.PropertyType == typeof(byte[]))
                {
                    inputControl = new Button
                    {
                        Text = "Select An Image",
                        Name = property.Name,
                        Location = new System.Drawing.Point(120, yPosition),
                        Width = 200
                    };
                    inputControl.Click += (s, e) => SelectImage((Button)s);
                    PictureBox pictureBox = new PictureBox
                    {
                        Name = $"{property.Name}_PictureBox",
                        Location = new System.Drawing.Point(360, yPosition),
                        Size = new System.Drawing.Size(200, 150),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BorderStyle = BorderStyle.Fixed3D
                    };
                    this.Controls.Add(pictureBox);
                    this.Controls.Add(inputControl);
                    yPosition += 30;
                }
                else
                {
                    inputControl = new TextBox
                    {
                        Name = property.Name,
                        Location = new System.Drawing.Point(120, yPosition),
                        Width = 200
                    };
                }
                if (inputControl != null) 
                { 
                    this.Controls.Add(inputControl);
                }
                yPosition += 30;
            }

            Button btnSave = new Button
            {
                Text = "Save",
                Location = new System.Drawing.Point(120, yPosition)
            };
            btnSave.Click += BtnSave_Click;

            this.Controls.Add(btnSave);
        }

        private void SelectImage(Button button)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp";
                openFileDialog.Title = "Select an Image";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Store the selected image path as the button's tag value
                    byte[] imageBytes = File.ReadAllBytes(openFileDialog.FileName);
                    //Send the byte array tou the equivelant of SQL Server Image
                    button.Tag = imageBytes;
                    button.Text = "Image Selected";

                    // Find the associated PictureBox and update its Image property
                    var pictureBox = this.Controls.Find($"{button.Name}_PictureBox", true).FirstOrDefault() as PictureBox;
                    if (pictureBox != null)
                    {
                        pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                    }
                }
            }
        }


        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var entity = Activator.CreateInstance(_entityType);
                var properties = _entityType.GetProperties()
                .Where(p => !p.Name.Equals("EmployeeID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("OrderID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("CategoryID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("CustomerID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("ProductID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("RegionID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("ShipperID", StringComparison.OrdinalIgnoreCase))
                .Where(p => !p.Name.Equals("SupplierID", StringComparison.OrdinalIgnoreCase))
                 .Where(p => !p.Name.Equals("TerritoryID", StringComparison.OrdinalIgnoreCase))
                .ToList();
               
                foreach (var property in _entityType.GetProperties())
                    {

                    var control = this.Controls.Find(property.Name, true).FirstOrDefault();
                    if (control == null) continue;

                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        DateTimePicker? dateTimePicker = control as DateTimePicker;
                        property.SetValue(entity, dateTimePicker?.Value);
                    }
                    else if (property.PropertyType == typeof(byte[]))
                    {
                        Button? button = control as Button;
                        byte[]? imageBytes = button?.Tag as byte[];
                        property.SetValue(entity, imageBytes);
                    }
                    //else if (property.PropertyType == typeof(string) && property.Name.Equals("PhotoPath"))
                    //{
                    //    Button? button = control as Button;
                    //    property.SetValue(entity, button?.Tag?.ToString());
                    //}
                    else if (property.PropertyType == typeof(string))
                    {
                        TextBox? textBox = control as TextBox;
                        property.SetValue(entity, textBox?.Text);
                    }
                    else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    {
                        NumericUpDown? numeric = control as NumericUpDown;
                        property.SetValue(entity, Convert.ToInt32(numeric?.Value));
                    }
                    else if (property.PropertyType == typeof(double) || property.PropertyType == typeof(double?))
                    {
                        TextBox? textBox = control as TextBox;
                        if (double.TryParse(textBox?.Text, out double doubleValue))
                        {
                            property.SetValue(entity, doubleValue);
                        }
                    }
                    else if (property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)) 
                    {
                        TextBox? textBox = control as TextBox;
                        if (decimal.TryParse(textBox?.Text, out decimal decimalValue))
                        {
                            property.SetValue(entity, decimalValue);
                        }
                    }
                }
                _context.Add(entity);
                _context.SaveChanges();
                MessageBox.Show("Record added successfully!");
                this.Refresh();
                this.Close();
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}