﻿using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 日志管理控制器
    /// </summary>
    [Authorize]
    public class LogMgtController : Controller
    {
        private readonly ILogger<LogMgtController> _logger;

        public LogMgtController(ILogger<LogMgtController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="logType"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<LogVMod> PagingSearch(int page = 1, int limit = 10, string logType = "", string stime = "", string etime = "")
        {
            var rd = new ResultPaging<LogVMod>();

            var tlst = DataMocker.Instance.LogVMod_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(logType))
            {
                if (logType.StartsWith("zyd"))
                {
                    tlst = tlst.FindAll(p => "数据同步资源端".Equals(p.BelongSystem)).ToList();
                    if ("zyd_Info".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "INFO".Equals(p.LogType)).ToList();
                    }
                    else if ("zyd_Waring".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "WARING".Equals(p.LogType)).ToList();
                    }
                    else if ("zyd_Error".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "ERROR".Equals(p.LogType)).ToList();
                    }
                }
                else if (logType.StartsWith("mbd"))
                {
                    tlst = tlst.FindAll(p => "数据同步目标端".Equals(p.BelongSystem)).ToList();
                    if ("mbd_Info".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "INFO".Equals(p.LogType)).ToList();
                    }
                    else if ("mbd_Waring".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "WARING".Equals(p.LogType)).ToList();
                    }
                    else if ("mbd_Error".Equals(logType))
                    {
                        tlst = tlst.FindAll(p => "ERROR".Equals(p.LogType)).ToList();
                    }
                }
            }
            if (!string.IsNullOrEmpty(stime))
            {
                tlst = tlst.FindAll(p => Convert.ToDateTime(p.UpdateTime) >= Convert.ToDateTime(stime)).ToList();
            }
            if (!string.IsNullOrEmpty(etime))
            {
                tlst = tlst.FindAll(p => Convert.ToDateTime(p.UpdateTime) <= Convert.ToDateTime(etime)).ToList();
            }
            #endregion

            tlst = tlst.OrderByDescending(p => p.UpdateTime).ToList();
            var lst = tlst.Skip((page - 1) * limit).Take(limit).ToList();
            rd.Count = tlst.Count;
            rd.Data = lst;
            return rd;
        }

        /// <summary>
        /// 查看日志
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> ViewLog(string fileName = "")
        {
            if (string.IsNullOrEmpty(fileName)) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "参数为空" });
            string str = System.IO.File.ReadAllText(@$"C:\Users\Administrator\Desktop\2023-08-21_info.log");
            return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "启动手动任务成功", Data = str });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Delete(string ids = "")
        {
            if (string.IsNullOrEmpty(ids)) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "参数为空" });
            var idArr = ids.Split(',').ToList();
            int count = 0;
            idArr?.ForEach(i =>
            {
                var obj = DataMocker.Instance.LogVMod_Lst.Find(p => i.Equals(p.Id));
                if (obj != null)
                {
                    DataMocker.Instance.LogVMod_Lst.Remove(obj);
                    count++;
                }
            });
            if (count > 0)
            {
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "删除成功", Data = null });
            }
            else
            {
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "删除失败", Data = null });
            }
        }
    }

    public class LogVMod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 所属系统
        /// </summary>
        public string BelongSystem { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public string LogType { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string LogFileName { get; set; }
        /// <summary>
        /// 日志全文件名称
        /// </summary>
        public string LogFullFileName { get; set; }
    }
}