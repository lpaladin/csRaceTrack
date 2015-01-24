namespace csRaceTrack
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.lstLog = new System.Windows.Forms.ListBox();
            this.lstTrace = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.grpControlPanel = new System.Windows.Forms.GroupBox();
            this.ofdTrace = new System.Windows.Forms.OpenFileDialog();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.cmbImplement = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.timTracePlay = new System.Windows.Forms.Timer(this.components);
            this.lstGroupView = new System.Windows.Forms.ListView();
            this.cmbGroupView = new System.Windows.Forms.ComboBox();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblShiftCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRWCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCacheReadMiss = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefreshGroupView = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chkCountMissShifts = new System.Windows.Forms.CheckBox();
            this.chkFastMode = new System.Windows.Forms.CheckBox();
            this.grpControlPanel.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstLog
            // 
            this.lstLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstLog.FormattingEnabled = true;
            this.lstLog.ItemHeight = 18;
            this.lstLog.Location = new System.Drawing.Point(0, 463);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(794, 238);
            this.lstLog.TabIndex = 0;
            // 
            // lstTrace
            // 
            this.lstTrace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTrace.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.lstTrace.FullRowSelect = true;
            this.lstTrace.GridLines = true;
            this.lstTrace.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstTrace.Location = new System.Drawing.Point(0, 183);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(497, 267);
            this.lstTrace.TabIndex = 1;
            this.lstTrace.UseCompatibleStateImageBehavior = false;
            this.lstTrace.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "操作";
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "地址";
            this.columnHeader3.Width = 261;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "时间戳";
            this.columnHeader4.Width = 109;
            // 
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(0, 1);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(741, 28);
            this.txtPath.TabIndex = 3;
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(747, 1);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(47, 28);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // grpControlPanel
            // 
            this.grpControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControlPanel.Controls.Add(this.chkFastMode);
            this.grpControlPanel.Controls.Add(this.chkCountMissShifts);
            this.grpControlPanel.Controls.Add(this.cmbGroupView);
            this.grpControlPanel.Controls.Add(this.label2);
            this.grpControlPanel.Controls.Add(this.btnRefreshGroupView);
            this.grpControlPanel.Controls.Add(this.label1);
            this.grpControlPanel.Controls.Add(this.cmbImplement);
            this.grpControlPanel.Controls.Add(this.btnReset);
            this.grpControlPanel.Controls.Add(this.btnStep);
            this.grpControlPanel.Controls.Add(this.btnPlay);
            this.grpControlPanel.Location = new System.Drawing.Point(503, 183);
            this.grpControlPanel.Name = "grpControlPanel";
            this.grpControlPanel.Size = new System.Drawing.Size(291, 267);
            this.grpControlPanel.TabIndex = 5;
            this.grpControlPanel.TabStop = false;
            this.grpControlPanel.Text = "控制";
            // 
            // ofdTrace
            // 
            this.ofdTrace.Filter = "Trace 文件|*.trace";
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(9, 147);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(276, 34);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "运行";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnStep
            // 
            this.btnStep.Location = new System.Drawing.Point(9, 187);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(276, 34);
            this.btnStep.TabIndex = 0;
            this.btnStep.Text = "单步";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // cmbImplement
            // 
            this.cmbImplement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImplement.FormattingEnabled = true;
            this.cmbImplement.Location = new System.Drawing.Point(84, 27);
            this.cmbImplement.Name = "cmbImplement";
            this.cmbImplement.Size = new System.Drawing.Size(195, 26);
            this.cmbImplement.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "实现：";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(9, 227);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(276, 34);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // timTracePlay
            // 
            this.timTracePlay.Tick += new System.EventHandler(this.timTracePlay_Tick);
            // 
            // lstGroupView
            // 
            this.lstGroupView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstGroupView.FullRowSelect = true;
            this.lstGroupView.GridLines = true;
            this.lstGroupView.Location = new System.Drawing.Point(0, 35);
            this.lstGroupView.Name = "lstGroupView";
            this.lstGroupView.Size = new System.Drawing.Size(794, 142);
            this.lstGroupView.TabIndex = 6;
            this.lstGroupView.UseCompatibleStateImageBehavior = false;
            this.lstGroupView.View = System.Windows.Forms.View.Details;
            // 
            // cmbGroupView
            // 
            this.cmbGroupView.FormattingEnabled = true;
            this.cmbGroupView.Location = new System.Drawing.Point(84, 59);
            this.cmbGroupView.Name = "cmbGroupView";
            this.cmbGroupView.Size = new System.Drawing.Size(142, 26);
            this.cmbGroupView.TabIndex = 1;
            this.cmbGroupView.SelectedIndexChanged += new System.EventHandler(this.cmbGroupView_SelectedIndexChanged);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblShiftCount,
            this.lblRWCount,
            this.lblCacheReadMiss});
            this.statusBar.Location = new System.Drawing.Point(0, 705);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(794, 22);
            this.statusBar.TabIndex = 7;
            this.statusBar.Text = "statusStrip1";
            // 
            // lblShiftCount
            // 
            this.lblShiftCount.Name = "lblShiftCount";
            this.lblShiftCount.Size = new System.Drawing.Size(75, 17);
            this.lblShiftCount.Text = "移动次数：0";
            // 
            // lblRWCount
            // 
            this.lblRWCount.Name = "lblRWCount";
            this.lblRWCount.Size = new System.Drawing.Size(75, 17);
            this.lblRWCount.Text = "读写次数：0";
            // 
            // lblCacheReadMiss
            // 
            this.lblCacheReadMiss.Name = "lblCacheReadMiss";
            this.lblCacheReadMiss.Size = new System.Drawing.Size(90, 17);
            this.lblCacheReadMiss.Text = "读Miss次数：0";
            // 
            // btnRefreshGroupView
            // 
            this.btnRefreshGroupView.Image = global::csRaceTrack.Properties.Resources._112_RefreshArrow_Green_16x16_72;
            this.btnRefreshGroupView.Location = new System.Drawing.Point(232, 59);
            this.btnRefreshGroupView.Name = "btnRefreshGroupView";
            this.btnRefreshGroupView.Size = new System.Drawing.Size(47, 42);
            this.btnRefreshGroupView.TabIndex = 3;
            this.btnRefreshGroupView.UseVisualStyleBackColor = true;
            this.btnRefreshGroupView.Click += new System.EventHandler(this.btnRefreshGroupView_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "观看组：";
            // 
            // chkCountMissShifts
            // 
            this.chkCountMissShifts.AutoSize = true;
            this.chkCountMissShifts.Location = new System.Drawing.Point(18, 119);
            this.chkCountMissShifts.Name = "chkCountMissShifts";
            this.chkCountMissShifts.Size = new System.Drawing.Size(261, 22);
            this.chkCountMissShifts.TabIndex = 5;
            this.chkCountMissShifts.Text = "Miss时的移动也计入移动次数";
            this.chkCountMissShifts.UseVisualStyleBackColor = true;
            // 
            // chkFastMode
            // 
            this.chkFastMode.AutoSize = true;
            this.chkFastMode.Location = new System.Drawing.Point(18, 91);
            this.chkFastMode.Name = "chkFastMode";
            this.chkFastMode.Size = new System.Drawing.Size(207, 22);
            this.chkFastMode.TabIndex = 5;
            this.chkFastMode.Text = "精简（高速）运行模式";
            this.chkFastMode.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 727);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.lstGroupView);
            this.Controls.Add(this.grpControlPanel);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lstTrace);
            this.Controls.Add(this.lstLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "RaceTrack模拟器";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpControlPanel.ResumeLayout(false);
            this.grpControlPanel.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.ListView lstTrace;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox grpControlPanel;
        private System.Windows.Forms.OpenFileDialog ofdTrace;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbImplement;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Timer timTracePlay;
        private System.Windows.Forms.ListView lstGroupView;
        private System.Windows.Forms.ComboBox cmbGroupView;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel lblShiftCount;
        private System.Windows.Forms.ToolStripStatusLabel lblRWCount;
        private System.Windows.Forms.ToolStripStatusLabel lblCacheReadMiss;
        private System.Windows.Forms.Button btnRefreshGroupView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkFastMode;
        private System.Windows.Forms.CheckBox chkCountMissShifts;
    }
}

