namespace MvcEndpointsDiscovery
{
    internal class MvcEndpointInfo
    {
        public string ControllerName { get; set; }
        public string EndpointName { get; set; }
        public string HttpMethod { get; set; }
        public string Route { get; set; }

        public override string ToString()
        {
            return ControllerName + "," + EndpointName + "," + HttpMethod + "," + Route;
        }
    }
}