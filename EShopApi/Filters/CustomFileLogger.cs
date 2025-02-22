using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EShopApi.Filters
{
    public class CustomFileLogger : ILogger
    {
        protected readonly CustomFileLoggerProvider _loggerProvider;
        public CustomFileLogger([NotNull] CustomFileLoggerProvider provider)
        {
            _loggerProvider = provider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        private object lockobject = new object();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception ex, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            IReadOnlyList<KeyValuePair<string, object>> obj = (IReadOnlyList<KeyValuePair<string, object>>)state;
            string fullpath = _loggerProvider._options.FolderPath + "/" + string.Format(_loggerProvider._options.FilePath,(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0')));
            if (logLevel == LogLevel.Error)
            {
                lock (lockobject)
                {
                    StringBuilder sb = new StringBuilder();
                    using FileStream sw = new FileStream(fullpath, FileMode.Append, FileAccess.Write, FileShare.None);
                    sb.AppendLine("");
                    sb.AppendLine("=============================================================Start Exception Details=============================================================");
                    sb.AppendLine("Time : " + DateTime.Now.ToString()); 
                    sb.AppendLine("Name : " + obj.FirstOrDefault(m => m.Key == "Name").Value);
                    sb.AppendLine("USERNAME : " + obj.FirstOrDefault(m => m.Key == "USERNAME").Value);
                    sb.AppendLine("EMAILID : " + obj.FirstOrDefault(m => m.Key == "EMAILID").Value);
                    sb.AppendLine("Exception handled ClassName : " + obj.FirstOrDefault(m => m.Key == "TypeName").Value);
                    sb.AppendLine("");
                    sb.AppendLine((ex != null) ? ex.ToString() : "");
                    sb.AppendLine("");
                    sb.AppendLine("==============================================================End Exeption Details================================================================");
                    sb.AppendLine("");
                    byte[] bytedata = Encoding.Default.GetBytes(sb.ToString());
                    sw.Write(bytedata,0, bytedata.Length);
                    sw.Flush();
                }
            }
            else
            {
                lock (lockobject)
                {
                    StringBuilder sb = new StringBuilder();
                    using FileStream sw = new FileStream(fullpath, FileMode.Append, FileAccess.Write, FileShare.None);

                    sb.AppendLine("");
                    sb.AppendLine("=============================================================Start " + logLevel.ToString() + " Details=============================================================");
                    sb.AppendLine("");
                    sb.AppendLine("Time : " + DateTime.Now.ToString()); 
                    sb.AppendLine("Name : " + obj.FirstOrDefault(m => m.Key == "Name").Value);
                    sb.AppendLine("USERNAME : " + obj.FirstOrDefault(m => m.Key == "USERNAME").Value);
                    sb.AppendLine("EMAILID : " + obj.FirstOrDefault(m => m.Key == "EMAILID").Value);
                    sb.AppendLine(obj.ToString());
                    sb.AppendLine("");
                    sb.AppendLine("==============================================================End " + logLevel.ToString() + " Details================================================================");
                    sb.AppendLine("");
                    byte[] bytedata = Encoding.Default.GetBytes(sb.ToString());
                    sw.Write(bytedata, 0, bytedata.Length);
                    sw.Flush();
                }
            }
        }
    }

    [ProviderAlias("CustomApiLoggerProvider")]
    public class CustomFileLoggerProvider : ILoggerProvider
    {
        public readonly CustomFileLoggerOptions _options;
        public CustomFileLoggerProvider(IOptions<CustomFileLoggerOptions> loggerOptions)
        {
            _options = loggerOptions.Value;
            if (!Directory.Exists(_options.FolderPath))
            {
                Directory.CreateDirectory(_options.FolderPath);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CustomFileLogger(this);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class CustomFileLoggerOptions
    {
        public virtual string FilePath { get; set; }
        public virtual string FolderPath { get; set; }
    }
}
