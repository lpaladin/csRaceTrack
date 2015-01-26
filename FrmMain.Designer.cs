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
            try { srTraceReader.Close(); }
            catch { }
            try { bs.Close(); }
            catch { }
            try { fs.Close(); }
            catch { }
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
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtPath = new System.Windows.Forms.TextBox();
            this.grpControlPanel = new System.Windows.Forms.GroupBox();
            this.grpComparision = new System.Windows.Forms.GroupBox();
            this.chklstComparedImplementations = new System.Windows.Forms.CheckedListBox();
            this.mnuChkLstComparedImplementations = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExportCSV = new System.Windows.Forms.Button();
            this.btnCompareBegin = new System.Windows.Forms.Button();
            this.chkFastMode = new System.Windows.Forms.CheckBox();
            this.chkExternalCacheInfo = new System.Windows.Forms.CheckBox();
            this.chkCountMissShifts = new System.Windows.Forms.CheckBox();
            this.cmbGroupView = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRefreshGroupView = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbImplement = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.ofdTrace = new System.Windows.Forms.OpenFileDialog();
            this.timTracePlay = new System.Windows.Forms.Timer(this.components);
            this.lstGroupView = new System.Windows.Forms.ListView();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblShiftCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblRWCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblCacheReadMiss = new System.Windows.Forms.ToolStripStatusLabel();
            this.sfdExportCSV = new System.Windows.Forms.SaveFileDialog();
            this.ttCheckedListHint = new System.Windows.Forms.ToolTip(this.components);
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblRCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.grpControlPanel.SuspendLayout();
            this.grpComparision.SuspendLayout();
            this.mnuChkLstComparedImplementations.SuspendLayout();
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
            this.lstLog.Location = new System.Drawing.Point(0, 500);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(1012, 256);
            this.lstLog.TabIndex = 0;
            this.ttCheckedListHint.SetToolTip(this.lstLog, "这里是日志区");
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
            this.lstTrace.Size = new System.Drawing.Size(472, 311);
            this.lstTrace.TabIndex = 1;
            this.ttCheckedListHint.SetToolTip(this.lstTrace, "这里是非精简模式下的Trace内容");
            this.lstTrace.UseCompatibleStateImageBehavior = false;
            this.lstTrace.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 40;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "时间戳";
            this.columnHeader4.Width = 109;
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
            // txtPath
            // 
            this.txtPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPath.Location = new System.Drawing.Point(0, 2);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(960, 28);
            this.txtPath.TabIndex = 3;
            // 
            // grpControlPanel
            // 
            this.grpControlPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpControlPanel.Controls.Add(this.grpComparision);
            this.grpControlPanel.Controls.Add(this.chkFastMode);
            this.grpControlPanel.Controls.Add(this.chkExternalCacheInfo);
            this.grpControlPanel.Controls.Add(this.chkCountMissShifts);
            this.grpControlPanel.Controls.Add(this.cmbGroupView);
            this.grpControlPanel.Controls.Add(this.label2);
            this.grpControlPanel.Controls.Add(this.btnRefreshGroupView);
            this.grpControlPanel.Controls.Add(this.label1);
            this.grpControlPanel.Controls.Add(this.cmbImplement);
            this.grpControlPanel.Controls.Add(this.btnReset);
            this.grpControlPanel.Controls.Add(this.btnStep);
            this.grpControlPanel.Controls.Add(this.btnPlay);
            this.grpControlPanel.Location = new System.Drawing.Point(478, 183);
            this.grpControlPanel.Name = "grpControlPanel";
            this.grpControlPanel.Size = new System.Drawing.Size(534, 311);
            this.grpControlPanel.TabIndex = 5;
            this.grpControlPanel.TabStop = false;
            this.grpControlPanel.Text = "控制";
            // 
            // grpComparision
            // 
            this.grpComparision.Controls.Add(this.chklstComparedImplementations);
            this.grpComparision.Controls.Add(this.btnExportCSV);
            this.grpComparision.Controls.Add(this.btnCompareBegin);
            this.grpComparision.Location = new System.Drawing.Point(291, 27);
            this.grpComparision.Name = "grpComparision";
            this.grpComparision.Size = new System.Drawing.Size(231, 284);
            this.grpComparision.TabIndex = 6;
            this.grpComparision.TabStop = false;
            this.grpComparision.Text = "对比实验和结果导出";
            // 
            // chklstComparedImplementations
            // 
            this.chklstComparedImplementations.CheckOnClick = true;
            this.chklstComparedImplementations.ContextMenuStrip = this.mnuChkLstComparedImplementations;
            this.chklstComparedImplementations.FormattingEnabled = true;
            this.chklstComparedImplementations.Location = new System.Drawing.Point(6, 27);
            this.chklstComparedImplementations.Name = "chklstComparedImplementations";
            this.chklstComparedImplementations.Size = new System.Drawing.Size(218, 165);
            this.chklstComparedImplementations.TabIndex = 1;
            this.ttCheckedListHint.SetToolTip(this.chklstComparedImplementations, "点击右键可以全选或不选");
            // 
            // mnuChkLstComparedImplementations
            // 
            this.mnuChkLstComparedImplementations.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnuChkLstComparedImplementations.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSelectAll,
            this.mnuUnSelectAll});
            this.mnuChkLstComparedImplementations.Name = "mnuChkLstComparedImplementations";
            this.mnuChkLstComparedImplementations.Size = new System.Drawing.Size(135, 60);
            this.mnuChkLstComparedImplementations.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mnuChkLstComparedImplementations_ItemClicked);
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Size = new System.Drawing.Size(134, 28);
            this.mnuSelectAll.Text = "全选";
            // 
            // mnuUnSelectAll
            // 
            this.mnuUnSelectAll.Name = "mnuUnSelectAll";
            this.mnuUnSelectAll.Size = new System.Drawing.Size(134, 28);
            this.mnuUnSelectAll.Text = "全不选";
            // 
            // btnExportCSV
            // 
            this.btnExportCSV.Image = global::csRaceTrack.Properties.Resources.saveHS;
            this.btnExportCSV.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportCSV.Location = new System.Drawing.Point(5, 239);
            this.btnExportCSV.Name = "btnExportCSV";
            this.btnExportCSV.Size = new System.Drawing.Size(219, 36);
            this.btnExportCSV.TabIndex = 0;
            this.btnExportCSV.Text = "导出为CSV文件";
            this.btnExportCSV.UseVisualStyleBackColor = true;
            this.btnExportCSV.Click += new System.EventHandler(this.btnExportCSV_Click);
            // 
            // btnCompareBegin
            // 
            this.btnCompareBegin.Image = global::csRaceTrack.Properties.Resources.PlayHS;
            this.btnCompareBegin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCompareBegin.Location = new System.Drawing.Point(5, 199);
            this.btnCompareBegin.Name = "btnCompareBegin";
            this.btnCompareBegin.Size = new System.Drawing.Size(219, 34);
            this.btnCompareBegin.TabIndex = 0;
            this.btnCompareBegin.Text = "批量运行";
            this.ttCheckedListHint.SetToolTip(this.btnCompareBegin, "对每个文件运行以上所有实现");
            this.btnCompareBegin.UseVisualStyleBackColor = true;
            this.btnCompareBegin.Click += new System.EventHandler(this.btnCompareBegin_Click);
            // 
            // chkFastMode
            // 
            this.chkFastMode.AutoSize = true;
            this.chkFastMode.Location = new System.Drawing.Point(18, 92);
            this.chkFastMode.Name = "chkFastMode";
            this.chkFastMode.Size = new System.Drawing.Size(214, 22);
            this.chkFastMode.TabIndex = 5;
            this.chkFastMode.Text = "精简（高速）运行模式";
            this.chkFastMode.UseVisualStyleBackColor = true;
            // 
            // chkExternalCacheInfo
            // 
            this.chkExternalCacheInfo.AutoSize = true;
            this.chkExternalCacheInfo.Checked = true;
            this.chkExternalCacheInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExternalCacheInfo.Location = new System.Drawing.Point(18, 146);
            this.chkExternalCacheInfo.Name = "chkExternalCacheInfo";
            this.chkExternalCacheInfo.Size = new System.Drawing.Size(259, 22);
            this.chkExternalCacheInfo.TabIndex = 5;
            this.chkExternalCacheInfo.Text = "允许在条带外存储Cache信息";
            this.chkExternalCacheInfo.UseVisualStyleBackColor = true;
            this.chkExternalCacheInfo.CheckedChanged += new System.EventHandler(this.chkExternalCacheInfo_CheckedChanged);
            // 
            // chkCountMissShifts
            // 
            this.chkCountMissShifts.AutoSize = true;
            this.chkCountMissShifts.Checked = true;
            this.chkCountMissShifts.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCountMissShifts.Location = new System.Drawing.Point(18, 118);
            this.chkCountMissShifts.Name = "chkCountMissShifts";
            this.chkCountMissShifts.Size = new System.Drawing.Size(268, 22);
            this.chkCountMissShifts.TabIndex = 5;
            this.chkCountMissShifts.Text = "Miss时的移动也计入移动次数";
            this.chkCountMissShifts.UseVisualStyleBackColor = true;
            // 
            // cmbGroupView
            // 
            this.cmbGroupView.FormattingEnabled = true;
            this.cmbGroupView.Location = new System.Drawing.Point(84, 58);
            this.cmbGroupView.Name = "cmbGroupView";
            this.cmbGroupView.Size = new System.Drawing.Size(142, 26);
            this.cmbGroupView.TabIndex = 1;
            this.ttCheckedListHint.SetToolTip(this.cmbGroupView, "这里确定你希望上方显示哪个条带组");
            this.cmbGroupView.SelectedIndexChanged += new System.EventHandler(this.cmbGroupView_SelectedIndexChanged);
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
            // btnRefreshGroupView
            // 
            this.btnRefreshGroupView.Image = global::csRaceTrack.Properties.Resources._112_RefreshArrow_Green_16x16_72;
            this.btnRefreshGroupView.Location = new System.Drawing.Point(232, 58);
            this.btnRefreshGroupView.Name = "btnRefreshGroupView";
            this.btnRefreshGroupView.Size = new System.Drawing.Size(46, 42);
            this.btnRefreshGroupView.TabIndex = 3;
            this.btnRefreshGroupView.UseVisualStyleBackColor = true;
            this.btnRefreshGroupView.Click += new System.EventHandler(this.btnRefreshGroupView_Click);
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
            // cmbImplement
            // 
            this.cmbImplement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImplement.FormattingEnabled = true;
            this.cmbImplement.Location = new System.Drawing.Point(84, 27);
            this.cmbImplement.Name = "cmbImplement";
            this.cmbImplement.Size = new System.Drawing.Size(194, 26);
            this.cmbImplement.TabIndex = 1;
            // 
            // btnReset
            // 
            this.btnReset.Image = global::csRaceTrack.Properties.Resources.RestartHS;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(6, 267);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(276, 34);
            this.btnReset.TabIndex = 0;
            this.btnReset.Text = "重置";
            this.ttCheckedListHint.SetToolTip(this.btnReset, "重置RaceTrack状态并清空所有记录");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnStep
            // 
            this.btnStep.Image = global::csRaceTrack.Properties.Resources.GoToNextHS;
            this.btnStep.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStep.Location = new System.Drawing.Point(6, 229);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(276, 34);
            this.btnStep.TabIndex = 0;
            this.btnStep.Text = "单步";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::csRaceTrack.Properties.Resources.PlayHS;
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.Location = new System.Drawing.Point(6, 188);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(276, 34);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "运行";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // ofdTrace
            // 
            this.ofdTrace.Filter = "Trace 文件|*.trace";
            this.ofdTrace.Multiselect = true;
            this.ofdTrace.Title = "打开 Trace 文件";
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
            this.lstGroupView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstGroupView.Location = new System.Drawing.Point(0, 34);
            this.lstGroupView.Name = "lstGroupView";
            this.lstGroupView.Size = new System.Drawing.Size(1012, 142);
            this.lstGroupView.TabIndex = 6;
            this.ttCheckedListHint.SetToolTip(this.lstGroupView, "这里显示了条带组的数据状况");
            this.lstGroupView.UseCompatibleStateImageBehavior = false;
            this.lstGroupView.View = System.Windows.Forms.View.Details;
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblShiftCount,
            this.lblRWCount,
            this.lblRCount,
            this.lblCacheReadMiss});
            this.statusBar.Location = new System.Drawing.Point(0, 766);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(2, 0, 14, 0);
            this.statusBar.Size = new System.Drawing.Size(1012, 29);
            this.statusBar.TabIndex = 7;
            this.statusBar.Text = "statusStrip1";
            // 
            // lblShiftCount
            // 
            this.lblShiftCount.Name = "lblShiftCount";
            this.lblShiftCount.Size = new System.Drawing.Size(111, 24);
            this.lblShiftCount.Text = "移动次数：0";
            // 
            // lblRWCount
            // 
            this.lblRWCount.Name = "lblRWCount";
            this.lblRWCount.Size = new System.Drawing.Size(111, 24);
            this.lblRWCount.Text = "读写次数：0";
            // 
            // lblCacheReadMiss
            // 
            this.lblCacheReadMiss.Name = "lblCacheReadMiss";
            this.lblCacheReadMiss.Size = new System.Drawing.Size(132, 24);
            this.lblCacheReadMiss.Text = "读Miss次数：0";
            // 
            // sfdExportCSV
            // 
            this.sfdExportCSV.Filter = "逗号分隔表|*.csv";
            this.sfdExportCSV.Title = "指定要保存到的逗号分隔表";
            // 
            // ttCheckedListHint
            // 
            this.ttCheckedListHint.AutomaticDelay = 0;
            this.ttCheckedListHint.AutoPopDelay = 5000;
            this.ttCheckedListHint.InitialDelay = 1;
            this.ttCheckedListHint.IsBalloon = true;
            this.ttCheckedListHint.ReshowDelay = 100;
            this.ttCheckedListHint.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.ttCheckedListHint.ToolTipTitle = "提示";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Image = global::csRaceTrack.Properties.Resources.OpenSelectedItemHS;
            this.btnBrowse.Location = new System.Drawing.Point(964, 2);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(46, 28);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblRCount
            // 
            this.lblRCount.Name = "lblRCount";
            this.lblRCount.Size = new System.Drawing.Size(93, 24);
            this.lblRCount.Text = "读次数：0";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 795);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.lstGroupView);
            this.Controls.Add(this.grpControlPanel);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lstTrace);
            this.Controls.Add(this.lstLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "RaceTrack模拟器 - 周昊宇 / 1200012823";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.grpControlPanel.ResumeLayout(false);
            this.grpControlPanel.PerformLayout();
            this.grpComparision.ResumeLayout(false);
            this.mnuChkLstComparedImplementations.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox grpComparision;
        private System.Windows.Forms.Button btnExportCSV;
        private System.Windows.Forms.CheckedListBox chklstComparedImplementations;
        private System.Windows.Forms.Button btnCompareBegin;
        private System.Windows.Forms.SaveFileDialog sfdExportCSV;
        private System.Windows.Forms.ContextMenuStrip mnuChkLstComparedImplementations;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuUnSelectAll;
        private System.Windows.Forms.ToolTip ttCheckedListHint;
        private System.Windows.Forms.CheckBox chkExternalCacheInfo;
        private System.Windows.Forms.ToolStripStatusLabel lblRCount;
    }
}

