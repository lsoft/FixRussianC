using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace VsixNamespace
{
    /// <summary>
    /// Logging helpers.
    /// Taken from  https://github.com/bert2/microscope completely.
    /// Take a look to that repo, it's amazing!
    /// </summary>
    public static class Logging
    {
        private static readonly object _locker = new object();
        private static long _messageCount = 0L;

        // Logs go to: C:\Users\<user>\AppData\Local\Temp\fixs.vsix.log
        // We're using one log file for each process to prevent concurrent file access.
        private static readonly string vsLogFile = $"{Path.GetTempPath()}/fixc.vsix.log";

        static Logging()
        {
            if (File.Exists(vsLogFile))
            {
                try
                {
                    File.Delete(vsLogFile);
                }
                catch
                {
                    //we do nothing here
                }
            }
        }

        [Conditional("DEBUG")]
        public static void LogVS(
            object? data = null,
            [CallerFilePath] string? file = null,
            [CallerMemberName] string? method = null)
            => Log(vsLogFile, file!, method!, data);
        

        public static void Log(
            string logFile,
            string callingFile,
            string callingMethod,
            object? data = null)
        {
            lock (_locker)
            {
                _messageCount++;
                if ((_messageCount % 100) == 0)
                {
                    if (new FileInfo(logFile).Length > 100 * 1024 * 1024)
                    {
                        //файл лога слишком большой
                        try
                        {
                            File.Delete(logFile);
                        }
                        catch
                        {
                            //nothing to do here
                        }
                    }
                }

                File.AppendAllText(
                    logFile,
                    $"{DateTime.Now:HH:mm:ss.fff} "
                    + $"{Process.GetCurrentProcess().Id,5} "
                    + $"{Thread.CurrentThread.ManagedThreadId,3} "
                    + $"{Path.GetFileNameWithoutExtension(callingFile)}.{callingMethod}()"
                    + $"{(data == null ? "" : $": {data}")}"
                    + Environment.NewLine
                    + Environment.NewLine
                    );
            }
        }
    }
}
