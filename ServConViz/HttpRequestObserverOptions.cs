namespace ServConViz
{
    public class HttpRequestObserverOptions
    {
        public bool ShowInLog { get; set; } = true;
        public bool ShowIncoming { get; set; } = true;
        public bool ShowOutgoing { get; set; } = true;
        public string ServiceName { get; set; } = "TestService";
        public string ServConVizServerUrl { get; set; } = "localhost";
        public int ServConVizServerPort { get; set; } = 65535;
    }
}
