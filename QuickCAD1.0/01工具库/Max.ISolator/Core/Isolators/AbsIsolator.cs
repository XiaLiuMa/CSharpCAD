﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using Max.BaseKit;
using Max.BaseKit.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Max.ISolator.Core.Mods;
using Org.BouncyCastle.Utilities;

namespace Max.ISolator.Core.Isolators
{
    /// <summary>
    /// 抽象隔离器
    /// </summary>
    public abstract class AbsIsolator : IIsolator
    {
        public abstract string Id { get; set; }
        public abstract IsolatorModel Model { get; set; }
        public abstract void BootUp();
        public abstract void ProduceData(IsolatorData data);
        public abstract void ShutDown();
        /// <summary>
        /// 执行生产
        /// </summary>
        /// <param name="fname"></param>
        protected abstract void ExecuteProduce(string fname);

        protected CancellationTokenSource _BootUpProducerModelTokenSource1;
        protected CancellationTokenSource _BootUpProducerModelTokenSource2;

        /// <summary>
        /// 启动生产者模式
        /// </summary>
        protected void BootUpProducerModel()
        {
            #region 1、扫描生产缓存区的文件名加入到待生产区,模式启动之后生产的文件名是会自动加入到待生产区的，所以只需要启动的时候执行一次
            try
            {
                //0@1@20240106095407083350@001@BJ_T_YW_CRJRY@M1001_20230620000000_20230620235959_1_1.txt
                var files = FileUtil.FindFiles(new DirectoryInfo(Model.ProduceCachePath), Id, "*.txt");//M1001就是要找的文件
                files?.ForEach(f =>
                {
                    Model.WaitProduceFiles.Add(f.FullName);
                });
            }
            catch (Exception ex)
            {
                NLogger.Warn($"扫描生产缓存区文件异常：{ex.Message}");
            }
            #endregion

            #region 2、定时扫描(100毫秒扫描一次)待生产区队列，按优先级排序，在生产区容量未满的情况下将文件名加入到生产区
            _BootUpProducerModelTokenSource1 = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_BootUpProducerModelTokenSource1.IsCancellationRequested) return;
                    try
                    {
                        #region 文件名排序，然后依次加入正在生产的队列中
                        var files = Model.WaitProduceFiles.OrderBy(p => p).ToList();
                        foreach (var f in files)
                        {
                            if (Model.ProducingFiles.Count >= Model.ChannelSize) break;//管道满了就直接跳出
                            if (Model.ProducingFiles.Contains(f)) continue;
                            bool flag = Model.ProducingFiles.TryAdd(f);//将文件名加入正在生产区
                            if (flag) Model?.WaitProduceFiles.Remove(f);//从待生产区移除文件名
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"扫描待生产缓存区文件名异常：{ex.Message}");
                    }
                    Thread.Sleep(1 * 100);
                }
            }, _BootUpProducerModelTokenSource1.Token);
            #endregion

            #region 3、执行生产
            _BootUpProducerModelTokenSource2 = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                foreach (var fname in Model.ProducingFiles.GetConsumingEnumerable())
                {
                    if (_BootUpProducerModelTokenSource2.IsCancellationRequested) return;
                    try
                    {
                        ExecuteProduce(fname);
                        if (Model.IsBackupProduce)
                        {
                            var sfPath1 = fname;//源文件路径
                            var dfPath1 = fname.Replace(Model.ProduceCachePath, Model.ProduceBackupPath);//目标文件路径
                            dfPath1 = dfPath1.Replace($"{Id}_", "");//文件名去除隔离器Id
                            FileUtil.StreamCopy(sfPath1, dfPath1);//将文件复制到生产备份路径
                        }
                        File.Delete(fname);//删除缓存区的文件
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"生产数据发生异常：{ex.Message}");
                    }
                }
            }, _BootUpProducerModelTokenSource2.Token);
            #endregion
        }

        protected CancellationTokenSource _BootUpConsumerModelTokenSource1;
        protected CancellationTokenSource _BootUpConsumerModelTokenSource2;

        /// <summary>
        /// 启动消费者模式
        /// </summary>
        protected void BootUpConsumerModel()
        {
            #region 1、扫描消费缓存区的文件名加入到待消费区,模式启动之后生产的文件名是会自动加入到待消费区的，所以只需要启动的时候执行一次
            try
            {
                //0@1@20240106095407083350@001@BJ_T_YW_CRJRY@M1001_20230620000000_20230620235959_1_1.txt
                var files = FileUtil.FindFiles(new DirectoryInfo(Model.ConsumeCachePath), Id, "*.txt");//M1001就是要找的文件
                files?.ForEach(f =>
                {
                    Model.WaitConsumeFiles.Add(f.FullName);
                });
            }
            catch (Exception ex)
            {
                NLogger.Warn($"扫描消费缓存区文件异常：{ex.Message}");
            }
            #endregion

            #region 2、定时扫描(100毫秒扫描一次)待消费区队列，按优先级排序，在消费区容量未满的情况下将文件名加入到消费区
            _BootUpConsumerModelTokenSource1 = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_BootUpConsumerModelTokenSource1.IsCancellationRequested) return;
                    try
                    {
                        #region 文件名排序，然后依次加入正在消费的队列中
                        var files = Model.WaitConsumeFiles.OrderBy(p => p).ToList();
                        foreach (var f in files)
                        {
                            if (Model.ConsumingFiles.Count >= Model.ChannelSize) break;//管道满了就直接跳出
                            if (Model.ConsumingFiles.Contains(f)) continue;
                            bool flag = Model.ConsumingFiles.TryAdd(f);//将文件名加入正在消费区
                            if (flag) Model?.WaitConsumeFiles.Remove(f);//从待消费区移除文件名
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"扫描待消费缓存区文件名异常：{ex.Message}");
                    }
                    Thread.Sleep(1 * 100);
                }
            }, _BootUpConsumerModelTokenSource1.Token);
            #endregion

            #region 3、执行消费
            _BootUpConsumerModelTokenSource2 = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                foreach (var fname in Model.ConsumingFiles.GetConsumingEnumerable())
                {
                    if (_BootUpConsumerModelTokenSource1.IsCancellationRequested) return;
                    try
                    {
                        string datastr = FileUtil.StreamRead(fname, null);
                        var idata = JsonUtil.StrToObject<IsolatorData>(datastr);
                        if (Model.IsZip) //解压IsolatorData里面的Lst
                        {
                            idata.Lst.FirstOrDefault().TryGetValue("ZipData", out var base64);
                            byte[] zipBytes = Convert.FromBase64String((string)base64);
                            var bytes = ZipUtil.Decompress(zipBytes);//解压
                            var json = Encoding.Default.GetString(bytes);
                            idata.Lst = JsonUtil.StrToObject<List<Dictionary<string, object>>>(json);
                        }
                        MaxIsolator.I.IListener_Lst?.ForEach(listener =>
                        {
                            listener.ConsumeData(idata, (p1, p2) => { });//消费数据
                        });
                        if (Model.IsBackupConsume)
                        {
                            var sfPath1 = fname;//源文件路径
                            var dfPath1 = fname.Replace(Model.ConsumeCachePath, Model.ConsumeBackupPath);//目标文件路径
                            dfPath1 = dfPath1.Replace($"{Id}_", "");//文件名去除隔离器Id
                            FileUtil.StreamCopy(sfPath1, dfPath1);//将文件复制到消费备份路径
                        }
                        if (File.Exists(fname))
                        {
                            File.Delete(fname);//删除缓存区的文件
                        }
                    }
                    catch (Exception ex)
                    {
                        NLogger.Warn($"消费数据发生异常：{ex.Message}");
                    }
                }
            }, _BootUpConsumerModelTokenSource2.Token);
            #endregion
        }

        /// <summary>
        /// 生产隔离器数据
        /// </summary>
        /// <param name="idata"></param>
        protected void ProduceIsolatorData(IsolatorData idata)
        {
            if (idata == null || idata.Lst == null || idata.Lst.Count <= 0) return;
            var tlst = LimitIsolatorData(idata);//分页
            for (int i = 0; i < tlst.Count; i++)
            {
                try
                {
                    var fName = CacheData(tlst[i], i + 1, tlst.Count, Model.ProduceCachePath, true);//缓存文件
                    Model.WaitProduceFiles.Add(fName);//加入待生产队列
                }
                catch (Exception ex)
                {
                    NLogger.Error($"隔离器{Id}生产数据异常：{ex.Message}");
                }
            }
        }

        /// <summary>
        /// 缓存数据
        /// </summary>
        /// <param name="idata">数据</param>
        /// <param name="number">切片序号</param>
        /// <param name="count">切片总数</param>
        /// <param name="cachePath">缓存路径</param>
        /// <param name="isProduce">是否是生产者(生产着做压缩操作，消费者做解压操作)</param>
        /// <returns></returns>
        protected string CacheData(IsolatorData idata, int number, int count, string cachePath, bool isProduce)
        {
            string rfName = string.Empty;
            if (idata == null || idata.Lst == null || idata.Lst.Count <= 0) return rfName;
            try
            {
                idata.CutNum = number;
                idata.CutCount = count;
                var fId = $"{Id}_{idata.Stime}_{idata.Etime}_{idata.CutNum}_{idata.CutCount}_{idata.Lst.Count}";
                rfName = $@"{cachePath}/{idata.Cfbs}@{idata.Level}@{idata.Rtime}@{idata.Cmd}@{idata.Rwmc}@{fId}.txt";
                if (Model.IsZip) //压缩IsolatorData里面的Lst
                {
                    if (isProduce) //生产者--压缩
                    {
                        string json = JsonUtil.ObjectToStr(idata.Lst);
                        byte[] bytes = Encoding.Default.GetBytes(json);
                        var zipBytes = ZipUtil.Compress(bytes);//压缩的字节数组
                        var base64 = Convert.ToBase64String(zipBytes);
                        var dic = new Dictionary<string, object>();
                        dic.Add("ZipData", base64);
                        idata.Lst = new List<Dictionary<string, object>> { dic };
                    }
                    else //消费者--解压
                    {
                        idata.Lst.FirstOrDefault().TryGetValue("ZipData", out var base64);
                        if (string.IsNullOrEmpty((string)base64)) return rfName;
                        var zipBytes = Convert.FromBase64String((string)base64);//压缩的字节数组
                        byte[] bytes = ZipUtil.Decompress(zipBytes);
                        string json = Encoding.Default.GetString(bytes);
                        idata.Lst = JsonUtil.StrToObject<List<Dictionary<string, object>>>(json);
                    }
                }
                using (FileStream fs = new FileStream(rfName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    byte[] datas = Encoding.Default.GetBytes(JsonUtil.ObjectToStr(idata));
                    fs.Write(datas, 0, datas.Length);
                    fs.Close();
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {
                NLogger.Error($"缓存数据到{rfName}异常:{ex.Message}");
            }
            return rfName;
        }

        /// <summary>
        /// 根据条数分页成隔离器数据，确保单个文件不超过N条数据，且单个文件不超过指定大小
        /// ①优先考虑按条数分页(最大N条数据一个文件)，确保单个文件不超过N条数据
        /// ②如果条数据大小超过了指定大小，继续将数据根据大小分页，确保单个文件不超过指定大小
        /// </summary>
        /// <param name="idata"></param>
        /// <returns></returns>
        protected List<IsolatorData> LimitIsolatorData(IsolatorData idata)
        {
            if (!(idata?.Lst?.Count > 0)) return new List<IsolatorData>();
            #region ①按条数分页
            var byCountLst = LimitIsolatorDataByCount(idata, Model.ItemSize);//条数分页
            string idataStr1 = JsonUtil.ObjectToStr(byCountLst);
            byte[] idataBytes1 = Encoding.UTF8.GetBytes(idataStr1);
            var const_size = Model.BulkSize * 1024 * 1024;//最大容量为多少M
            if (idataBytes1.Length <= const_size)
            {
                List<IsolatorData> by_count_lst = new List<IsolatorData>();
                by_count_lst.AddRange(byCountLst);
                return by_count_lst;
            }
            #endregion
            #region ②按大小分页
            List<IsolatorData> by_size_lst = new List<IsolatorData>();
            foreach (var item in byCountLst)
            {
                var bySizeLst = LimitIsolatorDataBySize(item, Model.BulkSize);
                by_size_lst.AddRange(bySizeLst);
            }
            return by_size_lst;
            #endregion
        }

        /// <summary>
        /// 根据条数分页成隔离器数据(最大多少条数据一个文件)
        /// </summary>
        /// <param name="idata">数据</param>
        /// <param name="itemSize">数据最大条数</param>
        /// <returns></returns>
        private List<IsolatorData> LimitIsolatorDataByCount(IsolatorData idata, int itemSize)
        {
            List<IsolatorData> datalst = new List<IsolatorData>();
            if (idata == null || idata.Lst == null || idata.Lst.Count <= 0) return datalst;

            int icount = idata.Lst.Count;
            int num = icount % itemSize == 0 ? icount / itemSize : icount / itemSize + 1;//页数
            for (int i = 0; i < num; i++)   //按最大N条数据来划分文件
            {
                try
                {
                    int tnum = icount - (i + 1) * itemSize >= 0 ? itemSize : icount - i * itemSize;//数据量
                    var tlst = idata.Lst.Skip(i * itemSize).Take(tnum).ToList();
                    IsolatorData tempdata = new IsolatorData()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Cmd = idata.Cmd,
                        Rwmc = idata.Rwmc,
                        Cddm = idata.Cddm,
                        Cfbs = idata.Cfbs,
                        Czlx = idata.Czlx,
                        Stime = idata.Stime,
                        Etime = idata.Etime,
                        Lst = tlst,
                        IsolatorId = idata.IsolatorId,
                        Level = idata.Level,
                        Rtime = idata.Rtime
                    };
                    datalst.Add(tempdata);
                }
                catch (Exception ex)
                {
                    NLogger.Error($"数据根据条数分页成IsolatorData异常:{ex.Message}");
                }
            }
            return datalst;
        }

        /// <summary>
        /// 根据大小分页成隔离器数据(最大多少M一个文件)
        /// </summary>
        /// <param name="idata">数据</param>
        /// <param name="bulkSize">数据最大容量多少M</param>
        /// <returns></returns>
        private List<IsolatorData> LimitIsolatorDataBySize(IsolatorData idata, int bulkSize)
        {
            List<IsolatorData> Resultlist = new List<IsolatorData>();
            if (idata == null || idata.Lst == null || idata.Lst.Count <= 0) return Resultlist;
            OperationChildData(idata, Resultlist, bulkSize);
            return Resultlist;
        }

        /// <summary>
        /// 递归子级数据
        /// </summary>
        /// <param name="idata">源数据</param>
        /// <param name="Resultlist">目标数据列表</param>
        /// <param name="bulkSize">数据最大容量多少M</param>
        private void OperationChildData(IsolatorData idata, List<IsolatorData> Resultlist, int bulkSize)
        {
            if (idata != null && idata.Lst != null && idata.Lst.Count > 0)
            {
                int const_size = bulkSize * 1024 * 1024;//最大容量为多少M
                string idataStr = JsonUtil.ObjectToStr(idata);
                byte[] idataBytes = Encoding.UTF8.GetBytes(idataStr);
                if (idataBytes.Length <= const_size)
                {
                    Resultlist.Add(idata);
                }
                else
                {
                    int idataLength = idataBytes.Length;//数据长度
                    int cs_count = idataLength % const_size == 0 ? idataLength / const_size : idataLength / const_size + 1;//生成次数
                    int ts_count = idata.Lst.Count % cs_count == 0 ? idata.Lst.Count / cs_count : idata.Lst.Count / cs_count + 1;//每次生成的条数

                    for (int i = 0; i < cs_count; i++)//分多次生成
                    {
                        try
                        {
                            var lst = idata.Lst.Skip(i * ts_count).Take(ts_count).ToList();
                            if (lst == null || lst.Count <= 0) continue;
                            IsolatorData tempdata = new IsolatorData()
                            {
                                Id = Guid.NewGuid().ToString("N"),
                                Cmd = idata.Cmd,
                                Rwmc = idata.Rwmc,
                                Cddm = idata.Cddm,
                                Cfbs = idata.Cfbs,
                                Czlx = idata.Czlx,
                                Stime = idata.Stime,
                                Etime = idata.Etime,
                                Lst = lst,
                                IsolatorId = idata.IsolatorId,
                                Level = idata.Level,
                                Rtime = idata.Rtime
                            };
                            OperationChildData(tempdata, Resultlist, bulkSize);
                        }
                        catch (Exception ex)
                        {
                            NLogger.Error($"数据根据大小分页成IsolatorData异常:{ex.Message}");
                        }
                    }
                }
            }
        }
    }
}