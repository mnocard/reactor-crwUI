using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System;
using r_crwUI_A.Interfaces;

namespace r_crwUI_A.Services
{
    internal class Logger : ILogger
    {
        private const string ext = ".log";
        private readonly string dateTimeFormat;
        private readonly string logFileName;
        private readonly object lockObject;
        private string logBuffer = string.Empty;
        private int Lines;
        private FileInfo logFile;
        public Logger()
        {
            dateTimeFormat = "dd.MM.yyyy HH:mm:ss.fff";
            logFileName = Assembly.GetExecutingAssembly().GetName().Name + ext;
            lockObject = new object();

            string logHEader = logFileName + " is created.";
            if (!File.Exists(logFileName))
                File.CreateText(logFileName);
            WriteLog("INFO: " + logHEader);
        }

        public void WriteLog(string Message,
            [CallerMemberName] string memberName = null,
            [CallerFilePath] string sourceFilePath = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {

            if (Lines < 20 && !Message.Equals("FATAL") && !Message.Equals("EXIT") && !Message.Equals("DONE"))
            {
                logBuffer += DateTime.Now.ToString(dateTimeFormat) + $" -- {Message}" + "\n" +
                    "Member name: " + memberName + "\n" +
                    "source file path: " + sourceFilePath + "\n" +
                    "source line number: " + sourceLineNumber + "\n";
                Lines++;
            }
            else if (Message.Equals("DONE")) logBuffer += DateTime.Now.ToString(dateTimeFormat) + $" -- {Message}\n\n";
            else
            {
                lock (lockObject)
                {
                    logFile = new FileInfo(logFileName);

                    using (StreamWriter writer = new StreamWriter(logFileName, !(logFile.Length > 500000), System.Text.Encoding.UTF8))
                    {
                        writer.WriteLine(logBuffer);
                        writer.WriteLine(DateTime.Now.ToString(dateTimeFormat) + $" -- {Message}");
                        writer.WriteLine("Member name: " + memberName);
                        writer.WriteLine("source file path: " + sourceFilePath);
                        writer.WriteLine("source line number: " + sourceLineNumber + "\n");
                    }

                    Lines = 0;
                }
            }
        }
    }
}