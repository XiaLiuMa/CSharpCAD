using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 串口服务器控制器
    /// </summary>
    [Authorize]
    public class SerialServerController : Controller
    {
        private readonly ILogger<SerialServerController> _logger;
        public SerialServerController(ILogger<SerialServerController> logger)
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
        public ActionResult<ResultMsg<List<SerialServerVmod>>> SearchAll()
        {
            return Ok(new ResultMsg<List<SerialServerVmod>>() { Code = StatusType.Success, Msg = "查询成功", Data = DataMocker.Instance.SerialServerVmod_Lst });
        }

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<SerialServerVmod> PagingSearch(int page = 1, int limit = 10, string condition = "")
        {
            var rd = new ResultPaging<SerialServerVmod>();

            var tlst = DataMocker.Instance.SerialServerVmod_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(condition))
            {
                tlst = tlst.FindAll(p => p.Id.Contains(condition) || p.ServerPort.ToString().Contains(condition)).ToList();
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
        /// <param name="sdb"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Add(SerialServerVmod sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增失败，参数为空" });
            try
            {
                sdb.Id = Guid.NewGuid().ToString("N");
                DataMocker.Instance.SerialServerVmod_Lst.Add(sdb);
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
        /// <param name="sdb"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<ResultMsg<string>> Update(SerialServerVmod sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改失败，参数为空" });
            try
            {
                var obj = DataMocker.Instance.SerialServerVmod_Lst.Find(p => p.Id.Equals(sdb.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SerialServerVmod_Lst.Remove(obj);
                    DataMocker.Instance.SerialServerVmod_Lst.Add(sdb);
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
                var obj = DataMocker.Instance.SerialServerVmod_Lst.Find(p => i.Equals(p.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SerialServerVmod_Lst.Remove(obj);
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
    /// 串口服务器配置
    /// </summary>
    public class SerialServerVmod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// IP
        /// </summary>
        public string ServerIP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int ServerPort { get; set; }
    }
}