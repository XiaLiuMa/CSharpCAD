﻿using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 资源数据库控制器
    /// </summary>
    [Authorize]
    public class SourceDbController : Controller
    {
        private readonly ILogger<SourceDbController> _logger;
        public SourceDbController(ILogger<SourceDbController> logger)
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
        public ActionResult<ResultMsg<List<SourceDb>>> SearchAll()
        {
            return Ok(new ResultMsg<List<SourceDb>>() { Code = StatusType.Success, Msg = "查询成功", Data = DataMocker.Instance.SourceDb_Lst });
        }

        /// <summary>
        /// 分页搜索
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="condition"></param>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultPaging<SourceDb> PagingSearch(int page = 1, int limit = 10, string condition = "", string dbtype = "")
        {
            var rd = new ResultPaging<SourceDb>();

            var tlst = DataMocker.Instance.SourceDb_Lst;
            #region 搜索
            if (!string.IsNullOrEmpty(condition))
            {
                tlst = tlst.FindAll(p => p.ConnStr.Contains(condition) || p.DbDescribe.Contains(condition)).ToList();
            }
            if (!string.IsNullOrEmpty(dbtype))
            {
                tlst = tlst.FindAll(p => p.DbType.Equals(dbtype)).ToList();
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
        public ActionResult<ResultMsg<string>> Add(SourceDb sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "新增失败，参数为空" });
            try
            {
                sdb.Id = Guid.NewGuid().ToString("N");
                DataMocker.Instance.SourceDb_Lst.Add(sdb);
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
        public ActionResult<ResultMsg<string>> Update(SourceDb sdb)
        {
            if (sdb == null) return Ok(new ResultMsg<string>() { Code = StatusType.Success, Msg = "修改失败，参数为空" });
            try
            {
                var obj = DataMocker.Instance.SourceDb_Lst.Find(p => p.Id.Equals(sdb.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SourceDb_Lst.Remove(obj);
                    DataMocker.Instance.SourceDb_Lst.Add(sdb);
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
                var obj = DataMocker.Instance.SourceDb_Lst.Find(p => i.Equals(p.Id));
                if (obj != null)
                {
                    DataMocker.Instance.SourceDb_Lst.Remove(obj);
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
    /// 源头数据库
    /// </summary>
    public class SourceDb
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 场地代码(用于区分数据来源)
        /// </summary>
        public string Cddm { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnStr { get; set; }
        /// <summary>
        /// 最大并行连接数
        /// </summary>
        public int Concurrency { get; set; } = 1;
        /// <summary>
        /// 驱动扩展配置
        /// </summary>
        public string DriverEx { get; set; }
        /// <summary>
        /// 数据库描述
        /// </summary>
        public string DbDescribe { get; set; }
    }
}