using Max.BaseKit;
using Max.BaseKit.Utils;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Max.BaseKit.Customs;
using Max.ISolator.Core.Mods;
using Max.BaseKit.Exts;
using Newtonsoft.Json;
using System.Text;

namespace Max.ISolator.Core.Isolators
{
    /// <summary>
    /// 网闸隔离器
    /// </summary>
    public class GatekeeperIsolator : AbsIsolator
    {
        public override string Id { get; set; }
        public override IsolatorModel Model { get; set; }
        /// <summary>
        /// 同步路径
        /// </summary>
        public string SyncPath { get; private set; }
        /// <summary>
        /// 扫描路径
        /// </summary>
        public string ScanPath { get; private set; }

        /// <summary>
        /// 创建网闸隔离器
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="syncPath">同步路径</param>
        /// <param name="scanPath">扫描路径</param>
        /// <param name="isolatorConfig">隔离器通用配置</param>
        public GatekeeperIsolator(string id, string syncPath, string scanPath, IsolatorConfig isolatorConfig)
        {
            this.Id = id;
            this.SyncPath = syncPath;
            this.ScanPath = scanPath;
            Model = isolatorConfig?.Convert<IsolatorModel>();
            if (Model != null)
            {
                Model.WaitProduceFiles = new CustomConcurrentList<string>();
                Model.ProducingFiles = new BlockingCollection<string>(Model.ChannelSize);
                Model.WaitConsumeFiles = new CustomConcurrentList<string>();
                Model.ConsumingFiles = new BlockingCollection<string>(Model.ChannelSize);
            }
            else
            {
                NLogger.Warn("创建网闸隔离器失败，请检查配置信息。");
            }
        }

        public override void BootUp()
        {
            if (Model.Mode == 0) //双向模式
            {
                BootUpProducerModel();
                BootUpConsumerModel();
                OnDataReceived();
            }
            else if (Model.Mode == 1) //生产者模式
            {
                BootUpProducerModel();
            }
            else if (Model.Mode == 2) //消费者模式
            {
                BootUpConsumerModel();
                OnDataReceived();
            }
        }

        public override void ProduceData(IsolatorData idata) { ProduceIsolatorData(idata); }

        /// <summary>
        /// 执行生产
        /// </summary>
        /// <param name="fname"></param>
        protected override void ExecuteProduce(string fname)
        {
            var sfPath = fname;//源文件路径
            var dfPath = fname.Replace(Model.ProduceCachePath, SyncPath);//目标文件路径
            dfPath = dfPath.Replace($"{Id}_", "");//文件名去除隔离器Id
            FileUtil.StreamCopy(sfPath, dfPath);//将文件复制到网闸同步路径
        }


        private CancellationTokenSource _OnDataReceivedTokenSource;

        /// <summary>
        /// 接收到网闸数据：在网闸消费者模式中其实就是定时扫描网闸同步的文件
        /// </summary>
        private void OnDataReceived()
        {
            _OnDataReceivedTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (_OnDataReceivedTokenSource.IsCancellationRequested) return;
                    try
                    {
                        var files = FileUtil.FindFiles(new DirectoryInfo(ScanPath), null, "*.txt");
                        files?.ForEach(f =>
                        {
                            var bytes = FileUtil.StreamRead(f.FullName);
                            string datastr = Encoding.UTF8.GetString(bytes);
                            IsolatorData idata = JsonConvert.DeserializeObject<IsolatorData>(datastr);
                            var fName = CacheData(idata, idata.CutNum, idata.CutCount, Model.ConsumeCachePath, false);//缓存到文件
                            Model.WaitConsumeFiles.Add(fName);//加入待消费队列

                            #region 为了匹配通用的缓存方案，废弃该方案
                            //var sfPath = f.FullName;//源文件路径
                            //var dfPath = f.FullName.Replace(ScanPath, Model.ConsumeCachePath);//目标文件路径
                            //FileUtil.StreamMove(sfPath, dfPath);
                            //Model.WaitConsumeFiles.Add(dfPath);//加入待消费队列 
                            #endregion
                        });
                    }
                    catch (Exception ex)
                    {
                        NLogger.Error($"扫描待消费的缓存文件异常：{ex.Message}");
                    }
                    Thread.Sleep(1 * 100);
                }
            }, _OnDataReceivedTokenSource.Token);
        }

        public override void ShutDown()
        {
            _OnDataReceivedTokenSource?.Cancel();
            _BootUpConsumerModelTokenSource2?.Cancel();
            _BootUpConsumerModelTokenSource1?.Cancel();
            _BootUpProducerModelTokenSource2?.Cancel();
            _BootUpProducerModelTokenSource1?.Cancel();
        }
    }
}