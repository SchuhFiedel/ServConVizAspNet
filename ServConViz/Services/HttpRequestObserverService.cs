using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServConViz.Services
{
    public class HttpRequestObserverService
    {
        private HttpClient _client;

        public readonly Uri serviceUri;
        public readonly HttpRequestObserverOptions options;

        public HttpRequestObserverService(ILogger<HttpRequestsObserver> logger, IOptions<HttpRequestObserverOptions> options)
        {
            DiagnosticListener.AllListeners.Subscribe(new HttpRequestsObserver(logger, this));
            this.options = options.Value;
            var uriBuilder = new UriBuilder();
            uriBuilder.Host = this.options.ServConVizServerUrl;
            uriBuilder.Port = this.options.ServConVizServerPort;
            serviceUri = uriBuilder.Uri;

            _client = new HttpClient();

            logger.LogInformation($"Using Options: {JsonSerializer.Serialize(this.options)}");
        }


        public async void LogIncomingAsync(Uri uri, HttpMethod requestType)
        {
            var data = new HttpRequestObserverDTO()
            {
                Incoming = true,
                requestUri = uri,
                HttpRequestType = requestType
            };

            _ = _client.PostAsJsonAsync(serviceUri, data);
        }

        public async void LogOutgoingAsync(Uri uri, HttpMethod requestType)
        {
            var data = new HttpRequestObserverDTO()
            {
                Incoming = false,
                requestUri = uri,
                HttpRequestType = requestType
            };

            _ = _client.PostAsJsonAsync(serviceUri, data);
        }

        class HttpRequestObserverDTO
        {
            public Uri requestUri { get; set; }
            public bool Incoming { get; set; }
            public HttpMethod HttpRequestType { get; set; }
        }

    }


}
