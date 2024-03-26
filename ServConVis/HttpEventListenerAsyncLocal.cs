using System.Diagnostics.Tracing;
using System.Diagnostics;

namespace ServConVis
{
    internal sealed class HttpEventListenerAsyncLocal : EventListener
    {
        private readonly AsyncLocal<Request> _currentRequest = new();

        private sealed record Request(string Url, Stopwatch ExecutionTime);

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == "System.Net.Http")
            {
                EnableEvents(eventSource, EventLevel.Informational, EventKeywords.All);
            }

            base.OnEventSourceCreated(eventSource);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventId == 1) // eventData.EventName == "RequestStart"
            {
                var scheme = (string)eventData.Payload[0];
                var host = (string)eventData.Payload[1];
                var port = (int)eventData.Payload[2];
                var pathAndQuery = (string)eventData.Payload[3];
                _currentRequest.Value = new Request($"{scheme}://{host}:{port}{pathAndQuery}", Stopwatch.StartNew());
            }
            else if (eventData.EventId == 2) // eventData.EventName == "RequestStop"
            {
                var currentRequest = _currentRequest.Value;
                if (currentRequest != null)
                {
                    Console.WriteLine($"{currentRequest.Url} executed in {currentRequest.ExecutionTime.ElapsedMilliseconds:F1}ms");
                }
            }
        }
    }
}
