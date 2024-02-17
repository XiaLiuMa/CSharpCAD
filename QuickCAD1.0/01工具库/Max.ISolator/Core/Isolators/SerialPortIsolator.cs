﻿using Max.BaseKit;
using Max.BaseKit.Customs;
using Max.BaseKit.Exts;
using Max.BaseKit.Utils;
using Max.ISolator.Core.Mods;
using Max.ISolator.SeriaPortKit;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Max.ISolator.Core.Isolators
{
    /// <summary>
    /// 串口隔离器
    /// </summary>
    public class SerialPortIsolator : AbsIsolator
    {
        /// <summary>
        /// 串口客户端
        /// </summary>
        private SerialPortCilent spClient;
        public override string Id { get; set; }
        public override IsolatorModel Model { get; set; }
        /// <summary>
        /// 串口名称
        /// </summary>
        public string PortName { get; private set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; private set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBit { get; private set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public int StopBit { get; private set; }
        /// <summary>
        /// 校验位
        /// </summary>
        public int Parity { get; private set; }

        /// <summary>
        /// 创建串口隔离器
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="dataBit">数据位</param>
        /// <param name="stopBit">停止位</param>
        /// <param name="parity">校验位</param>
        /// <param name="isolatorConfig">隔离器通用配置</param>
        public SerialPortIsolator(string id, string portName, int baudRate, int dataBit, int stopBit, int parity, IsolatorConfig isolatorConfig)
        {
            this.Id = id;
            this.PortName = portName;
            this.BaudRate = baudRate;
            this.DataBit = dataBit;
            this.StopBit = stopBit;
            this.Parity = parity;
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
                NLogger.Warn("创建串口隔离器失败，请检查配置信息。");
            }
        }

        public override void BootUp()
        {
            try
            {
                spClient = spClient ?? new SerialPortCilent(PortName, BaudRate, DataBit, StopBit, Parity);
                if (Model.Mode == 0) //双向模式
                {
                    BootUpProducerModel();
                    BootUpConsumerModel();
                    spClient.OnReceived += OnDataReceived;
                }
                else if (Model.Mode == 1) //生产者模式
                {
                    BootUpProducerModel();
                }
                else if (Model.Mode == 2) //消费者模式
                {
                    BootUpConsumerModel();
                    spClient.OnReceived += OnDataReceived;
                }
            }
            catch (Exception ex)
            {
                NLogger.Error($"启动串口隔离器异常：{ex.Message}");
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
            spClient?.SendBytes(dataBytes);
            //NLogger.Debug($"串口发送{idata.Cmd}数据...");
        }

        /// <summary>
        /// 接收到串口数据
        /// </summary>
        /// <param name="bytes"></param>
        private void OnDataReceived(byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length <= 0) return;
                string datastr = Encoding.UTF8.GetString(bytes);
                IsolatorData idata = JsonConvert.DeserializeObject<IsolatorData>(datastr);
                NLogger.Debug($"串口接收到{idata.Cmd}数据...");
                if (idata == null) return;
                var fName = CacheData(idata, idata.CutNum, idata.CutCount, Model.ConsumeCachePath, false);//缓存到文件
                Model.WaitConsumeFiles.Add(fName);//加入待消费队列
            }
            catch (Exception ex)
            {
                NLogger.Error($"接收串口数据解析异常>>{ex.Message}");
            }
        }

        public override void ShutDown()
        {
            if (spClient != null) spClient.OnReceived -= OnDataReceived;
            _BootUpConsumerModelTokenSource2?.Cancel();
            _BootUpConsumerModelTokenSource1?.Cancel();
            _BootUpProducerModelTokenSource2?.Cancel();
            _BootUpProducerModelTokenSource1?.Cancel();
        }
    }
}