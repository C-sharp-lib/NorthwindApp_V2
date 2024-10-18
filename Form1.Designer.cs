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
            btnLoadData = new Button();
            btnExit = new Button();
            btnDeleteRecord = new Button();
            btnUpdateRecord = new Button();
            btnAddForm = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewTable).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewTable
            // 
            dataGridViewTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewTable.Location = new Point(12, 12);
            dataGridViewTable.Name = "dataGridViewTable";
            dataGridViewTable.Size = new Size(606, 580);
            dataGridViewTable.TabIndex = 0;
            // 
            // comboBoxTable
            // 
            comboBoxTable.FormattingEnabled = true;
            comboBoxTable.Location = new Point(624, 29);
            comboBoxTable.Name = "comboBoxTable";
            comboBoxTable.Size = new Size(164, 23);
            comboBoxTable.TabIndex = 1;
            comboBoxTable.SelectedIndexChanged += comboBoxTable_SelectedIndexChanged;
            // 
            // comboBoxColumn
            // 
            comboBoxColumn.FormattingEnabled = true;
            comboBoxColumn.Location = new Point(624, 60);
            comboBoxColumn.Name = "comboBoxColumn";
            comboBoxColumn.Size = new Size(164, 23);
            comboBoxColumn.TabIndex = 2;
            comboBoxColumn.SelectedIndexChanged += comboBoxColumn_SelectedIndexChanged;
            // 
            // btnLoadData
            // 
            btnLoadData.BackColor = Color.DodgerBlue;
            btnLoadData.FlatStyle = FlatStyle.Popup;
            btnLoadData.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLoadData.ForeColor = Color.White;
            btnLoadData.Location = new Point(634, 206);
            btnLoadData.Name = "btnLoadData";
            btnLoadData.Size = new Size(154, 53);
            btnLoadData.TabIndex = 4;
            btnLoadData.Text = "Load Data";
            btnLoadData.UseVisualStyleBackColor = false;
            btnLoadData.Click += btnLoadData_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.DarkOrange;
            btnExit.FlatStyle = FlatStyle.Popup;
            btnExit.Font = new Font("Verdana", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(634, 442);
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
            btnDeleteRecord.Location = new Point(634, 383);
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
            btnUpdateRecord.Location = new Point(634, 324);
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
            btnAddForm.Location = new Point(634, 265);
            btnAddForm.Name = "btnAddForm";
            btnAddForm.Size = new Size(154, 53);
            btnAddForm.TabIndex = 8;
            btnAddForm.Text = "Add Record";
            btnAddForm.UseVisualStyleBackColor = false;
            btnAddForm.Click += btnAddForm_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 604);
            Controls.Add(btnAddForm);
            Controls.Add(btnUpdateRecord);
            Controls.Add(btnDeleteRecord);
            Controls.Add(btnExit);
            Controls.Add(btnLoadData);
            Controls.Add(comboBoxColumn);
            Controls.Add(comboBoxTable);
            Controls.Add(dataGridViewTable);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)dataGridViewTable).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewTable;
        private ComboBox comboBoxTable;
        private ComboBox comboBoxColumn;
        private Button btnLoadData;
        private Button btnExit;
        private Button btnDeleteRecord;
        private Button btnUpdateRecord;
        private Button btnAddForm;
    }
}
