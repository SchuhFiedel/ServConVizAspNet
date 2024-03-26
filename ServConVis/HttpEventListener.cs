using System.Collections.Concurrent;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using System.Globalization;

namespace ServConVis
{
    sealed class HttpEventListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            switch (eventSource.Name)
            {
                case "System.Net.Http":
                    EnableEvents(eventSource, EventLevel.Informational, EventKeywords.All);
                    break;

                // Enable EventWrittenEventArgs.ActivityId to correlate Start and Stop events
                case "System.Threading.Tasks.TplEventSource":
                    const EventKeywords TasksFlowActivityIds = (EventKeywords)0x80;
                    EnableEvents(eventSource, EventLevel.LogAlways, TasksFlowActivityIds);
                    break;
            }

            base.OnEventSourceCreated(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // note: Use eventData.ActivityId to correlate Start and Stop events
            if (eventData.EventId == 1) // eventData.EventName == "RequestStart"
            {
                var scheme = (string)eventData.Payload[0];
                var host = (string)eventData.Payload[1];
                var port = (int)eventData.Payload[2];
                var pathAndQuery = (string)eventData.Payload[3];
                var versionMajor = (byte)eventData.Payload[4];
                var versionMinor = (byte)eventData.Payload[5];
                var policy = (HttpVersionPolicy)eventData.Payload[6];

                Console.WriteLine($"{eventData.ActivityId} {eventData.EventName} {scheme}://{host}:{port}{pathAndQuery} HTTP/{versionMajor}.{versionMinor}");
            }
            else if (eventData.EventId == 2) // eventData.EventName == "RequestStop"
            {
                Console.WriteLine(eventData.ActivityId + " " + eventData.EventName);
            }
        }
    }
}
