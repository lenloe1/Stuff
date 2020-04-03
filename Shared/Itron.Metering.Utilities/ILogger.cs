using System;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// The type of event to log.
    /// </summary>
    public enum LoggingEventType
    {
        /// <summary>Debug</summary>
        Debug,
        /// <summary>Information</summary>
        Information,
        /// <summary>Warning</summary>
        Warning,
        /// <summary>Error</summary>
        Error,
        /// <summary>Fatal</summary>
        Fatal
    };

    /// <summary>
    /// Logger interface. Use to implement a logger that can be injected into any
    /// application as an ILogger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Adds an entry to the log.
        /// </summary>
        /// <param name="entry"></param>
        void Log(LogEntry entry);
    }

    /// <summary>
    /// A log entry.
    /// </summary>
    public class LogEntry
    {
        #region Methods

        /// <summary>
        /// Adds an entry to the log.
        /// </summary>
        /// <param name="severity">The type of event to log.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">The exception (optional).</param>
        public LogEntry(LoggingEventType severity, string message, Exception exception = null)
        {
            if (message == null) throw new ArgumentNullException("message");

            Severity = severity;
            Message = message;
            Exception = exception;
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// The type of event to log.
        /// </summary>
        public LoggingEventType Severity { get; private set; }

        /// <summary>
        /// The message to log.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The exception to log.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion Properties
    }

    /// <summary>
    /// Extensions for ILogger.
    /// </summary>
    public static class LoggerExtensions
    {
        #region Methods

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="type">The type of message to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Log(this ILogger logger, LoggingEventType type, string message)
        {
            logger.Log(new LogEntry(type, message));
        }

        /// <summary>
        /// Logs a message of the information event type.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">The message to log.</param>
        public static void Log(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message));
        }

        /// <summary>
        /// Logs an exception.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exception">The exception to log.</param>
        public static void Log(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, exception));
        }

        #endregion Methods
    }
}
