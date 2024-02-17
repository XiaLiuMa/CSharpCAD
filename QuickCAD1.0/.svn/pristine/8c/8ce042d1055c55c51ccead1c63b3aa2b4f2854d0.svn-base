using Max.BaseKit.Utils;
using WebApplication1.Controllers;

namespace WebApplication1
{
    public class DataMocker
    {
        #region 单例
        private static DataMocker? instance;
        private readonly static object objLock = new object();
        public static DataMocker Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new DataMocker();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        public List<GatekeeperVmod> GatekeeperVmod_Lst { get; set; }
        public List<SerialPortVmod> SerialPortVmod_Lst { get; set; }
        public List<SerialServerVmod> SerialServerVmod_Lst { get; set; }
        public List<SourceDb> SourceDb_Lst { get; set; }
        public List<SubTask> SubTask_Lst { get; set; }
        public List<CronJob> CronJob_Lst { get; set; }
        public List<LogVMod> LogVMod_Lst { get; set; }


        public DataMocker()
        {
            GatekeeperVmod_Lst = new List<GatekeeperVmod>();
            MockerGatekeeperVmod();
            SerialPortVmod_Lst = new List<SerialPortVmod>();
            MockerSerialPortVmod();
            SerialServerVmod_Lst = new List<SerialServerVmod>();
            MockerSerialServerVmod();
            SourceDb_Lst = new List<SourceDb>();
            MockerSurceDb();
            SubTask_Lst = new List<SubTask>();
            MockerSubTask();
            CronJob_Lst = new List<CronJob>();
            MockerCronJob();
            LogVMod_Lst = new List<LogVMod>();
            MockerLogVMod();
        }

        public void MockerSurceDb()
        {
            var dbtypes = new List<string>() { "MYSQL", "ORACLE", "MONGO", "DAMENG", "GBASE" };
            for (int i = 0; i < 36; i++)
            {
                string dbtype = RandomUtil.PickOne(dbtypes);
                string connstr = dbtype.Equals("ORACLE") ? "DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={db.DbIp})(PORT={db.DbPort})))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={db.DbName})));PASSWORD={db.Pwd};PERSIST SECURITY INFO=True;USER ID={db.Uname};enlist=dynamic;Pooling=false;Connection Timeout=300;" : dbtype.Equals("MONGO") ? "mongodb://fred:foobar@localhost/baz" : dbtype.Equals("DAMENG") ? "Server={db.DbIp};Port={db.DbPort};User ID={db.Uname};Password={db.Pwd};Database={db.DbName}" : dbtype.Equals("GBASE") ? "Protocol=onsoctcp;Db_locale=zh_CN.utf8;Client_locale=zh_CN.utf8;Host={db.DbIp};Service={db.DbPort};Database={db.DbName};Uid={db.Uname};Pwd={db.Pwd};{db.Ext}" : "Server={db.DbIp};Port={db.DbPort};Database={db.DbName};Charset=utf8;Uid={db.Uname};Pwd={db.Pwd};SSL Mode=None";
                SourceDb_Lst.Add(new SourceDb()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Cddm = $"01{i}",
                    ConnStr = connstr,
                    Concurrency = RandomUtil.Next(1, 50),
                    DbType = dbtype,
                    DbDescribe = $"测试用{dbtype}数据库01{i}"
                });
                SourceDb_Lst = SourceDb_Lst.OrderBy(p => p.Id).ToList();
            }
        }

        /// <summary>
        /// 根据主键Id搜索SourceDb
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SourceDb>? SearchSourceDbByIds(string ids = "")
        {
            if (string.IsNullOrEmpty(ids)) return default;
            var idArr = ids.Split(',').ToList();
            return SourceDb_Lst.FindAll(p => idArr.Contains(p.Id)).ToList();
        }

