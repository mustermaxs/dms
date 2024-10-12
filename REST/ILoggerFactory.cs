// using System.Diagnostics;
// using log4net.Core;
// using ILogger = log4net.Core.ILogger;
// using DMS.REST.Logging;

// namespace DMS.REST.Logging
// {
//     /// <summary>
//     /// Wraps the log4j2 logger instances by realizing interface ILoggerWrapper.
//     /// This avoids direct dependencies to log4j2 package.
//     /// </summary>
//     public class Log4NetWrapper : ILoggerWrapper
//     {
//         private readonly log4net.ILog logger;
//         public ILogger Logger { get; }
//         private static Log4NetWrapper? _loggerWrapper = null;

//         public static Log4NetWrapper CreateLogger(string configPath, string caller) // BUG doesn't return loggerWrapper
//         {
//             // if (_loggerWrapper is not null) return _loggerWrapper;

//             if (!File.Exists(configPath))
//             {
//                 throw new ArgumentException("Does not exist.", nameof(configPath));
//             }

//             log4net.Config.XmlConfigurator.Configure(new FileInfo(configPath));
//             var logger =
//                 log4net.LogManager.GetLogger(caller); // System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
//             return new Log4NetWrapper(logger);
//             // return _loggerWrapper;
//         }

//         private Log4NetWrapper(log4net.ILog logger)
//         {
//             this.logger = logger;
//         }

//         public void Debug(string message)
//         {
//             this.logger.Debug(message);
//         }

//         public void Warn(string message)
//         {
//             this.logger.Warn(message);
//         }

//         public void Error(string message)
//         {
//             this.logger.Error(message);
//         }

//         public void Fatal(string message)
//         {
//             this.logger.Fatal(message);
//         }

//         public void Info(string message)
//         {
//             this.logger.Info(message);
//         }
//     }

//     public interface ILoggerWrapper
//     {
//         void Debug(string message);
//         void Error(string message);
//         void Fatal(string message);
//         void Warn(string message);
//         void Info(string message);
//     }


//     public static class LoggerFactory
//     {
//         public static ILoggerWrapper GetLogger()
//         {
//             StackTrace
//                 stackTrace = new(1, false); //Captures 1 frame, false for not collecting information about the file
//             var type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
//             return Log4NetWrapper.CreateLogger("./log4net.config", type?.FullName ?? string.Empty);
//         }
//     }
// }