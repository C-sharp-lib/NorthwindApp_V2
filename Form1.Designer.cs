namespace NorthwindApplication
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridViewTable = new DataGridView();
            comboBoxTable = new ComboBox();
            comboBoxColumn = new ComboBox();
            btnExit = new Button();
            btnDeleteRecord = new Button();
            btnUpdateRecord = new Button();
            btnAddForm = new Button();
            button1 = new Button();
            labelTableName = new Label();
            labelTableNameRowCount = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTable).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewTable
            // 
            dataGridViewTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTable.Location = new Point(12, 12);
            dataGridViewTable.Name = "dataGridViewTable";
            dataGridViewTable.Size = new Size(1476, 700);
            dataGridViewTable.TabIndex = 0;
            // 
            // comboBoxTable
            // 
            comboBoxTable.FormattingEnabled = true;
            comboBoxTable.Location = new Point(1528, 55);
            comboBoxTable.Name = "comboBoxTable";
            comboBoxTable.Size = new Size(164, 23);
            comboBoxTable.TabIndex = 1;
            comboBoxTable.SelectedIndexChanged += comboBoxTable_SelectedIndexChanged;
            // 
            // comboBoxColumn
            // 
            comboBoxColumn.FormattingEnabled = true;
            comboBoxColumn.Location = new Point(1528, 84);
            comboBoxColumn.Name = "comboBoxColumn";
            comboBoxColumn.Size = new Size(164, 23);
            comboBoxColumn.TabIndex = 2;
            comboBoxColumn.SelectedIndexChanged += comboBoxColumn_SelectedIndexChanged;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.DarkOrange;
            btnExit.FlatStyle = FlatStyle.Popup;
            btnExit.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(1538, 441);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(154, 53);
            btnExit.TabIndex = 5;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // btnDeleteRecord
            // 
            btnDeleteRecord.BackColor = Color.Red;
            btnDeleteRecord.FlatStyle = FlatStyle.Popup;
            btnDeleteRecord.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteRecord.ForeColor = Color.White;
            btnDeleteRecord.Location = new Point(1538, 382);
            btnDeleteRecord.Name = "btnDeleteRecord";
            btnDeleteRecord.Size = new Size(154, 53);
            btnDeleteRecord.TabIndex = 6;
            btnDeleteRecord.Text = "Delete Record";
            btnDeleteRecord.UseVisualStyleBackColor = false;
            btnDeleteRecord.Click += btnDeleteRecord_Click;
            // 
            // btnUpdateRecord
            // 
            btnUpdateRecord.BackColor = Color.MediumSlateBlue;
            btnUpdateRecord.FlatStyle = FlatStyle.Popup;
            btnUpdateRecord.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUpdateRecord.ForeColor = Color.White;
            btnUpdateRecord.Location = new Point(1538, 323);
            btnUpdateRecord.Name = "btnUpdateRecord";
            btnUpdateRecord.Size = new Size(154, 53);
            btnUpdateRecord.TabIndex = 7;
            btnUpdateRecord.Text = "Update Record";
            btnUpdateRecord.UseVisualStyleBackColor = false;
            btnUpdateRecord.Click += btnUpdateRecord_Click;
            // 
            // btnAddForm
            // 
            btnAddForm.BackColor = Color.LimeGreen;
            btnAddForm.FlatStyle = FlatStyle.Popup;
            btnAddForm.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddForm.ForeColor = Color.White;
            btnAddForm.Location = new Point(1538, 264);
            btnAddForm.Name = "btnAddForm";
            btnAddForm.Size = new Size(154, 53);
            btnAddForm.TabIndex = 8;
            btnAddForm.Text = "Add Record";
            btnAddForm.UseVisualStyleBackColor = false;
            btnAddForm.Click += btnAddForm_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.Teal;
            button1.FlatStyle = FlatStyle.Popup;
            button1.Font = new Font("Verdana", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            button1.ForeColor = Color.White;
            button1.Location = new Point(1538, 205);
            button1.Name = "button1";
            button1.Size = new Size(154, 53);
            button1.TabIndex = 9;
            button1.Text = "Reload";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // labelTableName
            // 
            labelTableName.AutoSize = true;
            labelTableName.Font = new Font("Verdana", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTableName.ForeColor = SystemColors.MenuHighlight;
            labelTableName.Location = new Point(1528, 119);
            labelTableName.Name = "labelTableName";
            labelTableName.Size = new Size(144, 25);
            labelTableName.TabIndex = 10;
            labelTableName.Text = "TableName";
            // 
            // labelTableNameRowCount
            // 
            labelTableNameRowCount.AutoSize = true;
            labelTableNameRowCount.Font = new Font("Verdana", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelTableNameRowCount.ForeColor = Color.OrangeRed;
            labelTableNameRowCount.Location = new Point(1566, 155);
            labelTableNameRowCount.Name = "labelTableNameRowCount";
            labelTableNameRowCount.Size = new Size(77, 23);
            labelTableNameRowCount.TabIndex = 11;
            labelTableNameRowCount.Text = "label2";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1704, 931);
            Controls.Add(labelTableNameRowCount);
            Controls.Add(labelTableName);
            Controls.Add(button1);
            Controls.Add(btnAddForm);
            Controls.Add(btnUpdateRecord);
            Controls.Add(btnDeleteRecord);
            Controls.Add(btnExit);
            Controls.Add(comboBoxColumn);
            Controls.Add(comboBoxTable);
            Controls.Add(dataGridViewTable);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)dataGridViewTable).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridViewTable;
        private ComboBox comboBoxTable;
        private ComboBox comboBoxColumn;
        private Button btnExit;
        private Button btnDeleteRecord;
        private Button btnUpdateRecord;
        private Button btnAddForm;
        private Button button1;
        private Label labelEmp;
        private Label labelCat;
        private Label labelPro;
        private Label labelCus;
        private Label labelOrd;
        private Label labelReg;
        private Label labelSup;
        private Label labelShi;
        private Label labelTer;
        private Label labelTableName;
        private Label labelTableNameRowCount;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
    }
}
