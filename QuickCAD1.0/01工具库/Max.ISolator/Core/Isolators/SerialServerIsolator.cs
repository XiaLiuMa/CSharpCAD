﻿using Max.BaseKit;
using Max.BaseKit.Customs;
using Max.BaseKit.Exts;
using Max.BaseKit.Utils;
using Max.ISolator.Core.Mods;
using Max.ISolator.TcpKit;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System;
using System.Text;

namespace Max.ISolator.Core.Isolators
{
    /// <summary>
    /// 串口服务器隔离器
    /// </summary>
    public class SerialServerIsolator : AbsIsolator
    {
        private MaxTcpClient tcpClient;
        public override string Id { get; set; }
        public override IsolatorModel Model { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string ServerIP { get; private set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// 创建串口服务器隔离器
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="serverIP">IP地址</param>
        /// <param name="serverPort">端口号</param>
        /// <param name="isolatorConfig">隔离器通用配置</param>
        public SerialServerIsolator(string id, string serverIP, int serverPort, IsolatorConfig isolatorConfig)
        {
            this.Id = id;
            this.ServerIP = serverIP;
            this.ServerPort = serverPort;
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
                NLogger.Warn("创建串口服务器隔离器失败，请检查配置信息。");
            }
        }

        public override void BootUp()
        {
            try
            {
                tcpClient = tcpClient ?? new MaxTcpClient(ServerIP, ServerPort);
                if (Model.Mode == 0) //双向模式
                {
                    BootUpProducerModel();
                    BootUpConsumerModel();
                    tcpClient.OnReceived += OnDataReceived;
                }
                else if (Model.Mode == 1) //生产者模式
                {
                    BootUpProducerModel();
                }
                else if (Model.Mode == 2) //消费者模式
                {
                    BootUpConsumerModel();
                    tcpClient.OnReceived += OnDataReceived;
                }
                tcpClient.ConnectAsync();
            }
            catch (Exception ex)
            {
                NLogger.Error($"启动串口服务器隔离器异常：{ex.Message}");
            }
        }

        public override void ProduceData(IsolatorData idata) { ProduceIsolatorData(idata); }

        /// <summary>
        /// 执行生产
        /// </summary>
        /// <param name="fname"></param>
        protected override void ExecuteProduce(string fname)
        {
            byte[] dataBytes = FileUtil.StreamRead(fname);
            tcpClient?.SendAsync(dataBytes);
            //NLogger.Debug($"串口发送{idata.Cmd}数据...");
        }

        private void OnDataReceived(byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length <= 0) return;
                string msg = Encoding.UTF8.GetString(bytes);
                IsolatorData idata = JsonConvert.DeserializeObject<IsolatorData>(msg);
                NLogger.Debug($"串口服务器接收到{idata.Cmd}数据...");
                if (idata == null) return;
                var fName = CacheData(idata, idata.CutNum, idata.CutCount, Model.ConsumeCachePath, false);//缓存到文件
                Model.WaitConsumeFiles.Add(fName);//加入待消费队列
            }
            catch (Exception ex)
            {
                NLogger.Error($"串口服务器接收数据异常：{ex.Message}");
            }
        }

        public override void ShutDown()
        {
            if (tcpClient != null) tcpClient.OnReceived -= OnDataReceived;
            _BootUpConsumerModelTokenSource2?.Cancel();
            _BootUpConsumerModelTokenSource1?.Cancel();
            _BootUpProducerModelTokenSource2?.Cancel();
            _BootUpProducerModelTokenSource1?.Cancel();
        }
    }
}