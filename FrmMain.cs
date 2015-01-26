using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csRaceTrack
{
    public partial class FrmMain : Form
    {
        Stream fs;
        BufferedStream bs;
        StreamReader srTraceReader;

        /// <summary>
        /// Trace列表容纳大小
        /// </summary>
        const int TRACE_WINDOW_SIZE = 10;
        int traceCount = 0, readMissCount = 0, shiftCount = 0, rwCount = 0;
        Queue<Trace> traces;

        public FrmMain()
        {
            InitializeComponent();
            RaceTrackLogic.mainForm = this;
            _FastPlayFinish = new Action<string>(FastPlayFinish);
        }

        /// <summary>
        /// 在主界面打印一条Log。
        /// </summary>
        /// <param name="str">Log内容。</param>
        public void Log(string str)
        {
            if (fastPlayInProgress)
                return;
            lstLog.Items.Add(str);
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
        }

        private static ListViewItem Trace2Row(int id, Trace trace)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = id.ToString();
            item.SubItems.Add(trace.timeStamp.ToString());
            item.SubItems.Add(trace.isRead ? "读" : "写");
            item.SubItems.Add("0x" + Convert.ToString(trace.address, 16));
            return item;
        }

        int currTraceFileID = 0;
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdTrace.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                currTraceFileID = 0;
                btnReset_Click(null, null);
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            cmbImplement.Enabled = false;
            chkCountMissShifts.Enabled = false;
            chkFastMode.Enabled = false;
            chkExternalCacheInfo.Enabled = false;
            RaceTrackLogic logic = cmbImplement.SelectedItem as RaceTrackLogic;
            try
            {
                logic.ProcessTrace(traces.Dequeue(), ref shiftCount, ref rwCount, chkCountMissShifts.Checked);
                lblShiftCount.Text = "移动次数：" + shiftCount;
                lblRWCount.Text = "读写次数：" + rwCount;
            }
            catch (RaceTrackLogic.CacheReadMissException)
            {
                Log("CacheReadMiss！");
                lblCacheReadMiss.Text = "读Miss次数：" + ++readMissCount;
                lblShiftCount.Text = "移动次数：" + shiftCount;
                lblRWCount.Text = "读写次数：" + rwCount;
            }
            catch (RaceTrackLogic.CacheCorruptException)
            {
                Log("【致命】Cache损坏！");
                timTracePlay.Stop();
                return;
            }
            if (traceCount >= TRACE_WINDOW_SIZE)
            {
                lstTrace.Items.RemoveAt(0);
                lstTrace.Items[TRACE_WINDOW_SIZE / 2 - 1].BackColor = Color.White;
                lstTrace.Items[TRACE_WINDOW_SIZE / 2].BackColor = Color.Yellow;
            }
            else
            {
                lstTrace.Items[traceCount - TRACE_WINDOW_SIZE / 2].BackColor = Color.White;
                lstTrace.Items[traceCount - TRACE_WINDOW_SIZE / 2 + 1].BackColor = Color.Yellow;
            }
            if (!srTraceReader.EndOfStream)
            {
                Trace t = new Trace(srTraceReader.ReadLine());
                traces.Enqueue(t);
                lstTrace.Items.Add(Trace2Row(traceCount++, t));
            }
            if (traces.Count == 0)
            {
                btnStep.Enabled = false;
                if (timTracePlay.Enabled == true)
                    btnPlay_Click(null, null);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            (cmbImplement.SelectedItem as RaceTrackLogic).Reset();
            timTracePlay.Stop();
            shiftCount = 0;
            rwCount = 0;
            readMissCount = 0;
            lblCacheReadMiss.Text = "读Miss次数：" + readMissCount;
            lblShiftCount.Text = "移动次数：" + shiftCount;
            lblRWCount.Text = "读写次数：" + rwCount;
            fastPlayInProgress = false;
            if (srTraceReader != null)
            {
                srTraceReader.Close();
                bs.Close();
                fs.Close();
                lstLog.Items.Clear();
                lstTrace.Items.Clear();
            }
            if (File.Exists(ofdTrace.FileNames[currTraceFileID]))
            {
                txtPath.Text = string.Format("【第{0}个文件，共{1}个文件】", currTraceFileID + 1, ofdTrace.FileNames.Length) + ofdTrace.FileNames[currTraceFileID];
                traces = new Queue<Trace>();
                fs = new FileStream(ofdTrace.FileNames[currTraceFileID], FileMode.Open);
                bs = new BufferedStream(fs);
                srTraceReader = new StreamReader(bs);
                for (traceCount = 0; traceCount < TRACE_WINDOW_SIZE / 2 && !srTraceReader.EndOfStream; traceCount++)
                {
                    Trace t = new Trace(srTraceReader.ReadLine());
                    traces.Enqueue(t);
                    lstTrace.Items.Add(Trace2Row(traceCount, t));
                }
                lstTrace.Items[0].BackColor = Color.Yellow;
                foreach (Control ctrl in grpControlPanel.Controls)
                    ctrl.Enabled = true;
            }
        }

        private void FastPlayFinish(string finalMessage)
        {
            fastPlayInProgress = false;
            requestFastPlayEnd = false;

            if (finalMessage != "")
                Log(finalMessage);

            lblCacheReadMiss.Text = "读Miss次数：" + readMissCount;
            lblShiftCount.Text = "移动次数：" + shiftCount;
            lblRWCount.Text = "读写次数：" + rwCount;

            if (inComparisionMode)
            {
                compareResults[cmp_currentLogicID + chklstComparedImplementations.CheckedIndices.Count * currTraceFileID] = new Result
                {
                    implementationName = RaceTrackLogic.currentLogic.ToString(),
                    shiftCount = shiftCount,
                    rwCount = rwCount,
                    readMissCount = readMissCount,
                    traceName = Path.GetFileName(ofdTrace.FileNames[currTraceFileID])
                };
                if (chklstComparedImplementations.CheckedIndices.Count > cmp_currentLogicID + 1)
                {
                    cmp_currentLogicID++;
                    cmbImplement.SelectedIndex = chklstComparedImplementations.CheckedIndices[cmp_currentLogicID];
                    btnReset_Click(null, null);
                    btnPlay_Click(null, null);
                    return;
                }
                else if (currTraceFileID + 1 < ofdTrace.FileNames.Length)
                {
                    currTraceFileID++;
                    cmp_currentLogicID = 0;
                    cmbImplement.SelectedIndex = chklstComparedImplementations.CheckedIndices[cmp_currentLogicID];
                    btnReset_Click(null, null);
                    btnPlay_Click(null, null);
                }
                else
                {
                    Log("实验完成。结果如下：");
                    foreach (Result r in compareResults)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(r.traceName).Append('\t')
                            .Append(r.implementationName).Append('\t')
                            .Append(r.shiftCount).Append('\t')
                            .Append(r.rwCount).Append('\t')
                            .Append(r.readMissCount);
                        Log(sb.ToString());
                    }
                    btnCompareBegin.Enabled = true;
                    chklstComparedImplementations.Enabled = true;
                }
            }

            btnPlay.Text = "运行";
            btnStep.Enabled = false;
            btnPlay.Enabled = false;
            btnReset.Enabled = true;
        }
        private Action<string> _FastPlayFinish;

        bool fastPlayInProgress = false, requestFastPlayEnd = false;
        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (chkFastMode.Checked)
            {
                if (fastPlayInProgress)
                {
                    requestFastPlayEnd = true;
                    inComparisionMode = false;
                    btnPlay.Text = "运行";
                    btnStep.Enabled = true;
                    btnReset.Enabled = true;
                    btnCompareBegin.Enabled = true;
                    chklstComparedImplementations.Enabled = true;
                }
                else
                {
                    btnReset_Click(null, null);
                    Log("模拟开始！请耐心等待或点击“暂停”来结束。");
                    lstTrace.Items.Clear();
                    fastPlayInProgress = true;
                    requestFastPlayEnd = false;
                    btnPlay.Text = "暂停";
                    btnStep.Enabled = false;
                    cmbImplement.Enabled = false;
                    chkFastMode.Enabled = false;
                    chkExternalCacheInfo.Enabled = false;
                    chkCountMissShifts.Enabled = false;
                    btnReset.Enabled = false;
                    RaceTrackLogic logic = cmbImplement.SelectedItem as RaceTrackLogic;
                    bool countMissShifts = chkCountMissShifts.Checked;
                    new Thread(() =>
                    {
                        while (!srTraceReader.EndOfStream)
                            traces.Enqueue(new Trace(srTraceReader.ReadLine()));
                        while (traces.Count > 0)
                        {
                            if (requestFastPlayEnd)
                            {
                                this.Invoke(_FastPlayFinish, "模拟中止。请重置后再次进行。");
                                return;
                            }
                            try
                            {
                                logic.ProcessTrace(traces.Dequeue(), ref shiftCount, ref rwCount, countMissShifts);
                            }
                            catch (RaceTrackLogic.CacheReadMissException)
                            {
                                ++readMissCount;
                            }
                            catch (RaceTrackLogic.CacheCorruptException)
                            {
                                this.Invoke(_FastPlayFinish, "【致命】Cache损坏！");
                                return;
                            }
                        }
                        this.Invoke(_FastPlayFinish, "模拟结束，请关注状态栏结果。");
                    }).Start();
                }
            }
            else
            {
                if (timTracePlay.Enabled)
                {
                    timTracePlay.Stop();
                    btnPlay.Text = "运行";
                    btnStep.Enabled = true;
                    btnReset.Enabled = true;
                }
                else
                {
                    timTracePlay.Start();
                    btnPlay.Text = "暂停";
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                }
            }
        }

        private void timTracePlay_Tick(object sender, EventArgs e)
        {
            btnStep_Click(null, null);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            foreach (Control ctrl in grpControlPanel.Controls)
                ctrl.Enabled = false;

            // 找到所有实现并加入列表
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(RaceTrackLogic)))
                    cmbImplement.Items.Add(t.GetConstructor(Type.EmptyTypes).Invoke(new object[0]));
            }
            cmbImplement.SelectedIndex = 0;

            foreach (object o in cmbImplement.Items)
                chklstComparedImplementations.Items.Add(o);

            lstGroupView.Columns.Add(new ColumnHeader { Text = "偏移" });
            for (int i = 0; i < RaceTrackStatics.DOMAIN_PER_TRACK; i++)
                lstGroupView.Columns.Add(new ColumnHeader { Text = i.ToString() });
            ListViewItem item = new ListViewItem();
            item.UseItemStyleForSubItems = false;
            for (int i = 0; i < RaceTrackStatics.DOMAIN_PER_TRACK; i++)
                item.SubItems.Add("");
            lstGroupView.Items.Add(item);
            item = new ListViewItem();
            item.SubItems[0].Text = "时间戳";
            for (int i = 0; i < RaceTrackStatics.DOMAIN_PER_TRACK; i++)
                item.SubItems.Add("");
            lstGroupView.Items.Add(item);
            item = new ListViewItem();
            item.SubItems[0].Text = "地址";
            for (int i = 0; i < RaceTrackStatics.DOMAIN_PER_TRACK; i++)
                item.SubItems.Add("");
            lstGroupView.Items.Add(item);

            for (int i = 0; i < RaceTrackStatics.GROUP_COUNT; i++)
                cmbGroupView.Items.Add(i);
            cmbGroupView.SelectedIndex = 0;
        }

        private void cmbGroupView_SelectedIndexChanged(object sender, EventArgs e)
        {
            (cmbImplement.SelectedItem as RaceTrackLogic).UpdateView(lstGroupView, int.Parse(cmbGroupView.Text));
        }

        private void btnRefreshGroupView_Click(object sender, EventArgs e)
        {
            (cmbImplement.SelectedItem as RaceTrackLogic).UpdateView(lstGroupView, int.Parse(cmbGroupView.Text));
        }

        bool inComparisionMode = false;
        int cmp_currentLogicID = 0;
        struct Result
        {
            public int shiftCount, rwCount, readMissCount;
            public string implementationName, traceName;
        }
        Result[] compareResults;
        private void btnCompareBegin_Click(object sender, EventArgs e)
        {
            chklstComparedImplementations.Enabled = false;
            btnCompareBegin.Enabled = false;
            chkFastMode.Checked = true;
            inComparisionMode = true;
            cmp_currentLogicID = 0;
            compareResults = new Result[chklstComparedImplementations.CheckedIndices.Count * ofdTrace.FileNames.Length];
            cmbImplement.SelectedIndex = chklstComparedImplementations.CheckedIndices[0];
            btnReset_Click(null, null);
            btnPlay_Click(null, null);
        }

        private void btnExportCSV_Click(object sender, EventArgs e)
        {
            if (sfdExportCSV.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            using (StreamWriter sw = new StreamWriter(sfdExportCSV.FileName, false, Encoding.UTF8))
            {
                StringBuilder sb = new StringBuilder("Trace,名称,Shift次数,读写次数,读Miss次数").Append(Environment.NewLine);
                foreach (Result r in compareResults)
                    sb.Append(r.traceName).Append(',')
                        .Append(r.implementationName).Append(',')
                        .Append(r.shiftCount).Append(',')
                        .Append(r.rwCount).Append(',')
                        .Append(r.readMissCount).Append(Environment.NewLine);
                sw.Write(sb.ToString());
            }
        }

        private void mnuChkLstComparedImplementations_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Name == "mnuSelectAll")
            {
                for (int i = 0; i < chklstComparedImplementations.Items.Count; i++)
                    chklstComparedImplementations.SetItemChecked(i, true);
            }
            else
            {
                for (int i = 0; i < chklstComparedImplementations.Items.Count; i++)
                    chklstComparedImplementations.SetItemChecked(i, false);
            }
        }

        private void chkExternalCacheInfo_CheckedChanged(object sender, EventArgs e)
        {
            RaceTrackLogic.externalCacheInfo = chkExternalCacheInfo.Checked;
        }
    }
}
