﻿using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 子任务控制器
    /// </summary>
    [Authorize]
    public class SubTaskController : Controller
    {
        private readonly ILogger<SubTaskController> _logger;

        public SubTaskController(ILogger<SubTaskController> logger)
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
        /// 搜索所有
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResultMsg<List<SubTask>>> SearchAll()
        {
            return Ok(new ResultMsg<List<SubTask>>() { Code = StatusType.Success, Msg = "查询成功", Data = DataMocker.Instance.SubTask_Lst });
        }

        /// <summary>
        /// 根据类型搜索子任务
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResultMsg<List<SubTask>>> SearchByType(string taskType = "")
        {
            var tlst = DataMocker.Instance.SubTask_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(taskType))
            {
                tlst = tlst.FindAll(p => taskType.Equals(p.TaskType)).ToList();
            }
            #endregion
            return Ok(new ResultMsg<List<SubTask>>() { Code = StatusType.Success, Msg = "查询成功", Data = tlst });
        }

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <param name="rwtype"></param>
        /// <param name="rwstate"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<SubTask> PagingSearch(int page = 1, int limit = 10, string condition = "", string rwtype = "", string rwstate = "")
        {
            var rd = new ResultPaging<SubTask>();

            var tlst = DataMocker.Instance.SubTask_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(condition))
            {
                tlst = tlst.FindAll(p => p.Cmd.Contains(condition) || p.TaskName.Contains(condition) || p.TaskDes.Contains(condition) || p.SqlStr.Contains(condition)).ToList();
            }
            if (!string.IsNullOrEmpty(rwtype))
            {
                tlst = tlst.FindAll(p => p.TaskType.Equals(rwtype)).ToList();
            }
            if (!string.IsNullOrEmpty(rwstate))
            {
                tlst = tlst.FindAll(p => p.TaskState.Equals(rwstate)).ToList();
            }
            #endregion

            var lst = tlst.Skip((page - 1) * limit).Take(limit).ToList();
            rd.Count = tlst.Count;
            rd.Data = lst;
            return rd;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Add(SubTask st)
        {
            if (st == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增失败，参数为空" });
            try
            {
                DataMocker.Instance.SubTask_Lst.Add(st);
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增成功" });
            }
            catch (Exception ex)
            {
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = $"新增失败，{ex.Message}" });
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Update(SubTask st)
        {
            if (st == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改失败，参数为空" });
            try
            {
                var obj = DataMocker.Instance.SubTask_Lst.Find(p => p.Cmd.Equals(st.Cmd));
                if (obj != null)
                {
                    DataMocker.Instance.SubTask_Lst.Remove(obj);
                    DataMocker.Instance.SubTask_Lst.Add(st);
                }
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改成功" });
            }
            catch (Exception ex)
            {
                return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = $"修改失败，{ex.Message}" });
            }
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
                var obj = DataMocker.Instance.SubTask_Lst.Find(p => i.Equals(p.Cmd));
                if (obj != null)
                {
                    DataMocker.Instance.SubTask_Lst.Remove(obj);
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
    /// <summary>
    /// 子任务
    /// </summary>
    public class SubTask
    {
        /// <summary>
        /// 主键命令码
        /// </summary>
        public string Cmd { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDes { get; set; }
        /// <summary>
        /// 任务类型(AJCJob/CCGJob/CDKJob/TCGJob/TDKJob等)
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 任务优先级
        /// </summary>
        public int PriorityLevel { get; set; }
        /// <summary>
        /// 切片类型,只用C类任务和T类任务，A类任务配置无效
        /// 用于C类任务时：用于减少数据压力，单个配置(Y/M/D/H)用于拆分查询时间
        /// 用于T类任务时：指定统计维度(多个用,隔开；如："Y,M,D,H")
        /// </summary>
        public string CutType { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string CzType { get; set; } = "R";
        /// <summary>
        /// 任务状态(是否启用)
        /// </summary>
        public string TaskState { get; set; }
        /// <summary>
        /// 数据延迟时长(分钟)
        /// </summary>
        public double DelayTime { get; set; }
        /// <summary>
        /// 隔离器(网闸、串口、串口服务器；多个用,隔开)
        /// </summary>
        public string Isolators { get; set; }
        /// <summary>
        /// 关联的数据库(多个用,隔开)
        /// </summary>
        public string Dbs { get; set; }
        /// <summary>
        /// Sql语句
        /// </summary>
        public string SqlStr { get; set; }
    }
}