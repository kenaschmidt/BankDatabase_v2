namespace BankDatabase_v2.UI
{
    partial class MainController
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.systemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initializeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectXBRLPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadBanksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadXBRLReportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.progressbarWebData = new System.Windows.Forms.ProgressBar();
            this.textBoxWebDataStatus = new System.Windows.Forms.TextBox();
            this.checkedListBox_ReportingPeriods = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_StartReportDownload = new System.Windows.Forms.Button();
            this.printStatisticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.systemToolStripMenuItem,
            this.dataToolStripMenuItem,
            this.testingToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(434, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // systemToolStripMenuItem
            // 
            this.systemToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initializeToolStripMenuItem});
            this.systemToolStripMenuItem.Name = "systemToolStripMenuItem";
            this.systemToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.systemToolStripMenuItem.Text = "System";
            // 
            // initializeToolStripMenuItem
            // 
            this.initializeToolStripMenuItem.Name = "initializeToolStripMenuItem";
            this.initializeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.initializeToolStripMenuItem.Text = "Initialize";
            // 
            // dataToolStripMenuItem
            // 
            this.dataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectXBRLPathToolStripMenuItem,
            this.LoadBanksToolStripMenuItem});
            this.dataToolStripMenuItem.Name = "dataToolStripMenuItem";
            this.dataToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.dataToolStripMenuItem.Text = "Data";
            // 
            // selectXBRLPathToolStripMenuItem
            // 
            this.selectXBRLPathToolStripMenuItem.Name = "selectXBRLPathToolStripMenuItem";
            this.selectXBRLPathToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.selectXBRLPathToolStripMenuItem.Text = "Select XBRL Path";
            // 
            // LoadBanksToolStripMenuItem
            // 
            this.LoadBanksToolStripMenuItem.Name = "LoadBanksToolStripMenuItem";
            this.LoadBanksToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.LoadBanksToolStripMenuItem.Text = "Reload Reporting Banks";
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadXBRLReportsToolStripMenuItem,
            this.printStatisticsToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.testingToolStripMenuItem.Text = "Testing";
            // 
            // downloadXBRLReportsToolStripMenuItem
            // 
            this.downloadXBRLReportsToolStripMenuItem.Name = "downloadXBRLReportsToolStripMenuItem";
            this.downloadXBRLReportsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.downloadXBRLReportsToolStripMenuItem.Text = "Download XBRL Reports";
            // 
            // progressbarWebData
            // 
            this.progressbarWebData.Location = new System.Drawing.Point(184, 241);
            this.progressbarWebData.Name = "progressbarWebData";
            this.progressbarWebData.Size = new System.Drawing.Size(238, 20);
            this.progressbarWebData.TabIndex = 2;
            // 
            // textBoxWebDataStatus
            // 
            this.textBoxWebDataStatus.BackColor = System.Drawing.Color.Red;
            this.textBoxWebDataStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWebDataStatus.Location = new System.Drawing.Point(13, 241);
            this.textBoxWebDataStatus.Name = "textBoxWebDataStatus";
            this.textBoxWebDataStatus.Size = new System.Drawing.Size(165, 20);
            this.textBoxWebDataStatus.TabIndex = 3;
            this.textBoxWebDataStatus.Text = "NOT CONNECTED";
            this.textBoxWebDataStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkedListBox_ReportingPeriods
            // 
            this.checkedListBox_ReportingPeriods.FormattingEnabled = true;
            this.checkedListBox_ReportingPeriods.Location = new System.Drawing.Point(13, 58);
            this.checkedListBox_ReportingPeriods.Name = "checkedListBox_ReportingPeriods";
            this.checkedListBox_ReportingPeriods.Size = new System.Drawing.Size(125, 169);
            this.checkedListBox_ReportingPeriods.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Reporting Periods";
            // 
            // button_StartReportDownload
            // 
            this.button_StartReportDownload.Enabled = false;
            this.button_StartReportDownload.Location = new System.Drawing.Point(144, 204);
            this.button_StartReportDownload.Name = "button_StartReportDownload";
            this.button_StartReportDownload.Size = new System.Drawing.Size(75, 23);
            this.button_StartReportDownload.TabIndex = 6;
            this.button_StartReportDownload.Text = "Start Download";
            this.button_StartReportDownload.UseVisualStyleBackColor = true;
            // 
            // printStatisticsToolStripMenuItem
            // 
            this.printStatisticsToolStripMenuItem.Name = "printStatisticsToolStripMenuItem";
            this.printStatisticsToolStripMenuItem.Size = new System.Drawing.Size(201, 22);
            this.printStatisticsToolStripMenuItem.Text = "Print Statistics";
            // 
            // MainController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 273);
            this.Controls.Add(this.button_StartReportDownload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkedListBox_ReportingPeriods);
            this.Controls.Add(this.textBoxWebDataStatus);
            this.Controls.Add(this.progressbarWebData);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainController";
            this.Text = "MainController";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem systemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem initializeToolStripMenuItem;
        private System.Windows.Forms.ProgressBar progressbarWebData;
        private System.Windows.Forms.TextBox textBoxWebDataStatus;
        private System.Windows.Forms.ToolStripMenuItem dataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadBanksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectXBRLPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadXBRLReportsToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBox_ReportingPeriods;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_StartReportDownload;
        private System.Windows.Forms.ToolStripMenuItem printStatisticsToolStripMenuItem;
    }
}