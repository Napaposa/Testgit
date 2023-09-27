using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ATD_ID4P.Class
{
    internal class LogCls
    {
        private string logFilePath = Application.StartupPath + "\\Log\\HistoryLog.txt";

        public void WriteLog(string logMessage, LogType logType)
        {
            try
            {
                string Method = "";
                string Module = "";
                string LineNo = "0";
                try
                {
                    StackTrace stackTrace = new StackTrace(true);
                    Method = stackTrace.GetFrame(1).GetMethod().Name;
                    Module = stackTrace.GetFrame(1).GetMethod().DeclaringType.FullName;
                    LineNo = stackTrace.GetFrame(1).GetFileLineNumber().ToString();
                    logMessage = logMessage.Replace(Environment.NewLine, "");
                    logMessage = Regex.Replace(logMessage, @"\r\n", "");
                    logMessage = Regex.Replace(logMessage, @"\n", "");
                }
                catch (Exception)
                {


                }

                using (var logFile = File.AppendText(logFilePath))
                {
                    logFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ";" + logType.ToString() + ";" + Module + "." + Method + "[" + LineNo + "];" + logMessage.ToString() + "#");
                    logFile.Flush();
                    logFile.Close();
                }
            }
            catch (Exception)
            {

            }
        }

        public string GetLogFilePath()
        {
            return logFilePath;
        }

        public string GetAllLog()
        {
            string Result = "";
            try
            {
                if (!string.IsNullOrEmpty(logFilePath))
                {
                    if (File.Exists(logFilePath))
                        Result = File.ReadAllText(logFilePath);
                    else
                        Result = "Log file not found.";
                }
            }
            catch (Exception e)
            {
                Result = e.Message;
            }
            return Result;
        }

        internal void WriteLog(string message, object fail)
        {
            throw new NotImplementedException();
        }
    }

    public enum LogType
    {
        Success,
        Fail,
        Warning,
        Notes
    }
}
