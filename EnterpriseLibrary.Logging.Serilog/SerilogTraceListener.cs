using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;

namespace EnterpriseLibrary.Logging.Serilog
{
    [ConfigurationElementType(typeof(CustomTraceListenerData))]
    public class SerilogTraceListener: CustomTraceListener
    {
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (Filter != null &&
                !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }

            var level = MapTraceEventTypeToLogEventLevel(eventType);
            if (data is LogEntry logEntry)
            {
                if (Formatter != null)
                {
                    var formattedMessage = Formatter.Format(logEntry);
                    Log.Write(level, "{Source}: {Id}: {FormattedMessage}", source, id, formattedMessage);
                }
                else
                {
                    var logger = Log.ForContext(new ILogEventEnricher[] {
                        new PropertyEnricher("ActivityId", logEntry.ActivityId),
                        new PropertyEnricher("AppDomainName", logEntry.AppDomainName),
                        new PropertyEnricher("Categories", logEntry.Categories),
                        new PropertyEnricher("ErrorMessages", logEntry.ErrorMessages),
                        new PropertyEnricher("EventId", logEntry.EventId),
                        new PropertyEnricher("LoggedSeverity", logEntry.LoggedSeverity),
                        new PropertyEnricher("MachineName", logEntry.MachineName),
                        new PropertyEnricher("ManagedThreadName", logEntry.ManagedThreadName),
                        new PropertyEnricher("Message", logEntry.Message),
                        new PropertyEnricher("Priority", logEntry.Priority),
                        new PropertyEnricher("ProcessId", logEntry.ProcessId),
                        new PropertyEnricher("ProcessName", logEntry.ProcessName),
                        new PropertyEnricher("RelatedActivityId", logEntry.RelatedActivityId),
                        new PropertyEnricher("Severity", logEntry.Severity),
                        new PropertyEnricher("TimeStamp", logEntry.TimeStamp),
                        new PropertyEnricher("Title", logEntry.Title),
                        new PropertyEnricher("Win32ThreadId", logEntry.Win32ThreadId),
                    });
                    foreach (var item in logEntry.ExtendedProperties)
                    {
                        logger = logger.ForContext(item.Key, item.Value);
                    }
                    logger.Write(level, "{Source}: {EventId}: {Message}", source, id, logEntry.Message);
                }
            }
            else
            {
                Log.Write(level, "{Source}: {Id}: {@Data}", source, id, data);
            }
        }

        public override void Write(string message)
        {
            Log.Write(LogEventLevel.Verbose, message);
        }

        public override void WriteLine(string message)
        {
            Log.Write(LogEventLevel.Verbose, message);
        }

        private static LogEventLevel MapTraceEventTypeToLogEventLevel(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Critical:
                    return LogEventLevel.Fatal;
                case TraceEventType.Error:
                    return LogEventLevel.Error;
                case TraceEventType.Warning:
                    return LogEventLevel.Warning;
                case TraceEventType.Information:
                    return LogEventLevel.Information;
                case TraceEventType.Verbose:
                    return LogEventLevel.Debug;
                case TraceEventType.Start:
                    return LogEventLevel.Information;
                case TraceEventType.Stop:
                    return LogEventLevel.Information;
                case TraceEventType.Suspend:
                    return LogEventLevel.Information;
                case TraceEventType.Resume:
                    return LogEventLevel.Information;
                case TraceEventType.Transfer:
                    return LogEventLevel.Information;
                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}
