﻿using Max.BaseKit.Utils;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 手动同步控制器
    /// </summary>
    [Authorize]
    public class ManualSyncController : Controller
    {
        private readonly ILogger<ManualSyncController> _logger;

        public ManualSyncController(ILogger<ManualSyncController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 启动手动任务
        /// </summary>
        /// <param name="mTasks"></param>
        /// <param name="stime"></param>
        /// <param name="etime"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Start(List<ManualTask> mTasks, string stime = "", string etime = "")
        {
            if (mTasks == null || mTasks.Count <= 0) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "参数为空" });
            foreach (ManualTask mTask in mTasks)
            {
                var subTask = DataMocker.Instance.SubTask_Lst.Find(p => mTask.Cmd.Contains(p.Cmd));
                var sDb = DataMocker.Instance.SourceDb_Lst.Find(p => mTask.DbId.Contains(p.Id));
            }
            return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "启动手动任务成功" });
        }

        /// <summary>
        /// 查询正在运行的手动任务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResultMsg<List<RuningManualTask>>> SearchRuning()
        {
            var tlst = RandomUtil.PickAny(DataMocker.Instance.SubTask_Lst, 5);
            var rlst = new List<RuningManualTask>();
            foreach (var t in tlst)
            {
                rlst.Add(new RuningManualTask()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Cmd = t.Cmd,
                    TaskName = t.TaskName,
                    DbDesc = RandomUtil.PickOne(DataMocker.Instance.SourceDb_Lst).DbDescribe,
                    STime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss"),
                    ETime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"),
                    RTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return Ok(new ResultMsg<List<RuningManualTask>>()
            {
                Code = StatusType.Success,
                Msg = "启动手动任务成功",
                Data = rlst
            });
        }

        /// <summary>
        /// 通过命令码查询任务树
        /// </summary>
        /// <param name="cmds"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<ResultMsg<List<ManualTaskTree>>> SearchTaskTree(string cmds = "")
        {
            if (string.IsNullOrEmpty(cmds)) return Ok(new ResultMsg<List<ManualTaskTree>>() { Code = StatusType.Success, Msg = "参数为空" });
            var rlst = new List<ManualTaskTree>();
            var firstNode = new ManualTaskTree() //1级节点
            {
                Id = "1",
                Title = "全部",
                Field = "",
                IsFoot = false,
                Children = new List<ManualTaskTree>()
            };
            var subTasks = DataMocker.Instance.SearchSubTaskByIds(cmds);
            int i = 1;
            int j = 1;
            subTasks?.ForEach(subTask =>
            {
                var twoNode = new ManualTaskTree() //2级节点
                {
                    Id = $"{i + 1}",
                    Title = subTask.TaskName,
                    Field = subTask.Cmd,
                    IsFoot = false,
                    Children = new List<ManualTaskTree>()
                };
                i++;
                j += i;
                var sourceDbs = DataMocker.Instance.SearchSourceDbByIds(subTask.Dbs);
                sourceDbs?.ForEach(sourceDbs =>
                {
                    var threeNode = new ManualTaskTree() //3级节点
                    {
                        Id = $"{j + 1}",
                        Title = sourceDbs.DbDescribe,
                        Field = sourceDbs.Id,
                        IsFoot = true,
                        Children = new List<ManualTaskTree>()
                    };
                    twoNode.Children.Add(threeNode);
                    j++;
                });
                firstNode.Children.Add(twoNode);
            });
            rlst.Add(firstNode);
            return Ok(new ResultMsg<List<ManualTaskTree>>()
            {
                Code = StatusType.Success,
                Msg = "启动手动任务成功",
                Data = rlst
            });
        }
    }

    /// <summary>
    /// 正在运行的手动任务
    /// </summary>
    public class RuningManualTask
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 命令码
        /// </summary>
        public string Cmd { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 数据库描述
        /// </summary>
        public string DbDesc { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string STime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string ETime { get; set; }
        /// <summary>
        /// 运行时间
        /// </summary>
        public string RTime { get; set; }
    }
    /// <summary>
    /// 任务&数据库的关系菜单树
    /// </summary>
    public class ManualTaskTree
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 标题(名称)
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 标题(名称)
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 是否边界节点
        /// </summary>
        public bool IsFoot { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool Spread { get; set; } = true;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked { get; set; } = true;
        /// <summary>
        /// 子节点集
        /// </summary>
        public List<ManualTaskTree> Children { get; set; }
    }
    /// <summary>
    /// 手动任务
    /// </summary>
    public class ManualTask
    {
        /// <summary>
        /// 命令码
        /// </summary>
        public string Cmd { get; set; }
        /// <summary>
        /// 数据库Id
        /// </summary>
        public string DbId { get; set; }
    }
}