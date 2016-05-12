using System;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace MvcEndpointsDiscovery
{
    [CmdletBinding]
    [Cmdlet(VerbsCommon.Get, "MvcEndpoints")]
    [OutputType(typeof (MvcEndpointInfo))]
    public class MvcEnpointsCmdlet : Cmdlet
    {
        [Parameter(ValueFromPipelineByPropertyName = true, ValueFromPipeline = true,
            HelpMessage = "File path for MVC controller code in C#", Position = 0)]
        [Alias("file", "FullName", "Name")]
        public string[] FilePath { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, ValueFromPipeline = true,
            HelpMessage = "Base class name that controller supposed to be inherited from", Position = 1)]
        public string[] BaseClass { get; set; }

        protected override void BeginProcessing()
        {
            WriteDebug("Before start");
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            WriteDebug("Started processing");

            try
            {
                if (FilePath == null || !FilePath.Any())
                {
                    WriteError(new ErrorRecord(new ArgumentNullException("FilePath"), "FilePathNull",
                        ErrorCategory.InvalidArgument, null));
                    return;
                }

                foreach (var file in FilePath)
                {
                    if (string.IsNullOrWhiteSpace(file) || !file.EndsWith(".cs")) return;

                    var root = MvcControllerParser.GetControllerRoot(file, BaseClass);

                    if (root == null) continue;

                    var endpoints = MvcControllerParser.GetEndpointsCollection(root);

                    foreach (var endpoint in endpoints)
                    {
                        endpoint.ControllerName = Path.GetFileNameWithoutExtension(file).Replace("Controller", string.Empty);

                        WriteObject(endpoint);
                    }
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(e, "error", ErrorCategory.InvalidOperation, null));
            }

            WriteDebug("Finished processing");
        }

        internal void Execute()
        {
            ProcessRecord();
        }
    }
}