using Max.ISolator.Core.Mods;

namespace Max.ISolator.Core
{
    /// <summary>
    /// 隔离器接口
    /// </summary>
    public interface IIsolator
    {
        /// <summary>
        /// 隔离器编号
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 隔离器公用配置模型
        /// </summary>
        IsolatorModel Model { get; set; }
        /// <summary>
        /// 启动隔离器
        /// </summary>
        void BootUp();
        /// <summary>
        /// 生产数据
        /// </summary>
        /// <param name="data"></param>
        void ProduceData(IsolatorData data);
        /// <summary>
        /// 停止隔离器
        /// </summary>
        void ShutDown();
    }
}