        public void MockerSubTask()
        {
            var rwtypes = new List<string>() { "AJCJob", "CCGJob", "CDKJob", "TCGJob", "TDKJob" };//任务类型
            var qptypes = new List<string>() { "Y", "M", "D", "H" };//切片类型
            for (int i = 0; i < 36; i++)
            {
                string rwtype = RandomUtil.PickOne(rwtypes);
                string sqlstr = rwtype.Equals("CCGJob") ? "SELECT * FROM TAB01 WHERE CRRQSJ >= '{0}' AND CRRQSJ <= '{1}'" : rwtype.Equals("CDKJob") ? "SELECT * FROM TAB01{0} WHERE CRRQSJ >= '{1}' AND CRRQSJ <= '{2}'" : rwtype.Equals("TCGJob") ? "SELECT SUBSTRING(CRRQSJ,0,{0}) AS CRRQ,SUM(COL1) AS NUM FROM TAB01 WHERE CRRQSJ >= '{1}' AND CRRQSJ <= '{2}' GROUP BY SUBSTRING(CRRQSJ,0,{0})" : rwtype.Equals("TDKJob") ? "SELECT SUBSTRING(CRRQSJ,0,{0}) AS CRRQ,SUM(COL1) AS NUM FROM TAB01{1} WHERE CRRQSJ >= '{2}' AND CRRQSJ <= '{3}' GROUP BY SUBSTRING(CRRQSJ,0,{0})" : "SELECT * FROM TAB01";
                string qpstr = rwtype.Equals("AJCJob") ? "" : RandomUtil.PickOne(qptypes);
                var dbs = RandomUtil.PickAny(SourceDb_Lst, RandomUtil.Next(3, 10));

                #region 模拟网闸类型
                var isolatorIds = new List<string>();
                var isolators1 = RandomUtil.PickAny(GatekeeperVmod_Lst, RandomUtil.Next(1, 3));
                isolators1?.ForEach(p => isolatorIds.Add(p.Id));
                var isolators2 = RandomUtil.PickAny(SerialPortVmod_Lst, RandomUtil.Next(1, 3));
                isolators2?.ForEach(p => isolatorIds.Add(p.Id));
                var isolators3 = RandomUtil.PickAny(SerialServerVmod_Lst, RandomUtil.Next(1, 3));
                isolators3?.ForEach(p => isolatorIds.Add(p.Id));
                isolatorIds = RandomUtil.PickAny(isolatorIds, RandomUtil.Next(1, 3));
                string isolatorssstr = string.Empty;
                isolatorIds?.ForEach(p => { isolatorssstr += $"{p},"; });
                #endregion

                string dbsstr = string.Empty;
                dbs?.ForEach(p => { dbsstr += $"{p.Id},"; });
                SubTask_Lst.Add(new SubTask()
                {
                    Cmd = $"D&01{i}",
                    TaskName = $"TEST01{i}",
                    TaskDes = $"测试表TEST01{i}",
                    TaskType = rwtype,
                    PriorityLevel = RandomUtil.Next(1, 5),
                    CutType = qpstr,
                    CzType = "R",
                    TaskState = RandomUtil.PickOne(new List<string>() { "true", "false" }),
                    DelayTime = RandomUtil.Next(60, 180),
                    Isolators = isolatorssstr.TrimEnd(','),
                    Dbs = dbsstr.TrimEnd(','),
                    SqlStr = sqlstr
                });
                SubTask_Lst = SubTask_Lst.OrderBy(p => p.Cmd).ToList();
            }
        }

        /// <summary>
        /// 根据主键Id搜索SubTask
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SubTask>? SearchSubTaskByIds(string ids = "")
        {
            if (string.IsNullOrEmpty(ids)) return default;
            var idArr = ids.Split(',').ToList();
            return SubTask_Lst.FindAll(p => idArr.Contains(p.Cmd)).ToList();
        }

        public void MockerCronJob()
        {
            for (int i = 0; i < 36; i++)
            {
                var subTasks = RandomUtil.PickAny(SubTask_Lst, RandomUtil.Next(3, 10));
                string stbTasksstr = string.Empty;
                subTasks.ForEach(p => { stbTasksstr += $"{p.Cmd},"; });
                int ttime = RandomUtil.Next(1, 59);
                CronJob_Lst.Add(new CronJob()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    JobDes = $"定时任务TEST01{i}",
                    SubCmds = stbTasksstr.TrimEnd(','),
                    CronExpr = $"0 1/{ttime} * * * ? ",
                    CronDes = $"每{ttime}分钟执行一次",
                    RunTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
                CronJob_Lst = CronJob_Lst.OrderBy(p => p.Id).ToList();
            }
        }

        public void MockerGatekeeperVmod()
        {
            for (int i = 0; i < 3; i++)
            {
                GatekeeperVmod_Lst.Add(new GatekeeperVmod()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    BuildPath = @$"D:\SjtbTemp\BuildPath{i}",
                    SyncPath = @$"D:\SjtbTemp\SyncPath{i}",
                    ScanPath = @$"D:\SjtbTemp\ScanPath{i}"
                });
                GatekeeperVmod_Lst = GatekeeperVmod_Lst.OrderBy(p => p.Id).ToList();
            }
        }

        public void MockerSerialPortVmod()
        {
            for (int i = 0; i < 3; i++)
            {
                SerialPortVmod_Lst.Add(new SerialPortVmod()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    PortName = $"COM{2 + i}",
                    BaudRate = 115200,
                    DataBit = 8,
                    StopBit = 1,
                    Parity = "None"
                });
                SerialPortVmod_Lst = SerialPortVmod_Lst.OrderBy(p => p.Id).ToList();
            }
        }

        public void MockerSerialServerVmod()
        {
            for (int i = 0; i < 3; i++)
            {
                SerialServerVmod_Lst.Add(new SerialServerVmod()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ServerIP = "127.0.0.1",
                    ServerPort = RandomUtil.Next(8000, 15000)
                });
                SerialServerVmod_Lst = SerialServerVmod_Lst.OrderBy(p => p.Id).ToList();
            }
        }

        public void MockerLogVMod()
        {
            for (int i = 0; i < 87; i++)
            {
                string belongSystem = RandomUtil.PickOne(new List<string> { "数据同步资源端", "数据同步目标端" });
                string logType = RandomUtil.PickOne(new List<string> { "INFO", "WARING", "ERROR" });
                DateTime randomTime = RandomUtil.RandomTime(new DateTime(2023, 1, 1, 0, 0, 0), new DateTime(2023, 12, 31, 23, 59, 59));
                LogVMod_Lst.Add(new LogVMod()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    BelongSystem = belongSystem,
                    LogType = logType,
                    UpdateTime = randomTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    LogFileName = $"{logType}_{randomTime.ToString("yyyy-MM-dd")}_{i}.log",
                    LogFullFileName = @$"C:/{belongSystem}/{logType}_{randomTime.ToString("yyyy-MM-dd")}_{i}.log"
                });
                CronJob_Lst = CronJob_Lst.OrderBy(p => p.Id).ToList();
            }
        }
    }
}
