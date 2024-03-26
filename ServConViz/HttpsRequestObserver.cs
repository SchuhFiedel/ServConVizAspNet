using Microsoft.Extensions.Logging;
using ServConViz.Services;
using System.Diagnostics;

namespace ServConViz
{
    public sealed class HttpRequestsObserver : IDisposable, IObserver<DiagnosticListener>
    {
        private IDisposable _subscription;
        private ILogger _logger;
        private readonly HttpRequestObserverService _service;

        public HttpRequestsObserver(ILogger logger, HttpRequestObserverService service)
        {
            _logger = logger;
            _service = service;
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "HttpHandlerDiagnosticListener")
            {
                Debug.Assert(_subscription == null);
                _subscription = value.Subscribe(new HttpHandlerDiagnosticListener(_logger, _service));
            }
        }

        public void OnCompleted() { }
        public void OnError(Exception error) { }

        public void Dispose()
        {
            _subscription?.Dispose();
        }

        private sealed class HttpHandlerDiagnosticListener : IObserver<KeyValuePair<string, object>>
        {
            private static readonly Func<object, HttpRequestMessage> RequestAccessor = CreateGetRequest();
            private static readonly Func<object, HttpResponseMessage> ResponseAccessor = CreateGetResponse();
            private ILogger _logger;
            private HttpRequestObserverService _service;
            public HttpHandlerDiagnosticListener(ILogger logger, HttpRequestObserverService service)
            {
                _logger = logger;
                _service = service;
            }

            public void OnCompleted() { }
            public void OnError(Exception error) { }

            public void OnNext(KeyValuePair<string, object> value)
            {
                // note: Legacy applications can use "System.Net.Http.HttpRequest" and "System.Net.Http.Response"
                switch (value.Key)
                {
                    case "System.Net.Http.HttpRequestOut.Start":
                        {
                            // The type is private, so we need to use reflection to access it.
                            var request = RequestAccessor(value.Value);
                            if (request.RequestUri != _service.serviceUri)
                            {
                                _logger.LogInformation($"{request.Method} {request.RequestUri} {request.Version} (UserAgent: {request.Headers.UserAgent})");
                                _service.LogOutgoingAsync(request.RequestUri, request.Method);
                            }
                            break;
                        }
                    case "System.Net.Http.HttpRequestOut.Stop":
                        {
                            // The type is private, so we need to use reflection to access it.
                            var response = ResponseAccessor(value.Value);
                            if (response != null && response?.RequestMessage?.RequestUri != _service.serviceUri)
                            {
                                _logger.LogInformation($"{response.StatusCode} {response.RequestMessage.RequestUri} {response.RequestMessage.Content}");
                                //_service.LogOutgoingAsync(response.RequestMessage.RequestUri, response.RequestMessage.Method);
                            }
                            break;
                        }
                    
                }
            }

            private static Func<object, HttpRequestMessage> CreateGetRequest()
            {
                var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStartData, System.Net.Http", throwOnError: true);
                var requestProperty = requestDataType.GetProperty("Request");
                return (o) => (HttpRequestMessage)requestProperty.GetValue(o);
            }

            private static Func<object, HttpResponseMessage> CreateGetResponse()
            {
                var requestDataType = Type.GetType("System.Net.Http.DiagnosticsHandler+ActivityStopData, System.Net.Http", throwOnError: true);
                var requestProperty = requestDataType.GetProperty("Response");
                return (o) => (HttpResponseMessage)requestProperty.GetValue(o);
            }
        }
    }
}
