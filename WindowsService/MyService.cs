using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using System.IO;

namespace WindowsService
{
    public partial class MyService : ServiceBase
    {
        public MyService()
        {
            InitializeComponent();
        }
        
        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer();
            timer.Interval = int.Parse(ConfigurationManager.AppSettings["IntervalTimer"]); //设置计时器事件间隔执行时间
            //判断是否在每天5点到6点之间
            TimeSpan nowDt = DateTime.Now.TimeOfDay;
            TimeSpan workStartDT = DateTime.Parse(ConfigurationManager.AppSettings["workStartDT"]).TimeOfDay;
            TimeSpan workEndDT = DateTime.Parse(ConfigurationManager.AppSettings["workEndDT"]).TimeOfDay;
            if (nowDt > workStartDT && nowDt < workEndDT)
            {
                timer.Elapsed += Timer_Elapsed;
                timer.Enabled = true;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                DeleteFile(ConfigurationManager.AppSettings["FilePath"], int.Parse(ConfigurationManager.AppSettings["DeleteTimeForDays"]));  //删除该目录下 超过 7天的文件
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DeleteFile(string fileDirect,int saveDay)

        {




            DateTime nowTime = DateTime.Now;

            string[] files = Directory.GetFiles(fileDirect , ConfigurationManager.AppSettings["GetFiles"], SearchOption.AllDirectories);  //获取该目录下所有 .txt文件
            foreach (string file in files)
            { 
                FileInfo fileInfo = new FileInfo(file);
                TimeSpan t = nowTime - fileInfo.CreationTime;  //当前时间  减去 文件创建时间
                int day = t.Days;
                if (day >= saveDay)   //保存的时间 ；  单位：天
                {

                    File.Delete(file);  //删除超过时间的文件
                    log.Info(string.Format("时间{0}删除了文件名{1}",DateTime.Now.ToString("yyyyMMddhhmmss"),fileInfo.Name));
                }
            }
        }
        //首先实例化Log4net
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

     
    }
}
