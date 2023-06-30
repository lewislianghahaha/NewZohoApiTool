using System;
using System.IO;

namespace ZohoApiTool
{
    public class LogHelper
    {
        private LogHelper()
        {

        }
        public static readonly log4net.ILog Loginfo = log4net.LogManager.GetLogger("loginfo");
        public static readonly log4net.ILog Logerror = log4net.LogManager.GetLogger("logerror");
        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public static void SetConfig(FileInfo configFile)
        {
            log4net.Config.XmlConfigurator.Configure(configFile);
        }
        public static void WriteLog(string info)
        {
            if (Loginfo.IsInfoEnabled)
            {
                Loginfo.Info(info);
            }
        }
        public static void WriteErrorLog(string info, Exception se)
        {
            if (Logerror.IsErrorEnabled)
            {
                Logerror.Error(info, se);
            }
        }
    }
}
