using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 定时任务控制器
    /// </summary>
    [Authorize]
    public class CroJobController : Controller
    {
        private readonly ILogger<CroJobController> _logger;

        public CroJobController(ILogger<CroJobController> logger)
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
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<CronJob> PagingSearch(int page = 1, int limit = 10, string condition = "")
        {
            var rd = new ResultPaging<CronJob>();

            var tlst = DataMocker.Instance.CronJob_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(condition))
            {
                tlst = tlst.FindAll(p => p.Id.Contains(condition) || p.JobDes.Contains(condition) || p.CronDes.Contains(condition)).ToList();
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
        public ActionResult<ResultMsg<string>> Add(CronJob st)
        {
            if (st == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增失败，参数为空" });
            try
            {
                DataMocker.Instance.CronJob_Lst.Add(st);
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
        public ActionResult<ResultMsg<string>> Update(CronJob st)
        {
            if (st == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改失败，参数为空" });
            try
            {
                var obj = DataMocker.Instance.CronJob_Lst.Find(p => p.Id.Equals(st.Id));
                if (obj != null)
                {
                    DataMocker.Instance.CronJob_Lst.Remove(obj);
                    DataMocker.Instance.CronJob_Lst.Add(st);
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
                var obj = DataMocker.Instance.CronJob_Lst.Find(p => i.Equals(p.Id));
                if (obj != null)
                {
                    DataMocker.Instance.CronJob_Lst.Remove(obj);
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
    /// 定时任务
    /// </summary>
    public class CronJob
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 定时任务任务描述
        /// </summary>
        public string JobDes { get; set; }
        /// <summary>
        /// 子任务命令码(多个用,隔开)
        /// </summary>
        public string SubCmds { get; set; }
        /// <summary>
        /// Cron表达式
        /// </summary>
        public string CronExpr { get; set; }
        /// <summary>
        /// 运行计划。主要是显示对Quartz表达式的文字说明
        /// </summary>
        public string CronDes { get; set; }
        /// <summary>
        /// 用于记录运行时间
        /// </summary>
        public string RunTime { get; set; }
    }
}