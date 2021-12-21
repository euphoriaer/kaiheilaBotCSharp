using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Log
{
    /// <summary>
    /// 文件名与文件路径
    /// </summary>
    private Dictionary<string, string> LogsDictionary = new Dictionary<string, string>();

    private string LogFolder;
    private bool CanDelete = false;
    private int OverdueDay;
    private string LogSuffix;

    /// <summary>
    /// 当前日期加上 overdueDay 计算出对应文件，删除，每天算一次
    /// </summary>
    /// <param name="logFolder">日志所在文件夹</param>
    /// <param name="overdueDay">过期天数</param>
    /// <param name="logSuffix">日志文件名，日期+后缀，默认纯日期</param>
    public Log(string logFolder, int overdueDay, string logSuffix = "")
    {
        LogSuffix = logSuffix;
        OverdueDay = overdueDay;
        LogFolder = logFolder;
        Directory.CreateDirectory(logFolder);
        Task.Run((() =>
        {
            while (true)
            {
                Thread.Sleep(10000);
                DeleteLogs();
            }
        }));
    }

    private void DeleteLogs()
    {
        if (DateTime.Now.Hour == 1)
        {
            CanDelete = true;
        }

        if (DateTime.Now.Hour == 6 && CanDelete == true)
        {
            //error 减30天
            string overDay = DateTime.Today.AddDays(OverdueDay).ToString().Split(" ")[0].Replace("/", "：") + LogSuffix;

            LogsDictionary.TryGetValue(overDay, out var logPath);
            if (logPath != null)
            {
                File.Delete(logPath);
                LogsDictionary.Remove(overDay);
            }
            CanDelete = false;
        }
    }

    //例2021/12/6 15:39:14"
    public void Record(string content)
    {
        string curDay = DateTime.Now.ToString().Split(" ")[0].Replace("/", "：") + LogSuffix;
        var logPath = Path.Combine(LogFolder, curDay);
        LogsDictionary[curDay] = logPath;
        File.AppendAllText(logPath, "\n\n\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + content);
    }
}