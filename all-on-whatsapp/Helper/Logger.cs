using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//日志记录专用类

namespace all_on_whatsapp
{
    public static class Logger
    {
        private static readonly object lockObj = new object();
        private static string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");

        static Logger()
        {
            // 确保日志目录存在
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public static void Warning(string message)
        {
            WriteLog("WARNING", message);
        }

        public static void Error(string message)
        {
            WriteLog("ERROR", message);
        }

        private static void WriteLog(string logLevel, string message)
        {
            string fileName = Path.Combine(LogDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {message}";

            lock (lockObj)
            {
                File.AppendAllText(fileName, logEntry + Environment.NewLine, Encoding.UTF8);
            }
        }
    }
}
