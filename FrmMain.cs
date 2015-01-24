using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
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

        private ListViewItem Trace2Row(int id, Trace trace)
        {
            ListViewItem item = new ListViewItem();
            item.SubItems[0].Text = id.ToString();
            item.SubItems.Add(trace.timeStamp.ToString());
            item.SubItems.Add(trace.isRead ? "读" : "写");
            item.SubItems.Add("0x" + Convert.ToString(trace.address, 16));
            return item;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofdTrace.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btnReset_Click(null, null);
            }
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            cmbImplement.Enabled = false;
            chkCountMissShifts.Enabled = false;
            chkFastMode.Enabled = false;
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
            if (File.Exists(ofdTrace.FileName))
            {
                txtPath.Text = ofdTrace.FileName;
                traces = new Queue<Trace>();
                fs = ofdTrace.OpenFile();
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
                    btnPlay.Text = "运行";
                    btnStep.Enabled = true;
                    btnReset.Enabled = true;
                }
                else
                {
                    Log("模拟开始！请耐心等待或点击“暂停”来结束。");
                    lstTrace.Items.Clear();
                    fastPlayInProgress = true;
                    requestFastPlayEnd = false;
                    btnPlay.Text = "暂停";
                    btnStep.Enabled = false;
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
            cmbImplement.Items.Add(new BaseLine());
            cmbImplement.Items.Add(new Naive());
            cmbImplement.SelectedIndex = 0;

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
    }
}
