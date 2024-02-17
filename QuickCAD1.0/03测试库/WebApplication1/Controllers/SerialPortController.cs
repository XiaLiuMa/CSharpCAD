using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 串口控制器
    /// </summary>
    [Authorize]
    public class SerialPortController : Controller
    {
        private readonly ILogger<SerialPortController> _logger;
        public SerialPortController(ILogger<SerialPortController> logger)
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
        public ActionResult<ResultMsg<List<SerialPortVmod>>> SearchAll()
        {
            return Ok(new ResultMsg<List<SerialPortVmod>>() { Code = StatusType.Success, Msg = "查询成功", Data = DataMocker.Instance.SerialPortVmod_Lst });
        }

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<SerialPortVmod> PagingSearch(int page = 1, int limit = 10, string condition = "")
        {
            var rd = new ResultPaging<SerialPortVmod>();

            var tlst = DataMocker.Instance.SerialPortVmod_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(condition))
            {
                tlst = tlst.FindAll(p => p.PortName.Contains(condition)).ToList();
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
        public ActionResult<ResultMsg<string>> Add(SerialPortVmod sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增失败，参数为空" });
            try
            {
                sdb.Id = Guid.NewGuid().ToString("N");
                DataMocker.Instance.SerialPortVmod_Lst.Add(sdb);
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
        public ActionResult<ResultMsg<string>> Update(SerialPortVmod sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改失败，参数为空" });
            try
            {
                var obj = DataMocker.Instance.SerialPortVmod_Lst.Find(p => p.Id.Equals(sdb.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SerialPortVmod_Lst.Remove(obj);
                    DataMocker.Instance.SerialPortVmod_Lst.Add(sdb);
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
                var obj = DataMocker.Instance.SerialPortVmod_Lst.Find(p => i.Equals(p.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SerialPortVmod_Lst.Remove(obj);
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
    /// 串口
    /// </summary>
    public class SerialPortVmod
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 串口名称
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBit { get; set; }
        /// <summary>
        /// 停止位
        /// </summary>
        public int StopBit { get; set; }
        /// <summary>
        /// 校验位
        /// </summary>
        public string Parity { get; set; }
    }
}