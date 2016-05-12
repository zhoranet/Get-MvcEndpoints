using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcEndpointsDiscovery;

namespace GetMvcEndpointsCmdlet.TestBench
{
    [TestClass]
    public class CodeParserTests
    {
        [TestMethod]
        public void VerifyUniqueName_ThreeNames()
        {
            HashSet<string> uniqueNames = new HashSet<string>();
            MvcControllerParser.VerifyUniqueName(uniqueNames, "name");
            MvcControllerParser.VerifyUniqueName(uniqueNames, "name");
            MvcControllerParser.VerifyUniqueName(uniqueNames, "name");

            uniqueNames.Should().HaveCount(3);
            uniqueNames.ElementAt(0).Should().Be("name");
            uniqueNames.ElementAt(1).Should().Be("name1");
            uniqueNames.ElementAt(2).Should().Be("name2");
        }
    }
}
