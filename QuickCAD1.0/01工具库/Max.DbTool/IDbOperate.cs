using System;
using System.Collections.Generic;
using Max.DbTool.Mod;

namespace Max.DbTool
{
    /// <summary>
    /// 数据库操作接口
    /// </summary>
    public interface IDbOperate : IDisposable
    {
        /// <summary>
        /// 数据库配置
        /// </summary>
        DbConfig Config { get; set; }
        /// <summary>
        /// 尝试连接数据库
        /// </summary>
        /// <returns></returns>
        bool TryConnect();
        /// <summary>
        /// 查询所有表名
        /// </summary>
        /// <returns></returns>
        List<string> QueryTableNames();
        /// <summary>
        /// 执行SQL【只适用于支持sql的数据库】
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        bool ExecuteSql(string sqlstr);
        /// <summary>
        /// 查询SQL【只适用于支持sql的数据库】
        /// </summary>
        /// <param name="sqlstr"></param>
        /// <returns></returns>
        List<Dictionary<string, object>> QuerySql(string sqlstr);
        /// <summary>
        /// 插入一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Insert<T>(T model);
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Insert(string tname, IDictionary<string, object> data);
        /// <summary>
        /// 批量插入实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        bool BatchInsert<T>(List<T> models);
        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        bool BatchInsert(string tname, List<IDictionary<string, object>> datas);
        /// <summary>
        /// 删除一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Delete<T>(T model);
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Delete(string tname, List<string> keys, IDictionary<string, object> data);
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        bool BatchDelete<T>(List<T> models);
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        bool BatchDelete(string tname, List<string> keys, List<IDictionary<string, object>> datas);
        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="filter">条件表达式</param>
        /// <returns></returns>
        bool DeleteByCondition<T>(Func<T, bool> filter);
        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="conditions">条件集</param>
        /// <returns></returns>
        bool DeleteByCondition(string table, params ConditionModel[] conditions);
        /// <summary>
        /// 清空指定实体
        /// </summary>
        /// <returns></returns>
        bool Clear<T>();
        /// <summary>
        /// 清空指定表
        /// </summary>
        /// <param name="tname"></param>
        /// <returns></returns>
        bool Clear(string tname);
        /// <summary>
        /// 修改一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Update<T>(T model);
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(string tname, List<string> keys, IDictionary<string, object> data);
        /// <summary>
        /// 批量修改实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        bool BatchUpdate<T>(List<T> models);
        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        bool BatchUpdate(string tname, List<string> keys, List<IDictionary<string, object>> datas);
        /// <summary>
        /// 覆盖一个实体(有就修改，没有就新增)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        bool Cover<T>(T model);
        /// <summary>
        /// 覆盖一条数据(有就修改，没有就新增)
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Cover(string tname, List<string> keys, IDictionary<string, object> data);
        /// <summary>
        /// 批量覆盖实体(有就修改，没有就新增)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        bool BatchCover<T>(List<T> models);
        /// <summary>
        /// 批量覆盖数据(有就修改，没有就新增)
        /// </summary>
        /// <param name="tname"></param>
        /// <param name="keys"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        bool BatchCover(string tname, List<string> keys, List<IDictionary<string, object>> datas);
        /// <summary>
        /// 条件查询第一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">此处的值的格式应与主键个数对应</param>
        /// <returns></returns>
        T QueryFirst<T>(params object[] values);
        /// <summary>
        /// 条件查询实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter">条件表达式</param>
        /// <returns></returns>
        List<T> QueryByCondition<T>(Func<T, bool> filter = null);
        /// <summary>
        /// 条件查询数据
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="conditions">条件集</param>
        /// <returns></returns>
        List<Dictionary<string, object>> QueryByCondition(string table, params ConditionModel[] conditions);
    }
}
