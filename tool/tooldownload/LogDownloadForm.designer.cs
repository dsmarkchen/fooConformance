namespace CommanSenderSpace
{
    partial class LoggingDownloadForm
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
            this.listBox_Info = new System.Windows.Forms.ListBox();
            this.panel_operation = new System.Windows.Forms.Panel();
            this.button_EraseLog = new System.Windows.Forms.Button();
            this.button_close = new System.Windows.Forms.Button();
            this.button_action = new System.Windows.Forms.Button();
            this.button_uncheck = new System.Windows.Forms.Button();
            this.button_check = new System.Windows.Forms.Button();
            this.button_refresh = new System.Windows.Forms.Button();
            this.panel_container = new System.Windows.Forms.Panel();
            this.tableLayoutPanel_process = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.process_display1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar_part = new EVO.ProgressbarWithLabel();
            this.label_remainingTime = new System.Windows.Forms.Label();
            this.process_display2 = new System.Windows.Forms.Panel();
            this.progressBar_total = new EVO.ProgressbarWithLabel();
            this.label_remainingTime_total = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelBox_logDir = new EVO.LabelBox();
            this.panel_operation3 = new System.Windows.Forms.Panel();
            this.radioButton_erase = new System.Windows.Forms.RadioButton();
            this.radioButton_upload = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel_message = new System.Windows.Forms.TableLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel_operation2 = new System.Windows.Forms.Panel();
            this.menuStripAll = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.converterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infomationBoxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel_operation.SuspendLayout();
            this.panel_container.SuspendLayout();
            this.tableLayoutPanel_process.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.process_display1.SuspendLayout();
            this.process_display2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel_operation3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tableLayoutPanel_message.SuspendLayout();
            this.panel_operation2.SuspendLayout();
            this.menuStripAll.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox_Info
            // 
            this.listBox_Info.FormattingEnabled = true;
            this.listBox_Info.Location = new System.Drawing.Point(4, 0);
            this.listBox_Info.Margin = new System.Windows.Forms.Padding(0);
            this.listBox_Info.Name = "listBox_Info";
            this.listBox_Info.Size = new System.Drawing.Size(749, 147);
            this.listBox_Info.TabIndex = 64;
            // 
            // panel_operation
            // 
            this.panel_operation.Controls.Add(this.button_EraseLog);
            this.panel_operation.Location = new System.Drawing.Point(757, 7);
            this.panel_operation.Margin = new System.Windows.Forms.Padding(2);
            this.panel_operation.Name = "panel_operation";
            this.panel_operation.Size = new System.Drawing.Size(91, 39);
            this.panel_operation.TabIndex = 85;
            this.panel_operation.Visible = false;
            // 
            // button_EraseLog
            // 
            this.button_EraseLog.AutoSize = true;
            this.button_EraseLog.Location = new System.Drawing.Point(2, 4);
            this.button_EraseLog.Margin = new System.Windows.Forms.Padding(2);
            this.button_EraseLog.Name = "button_EraseLog";
            this.button_EraseLog.Size = new System.Drawing.Size(80, 29);
            this.button_EraseLog.TabIndex = 72;
            this.button_EraseLog.Text = "Erase";
            this.button_EraseLog.UseVisualStyleBackColor = true;
            // 
            // button_close
            // 
            this.button_close.AutoSize = true;
            this.button_close.Location = new System.Drawing.Point(658, 12);
            this.button_close.Margin = new System.Windows.Forms.Padding(2);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(80, 29);
            this.button_close.TabIndex = 101;
            this.button_close.Text = "Close";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_action
            // 
            this.button_action.AutoSize = true;
            this.button_action.Location = new System.Drawing.Point(539, 12);
            this.button_action.Margin = new System.Windows.Forms.Padding(2);
            this.button_action.Name = "button_action";
            this.button_action.Size = new System.Drawing.Size(80, 29);
            this.button_action.TabIndex = 59;
            this.button_action.Text = "Upload";
            this.button_action.UseVisualStyleBackColor = true;
            this.button_action.Click += new System.EventHandler(this.messageSendFunction);
            // 
            // button_uncheck
            // 
            this.button_uncheck.AutoSize = true;
            this.button_uncheck.Location = new System.Drawing.Point(2, 69);
            this.button_uncheck.Margin = new System.Windows.Forms.Padding(2);
            this.button_uncheck.Name = "button_uncheck";
            this.button_uncheck.Size = new System.Drawing.Size(95, 29);
            this.button_uncheck.TabIndex = 95;
            this.button_uncheck.Text = "Deselect All";
            this.button_uncheck.UseVisualStyleBackColor = true;
            this.button_uncheck.Click += new System.EventHandler(this.CheckEvent_Click_1);
            // 
            // button_check
            // 
            this.button_check.AutoSize = true;
            this.button_check.Location = new System.Drawing.Point(2, 36);
            this.button_check.Margin = new System.Windows.Forms.Padding(2);
            this.button_check.Name = "button_check";
            this.button_check.Size = new System.Drawing.Size(95, 29);
            this.button_check.TabIndex = 94;
            this.button_check.Text = "Select All";
            this.button_check.UseVisualStyleBackColor = true;
            this.button_check.Click += new System.EventHandler(this.CheckEvent_Click_1);
            // 
            // button_refresh
            // 
            this.button_refresh.AutoSize = true;
            this.button_refresh.Location = new System.Drawing.Point(2, 3);
            this.button_refresh.Margin = new System.Windows.Forms.Padding(2);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(95, 29);
            this.button_refresh.TabIndex = 93;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.RefreshButton_clickEvent);
            // 
            // panel_container
            // 
            this.panel_container.Controls.Add(this.tableLayoutPanel_process);
            this.panel_container.Controls.Add(this.groupBox4);
            this.panel_container.Controls.Add(this.panel_operation2);
            this.panel_container.Controls.Add(this.menuStripAll);
            this.panel_container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_container.Location = new System.Drawing.Point(0, 0);
            this.panel_container.Margin = new System.Windows.Forms.Padding(2);
            this.panel_container.Name = "panel_container";
            this.panel_container.Size = new System.Drawing.Size(888, 550);
            this.panel_container.TabIndex = 89;
            // 
            // tableLayoutPanel_process
            // 
            this.tableLayoutPanel_process.ColumnCount = 1;
            this.tableLayoutPanel_process.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_process.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel_process.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel_process.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel_process.Location = new System.Drawing.Point(13, 276);
            this.tableLayoutPanel_process.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_process.Name = "tableLayoutPanel_process";
            this.tableLayoutPanel_process.RowCount = 3;
            this.tableLayoutPanel_process.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel_process.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel_process.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_process.Size = new System.Drawing.Size(863, 255);
            this.tableLayoutPanel_process.TabIndex = 103;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox_Info);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 50);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(863, 150);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.process_display1);
            this.panel2.Controls.Add(this.process_display2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(863, 50);
            this.panel2.TabIndex = 1;
            // 
            // process_display1
            // 
            this.process_display1.Controls.Add(this.label1);
            this.process_display1.Controls.Add(this.progressBar_part);
            this.process_display1.Controls.Add(this.label_remainingTime);
            this.process_display1.Location = new System.Drawing.Point(2, 0);
            this.process_display1.Margin = new System.Windows.Forms.Padding(0);
            this.process_display1.Name = "process_display1";
            this.process_display1.Size = new System.Drawing.Size(351, 50);
            this.process_display1.TabIndex = 101;
            this.process_display1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 98;
            this.label1.Text = "Current:";
            // 
            // progressBar_part
            // 
            this.progressBar_part.Location = new System.Drawing.Point(54, 23);
            this.progressBar_part.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar_part.Name = "progressBar_part";
            this.progressBar_part.Size = new System.Drawing.Size(261, 19);
            this.progressBar_part.TabIndex = 82;
            this.progressBar_part.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label_remainingTime
            // 
            this.label_remainingTime.AutoSize = true;
            this.label_remainingTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_remainingTime.Location = new System.Drawing.Point(140, 3);
            this.label_remainingTime.Name = "label_remainingTime";
            this.label_remainingTime.Size = new System.Drawing.Size(19, 15);
            this.label_remainingTime.TabIndex = 92;
            this.label_remainingTime.Text = "---";
            // 
            // process_display2
            // 
            this.process_display2.Controls.Add(this.progressBar_total);
            this.process_display2.Controls.Add(this.label_remainingTime_total);
            this.process_display2.Controls.Add(this.label2);
            this.process_display2.Location = new System.Drawing.Point(411, 0);
            this.process_display2.Margin = new System.Windows.Forms.Padding(0);
            this.process_display2.Name = "process_display2";
            this.process_display2.Size = new System.Drawing.Size(327, 50);
            this.process_display2.TabIndex = 102;
            this.process_display2.Visible = false;
            // 
            // progressBar_total
            // 
            this.progressBar_total.Location = new System.Drawing.Point(47, 22);
            this.progressBar_total.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar_total.Name = "progressBar_total";
            this.progressBar_total.Size = new System.Drawing.Size(261, 19);
            this.progressBar_total.TabIndex = 95;
            this.progressBar_total.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label_remainingTime_total
            // 
            this.label_remainingTime_total.AutoSize = true;
            this.label_remainingTime_total.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_remainingTime_total.Location = new System.Drawing.Point(113, 5);
            this.label_remainingTime_total.Name = "label_remainingTime_total";
            this.label_remainingTime_total.Size = new System.Drawing.Size(19, 15);
            this.label_remainingTime_total.TabIndex = 97;
            this.label_remainingTime_total.Text = "---";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 99;
            this.label2.Text = "Total:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelBox_logDir);
            this.panel3.Controls.Add(this.button_action);
            this.panel3.Controls.Add(this.button_close);
            this.panel3.Controls.Add(this.panel_operation);
            this.panel3.Controls.Add(this.panel_operation3);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 200);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(863, 55);
            this.panel3.TabIndex = 2;
            // 
            // labelBox_logDir
            // 
            this.labelBox_logDir.BackColor = System.Drawing.Color.Transparent;
            this.labelBox_logDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBox_logDir.HorizontalAlignment = System.Drawing.StringAlignment.Center;
            this.labelBox_logDir.Location = new System.Drawing.Point(5, 16);
            this.labelBox_logDir.Name = "labelBox_logDir";
            this.labelBox_logDir.Size = new System.Drawing.Size(357, 30);
            this.labelBox_logDir.TabIndex = 103;
            this.labelBox_logDir.VerticalAlignment = System.Drawing.StringAlignment.Near;
            // 
            // panel_operation3
            // 
            this.panel_operation3.Controls.Add(this.radioButton_erase);
            this.panel_operation3.Controls.Add(this.radioButton_upload);
            this.panel_operation3.Location = new System.Drawing.Point(384, 9);
            this.panel_operation3.Name = "panel_operation3";
            this.panel_operation3.Size = new System.Drawing.Size(136, 35);
            this.panel_operation3.TabIndex = 100;
            // 
            // radioButton_erase
            // 
            this.radioButton_erase.AutoSize = true;
            this.radioButton_erase.Location = new System.Drawing.Point(79, 11);
            this.radioButton_erase.Name = "radioButton_erase";
            this.radioButton_erase.Size = new System.Drawing.Size(52, 17);
            this.radioButton_erase.TabIndex = 103;
            this.radioButton_erase.Tag = "Erase";
            this.radioButton_erase.Text = "Erase";
            this.radioButton_erase.UseVisualStyleBackColor = true;
            this.radioButton_erase.CheckedChanged += new System.EventHandler(this.radioButton_erase_CheckedChanged);
            // 
            // radioButton_upload
            // 
            this.radioButton_upload.AutoSize = true;
            this.radioButton_upload.Location = new System.Drawing.Point(10, 11);
            this.radioButton_upload.Name = "radioButton_upload";
            this.radioButton_upload.Size = new System.Drawing.Size(59, 17);
            this.radioButton_upload.TabIndex = 102;
            this.radioButton_upload.Tag = "Upload";
            this.radioButton_upload.Text = "Upload";
            this.radioButton_upload.UseVisualStyleBackColor = true;
            this.radioButton_upload.CheckedChanged += new System.EventHandler(this.radioButton_upload_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.tableLayoutPanel_message);
            this.groupBox4.Location = new System.Drawing.Point(16, 25);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(752, 245);
            this.groupBox4.TabIndex = 91;
            this.groupBox4.TabStop = false;
            // 
            // tableLayoutPanel_message
            // 
            this.tableLayoutPanel_message.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tableLayoutPanel_message.ColumnCount = 6;
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel_message.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_message.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel_message.Controls.Add(this.label15, 1, 0);
            this.tableLayoutPanel_message.Controls.Add(this.label17, 3, 0);
            this.tableLayoutPanel_message.Controls.Add(this.label18, 4, 0);
            this.tableLayoutPanel_message.Controls.Add(this.label16, 2, 0);
            this.tableLayoutPanel_message.Controls.Add(this.label5, 5, 0);
            this.tableLayoutPanel_message.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_message.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel_message.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel_message.Name = "tableLayoutPanel_message";
            this.tableLayoutPanel_message.RowCount = 8;
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.2851F));
            this.tableLayoutPanel_message.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28939F));
            this.tableLayoutPanel_message.Size = new System.Drawing.Size(748, 228);
            this.tableLayoutPanel_message.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(4, 2);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 30);
            this.label10.TabIndex = 0;
            this.label10.Text = "Select";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(54, 2);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(71, 30);
            this.label15.TabIndex = 1;
            this.label15.Text = "Node";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(193, 2);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(136, 30);
            this.label17.TabIndex = 3;
            this.label17.Text = "Log Size";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(335, 2);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(146, 30);
            this.label18.TabIndex = 4;
            this.label18.Text = "Upload Time";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(131, 2);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 30);
            this.label16.TabIndex = 2;
            this.label16.Text = "Connect";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(488, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(255, 30);
            this.label5.TabIndex = 5;
            this.label5.Text = "Status";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel_operation2
            // 
            this.panel_operation2.Controls.Add(this.button_uncheck);
            this.panel_operation2.Controls.Add(this.button_refresh);
            this.panel_operation2.Controls.Add(this.button_check);
            this.panel_operation2.Location = new System.Drawing.Point(770, 40);
            this.panel_operation2.Margin = new System.Windows.Forms.Padding(0);
            this.panel_operation2.Name = "panel_operation2";
            this.panel_operation2.Size = new System.Drawing.Size(107, 194);
            this.panel_operation2.TabIndex = 93;
            // 
            // menuStripAll
            // 
            this.menuStripAll.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStripAll.Location = new System.Drawing.Point(0, 0);
            this.menuStripAll.Name = "menuStripAll";
            this.menuStripAll.Size = new System.Drawing.Size(888, 24);
            this.menuStripAll.TabIndex = 94;
            this.menuStripAll.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.converterToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.toolsToolStripMenuItem.Text = "Log";
            // 
            // converterToolStripMenuItem
            // 
            this.converterToolStripMenuItem.Name = "converterToolStripMenuItem";
            this.converterToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.converterToolStripMenuItem.Text = "Converter";
            this.converterToolStripMenuItem.Click += new System.EventHandler(this.converterToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.infomationBoxToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // infomationBoxToolStripMenuItem
            // 
            this.infomationBoxToolStripMenuItem.Name = "infomationBoxToolStripMenuItem";
            this.infomationBoxToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.infomationBoxToolStripMenuItem.Text = "Infomation Box";
            this.infomationBoxToolStripMenuItem.Click += new System.EventHandler(this.infomationBoxToolStripMenuItem_Click);
            // 
            // LoggingDownloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 550);
            this.Controls.Add(this.panel_container);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStripAll;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "LoggingDownloadForm";
            this.Text = "Tool Log Upload";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogDownloadFormClosing);
            this.Load += new System.EventHandler(this.LoggingDownloadForm_Load);
            this.panel_operation.ResumeLayout(false);
            this.panel_operation.PerformLayout();
            this.panel_container.ResumeLayout(false);
            this.panel_container.PerformLayout();
            this.tableLayoutPanel_process.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.process_display1.ResumeLayout(false);
            this.process_display1.PerformLayout();
            this.process_display2.ResumeLayout(false);
            this.process_display2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel_operation3.ResumeLayout(false);
            this.panel_operation3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tableLayoutPanel_message.ResumeLayout(false);
            this.tableLayoutPanel_message.PerformLayout();
            this.panel_operation2.ResumeLayout(false);
            this.panel_operation2.PerformLayout();
            this.menuStripAll.ResumeLayout(false);
            this.menuStripAll.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_Info;
        private System.Windows.Forms.Panel panel_operation;
        private System.Windows.Forms.Button button_EraseLog;
        private System.Windows.Forms.Button button_action;
        private EVO.ProgressbarWithLabel progressBar_part;
        private System.Windows.Forms.Panel panel_container;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_message;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.Label label_remainingTime;
        private System.Windows.Forms.Button button_uncheck;
        private System.Windows.Forms.Button button_check;
        private System.Windows.Forms.Panel panel_operation2;
        private System.Windows.Forms.MenuStrip menuStripAll;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem converterToolStripMenuItem;
        private System.Windows.Forms.Label label_remainingTime_total;
        private EVO.ProgressbarWithLabel progressBar_total;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel process_display2;
        private System.Windows.Forms.Panel process_display1;
        private System.Windows.Forms.Panel panel_operation3;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infomationBoxToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_process;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton radioButton_erase;
        private System.Windows.Forms.RadioButton radioButton_upload;
        private System.Windows.Forms.Button button_close;
        private EVO.LabelBox labelBox_logDir;

    }
}

