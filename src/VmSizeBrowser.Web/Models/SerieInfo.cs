using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualMachineSizeBrowser.Models
{
    public class SerieInfo
    {
        public int? MaxIOPS { get; set; }
        public int? MaxNICs { get; set; }
        public string NetworkPerformance { get; set; }
        public int GpuCount { get; set; }
    }
}
