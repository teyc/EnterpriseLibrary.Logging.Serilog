using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace EnterpriseLibrary.Logging.Serilog.Tests
{
    public class TestLogger
    {
        [Test]
        public void EnterpriseLogShouldWriteToSerilogSink()
        {
            var sink = new TestSink();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(sink).CreateLogger();

            var container = new UnityContainer().AddNewExtension<EnterpriseLibraryCoreExtension>();
            var logWriter = container.Resolve<LogWriter>();

            logWriter.Write(
                new LogEntry("Sample message", "General", 1, 1, TraceEventType.Information, "Sample Title", 
                    new Dictionary<string, object> { { "SampleKey1", "SampleValue1" }}));

            Assert.AreEqual(1, sink.LogEvents.Count);

            var firstSerilogEvent = sink.LogEvents[0];
            Assert.AreEqual("\"General\": 1: \"Sample message\"", firstSerilogEvent.RenderMessage());

        }
    }

    internal class TestSink: ILogEventSink
    {
        public List<LogEvent> LogEvents { get; } = new List<LogEvent>();

        public void Emit(LogEvent logEvent)
        {
            LogEvents.Add(logEvent);
        }
    }
}
