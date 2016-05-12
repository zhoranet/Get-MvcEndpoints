using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvcEndpointsDiscovery;

namespace GetMvcEndpointsCmdlet.TestBench
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdLet = new MvcEnpointsCmdlet
            {
                FilePath = new[] {@"X:\GitSource\343\ProductsBt\LobbyService\Microsoft.GoW.LobbyService.Web"}
            };
            cmdLet.Invoke<string>();
        }
    }
}
