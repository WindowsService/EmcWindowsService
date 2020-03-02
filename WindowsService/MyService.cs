using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace WindowsService
{
    public partial class MyService : ServiceBase
    {
        //首先实例化Log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MyService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer();
            timer.Interval = int.Parse(ConfigurationManager.AppSettings["IntervalTimer"]); //设置计时器事件间隔执行时间
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                //判断是否在每天5点到6点之间
                TimeSpan nowDt = DateTime.Now.TimeOfDay;
                TimeSpan workStartDT = DateTime.Parse(ConfigurationManager.AppSettings["workStartDT"]).TimeOfDay;
                TimeSpan workEndDT = DateTime.Parse(ConfigurationManager.AppSettings["workEndDT"]).TimeOfDay;
                if (nowDt > workStartDT && nowDt < workEndDT)
                {

                    string filePath = ConfigurationManager.AppSettings["FilePath"];
                    string[] filePaths = filePath.Split(',');
                    foreach (string item in filePaths)
                    {
                        DeleteFile(item, int.Parse(ConfigurationManager.AppSettings["DeleteTimeForDays"]));  //删除该目录下 超过 7天的文件
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        private void DeleteFile(string fileDirect, int saveDay)
        {
            DateTime nowTime = DateTime.Now;

            string[] dires = Directory.GetDirectories(fileDirect);
            string[] files = Directory.GetFiles(fileDirect, ConfigurationManager.AppSettings["GetFiles"], SearchOption.TopDirectoryOnly);  //获取该目录下所有文件
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                TimeSpan t = nowTime - fileInfo.CreationTime;  //当前时间  减去 文件创建时间
                int day = t.Days;
                if (day >= saveDay)   //保存的时间 ；  单位：天
                {
                    File.Delete(file);  //删除超过时间的文件
                    log.Info(string.Format("文件创建时间为{0},删除了文件{1}", fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"), file));
                }
                foreach (var item in dires)
                {
                    if (fileInfo.Name.Equals("Test"))
                        continue;
                    if (day >= saveDay)   //保存的时间 ；  单位：天
                    {
                        Directory.Delete(item, true);  //删除超过时间的文件夹
                        log.Info(string.Format("文件夹创建时间为:{0},删除了文件夹及其文件{1}", fileInfo.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"), item));
                    }
                }
            }
        }
    }
}
