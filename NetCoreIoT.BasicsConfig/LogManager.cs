using NetCoreIoT.Model.ConfigData;

namespace NetCoreIoT.BasicsConfig
{
    public class LogManager
    {
        private string logRootDirectory;
        LogShow logShow = new LogShow();
        ConfigurationManager configuration = new ConfigurationManager();

        public LogManager()
        {
            string rootDirectory = AppDomain.CurrentDomain.BaseDirectory;
            logRootDirectory = Path.Combine(rootDirectory, "logs");
            if (!Directory.Exists(logRootDirectory))
            {
                Directory.CreateDirectory(logRootDirectory);
            }
            var config = configuration.GetConfigValue();
            logShow = config.Logs_Setting;
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteDebugLog(string message)
        {
            if (logShow.Debugger)
            {
                WriteLog("debug", message);
            }
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteErrorLog(string message)
        {
            if (logShow.Error)
            {
                WriteLog("error", message);
            }
        }
        /// <summary>
        /// Info 日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteInfoLog(string message)
        {
            if (logShow.Info)
            {
                WriteLog("info", message);
            }
        }
        /// <summary>
        /// 写入Log
        /// </summary>
        /// <param name="logLevel">log 等级</param>
        /// <param name="message"></param>
        private void WriteLog(string logLevel, string message)
        {
            try
            {
                string logDirectory = Path.Combine(logRootDirectory, logLevel);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFilePath = GetLogFilePath(logDirectory, logLevel);
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine($"{DateTime.Now} - {logLevel.ToUpper()} - {message}");
                }
                CheckAndDeleteOldLogs(logDirectory, logLevel);
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
        /// <summary>
        /// 传教log 日式文件路径
        /// </summary>
        /// <param name="logDirectory"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private string GetLogFilePath(string logDirectory, string logLevel)
        {
            string logFileName = $"{DateTime.Now:yyyyMMdd}.log";
            return Path.Combine(logDirectory, logFileName);
        }
        /// <summary>
        /// 检查删除旧的Logs
        /// </summary>
        /// <param name="logDirectory"></param>
        /// <param name="logLevel"></param>
        private void CheckAndDeleteOldLogs(string logDirectory, string logLevel)
        {
            string[] logFiles = Directory.GetFiles(logDirectory, "*.log");
            foreach (string file in logFiles)
            {
                FileInfo fileInfo = new FileInfo(file);
                if (DateTime.Now - fileInfo.LastWriteTime > TimeSpan.FromDays(30))
                {
                    File.Delete(file);
                }
            }
        }
    }
}