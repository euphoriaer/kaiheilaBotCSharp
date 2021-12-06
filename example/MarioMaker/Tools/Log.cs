using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

internal class Log
{
    /// <summary>
    /// 文件名与文件路径
    /// </summary>
    private Dictionary<string, string> LogsDictionary = new Dictionary<string, string>();

    private string LogFolder;
    private static bool CanDelete = false;
    private static int OverdueDay;

    /// <summary>
    /// 当前日期加上 overdueDay 计算出对应文件，删除，每天算一次
    /// </summary>
    /// <param name="logFolder">日志所在文件夹</param>
    /// <param name="overdueDay">过期天数</param>
    public Log(string logFolder, int overdueDay)
    {
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
            string overDay = DateTime.Today.AddDays(OverdueDay).ToString().Split(" ")[0].Replace("/", "：");

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
        string curDay = DateTime.Now.ToString().Split(" ")[0].Replace("/", "：");
        var logPath = Path.Combine(LogFolder, curDay);
        LogsDictionary[curDay] = logPath;
        File.AppendAllTextAsync(logPath, "\n\n\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + content);
    }
}