namespace CommanSenderSpace
{
    partial class LogFileConvertForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel_operation2 = new System.Windows.Forms.Panel();
            this.label_leftTime = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_operation2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel_uploadingInfo = new System.Windows.Forms.Panel();
            this.gridTableView = new System.Windows.Forms.DataGridView();
            this.Node_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EstimatedTime_1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Process_1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_startTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_endTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_logDataFile = new System.Windows.Forms.TextBox();
            this.button_export = new System.Windows.Forms.Button();
            this.radioButton_hex = new System.Windows.Forms.RadioButton();
            this.button_logFileSelect = new System.Windows.Forms.Button();
            this.radioButton_xlsx = new System.Windows.Forms.RadioButton();
            this.radioButton_csv = new System.Windows.Forms.RadioButton();
            this.progressBar_convert = new EVO.ProgressbarWithLabel();
            this.panel_container = new System.Windows.Forms.Panel();
            this.panel_operation2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel_operation2.SuspendLayout();
            this.panel_uploadingInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridTableView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel_container.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_operation2
            // 
            this.panel_operation2.Controls.Add(this.label_leftTime);
            this.panel_operation2.Controls.Add(this.groupBox1);
            this.panel_operation2.Location = new System.Drawing.Point(28, 38);
            this.panel_operation2.Margin = new System.Windows.Forms.Padding(2);
            this.panel_operation2.Name = "panel_operation2";
            this.panel_operation2.Size = new System.Drawing.Size(663, 138);
            this.panel_operation2.TabIndex = 89;
            // 
            // label_leftTime
            // 
            this.label_leftTime.AutoSize = true;
            this.label_leftTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_leftTime.Location = new System.Drawing.Point(255, 141);
            this.label_leftTime.Name = "label_leftTime";
            this.label_leftTime.Size = new System.Drawing.Size(0, 15);
            this.label_leftTime.TabIndex = 91;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel_operation2);
            this.groupBox1.Location = new System.Drawing.Point(8, 2);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(633, 131);
            this.groupBox1.TabIndex = 90;
            this.groupBox1.TabStop = false;
            // 
            // tableLayoutPanel_operation2
            // 
            this.tableLayoutPanel_operation2.ColumnCount = 1;
            this.tableLayoutPanel_operation2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_operation2.Controls.Add(this.panel_uploadingInfo, 0, 1);
            this.tableLayoutPanel_operation2.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel_operation2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_operation2.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel_operation2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_operation2.Name = "tableLayoutPanel_operation2";
            this.tableLayoutPanel_operation2.RowCount = 2;
            this.tableLayoutPanel_operation2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_operation2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tableLayoutPanel_operation2.Size = new System.Drawing.Size(629, 114);
            this.tableLayoutPanel_operation2.TabIndex = 96;
            // 
            // panel_uploadingInfo
            // 
            this.panel_uploadingInfo.Controls.Add(this.gridTableView);
            this.panel_uploadingInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_uploadingInfo.Location = new System.Drawing.Point(0, 114);
            this.panel_uploadingInfo.Margin = new System.Windows.Forms.Padding(0);
            this.panel_uploadingInfo.Name = "panel_uploadingInfo";
            this.panel_uploadingInfo.Size = new System.Drawing.Size(629, 1);
            this.panel_uploadingInfo.TabIndex = 95;
            // 
            // gridTableView
            // 
            this.gridTableView.AllowUserToAddRows = false;
            this.gridTableView.AllowUserToDeleteRows = false;
            this.gridTableView.AllowUserToOrderColumns = true;
            this.gridTableView.AllowUserToResizeColumns = false;
            this.gridTableView.AllowUserToResizeRows = false;
            this.gridTableView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridTableView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTableView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Node_Name,
            this.EstimatedTime_1,
            this.Process_1,
            this.Column_startTime,
            this.Column_endTime});
            this.gridTableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridTableView.Enabled = false;
            this.gridTableView.Location = new System.Drawing.Point(0, 0);
            this.gridTableView.Margin = new System.Windows.Forms.Padding(0);
            this.gridTableView.MultiSelect = false;
            this.gridTableView.Name = "gridTableView";
            this.gridTableView.ReadOnly = true;
            this.gridTableView.RowHeadersVisible = false;
            this.gridTableView.Size = new System.Drawing.Size(629, 1);
            this.gridTableView.TabIndex = 95;
            // 
            // Node_Name
            // 
            this.Node_Name.FillWeight = 50F;
            this.Node_Name.HeaderText = "Node";
            this.Node_Name.Name = "Node_Name";
            this.Node_Name.ReadOnly = true;
            // 
            // EstimatedTime_1
            // 
            this.EstimatedTime_1.HeaderText = "Estimated Time";
            this.EstimatedTime_1.Name = "EstimatedTime_1";
            this.EstimatedTime_1.ReadOnly = true;
            // 
            // Process_1
            // 
            this.Process_1.HeaderText = "Process";
            this.Process_1.Name = "Process_1";
            this.Process_1.ReadOnly = true;
            // 
            // Column_startTime
            // 
            this.Column_startTime.HeaderText = "Start Time";
            this.Column_startTime.Name = "Column_startTime";
            this.Column_startTime.ReadOnly = true;
            // 
            // Column_endTime
            // 
            this.Column_endTime.HeaderText = "End Time";
            this.Column_endTime.Name = "Column_endTime";
            this.Column_endTime.ReadOnly = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox_logDataFile);
            this.panel1.Controls.Add(this.button_export);
            this.panel1.Controls.Add(this.radioButton_hex);
            this.panel1.Controls.Add(this.button_logFileSelect);
            this.panel1.Controls.Add(this.radioButton_xlsx);
            this.panel1.Controls.Add(this.radioButton_csv);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(629, 114);
            this.panel1.TabIndex = 95;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 93;
            this.label1.Text = "LogData File:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 45);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 94;
            this.label2.Text = "Export File Format:";
            // 
            // textBox_logDataFile
            // 
            this.textBox_logDataFile.Location = new System.Drawing.Point(104, 16);
            this.textBox_logDataFile.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_logDataFile.Name = "textBox_logDataFile";
            this.textBox_logDataFile.ReadOnly = true;
            this.textBox_logDataFile.Size = new System.Drawing.Size(402, 20);
            this.textBox_logDataFile.TabIndex = 88;
            // 
            // button_export
            // 
            this.button_export.Location = new System.Drawing.Point(217, 78);
            this.button_export.Margin = new System.Windows.Forms.Padding(2);
            this.button_export.Name = "button_export";
            this.button_export.Size = new System.Drawing.Size(94, 29);
            this.button_export.TabIndex = 86;
            this.button_export.Text = "Export";
            this.button_export.UseVisualStyleBackColor = true;
            this.button_export.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // radioButton_hex
            // 
            this.radioButton_hex.AutoSize = true;
            this.radioButton_hex.Location = new System.Drawing.Point(179, 43);
            this.radioButton_hex.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_hex.Name = "radioButton_hex";
            this.radioButton_hex.Size = new System.Drawing.Size(47, 17);
            this.radioButton_hex.TabIndex = 92;
            this.radioButton_hex.TabStop = true;
            this.radioButton_hex.Text = "HEX";
            this.radioButton_hex.UseVisualStyleBackColor = true;
            // 
            // button_logFileSelect
            // 
            this.button_logFileSelect.Location = new System.Drawing.Point(508, 11);
            this.button_logFileSelect.Margin = new System.Windows.Forms.Padding(2);
            this.button_logFileSelect.Name = "button_logFileSelect";
            this.button_logFileSelect.Size = new System.Drawing.Size(66, 29);
            this.button_logFileSelect.TabIndex = 87;
            this.button_logFileSelect.Text = "Select";
            this.button_logFileSelect.UseVisualStyleBackColor = true;
            this.button_logFileSelect.Click += new System.EventHandler(this.LogfileSelectButtion_ClickEvent);
            // 
            // radioButton_xlsx
            // 
            this.radioButton_xlsx.AutoSize = true;
            this.radioButton_xlsx.Location = new System.Drawing.Point(230, 43);
            this.radioButton_xlsx.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_xlsx.Name = "radioButton_xlsx";
            this.radioButton_xlsx.Size = new System.Drawing.Size(52, 17);
            this.radioButton_xlsx.TabIndex = 91;
            this.radioButton_xlsx.TabStop = true;
            this.radioButton_xlsx.Text = "XLSX";
            this.radioButton_xlsx.UseVisualStyleBackColor = true;
            this.radioButton_xlsx.Visible = false;
            // 
            // radioButton_csv
            // 
            this.radioButton_csv.AutoSize = true;
            this.radioButton_csv.Location = new System.Drawing.Point(128, 43);
            this.radioButton_csv.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_csv.Name = "radioButton_csv";
            this.radioButton_csv.Size = new System.Drawing.Size(46, 17);
            this.radioButton_csv.TabIndex = 90;
            this.radioButton_csv.TabStop = true;
            this.radioButton_csv.Text = "CSV";
            this.radioButton_csv.UseVisualStyleBackColor = true;
            // 
            // progressBar_convert
            // 
            this.progressBar_convert.Location = new System.Drawing.Point(28, 196);
            this.progressBar_convert.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar_convert.Name = "progressBar_convert";
            this.progressBar_convert.Size = new System.Drawing.Size(639, 19);
            this.progressBar_convert.TabIndex = 82;
            this.progressBar_convert.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // panel_container
            // 
            this.panel_container.Controls.Add(this.panel_operation2);
            this.panel_container.Controls.Add(this.progressBar_convert);
            this.panel_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_container.Location = new System.Drawing.Point(0, 0);
            this.panel_container.Margin = new System.Windows.Forms.Padding(2);
            this.panel_container.Name = "panel_container";
            this.panel_container.Size = new System.Drawing.Size(726, 257);
            this.panel_container.TabIndex = 89;
            // 
            // LogFileConvertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 257);
            this.Controls.Add(this.panel_container);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LogFileConvertForm";
            this.Text = "Log File Converter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogFileParseFormClosing);
            this.Load += new System.EventHandler(this.LogFileParseForm_Load);
            this.panel_operation2.ResumeLayout(false);
            this.panel_operation2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel_operation2.ResumeLayout(false);
            this.panel_uploadingInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridTableView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel_container.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_operation2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radioButton_hex;
        private System.Windows.Forms.RadioButton radioButton_xlsx;
        private System.Windows.Forms.RadioButton radioButton_csv;
        private System.Windows.Forms.Button button_logFileSelect;
        private System.Windows.Forms.Button button_export;
        private System.Windows.Forms.TextBox textBox_logDataFile;
        private EVO.ProgressbarWithLabel progressBar_convert;
        private System.Windows.Forms.Panel panel_container;
        private System.Windows.Forms.Label label_leftTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_operation2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel_uploadingInfo;
        private System.Windows.Forms.DataGridView gridTableView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Node_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn EstimatedTime_1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Process_1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_startTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_endTime;

    }
}

