using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csRaceTrack
{
    #region 基本实现
    /// <summary>
    /// 所有RaceTrack实现的基类。
    /// </summary>
    public abstract class RaceTrackLogic
    {
        [Serializable]
        public class CacheReadMissException : Exception { }
        [Serializable]
        public class CacheCorruptException : Exception { }

        /// <summary>
        /// 条带每个域所存储信息的标识符，用于验证所读内容正确性的类。（实际上属于调试用）
        /// </summary>
        protected class CacheEntry
        {
            public Trace trace;
            public int partID;
            public long lastTimeStamp;

            public override bool Equals(object obj)
            {
                if (!(obj is CacheEntry))
                    return false;
                CacheEntry b = obj as CacheEntry;
                return trace == b.trace && lastTimeStamp == b.lastTimeStamp && partID == b.partID;
            }
        }

        /// <summary>
        /// 端口定义。
        /// </summary>
        protected class RaceTrackPort
        {
            /// <summary>
            /// 端口类型。
            /// </summary>
            public enum PortType
            {
                ReadOnly, WriteOnly, ReadWrite
            }
            public PortType type;
            public int position;
        }

        internal static FrmMain mainForm;
        internal static RaceTrackLogic currentLogic;

        /// <summary>
        /// 标示是否允许将 Cache 相关信息（时间戳、Tag）等存入 Cache 外，以免除额外的读操作。
        /// </summary>
        internal static bool externalCacheInfo = true;

        private CacheEntry[,] blockInfo;

        /// <summary>
        /// 用于存储 Cache 相关信息的数组。如果不允许使用将会返回空。
        /// </summary>
        protected CacheEntry[,] BlockInfo
        {
            get { if (externalCacheInfo) return blockInfo; else return null; }
        }

        private int shiftAmount, rwCount;

        /// <summary>
        /// 条带组。
        /// </summary>
        protected class RaceTrackGroup
        {
            /// <summary>
            /// 存储读写请求相关信息的类。
            /// </summary>
            public class RaceTrackGroupRWRequestBundle
            {
                public int portID;
                public CacheEntry[] data;
            }

            private int position = 0;

            /// <summary>
            /// 注意第一维是列号（各个条带同一个Port所对的域集合）
            /// </summary>
            private CacheEntry[][] dataGrid = new CacheEntry[RaceTrackStatics.DOMAIN_PER_TRACK][];

            /// <summary>
            /// 仅用于显示条带状况。
            /// </summary>
            private long[] lastWrite = new long[RaceTrackStatics.DOMAIN_PER_TRACK];
            private int[] lastAddr = new int[RaceTrackStatics.DOMAIN_PER_TRACK];

            /// <summary>
            /// <para>条带位置，初始为0，进行改动将会记录shift次数。下图所示时为正值。</para>
            /// <para>[-------------RaceTrack-------------]</para>
            /// <para>|--Position--|-------------Ports-------------|</para>
            /// </summary>
            public int Position
            {
                get
                {
                    return position;
                }

                set
                {
                    currentLogic.shiftAmount += Math.Abs(value - position);
                    position = value;
                }
            }

            /// <summary>
            /// 与另一个条带组同时进行移动。
            /// </summary>
            /// <param name="b">另一个条带组。</param>
            /// <param name="amount">移动偏移。</param>
            public void ShiftWith(RaceTrackGroup b, int amount)
            {
                this.position += amount;
                b.position += amount;
                currentLogic.shiftAmount += Math.Abs(amount);
            }

            /// <summary>
            /// 读取Group中指定端口所对应的内容。
            /// </summary>
            /// <param name="requests">要读取内容的端口，最多4个。</param>
            public void Read(params RaceTrackGroupRWRequestBundle[] requests)
            {
                if (requests.Length > 4)
                    throw new ArgumentOutOfRangeException();
                currentLogic.rwCount++;
                foreach (RaceTrackGroupRWRequestBundle req in requests)
                {
                    RaceTrackPort port = currentLogic.Ports[req.portID];
                    if (port.type == RaceTrackPort.PortType.WriteOnly)
                        throw new InvalidOperationException();
                    req.data = dataGrid[port.position + Position];
                }
            }

            /// <summary>
            /// 将内容写入Group中所对应的指定端口。
            /// </summary>
            /// <param name="requests">要写入内容的端口和内容，最多4个。</param>
            public void Write(params RaceTrackGroupRWRequestBundle[] requests)
            {
                if (requests.Length > 4)
                    throw new ArgumentOutOfRangeException();
                currentLogic.rwCount++;
                foreach (RaceTrackGroupRWRequestBundle req in requests)
                {
                    RaceTrackPort port = currentLogic.Ports[req.portID];
                    if (port.type == RaceTrackPort.PortType.ReadOnly)
                        throw new InvalidOperationException();
                    dataGrid[port.position + Position] = req.data;
                    lastWrite[port.position + Position] = req.data[0].lastTimeStamp;
                    lastAddr[port.position + Position] = req.data[0].trace.address;
                }
            }

            /// <summary>
            /// 将部分内容写入Group中所对应的指定端口。
            /// </summary>
            /// <param name="offset">写入内容的起始条带。</param>
            /// <param name="requests">要写入内容的端口和内容，最多4个。</param>
            public void PartialWrite(int offset, params RaceTrackGroupRWRequestBundle[] requests)
            {
                if (requests.Length > 4)
                    throw new ArgumentOutOfRangeException();
                currentLogic.rwCount++;
                foreach (RaceTrackGroupRWRequestBundle req in requests)
                {
                    RaceTrackPort port = currentLogic.Ports[req.portID];
                    if (port.type == RaceTrackPort.PortType.ReadOnly)
                        throw new InvalidOperationException();
                    if (dataGrid[port.position + Position] == null)
                        dataGrid[port.position + Position] = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                    for (int i = 0; i < req.data.Length; i++)
                        dataGrid[port.position + Position][i + offset] = req.data[i];
                    lastWrite[port.position + Position] = req.data[0].lastTimeStamp;
                    lastAddr[port.position + Position] = req.data[0].trace.address;
                }
            }

            static string[] ColourValues = new string[] { "FFFFFF",
                "FF0000", "00FF00", "0000FF", "FFFF00", "FF00FF", "00FFFF", "000000", 
                "800000", "008000", "000080", "808000", "800080", "008080", "808080", 
                "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0", 
                "400000", "004000", "000040", "404000", "400040", "004040", "404040", 
                "200000", "002000", "000020", "202000", "200020", "002020", "202020", 
                "600000", "006000", "000060", "606000", "600060", "006060", "606060", 
                "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0", 
                "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0", 
            };

            /// <summary>
            /// 在列表视图中表现组情况。
            /// </summary>
            /// <param name="lst">列表控件。</param>
            public void UpdateView(ListView lst)
            {
                ListViewItem item = lst.Items[0], text = lst.Items[1], addr = lst.Items[2];
                item.SubItems[0].Text = Position.ToString();
                for (int i = 0; i < RaceTrackStatics.DOMAIN_PER_TRACK; i++)
                {
                    item.SubItems[i + 1].BackColor = ColorTranslator.FromHtml("#" + ColourValues[lastWrite[i] % ColourValues.Length]);
                    text.SubItems[i + 1].Text = lastWrite[i].ToString();
                    addr.SubItems[i + 1].Text = "0x" + Convert.ToString(lastAddr[i], 16);
                }
            }
        }
        private RaceTrackGroup[] groups = new RaceTrackGroup[RaceTrackStatics.GROUP_COUNT];

        public RaceTrackLogic()
        {
            Reset();
        }

        /// <summary>
        /// 复位RaceTrack。
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < RaceTrackStatics.GROUP_COUNT; i++)
                groups[i] = new RaceTrackGroup();
            blockInfo = new CacheEntry[RaceTrackStatics.GROUP_COUNT, RaceTrackStatics.DOMAIN_PER_TRACK];
        }

        /// <summary>
        /// 获取条带组。
        /// </summary>
        /// <param name="groupID">条带组编号。</param>
        /// <returns>指定编号的条带组。</returns>
        protected RaceTrackGroup this[int groupID]
        {
            get
            {
                return groups[groupID];
            }
        }

        /// <summary>
        /// 处理一条Trace。
        /// </summary>
        /// <param name="t">要处理的Trace。</param>
        public void ProcessTrace(Trace t, ref int shiftAmount, ref int rwCount, bool countMissShift)
        {
            currentLogic = this;

            // 重置计数器
            this.shiftAmount = 0;
            this.rwCount = 0;

            // 执行操作
            t.address = t.address / RaceTrackStatics.TRACK_PER_GROUP * RaceTrackStatics.TRACK_PER_GROUP;
            object result;
            try
            {
                result = ActualProcess(t);
            }
            catch (CacheReadMissException)
            {
                if (countMissShift)
                {
                    shiftAmount += this.shiftAmount;
                    rwCount += this.rwCount;
                }
                throw;
            }

            // 检查数据合法性
            if (t.isRead)
            {
                CacheEntry[] data = result as CacheEntry[];
                long timeStamp = data[0].trace.timeStamp;
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].trace.address != t.address || data[i].trace.timeStamp != timeStamp || data[i].partID != i)
                        throw new CacheCorruptException();
                }
            }
            shiftAmount += this.shiftAmount;
            rwCount += this.rwCount;
        }

        /// <summary>
        /// 在列表视图中表现组情况。
        /// </summary>
        /// <param name="lst">列表控件。</param>
        /// <param name="groupID">组号。</param>
        public void UpdateView(ListView lst, int groupID)
        {
            this[groupID].UpdateView(lst);
        }

        /// <summary>
        /// （调试用）与另一种实现比较外部Cache信息并在不同时触发中断。
        /// </summary>
        /// <param name="b">另一个实现。</param>
        public void CompareExtInfo(RaceTrackLogic b)
        {
            for (int i = 0; i < blockInfo.GetLength(0); i++)
                for (int j = 0; j < blockInfo.GetLength(1); j++)
                    if ((blockInfo[i, j] != null && !blockInfo[i, j].Equals(b.blockInfo[i, j])) || b.blockInfo[i, j] != null)
                    {
                        Debug.Assert(false);
                    }
        }

        /// <summary>
        /// 进行Trace的实际处理。
        /// </summary>
        /// <param name="t">要处理的Trace。</param>
        protected abstract object ActualProcess(Trace t);

        /// <summary>
        /// 对所有条带组的读写端口的定义。
        /// </summary>
        protected abstract RaceTrackPort[] Ports { get; }
    }
    
    /// <summary>
    /// 基准RaceTrack实现。（在所给的要求下实现了性能最佳的版本）
    /// </summary>
    public class BaseLine : RaceTrackLogic
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 0, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 16, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 32, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 48, type = RaceTrackPort.PortType.ReadWrite }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 就近原则移动端口到对应区域。
        /// </summary>
        /// <param name="currGroup">要移动的条带组。</param>
        /// <param name="isRead">是否是读操作。null表示不关心。</param>
        /// <param name="targetDomains">可接受的目标 Domain 列表。</param>
        /// <param name="bestPort">所找到的最近的端口。</param>
        /// <returns>是否停在第一个 Domain 的位置。</returns>
        protected bool NaiveMoveToRegion(RaceTrackGroup currGroup, bool? isRead, int[] targetDomains, out int bestPort)
        {
            int minDist = int.MaxValue, bestPos = Ports[0].position - targetDomains[0];
            bestPort = 0;
            bool forward = true;
            for (int i = 0; i < Ports.Length; i++)
            {
                if ((isRead == true && Ports[i].type == RaceTrackPort.PortType.WriteOnly) ||
                    (isRead == false && Ports[i].type == RaceTrackPort.PortType.ReadOnly))
                    continue;
                for (int j = 0; j < targetDomains.Length; j++)
                    if (Math.Abs(Ports[i].position + currGroup.Position - targetDomains[j]) < minDist)
                    {
                        minDist = Math.Abs(Ports[i].position + currGroup.Position - targetDomains[j]);
                        bestPort = i;
                        bestPos = targetDomains[j] - Ports[i].position;
                        forward = j == 0;
                    }
            }
            currGroup.Position = bestPos;
            return forward;
        }

        /// <summary>
        /// 用于从地址计算出Set编号。更改其实现相当于更改Set编址策略。
        /// </summary>
        /// <param name="addr">访问地址。</param>
        /// <returns>假设Set是顺序编号时，这个地址应该对应到的Set的编号。</returns>
        protected virtual int GetSetIDFromAddress(int addr)
        {
            return (addr / RaceTrackStatics.TRACK_PER_GROUP) % RaceTrackStatics.SET_COUNT;
        }

        protected override object ActualProcess(Trace t)
        {

            // 确定组号
            int setID = GetSetIDFromAddress(t.address);

            // 确定位置
            int groupID = setID / (RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY);
            int beginDomain = setID % (RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY), endDomain; // [begin, end]
            beginDomain = beginDomain * RaceTrackStatics.ASSOCIATIVITY;
            endDomain = beginDomain + RaceTrackStatics.ASSOCIATIVITY - 1;

            // 进行移动
            RaceTrackGroup currGroup = this[groupID];

            // 试图使用外部记录信息直接确定目标位置
            if (BlockInfo == null)
            {
                // 没有外部信息……
                bool forward = true;
                int bestPort;
                forward = NaiveMoveToRegion(currGroup, true, new int[] { beginDomain, endDomain }, out bestPort); // 移动条带到距离最近的读口

                mainForm.Log(String.Format("Set号：{0}，条带组号：{1}，域范围：[{2}, {3}]", setID, groupID, beginDomain, endDomain));
                int minTimeStampDomain = 0, lastEmptyDomain = -1;
                long minTimeStamp = long.MaxValue;
                if (t.isRead)
                {
                    // 读
                    for (int i = 0; i < RaceTrackStatics.ASSOCIATIVITY; i++)
                    {
                        if (i > 0)
                            if (forward)
                                currGroup.Position++;
                            else
                                currGroup.Position--;
                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        currGroup.Read(req);
                        if (req.data == null)
                        {
                            lastEmptyDomain = currGroup.Position + Ports[bestPort].position;
                            continue;
                        }
                        if (req.data[0].trace.address == t.address)
                        {
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            {
                                req.data[j].lastTimeStamp = t.timeStamp;
                            }
                            mainForm.Log(String.Format("【读取】Hit！{0}向测试到第{1}行，造成最初写入的Trace：{2}", forward ? "正" : "反", i, req.data[0].trace));
                            return req.data;
                        }
                        if (req.data[0].lastTimeStamp < minTimeStamp)
                        {
                            minTimeStamp = req.data[0].lastTimeStamp;
                            minTimeStampDomain = currGroup.Position + Ports[bestPort].position;
                        }
                    }
                    if (lastEmptyDomain == -1)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampDomain }, out bestPort); // 移动条带到距离最近的写口

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        mainForm.Log(String.Format("【读取】Evict！驱逐了{0}向测试到的第{1}行，写入时的Trace：{2}", forward ? "正" : "反", minTimeStampDomain, t));
                        throw new CacheReadMissException();
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, new int[] { lastEmptyDomain }, out bestPort); // 移动条带到距离最近的写口

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        mainForm.Log(String.Format("【读取】Miss！填充了{0}向测试到的第{1}行，写入时的Trace：{2}", forward ? "正" : "反", lastEmptyDomain, t));
                        throw new CacheReadMissException();
                    }
                }
                else
                {
                    // 写
                    for (int i = 0; i < RaceTrackStatics.ASSOCIATIVITY; i++)
                    {
                        if (i > 0)
                            if (forward)
                                currGroup.Position++;
                            else
                                currGroup.Position--;
                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        currGroup.Read(req);
                        if (req.data == null)
                        {
                            lastEmptyDomain = currGroup.Position + Ports[bestPort].position;
                            continue;
                        }
                        if (req.data[0].trace.address == t.address)
                        {
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            {
                                req.data[j].lastTimeStamp = t.timeStamp;
                                req.data[j].trace = t;
                            }
                            NaiveMoveToRegion(currGroup, false, new int[] { Ports[bestPort].position + currGroup.Position }, out bestPort); // 移动条带到距离最近的写口
                            currGroup.Write(req);
                            mainForm.Log(String.Format("【写入】Hit！{0}向测试到第{1}行，写入时的Trace：{2}", forward ? "正" : "反", i, t));
                            return null;
                        }
                        if (req.data[0].lastTimeStamp < minTimeStamp)
                        {
                            minTimeStamp = req.data[0].lastTimeStamp;
                            minTimeStampDomain = currGroup.Position + Ports[bestPort].position;
                        }
                    }
                    if (lastEmptyDomain == -1)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampDomain }, out bestPort); // 移动条带到距离最近的写口

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        mainForm.Log(String.Format("【写入】Evict！驱逐了{0}向测试到的第{1}行，写入时的Trace：{2}", forward ? "正" : "反", minTimeStampDomain, t));
                        return null;
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, new int[] { lastEmptyDomain }, out bestPort); // 移动条带到距离最近的写口

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        mainForm.Log(String.Format("【写入】Miss！填充了{0}向测试到的第{1}行，写入时的Trace：{2}", forward ? "正" : "反", lastEmptyDomain, t));
                        return null;
                    }
                }
            }
            else
            {
                int targetDomain = -1, minTimeStampDomain = 0;
                List<int> emptyDomains = new List<int>();
                long minTimeStamp = long.MaxValue;
                // 通过外部信息确定是否有要找的块
                for (int i = beginDomain; i <= endDomain; i++)
                    if (BlockInfo[groupID, i] != null)
                    {
                        if (BlockInfo[groupID, i].trace.address == t.address)
                            targetDomain = i;
                        else if (BlockInfo[groupID, i].lastTimeStamp < minTimeStamp)
                        {
                            minTimeStamp = BlockInfo[groupID, i].lastTimeStamp;
                            minTimeStampDomain = i;
                        }
                    }
                    else
                    {
                        emptyDomains.Add(i);
                    }

                int bestPort;

                mainForm.Log(String.Format("Set号：{0}，条带组号：{1}，目标域：{2}，最小时间戳：{3}，空域数：{4}",
                    setID, groupID, targetDomain, minTimeStampDomain, emptyDomains.Count));
                if (t.isRead)
                {
                    // 读
                    if (targetDomain != -1)
                    {
                        NaiveMoveToRegion(currGroup, true, new int[] { targetDomain }, out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        currGroup.Read(req);
                        BlockInfo[groupID, targetDomain].lastTimeStamp = t.timeStamp;
                        mainForm.Log(String.Format("【读取】Hit！造成最初写入的Trace：{0}", req.data[0].trace));
                        return req.data;
                    }
                    if (emptyDomains.Count == 0)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampDomain }, out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        BlockInfo[groupID, minTimeStampDomain] = req.data[0];
                        mainForm.Log(String.Format("【读取】Evict！驱逐了第{0}域，写入时的Trace：{1}", minTimeStampDomain, t));
                        throw new CacheReadMissException();
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, emptyDomains.ToArray(), out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        BlockInfo[groupID, currGroup.Position + Ports[bestPort].position] = req.data[0];
                        mainForm.Log(String.Format("【读取】Miss！填充了第{0}域，写入时的Trace：{1}", currGroup.Position + Ports[bestPort].position, t));
                        throw new CacheReadMissException();
                    }
                }
                else
                {
                    // 写
                    if (targetDomain != -1)
                    {
                        NaiveMoveToRegion(currGroup, false, new int[] { targetDomain }, out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };

                        currGroup.Write(req);
                        BlockInfo[groupID, targetDomain].lastTimeStamp = t.timeStamp;
                        mainForm.Log(String.Format("【写入】Hit！写入时的Trace：{0}", t));
                        return null;
                    }
                    if (emptyDomains.Count == 0)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampDomain }, out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        BlockInfo[groupID, minTimeStampDomain] = req.data[0];
                        mainForm.Log(String.Format("【写入】Evict！驱逐了第{0}域，写入时的Trace：{1}", minTimeStampDomain, t));
                        return null;
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, emptyDomains.ToArray(), out bestPort);

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle req = new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = bestPort };
                        req.data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP; j++)
                            req.data[j] = new CacheEntry { lastTimeStamp = t.timeStamp, trace = t, partID = j };
                        currGroup.Write(req);
                        BlockInfo[groupID, currGroup.Position + Ports[bestPort].position] = req.data[0];
                        mainForm.Log(String.Format("【写入】Miss！填充了第{0}域，写入时的Trace：{1}", currGroup.Position + Ports[bestPort].position, t));
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "BaseLine";
        }
    }
    #endregion

    #region 进阶实现

    /// <summary>
    /// 增加了读写端口密度。
    /// </summary>
    public class Naive : BaseLine
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 0, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 12, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 24, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 36, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 48, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 60, type = RaceTrackPort.PortType.ReadWrite }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "Naive";
        }
    }

    /// <summary>
    /// 读写端口平移2个domain。
    /// </summary>
    public class Naive2 : BaseLine
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 2, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 14, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 26, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 38, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 50, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 62, type = RaceTrackPort.PortType.ReadWrite }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "Naive2";
        }
    }

    /// <summary>
    /// 读写端口平移4个domain。
    /// </summary>
    public class Naive4 : BaseLine
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 4, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 16, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 28, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 40, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 52, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 64, type = RaceTrackPort.PortType.ReadWrite }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "Naive4";
        }
    }

    /// <summary>
    /// 读写端口分离，一读一写排布。
    /// </summary>
    public class RWPattern : BaseLine
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 0, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 4, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 12, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 16, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 24, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 28, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 36, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 40, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 48, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 52, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 60, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 64, type = RaceTrackPort.PortType.WriteOnly }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "RWPattern";
        }
    }

    /// <summary>
    /// 读写端口分离，一读一写排布，调整了方向。
    /// </summary>
    public class RWPattern2 : BaseLine
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 3, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 4, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 15, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 16, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 27, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 28, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 39, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 40, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 51, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 52, type = RaceTrackPort.PortType.WriteOnly },
            new RaceTrackPort { position = 63, type = RaceTrackPort.PortType.ReadOnly },
            new RaceTrackPort { position = 64, type = RaceTrackPort.PortType.WriteOnly }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "RWPattern2";
        }
    }

    /// <summary>
    /// 对Cache的Set进行重新编址，使得第一个条带组有原第0、128、256、……号Set。
    /// </summary>
    public class SetReordered : BaseLine
    {
        protected override int GetSetIDFromAddress(int addr)
        {
            int setPerGroup = RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY,
                original = base.GetSetIDFromAddress(addr),
                transformed = (original % setPerGroup) * RaceTrackStatics.GROUP_COUNT + original / setPerGroup;
            mainForm.Log("Set重新编址：" + original + " -> " + transformed);
            return transformed;
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "SetReordered";
        }
    }

    /// <summary>
    /// <para>改变Set划分，将Cache的Block打散，使得第一个条带组是这样的：</para>
    /// <para>条带000-127 |--Set0--|--Set1--|--Set0--|--Set1--|--Set0--|--Set1--|--Set0--|--Set1--|</para>
    /// <para>条带128-255 |--Set2--|--Set3--|--Set2--|--Set3--|--Set2--|--Set3--|--Set2--|--Set3--|</para>
    /// <para>条带256-383 |--Set4--|--Set5--|--Set4--|--Set5--|--Set4--|--Set5--|--Set4--|--Set5--|</para>
    /// <para>条带384-511 |--Set6--|--Set7--|--Set6--|--Set7--|--Set6--|--Set7--|--Set6--|--Set7--|</para>
    /// </summary>
    public class BlockShattered : RaceTrackLogic
    {
        /// <summary>
        /// 读写端口定义。
        /// </summary>
        private readonly RaceTrackPort[] ports = new RaceTrackPort[] {
            new RaceTrackPort { position = 0, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 16, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 32, type = RaceTrackPort.PortType.ReadWrite },
            new RaceTrackPort { position = 48, type = RaceTrackPort.PortType.ReadWrite }
        };
        protected override RaceTrackLogic.RaceTrackPort[] Ports
        {
            get { return ports; }
        }

        /// <summary>
        /// 就近原则移动端口到对应区域。
        /// </summary>
        /// <param name="currGroup">要移动的条带组。</param>
        /// <param name="isRead">是否是读操作。null表示不关心。</param>
        /// <param name="targetDomains">可接受的目标 Domain 列表。</param>
        protected void NaiveMoveToRegion(RaceTrackGroup currGroup, bool? isRead, int[] targetDomains)
        {
            int minDist = int.MaxValue, bestPos = Ports[0].position - targetDomains[0];
            if ((isRead == true && Ports[0].type == RaceTrackPort.PortType.WriteOnly) ||
                (isRead == false && Ports[0].type == RaceTrackPort.PortType.ReadOnly))
                return;
            for (int j = 0; j < targetDomains.Length; j++)
                if (Math.Abs(Ports[0].position + currGroup.Position - targetDomains[j]) < minDist)
                {
                    minDist = Math.Abs(Ports[0].position + currGroup.Position - targetDomains[j]);
                    bestPos = targetDomains[j] - Ports[0].position;
                }
            currGroup.Position = bestPos;
        }

        /// <summary>
        /// 用于从地址计算出Set编号。更改其实现相当于更改Set编址策略。
        /// </summary>
        /// <param name="addr">访问地址。</param>
        /// <returns>假设Set是顺序编号时，这个地址应该对应到的Set的编号。</returns>
        protected virtual int GetSetIDFromAddress(int addr)
        {
            return (addr / RaceTrackStatics.TRACK_PER_GROUP) % RaceTrackStatics.SET_COUNT;
        }

        protected override object ActualProcess(Trace t)
        {

            // 确定组号
            int setID = GetSetIDFromAddress(t.address);

            // 确定位置
            int groupID = setID / (RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY),
                inGroupSetOffset = (setID % (RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY)) * RaceTrackStatics.ASSOCIATIVITY, // 仅用于BlockInfo的下标
                trackOffset = ((setID % (RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY)) / 2) * 128,
                domainOffset = setID % 2 == 1 ? RaceTrackStatics.ASSOCIATIVITY : 0;

            // 进行移动
            RaceTrackGroup currGroup = this[groupID];

            // 试图使用外部记录信息直接确定目标位置
            if (BlockInfo == null)
            {
                // 没有外部信息……
                // 那还玩个鸟啊(╯‵□′)╯︵┻━┻
                throw new NotImplementedException();
            }
            else
            {
                int targetBlock = -1, minTimeStampBlock = 0;
                List<int> emptyBlocks = new List<int>();
                long minTimeStamp = long.MaxValue;

                // 通过外部信息确定是否有要找的块
                for (int i = 0; i < RaceTrackStatics.ASSOCIATIVITY; i++)
                    if (BlockInfo[groupID, inGroupSetOffset + i] != null)
                    {
                        if (BlockInfo[groupID, inGroupSetOffset + i].trace.address == t.address)
                            targetBlock = i;
                        else if (BlockInfo[groupID, inGroupSetOffset + i].lastTimeStamp < minTimeStamp)
                        {
                            minTimeStamp = BlockInfo[groupID, inGroupSetOffset + i].lastTimeStamp;
                            minTimeStampBlock = i;
                        }
                    }
                    else
                    {
                        emptyBlocks.Add(i + domainOffset);
                    }

                mainForm.Log(String.Format("Set号：{0}，条带组号：{1}，目标块：{2}，最小时间戳：{3}，空块数：{4}",
                    setID, groupID, targetBlock, minTimeStampBlock, emptyBlocks.Count));
                if (t.isRead)
                {
                    // 读
                    if (targetBlock != -1)
                    {
                        NaiveMoveToRegion(currGroup, true, new int[] { targetBlock + domainOffset });

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        currGroup.Read(reqs);
                        BlockInfo[groupID, inGroupSetOffset + targetBlock].lastTimeStamp = t.timeStamp;

                        CacheEntry[] data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP];
                        for (int i = 0; i < 4; i++)
                        {
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                data[i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j] = reqs[i].data[j + trackOffset];
                        }
                        mainForm.Log(String.Format("【读取】Hit！造成最初写入的Trace：{0}", data[0].trace));
                        return data;
                    }
                    if (emptyBlocks.Count == 0)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampBlock + domainOffset });

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            reqs[i].data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP / 4];
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                reqs[i].data[j] = new CacheEntry {
                                    lastTimeStamp = t.timeStamp, trace = t, partID = i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j
                                };
                        }
                        currGroup.PartialWrite(trackOffset, reqs);
                        BlockInfo[groupID, inGroupSetOffset + minTimeStampBlock] = reqs[0].data[0];
                        mainForm.Log(String.Format("【读取】Evict！驱逐了第{0}块，写入时的Trace：{1}", minTimeStampBlock, t));
                        throw new CacheReadMissException();
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, emptyBlocks.ToArray());

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            reqs[i].data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP / 4];
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                reqs[i].data[j] = new CacheEntry
                                {
                                    lastTimeStamp = t.timeStamp,
                                    trace = t,
                                    partID = i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j
                                };
                        }
                        currGroup.PartialWrite(trackOffset, reqs);
                        BlockInfo[groupID, inGroupSetOffset + currGroup.Position - domainOffset] = reqs[0].data[0];
                        mainForm.Log(String.Format("【读取】Miss！填充了第{0}块，写入时的Trace：{1}", currGroup.Position, t));
                        throw new CacheReadMissException();
                    }
                }
                else
                {
                    // 写
                    if (targetBlock != -1)
                    {
                        NaiveMoveToRegion(currGroup, false, new int[] { targetBlock + domainOffset });

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            reqs[i].data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP / 4];
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                reqs[i].data[j] = new CacheEntry
                                {
                                    lastTimeStamp = t.timeStamp,
                                    trace = t,
                                    partID = i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j
                                };
                        }
                        currGroup.PartialWrite(trackOffset, reqs);
                        BlockInfo[groupID, inGroupSetOffset + targetBlock].lastTimeStamp = t.timeStamp;
                        mainForm.Log(String.Format("【写入】Hit！写入时的Trace：{0}", t));
                        return null;
                    }
                    if (emptyBlocks.Count == 0)
                    {
                        // 需要驱逐

                        // LRU策略

                        NaiveMoveToRegion(currGroup, false, new int[] { minTimeStampBlock + domainOffset });

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            reqs[i].data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP / 4];
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                reqs[i].data[j] = new CacheEntry
                                {
                                    lastTimeStamp = t.timeStamp,
                                    trace = t,
                                    partID = i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j
                                };
                        }
                        currGroup.PartialWrite(trackOffset, reqs);
                        BlockInfo[groupID, inGroupSetOffset + minTimeStampBlock] = reqs[0].data[0];
                        mainForm.Log(String.Format("【写入】Evict！驱逐了第{0}块，写入时的Trace：{1}", minTimeStampBlock, t));
                        return null;
                    }
                    else
                    {
                        // 找空位插入

                        NaiveMoveToRegion(currGroup, false, emptyBlocks.ToArray());

                        RaceTrackLogic.RaceTrackGroup.RaceTrackGroupRWRequestBundle[] reqs = new RaceTrackGroup.RaceTrackGroupRWRequestBundle[] {
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 0 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 1 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 2 },
                            new RaceTrackGroup.RaceTrackGroupRWRequestBundle() { portID = 3 }
                        };
                        for (int i = 0; i < 4; i++)
                        {
                            reqs[i].data = new CacheEntry[RaceTrackStatics.TRACK_PER_GROUP / 4];
                            for (int j = 0; j < RaceTrackStatics.TRACK_PER_GROUP / 4; j++)
                                reqs[i].data[j] = new CacheEntry
                                {
                                    lastTimeStamp = t.timeStamp,
                                    trace = t,
                                    partID = i * RaceTrackStatics.TRACK_PER_GROUP / 4 + j
                                };
                        }
                        currGroup.PartialWrite(trackOffset, reqs);
                        BlockInfo[groupID, inGroupSetOffset + currGroup.Position - domainOffset] = reqs[0].data[0];
                        mainForm.Log(String.Format("【写入】Miss！填充了第{0}块，写入时的Trace：{1}", currGroup.Position, t));
                        return null;
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "BlockShattered";
        }
    }

    /// <summary>
    /// 对BlockShattered的Set进行重新编址，使得第一个条带组有原第0、128、256、……号Set。
    /// </summary>
    public class BSSetReordered : BlockShattered
    {
        protected override int GetSetIDFromAddress(int addr)
        {
            int setPerGroup = RaceTrackStatics.DOMAIN_PER_TRACK / RaceTrackStatics.ASSOCIATIVITY,
                original = base.GetSetIDFromAddress(addr),
                transformed = (original % setPerGroup) * RaceTrackStatics.GROUP_COUNT + original / setPerGroup;
            mainForm.Log("Set重新编址：" + original + " -> " + transformed);
            return transformed;
        }

        /// <summary>
        /// 获取当前RaceTrack实现的名字。
        /// </summary>
        /// <returns>当前RaceTrack实现的名字。</returns>
        public override string ToString()
        {
            return "BS_SetReordered";
        }
    }

    #endregion

    #region 工具类

    /// <summary>
    /// 包含Trace条目内容的类。
    /// </summary>
    public class Trace
    {
        public bool isRead;
        public int address;
        public long timeStamp;

        /// <summary>
        /// 通过文件中的行创建Trace条目。
        /// </summary>
        /// <param name="strTrace">文件中的一行。</param>
        public Trace(string strTrace)
        {
            string[] parts = strTrace.Split(' ');
            isRead = parts[0] == "r";
            address = int.Parse(parts[1]);
            timeStamp = long.Parse(parts[4]);
        }

        public override string ToString()
        {
            return "[" + timeStamp + "] " + (isRead ? "读 " : "写 ") + "0x" + Convert.ToString(address, 16);
        }
    }

    /// <summary>
    /// RaceTrack相关参数集合。
    /// </summary>
    static class RaceTrackStatics
    {
        /// <summary>
        /// CPU时钟频率。
        /// </summary>
        public const int CLOCK_FREQ_GHZ = 2;

        /// <summary>
        /// 每周期的Tick数。
        /// </summary>
        public const int TICK_PER_CYCLE = 1000;

        /// <summary>
        /// 缓存相联度。
        /// </summary>
        public const int ASSOCIATIVITY = 8;

        /// <summary>
        /// 读写延迟（cycle）。
        /// </summary>
        public const int R_W_LATENCY = 1;

        /// <summary>
        /// 请求访问延迟（cycle）。
        /// </summary>
        public const int ACCESS_LATENCY = 6;

        /// <summary>
        /// 单位平移操作延迟（cycle）。
        /// </summary>
        public const int SHIFT_LATENCY = 1;

        /// <summary>
        /// 每个条带的域数。
        /// </summary>
        public const int DOMAIN_PER_TRACK = 64;

        /// <summary>
        /// 每个条带组的条带数。
        /// </summary>
        public const int TRACK_PER_GROUP = 512;

        /// <summary>
        /// 整个RaceTrack缓存中的条带组数。
        /// </summary>
        public const int GROUP_COUNT = 128;

        /// <summary>
        /// 读写端口大小。
        /// </summary>
        public const int RW_PORT_WIDTH = 12;

        /// <summary>
        /// 写端口大小。
        /// </summary>
        public const int W_PORT_WIDTH = 8;

        /// <summary>
        /// 读端口大小。
        /// </summary>
        public const int R_PORT_WIDTH = 4;

        /// <summary>
        /// Cache的组数。
        /// </summary>
        public const int SET_COUNT = GROUP_COUNT * DOMAIN_PER_TRACK / ASSOCIATIVITY;
    }
    #endregion
}
